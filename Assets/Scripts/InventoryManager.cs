using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using UnityEngine.UI;

namespace Inventory.Main
{
    /// <summary>
    /// Class managing the global inventory behaviour.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        #region Serialize fields
        [SerializeField]
        private Text ItemSelectedText;

        [SerializeField]
        private AssetLabelReference assetLabel;

        [Tooltip("Number of random items to spawn.")]
        [SerializeField]
        private int nbRandomItem;

        [SerializeField]
        private ItemHolderGrid itemHolderManagers;

        [SerializeField]
        private GameObject ButtonAIcon;

        [SerializeField]
        private GameObject ButtonYIcon;

        [SerializeField]
        private Text ButtonAText;

        [SerializeField]
        private Text ButtonYText;

        [SerializeField]
        private AudioSource moveAudioSource;

        [SerializeField]
        private AudioSource selectAudioSource;

        [SerializeField]
        private AudioSource removeAudioSource;

        [SerializeField]
        private AudioSource shuffleAudioSource;

        [SerializeField]
        private AudioSource swapAudioSource;
        #endregion

        #region Private members   
        /// <summary>
        /// Holds the currently highlighted slot.
        /// </summary>
        private ItemSlotManager highlightedSlot;

        /// <summary>
        /// Holds thecurrently selected slot.
        /// </summary>
        private ItemSlotManager selectedSlot;

        /// <summary>
        /// Reference to the populate coroutine to manage its lifetime.
        /// </summary>
        private Coroutine populateCoroutine;

        /// <summary>
        /// Booleans to avoid diagonal moving.
        /// </summary>
        private bool horizontalAxisUsed;
        private bool verticalAxisUsed;
        #endregion

