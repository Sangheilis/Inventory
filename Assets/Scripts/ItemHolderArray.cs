using System.Collections.Generic;

namespace Inventory.Main
{
    /// <summary>
    /// This class represents an array of item slots.
    /// </summary>
    [System.Serializable]
    public class ItemHolderArray
    {
        /// <summary>
        /// List of item slots.
        /// </summary>
        public List<ItemSlotManager> cells = new List<ItemSlotManager>();

        /// <summary>
        /// Get an item slot depending on its index in the list.
        /// </summary>
        /// <param name="index"> Index of the slot to return.</param>
        /// <returns></returns>
        public ItemSlotManager this[int index] => cells[index];
    }
}
