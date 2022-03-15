using System.Collections.Generic;

namespace Inventory.Main
{
    /// <summary>
    /// Enumeration of possible directions in the inventory navigation.
    /// </summary>
    public enum DIRECTION
    {
        LEFT,
        UP,
        RIGHT,
        DOWN
    }

    /// <summary>
    /// This class represents the inventory grid.
    /// It is composed of an array of ItemHolderArrays to be bidimensionnal.
    /// </summary>
    [System.Serializable]
    public class ItemHolderGrid
    {
        public List<ItemHolderArray> arrays = new List<ItemHolderArray>();
        public ItemSlotManager this[int x, int y] => arrays[x][y];

        /// <summary>
        /// Returns the total number of slots.
        /// </summary>
        public int Count
        {
            get
            {
                int result = 0;

                foreach (ItemHolderArray array in arrays)
                {
                    result += array.cells.Count;
                }

                return result;
            }
        }

        /// <summary>
        /// Get the height of the grid.
        /// </summary>
        public int Height
        {
            get
            {
                return arrays.Count;
            }
        }

        /// <summary>
        /// Get the width of the grid.
        /// </summary>
        public int Width
        {
            get
            {
                return arrays[0].cells.Count;
            }
        }

        /// <summary>
        /// Method to determine the next slot highlighted during navigation.
        /// </summary>
        /// <param name="currentSlot"> Currently highlighted slot.</param>
        /// <param name="direction"> Direction of the next highlight.</param>
        /// <returns> The newly highlighted slot.</returns>
        public ItemSlotManager NextSlot(ItemSlotManager currentSlot, DIRECTION direction)
        {
            int currentItemColumn = -1;
            int currentItemRow = -1;

            // Determining currently highlighted slot's indexes
            for (int i = 0; i < arrays.Count; i++)
            {
                for (int j = 0; j < arrays[i].cells.Count; j++)
                {
                    if (arrays[i].cells[j] == currentSlot)
                    {
                        currentItemRow = i;
                        currentItemColumn = j;
                    }
                }
            }

            // Determining the nextly highlighted slot's indexes depending on direction.
            switch (direction)
            {
                case DIRECTION.LEFT:
                    --currentItemColumn;

                    if (currentItemColumn < 0)
                    {
                        currentItemColumn = Width - 1;
                    }
                    break;
                case DIRECTION.UP:
                    --currentItemRow;

                    if (currentItemRow < 0)
                    {
                        currentItemRow = Height - 1;
                    }
                    break;
                case DIRECTION.RIGHT:
                    ++currentItemColumn;

                    if (currentItemColumn >= Width)
                    {
                        currentItemColumn = 0;
                    }
                    break;
                case DIRECTION.DOWN:
                    ++currentItemRow;

                    if (currentItemRow >= Height)
                    {
                        currentItemRow = 0;
                    }
                    break;
                default:
                    break;
            }

            return arrays[currentItemRow][currentItemColumn];
        }

        /// <summary>
        /// Method to clear the entire grid.
        /// </summary>
        public void ClearIcons()
        {
            foreach (ItemHolderArray array in arrays)
            {
                foreach (ItemSlotManager itemHolder in array.cells)
                {
                    // Populate each slot with a null item.
                    itemHolder.PopulateItem(null, "");
                    itemHolder.UnselectItem();
                }
            }
        }
    }
}