        #region Monobehaviour methods
        // Start is called before the first frame update
        void Start()
        {
            highlightedSlot = itemHolderManagers[0, 0];
            highlightedSlot.HighlightSlot();
            selectedSlot = null;
            populateCoroutine = StartCoroutine(PopulateInventoryRandomly());

            ButtonAText.text = "Select";
            ButtonAIcon.SetActive(highlightedSlot.IsItemHeld);
            ButtonAText.gameObject.SetActive(highlightedSlot.IsItemHeld);

            ButtonYText.text = "Shuffle";
            ButtonYIcon.SetActive(true);
            ButtonYText.gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Jump"))
            {
                if (selectedSlot != null)
                {
                    RemoveSelectedItem();
                }
                else if (populateCoroutine == null)
                {
                    RandomizeItems();
                }
            }

            if (Input.GetAxis("Horizontal") == 0)
            {
                horizontalAxisUsed = false;
            }

            if (Input.GetAxis("Vertical") == 0)
            {
                verticalAxisUsed = false;
            }

            MoveLeft();

            MoveRight();

            MoveUp();

            MoveDown();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit"))
            {
                if (highlightedSlot.IsItemHeld && selectedSlot == null)
                {
                    SelectItem();
                }
                else if (selectedSlot != null)
                {
                    PlaceItem();
                }
            }
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Method to manage down moving of selection.
        /// </summary>
        private void MoveDown()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow)
                || (Input.GetAxis("Vertical") < 0
                && (!verticalAxisUsed && !horizontalAxisUsed)))
            {
                moveAudioSource.Play();
                verticalAxisUsed = true;
                highlightedSlot.UnHighlightSlot();
                highlightedSlot = itemHolderManagers.NextSlot(highlightedSlot, DIRECTION.DOWN);
                highlightedSlot.HighlightSlot();

                if (selectedSlot == null)
                {
                    ButtonAText.text = "Select";
                    ButtonAIcon.SetActive(highlightedSlot.IsItemHeld);
                    ButtonAText.gameObject.SetActive(highlightedSlot.IsItemHeld);
                }
            }
        }

        /// <summary>
        /// Method to manage up moving of selection.
        /// </summary>
        private void MoveUp()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)
                || (Input.GetAxis("Vertical") > 0
                && (!verticalAxisUsed && !horizontalAxisUsed)))
            {
                moveAudioSource.Play();
                verticalAxisUsed = true;
                highlightedSlot.UnHighlightSlot();
                highlightedSlot = itemHolderManagers.NextSlot(highlightedSlot, DIRECTION.UP);
                highlightedSlot.HighlightSlot();

                if (selectedSlot == null)
                {
                    ButtonAText.text = "Select";
                    ButtonAIcon.SetActive(highlightedSlot.IsItemHeld);
                    ButtonAText.gameObject.SetActive(highlightedSlot.IsItemHeld);
                }
            }
        }

        /// <summary>
        /// Method to manage rigth moving of selection.
        /// </summary>
        private void MoveRight()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)
                || (Input.GetAxis("Horizontal") > 0
                && (!horizontalAxisUsed && !verticalAxisUsed)))
            {
                moveAudioSource.Play();
                horizontalAxisUsed = true;
                highlightedSlot.UnHighlightSlot();
                highlightedSlot = itemHolderManagers.NextSlot(highlightedSlot, DIRECTION.RIGHT);
                highlightedSlot.HighlightSlot();

                if (selectedSlot == null)
                {
                    ButtonAText.text = "Select";
                    ButtonAIcon.SetActive(highlightedSlot.IsItemHeld);
                    ButtonAText.gameObject.SetActive(highlightedSlot.IsItemHeld);
                }
            }
        }

        /// <summary>
        /// Method to manage left moving of selection.
        /// </summary>
        private void MoveLeft()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)
                || (Input.GetAxis("Horizontal") < 0
                && (!horizontalAxisUsed && !verticalAxisUsed)))
            {
                moveAudioSource.Play();
                horizontalAxisUsed = true;
                highlightedSlot.UnHighlightSlot();
                highlightedSlot = itemHolderManagers.NextSlot(highlightedSlot, DIRECTION.LEFT);
                highlightedSlot.HighlightSlot();

                if (selectedSlot == null)
                {
                    ButtonAText.text = "Select";
                    ButtonAIcon.SetActive(highlightedSlot.IsItemHeld);
                    ButtonAText.gameObject.SetActive(highlightedSlot.IsItemHeld);
                }
            }
        }

        /// <summary>
        /// Method to place the currently selected item into the currently highlighte slot.
        /// If the slot is already populated with another item, it swaps them.
        /// </summary>
        private void PlaceItem()
        {
            swapAudioSource.Play();
            Sprite tempSprite = selectedSlot.ItemIcon;
            string tempName = selectedSlot.ItemName;

            selectedSlot.PopulateItem(highlightedSlot.ItemIcon, highlightedSlot.ItemName);
            highlightedSlot.PopulateItem(tempSprite, tempName);
            selectedSlot.UnselectItem();
            selectedSlot = null;
            ItemSelectedText.text = "";

            ButtonAText.text = "Select";
            ButtonAIcon.SetActive(highlightedSlot.IsItemHeld);
            ButtonAText.gameObject.SetActive(highlightedSlot.IsItemHeld);

            ButtonYText.text = "Shuffle";
            ButtonYIcon.SetActive(true);
            ButtonYText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Method to select the currently highlighted item.
        /// </summary>
        private void SelectItem()
        {
            selectAudioSource.Play();
            selectedSlot = highlightedSlot;
            selectedSlot.SelectItem();
            ItemSelectedText.text = selectedSlot.ItemName;

            ButtonAText.text = "Place";
            ButtonAIcon.SetActive(true);
            ButtonAText.gameObject.SetActive(true);

            ButtonYText.text = "Remove";
            ButtonYIcon.SetActive(true);
            ButtonYText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Method triggering random population of inventory.
        /// </summary>
        private void RandomizeItems()
        {
            shuffleAudioSource.Play();
            populateCoroutine = StartCoroutine(PopulateInventoryRandomly());
        }

        /// <summary>
        /// Method to remove currently selected item.
        /// </summary>
        private void RemoveSelectedItem()
        {
            removeAudioSource.Play();
            selectedSlot.UnselectItem();
            selectedSlot.PopulateItem(null, "");
            selectedSlot = null;
            ItemSelectedText.text = "";

            ButtonAText.text = "Select";
            ButtonAIcon.SetActive(highlightedSlot.IsItemHeld);
            ButtonAText.gameObject.SetActive(highlightedSlot.IsItemHeld);

            ButtonYText.text = "Shuffle";
            ButtonYIcon.SetActive(true);
            ButtonYText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Coroutine managing random inventory population.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PopulateInventoryRandomly()
        {
            if (nbRandomItem > itemHolderManagers.Count)
            {
                Debug.LogError("InventoryManager.PopulateInventoryRandomly: nbRandomItem is invalid.");
                yield return null;
            }

            List<Sprite> itemList = new List<Sprite>();

            AsyncOperationHandle<IList<Sprite>> handler = Addressables.LoadAssetsAsync<Sprite>(assetLabel, (x) => { });

            yield return handler;

            itemList = (List<Sprite>)handler.Result;

            HashSet<ItemSlotManager> itemHolderSet = new HashSet<ItemSlotManager>();

            while (itemHolderSet.Count < nbRandomItem)
            {
                itemHolderSet.Add(itemHolderManagers[Random.Range(0, itemHolderManagers.Height), Random.Range(0, itemHolderManagers.Width)]);
            }

            itemHolderManagers.ClearIcons();
            selectedSlot = null;

            foreach (var itemHolder in itemHolderSet)
            {
                Sprite item = itemList[Random.Range(0, itemList.Count)];

                itemHolder.PopulateItem(item, item.name.Replace('%', ' '));
            }

            ButtonAText.text = "Select";
            ButtonAIcon.SetActive(highlightedSlot.IsItemHeld);
            ButtonAText.gameObject.SetActive(highlightedSlot.IsItemHeld);

            ButtonYText.text = "Shuffle";
            ButtonYIcon.SetActive(true);
            ButtonYText.gameObject.SetActive(true);

            populateCoroutine = null;
        }
        #endregion
    }
}
