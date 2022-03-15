using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Main
{
    /// <summary>
    /// Class representing an item slot of the inventory.
    /// </summary>
    public class ItemSlotManager : MonoBehaviour
    {
        #region Serialize fields
        [SerializeField]
        private Image itemIcon;

        [SerializeField]
        private Image itemHighlight;
        #endregion

        #region Public properties
        /// <summary>
        /// Boolean indicating if this slot is selected.
        /// </summary>
        public bool IsItemSelected { get; private set; }

        /// <summary>
        /// Boolean indicating if this slot is highlighted.
        /// </summary>
        public bool IsItemHighlighted { get; private set; }

        /// <summary>
        /// Boolean indicating if this slot holds an item.
        /// </summary>
        public bool IsItemHeld { get; private set; }

        /// <summary>
        /// Name of the item held by the slot.
        /// </summary>
        public string ItemName { get; private set; }

        /// <summary>
        /// Getter of the item icon.
        /// </summary>
        public Sprite ItemIcon
        {
            get { return itemIcon.sprite; }
        }
        #endregion

        #region Private members
        /// <summary>
        /// Reference to the blinking coroutine to manage its lifetime.
        /// </summary>
        private Coroutine blinkCoroutine;
        #endregion

        #region Monobehaviour methods
        // Start is called before the first frame update
        void Start()
        {
            blinkCoroutine = null;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Highlight this slot.
        /// </summary>
        public void HighlightSlot()
        {
            itemHighlight.gameObject.SetActive(true);
            IsItemHighlighted = true;
        }

        /// <summary>
        /// Unhighlight this slot.
        /// </summary>
        public void UnHighlightSlot()
        {
            itemHighlight.gameObject.SetActive(false);
            IsItemHighlighted = false;
        }

        /// <summary>
        /// Method to set the slot with an item.
        /// </summary>
        /// <param name="itemSprite"> Sprite of the item to display in the slot.</param>
        /// <param name="itemName"> Name of the item.</param>
        public void PopulateItem(Sprite itemSprite, string itemName)
        {
            itemIcon.sprite = itemSprite;
            ItemName = itemName;
            itemIcon.gameObject.SetActive(itemIcon.sprite != null);
            IsItemHeld = itemIcon.sprite != null;
        }

        /// <summary>
        /// Method to select the slot.
        /// </summary>
        public void SelectItem()
        {
            IsItemSelected = true;
            if (blinkCoroutine == null)
            {
                blinkCoroutine = StartCoroutine(BlinkSelected());
            }
        }

        /// <summary>
        /// Method to unselect the slot.
        /// </summary>
        public void UnselectItem()
        {
            // Stop the blinking coroutine if it was running.
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }

            itemHighlight.gameObject.SetActive(IsItemHighlighted);
            IsItemSelected = false;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Coroutine to make the slot blink when its item is selected.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BlinkSelected()
        {
            while (true)
            {

                itemHighlight.gameObject.SetActive(!itemHighlight.gameObject.activeInHierarchy);

                yield return new WaitForSeconds(.3f);
            }
        }
        #endregion
    }
}
