﻿using System.Collections;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using SabreTools.Logging;
using SabreTools.Matching;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Item dictionary with statistics, bucketing, and sorting
    /// </summary>
    /// <remarks>
    /// TODO: Make this into a database model instead of just an in-memory object
    /// This will help handle extremely large sets
    /// </remarks>
    [JsonObject("items"), XmlRoot("items")]
    public class ItemDictionary : IDictionary<string, ConcurrentList<DatItem>?>
    {
        #region Private instance variables

        /// <summary>
        /// Determine the bucketing key for all items
        /// </summary>
        private ItemKey bucketedBy;

        /// <summary>
        /// Determine merging type for all items
        /// </summary>
        private DedupeType mergedBy;

        /// <summary>
        /// Internal dictionary for the class
        /// </summary>
#if NET40_OR_GREATER || NETCOREAPP
        private readonly ConcurrentDictionary<string, ConcurrentList<DatItem>?> items;
#else
        private readonly Dictionary<string, ConcurrentList<DatItem>?> items;
#endif

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly Logger logger;

        #endregion

        #region Publically available fields

        #region Keys

        /// <summary>
        /// Get the keys from the file dictionary
        /// </summary>
        /// <returns>List of the keys</returns>
        [JsonIgnore, XmlIgnore]
        public ICollection<string> Keys
        {
            get { return items.Keys; }
        }

        /// <summary>
        /// Get the keys in sorted order from the file dictionary
        /// </summary>
        /// <returns>List of the keys in sorted order</returns>
        [JsonIgnore, XmlIgnore]
        public List<string> SortedKeys
        {
            get
            {
                var keys = items.Keys.ToList();
                keys.Sort(new NaturalComparer());
                return keys;
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// DAT statistics
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public DatStatistics DatStatistics { get; } = new DatStatistics();

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Generic constructor
        /// </summary>
        public ItemDictionary()
        {
            bucketedBy = ItemKey.NULL;
            mergedBy = DedupeType.None;
#if NET40_OR_GREATER || NETCOREAPP
            items = new ConcurrentDictionary<string, ConcurrentList<DatItem>?>();
#else
            items = new Dictionary<string, ConcurrentList<DatItem>?>();
#endif
            logger = new Logger(this);
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Passthrough to access the file dictionary
        /// </summary>
        /// <param name="key">Key in the dictionary to reference</param>
        public ConcurrentList<DatItem>? this[string key]
        {
            get
            {
                // Explicit lock for some weird corner cases
                lock (key)
                {
                    // Ensure the key exists
                    EnsureKey(key);

                    // Now return the value
                    return items[key];
                }
            }
            set
            {
                Remove(key);
                if (value == null)
                    items[key] = null;
                else
                    AddRange(key, value);
            }
        }

        /// <summary>
        /// Add a value to the file dictionary
        /// </summary>
        /// <param name="key">Key in the dictionary to add to</param>
        /// <param name="value">Value to add to the dictionary</param>
        public void Add(string key, DatItem value)
        {
            // Explicit lock for some weird corner cases
            lock (key)
            {
                // Ensure the key exists
                EnsureKey(key);

                // If item is null, don't add it
                if (value == null)
                    return;

                // Now add the value
                items[key]!.Add(value);

                // Now update the statistics
                DatStatistics.AddItemStatistics(value);
            }
        }

        /// <summary>
        /// Add a range of values to the file dictionary
        /// </summary>
        /// <param name="key">Key in the dictionary to add to</param>
        /// <param name="value">Value to add to the dictionary</param>
        public void Add(string key, ConcurrentList<DatItem>? value)
        {
            AddRange(key, value);
        }

        /// <summary>
        /// Add a range of values to the file dictionary
        /// </summary>
        /// <param name="key">Key in the dictionary to add to</param>
        /// <param name="value">Value to add to the dictionary</param>
        public void AddRange(string key, ConcurrentList<DatItem>? value)
        {
            // Explicit lock for some weird corner cases
            lock (key)
            {
                // If the value is null or empty, just return
                if (value == null || !value.Any())
                    return;

                // Ensure the key exists
                EnsureKey(key);

                // Now add the value
                items[key]!.AddRange(value);

                // Now update the statistics
                foreach (DatItem item in value)
                {
                    DatStatistics.AddItemStatistics(item);
                }
            }
        }

        /// <summary>
        /// Remove any keys that have null or empty values
        /// </summary>
        public void ClearEmpty()
        {
            var keys = items.Keys.Where(k => k != null).ToList();
            foreach (string key in keys)
            {
                // If the key doesn't exist, skip
                if (!items.ContainsKey(key))
                    continue;

                // If the value is null, remove
                else if (items[key] == null)
#if NET40_OR_GREATER || NETCOREAPP
                    items.TryRemove(key, out _);
#else
                    items.Remove(key);
#endif

                // If there are no non-blank items, remove
                else if (!items[key]!.Any(i => i != null && i is not Blank))
#if NET40_OR_GREATER || NETCOREAPP
                    items.TryRemove(key, out _);
#else
                    items.Remove(key);
#endif
            }
        }

        /// <summary>
        /// Remove all items marked for removal
        /// </summary>
        public void ClearMarked()
        {
            var keys = items.Keys.ToList();
            foreach (string key in keys)
            {
                ConcurrentList<DatItem>? oldItemList = items[key];
                ConcurrentList<DatItem>? newItemList = oldItemList?.Where(i => i.GetBoolFieldValue(DatItem.RemoveKey) != true)?.ToConcurrentList();

                Remove(key);
                AddRange(key, newItemList);
            }
        }

        /// <summary>
        /// Get if the file dictionary contains the key
        /// </summary>
        /// <param name="key">Key in the dictionary to check</param>
        /// <returns>True if the key exists, false otherwise</returns>
        public bool ContainsKey(string key)
        {
            // If the key is null, we return false since keys can't be null
            if (key == null)
                return false;

            // Explicit lock for some weird corner cases
            lock (key)
            {
                return items.ContainsKey(key);
            }
        }

        /// <summary>
        /// Get if the file dictionary contains the key and value
        /// </summary>
        /// <param name="key">Key in the dictionary to check</param>
        /// <param name="value">Value in the dictionary to check</param>
        /// <returns>True if the key exists, false otherwise</returns>
        public bool Contains(string key, DatItem value)
        {
            // If the key is null, we return false since keys can't be null
            if (key == null)
                return false;

            // Explicit lock for some weird corner cases
            lock (key)
            {
                if (items.ContainsKey(key) && items[key] != null)
                    return items[key]!.Contains(value);
            }

            return false;
        }

        /// <summary>
        /// Ensure the key exists in the items dictionary
        /// </summary>
        /// <param name="key">Key to ensure</param>
        public void EnsureKey(string key)
        {
            // If the key is missing from the dictionary, add it
            if (!items.ContainsKey(key))
#if NET40_OR_GREATER || NETCOREAPP
                items.TryAdd(key, []);
#else
                items[key] = [];
#endif
        }

        /// <summary>
        /// Get a list of filtered items for a given key
        /// </summary>
        /// <param name="key">Key in the dictionary to retrieve</param>
        public ConcurrentList<DatItem> FilteredItems(string key)
        {
            lock (key)
            {
                // Get the list, if possible
                ConcurrentList<DatItem>? fi = items[key];
                if (fi == null)
                    return [];

                // Filter the list
                return fi.Where(i => i != null)
                    .Where(i => i.GetBoolFieldValue(DatItem.RemoveKey) != true)
                    .Where(i => i.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                    .ToConcurrentList();
            }
        }

        /// <summary>
        /// Remove a key from the file dictionary if it exists
        /// </summary>
        /// <param name="key">Key in the dictionary to remove</param>
        public bool Remove(string key)
        {
            // Explicit lock for some weird corner cases
            lock (key)
            {
                // If the key doesn't exist, return
                if (!ContainsKey(key) || items[key] == null)
                    return false;

                // Remove the statistics first
                foreach (DatItem item in items[key]!)
                {
                    DatStatistics.RemoveItemStatistics(item);
                }

                // Remove the key from the dictionary
#if NET40_OR_GREATER || NETCOREAPP
                return items.TryRemove(key, out _);
#else
                return items.Remove(key);
#endif
            }
        }

        /// <summary>
        /// Remove the first instance of a value from the file dictionary if it exists
        /// </summary>
        /// <param name="key">Key in the dictionary to remove from</param>
        /// <param name="value">Value to remove from the dictionary</param>
        public bool Remove(string key, DatItem value)
        {
            // Explicit lock for some weird corner cases
            lock (key)
            {
                // If the key and value doesn't exist, return
                if (!Contains(key, value) || items[key] == null)
                    return false;

                // Remove the statistics first
                DatStatistics.RemoveItemStatistics(value);

                return items[key]!.Remove(value);
            }
        }

        /// <summary>
        /// Reset a key from the file dictionary if it exists
        /// </summary>
        /// <param name="key">Key in the dictionary to reset</param>
        public bool Reset(string key)
        {
            // If the key doesn't exist, return
            if (!ContainsKey(key) || items[key] == null)
                return false;

            // Remove the statistics first
            foreach (DatItem item in items[key]!)
            {
                DatStatistics.RemoveItemStatistics(item);
            }

            // Remove the key from the dictionary
            items[key] = [];
            return true;
        }

        /// <summary>
        /// Override the internal ItemKey value
        /// </summary>
        /// <param name="newBucket"></param>
        public void SetBucketedBy(ItemKey newBucket)
        {
            bucketedBy = newBucket;
        }

        #endregion

        #region Bucketing

        /// <summary>
        /// Take the arbitrarily bucketed Files Dictionary and convert to one bucketed by a user-defined method
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="dedupeType">Dedupe type that should be used</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        public void BucketBy(ItemKey bucketBy, DedupeType dedupeType, bool lower = true, bool norename = true)
        {
            // If we have a situation where there's no dictionary or no keys at all, we skip
#if NET40_OR_GREATER || NETCOREAPP
            if (items == null || items.IsEmpty)
#else
            if (items == null || items.Count == 0)
#endif
                return;

            // If the sorted type isn't the same, we want to sort the dictionary accordingly
            if (bucketedBy != bucketBy && bucketBy != ItemKey.NULL)
            {
                logger.User($"Organizing roms by {bucketBy}");
                PerformBucketing(bucketBy, lower, norename);
            }

            // If the merge type isn't the same, we want to merge the dictionary accordingly
            if (mergedBy != dedupeType)
            {
                logger.User($"Deduping roms by {dedupeType}");
                PerformDeduplication(bucketBy, dedupeType);
            }
            // If the merge type is the same, we want to sort the dictionary to be consistent
            else
            {
                logger.User($"Sorting roms by {bucketBy}");
                PerformSorting();
            }
        }
        
        /// <summary>
        /// List all duplicates found in a DAT based on a DatItem
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>List of matched DatItem objects</returns>
        public ConcurrentList<DatItem> GetDuplicates(DatItem datItem, bool sorted = false)
        {
            ConcurrentList<DatItem> output = [];

            // Check for an empty rom list first
            if (DatStatistics.TotalCount == 0)
                return output;

            // We want to get the proper key for the DatItem
            string key = SortAndGetKey(datItem, sorted);

            // If the key doesn't exist, return the empty list
            if (!ContainsKey(key))
                return output;

            // Try to find duplicates
            ConcurrentList<DatItem>? roms = this[key];
            if (roms == null)
                return output;

            ConcurrentList<DatItem> left = [];
            for (int i = 0; i < roms.Count; i++)
            {
                DatItem other = roms[i];
                if (other.GetBoolFieldValue(DatItem.RemoveKey) == true)
                    continue;

                if (datItem.Equals(other))
                {
                    other.SetFieldValue<bool?>(DatItem.RemoveKey, true);
                    output.Add(other);
                }
                else
                {
                    left.Add(other);
                }
            }

            // Add back all roms with the proper flags
            Remove(key);
            AddRange(key, output);
            AddRange(key, left);

            return output;
        }

        /// <summary>
        /// Check if a DAT contains the given DatItem
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>True if it contains the rom, false otherwise</returns>
        public bool HasDuplicates(DatItem datItem, bool sorted = false)
        {
            // Check for an empty rom list first
            if (DatStatistics.TotalCount == 0)
                return false;

            // We want to get the proper key for the DatItem
            string key = SortAndGetKey(datItem, sorted);

            // If the key doesn't exist, return the empty list
            if (!ContainsKey(key))
                return false;

            // Try to find duplicates
            ConcurrentList<DatItem>? roms = this[key];
            return roms?.Any(r => datItem.Equals(r)) == true;
        }

        /// <summary>
        /// Get the highest-order Field value that represents the statistics
        /// </summary>
        private ItemKey GetBestAvailable()
        {
            // Get the required counts
            long diskCount = DatStatistics.GetItemCount(ItemType.Disk);
            long mediaCount = DatStatistics.GetItemCount(ItemType.Media);
            long romCount = DatStatistics.GetItemCount(ItemType.Rom);
            long nodumpCount = DatStatistics.GetStatusCount(ItemStatus.Nodump);

            // If all items are supposed to have a SHA-512, we bucket by that
            if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA512))
                return ItemKey.SHA512;

            // If all items are supposed to have a SHA-384, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA384))
                return ItemKey.SHA384;

            // If all items are supposed to have a SHA-256, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA256))
                return ItemKey.SHA256;

            // If all items are supposed to have a SHA-1, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA1))
                return ItemKey.SHA1;

            // If all items are supposed to have a MD5, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.MD5))
                return ItemKey.MD5;

            // Otherwise, we bucket by CRC
            else
                return ItemKey.CRC;
        }

        /// <summary>
        /// Perform bucketing based on the item key provided
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="lower">True if the key should be lowercased, false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        private void PerformBucketing(ItemKey bucketBy, bool lower, bool norename)
        {
            // Set the sorted type
            bucketedBy = bucketBy;

            // Reset the merged type since this might change the merge
            mergedBy = DedupeType.None;

            // First do the initial sort of all of the roms inplace
            List<string> oldkeys = [.. Keys];

#if NET452_OR_GREATER || NETCOREAPP
                Parallel.For(0, oldkeys.Count, Globals.ParallelOptions, k =>
#elif NET40_OR_GREATER
            Parallel.For(0, oldkeys.Count, k =>
#else
                for (int k = 0; k < oldkeys.Count; k++)
#endif
            {
                string key = oldkeys[k];
                if (this[key] == null)
                    Remove(key);

                // Now add each of the roms to their respective keys
                for (int i = 0; i < this[key]!.Count; i++)
                {
                    DatItem item = this[key]![i];
                    if (item == null)
                        continue;

                    // We want to get the key most appropriate for the given sorting type
                    string newkey = item.GetKey(bucketBy, lower, norename);

                    // If the key is different, move the item to the new key
                    if (newkey != key)
                    {
                        Add(newkey, item);
                        Remove(key, item);
                        i--; // This make sure that the pointer stays on the correct since one was removed
                    }
                }

                // If the key is now empty, remove it
                if (this[key]!.Count == 0)
                    Remove(key);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
                }
#endif
        }

        /// <summary>
        /// Perform deduplication based on the deduplication type provided
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="dedupeType">Dedupe type that should be used</param>
        private void PerformDeduplication(ItemKey bucketBy, DedupeType dedupeType)
        {
            // Set the sorted type
            mergedBy = dedupeType;

            List<string> keys = [.. Keys];
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(keys, key =>
#else
                foreach (var key in keys)
#endif
            {
                // Get the possibly unsorted list
                ConcurrentList<DatItem>? sortedlist = this[key]?.ToConcurrentList();
                if (sortedlist == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                        continue;
#endif

                // Sort the list of items to be consistent
                DatItem.Sort(ref sortedlist, false);

                // If we're merging the roms, do so
                if (dedupeType == DedupeType.Full || (dedupeType == DedupeType.Game && bucketBy == ItemKey.Machine))
                    sortedlist = DatItem.Merge(sortedlist);

                // Add the list back to the dictionary
                Reset(key);
                AddRange(key, sortedlist);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
                }
#endif
        }

        /// <summary>
        /// Perform inplace sorting of the dictionary
        /// </summary>
        private void PerformSorting()
        {
            List<string> keys = [.. Keys];
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(keys, key =>
#else
                foreach (var key in keys)
#endif
            {
                // Get the possibly unsorted list
                ConcurrentList<DatItem>? sortedlist = this[key];

                // Sort the list of items to be consistent
                if (sortedlist != null)
                    DatItem.Sort(ref sortedlist, false);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
                }
#endif
        }

        /// <summary>
        /// Sort the input DAT and get the key to be used by the item
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>Key to try to use</returns>
        private string SortAndGetKey(DatItem datItem, bool sorted = false)
        {
            // If we're not already sorted, take care of it
            if (!sorted)
                BucketBy(GetBestAvailable(), DedupeType.None);

            // Now that we have the sorted type, we get the proper key
            return datItem.GetKey(bucketedBy);
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Recalculate the statistics for the Dat
        /// </summary>
        public void RecalculateStats()
        {
            // Wipe out any stats already there
            DatStatistics.ResetStatistics();

            // If we have a blank Dat in any way, return
            if (items == null)
                return;

            // Loop through and add
            foreach (string key in items.Keys)
            {
                ConcurrentList<DatItem>? datItems = items[key];
                if (datItems == null)
                    continue;

                foreach (DatItem item in datItems)
                {
                    DatStatistics.AddItemStatistics(item);
                }
            }
        }

        #endregion

        #region IDictionary Implementations

        public ICollection<ConcurrentList<DatItem>?> Values => ((IDictionary<string, ConcurrentList<DatItem>?>)items).Values;

        public int Count => ((ICollection<KeyValuePair<string, ConcurrentList<DatItem>?>>)items).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, ConcurrentList<DatItem>?>>)items).IsReadOnly;

        public bool TryGetValue(string key, out ConcurrentList<DatItem>? value)
        {
            return ((IDictionary<string, ConcurrentList<DatItem>?>)items).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, ConcurrentList<DatItem>?> item)
        {
            ((ICollection<KeyValuePair<string, ConcurrentList<DatItem>?>>)items).Add(item);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<string, ConcurrentList<DatItem>?>>)items).Clear();
        }

        public bool Contains(KeyValuePair<string, ConcurrentList<DatItem>?> item)
        {
            return ((ICollection<KeyValuePair<string, ConcurrentList<DatItem>?>>)items).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, ConcurrentList<DatItem>?>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, ConcurrentList<DatItem>?>>)items).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, ConcurrentList<DatItem>?> item)
        {
            return ((ICollection<KeyValuePair<string, ConcurrentList<DatItem>?>>)items).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, ConcurrentList<DatItem>?>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, ConcurrentList<DatItem>?>>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        #endregion
    }
}
