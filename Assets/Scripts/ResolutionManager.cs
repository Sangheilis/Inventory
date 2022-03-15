using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Resolution
{
    /// <summary>
    /// Struct representing a resolution.
    /// Serializable.
    /// </summary>
    [Serializable]
    public struct SerializableResolution
    {
        public int Width;
        public int Height;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ResolutionManager : MonoBehaviour
    {
        [SerializeField]
        private SerializableResolution[] resolutions;

        [SerializeField]
        private Text currentResolutionText;

        [SerializeField]
        private GameObject leftButtonIcon;

        [SerializeField]
        private Text leftButtonText;

        [SerializeField]
        private GameObject rightButtonIcon;

        [SerializeField]
        private Text rightButtonText;

        private int currentResolutionIndex;

        // Start is called before the first frame update
        void Start()
        {
            if (resolutions.Length > 1)
            {
                currentResolutionIndex = 0;
                Screen.SetResolution(resolutions[currentResolutionIndex].Width, resolutions[currentResolutionIndex].Height, FullScreenMode.FullScreenWindow);
                currentResolutionText.text = "Resolution: " + resolutions[currentResolutionIndex].Width + "x" + resolutions[currentResolutionIndex].Height;
                leftButtonIcon.SetActive(false);
                leftButtonText.text = "";

                rightButtonIcon.SetActive(true);
                rightButtonText.text = resolutions[currentResolutionIndex + 1].Width + "x" + resolutions[currentResolutionIndex + 1].Height;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.U) || Input.GetButtonDown("Debug Next"))
            {
                if(currentResolutionIndex < resolutions.Length - 1)
                {
                    ++currentResolutionIndex;
                    Screen.SetResolution(resolutions[currentResolutionIndex].Width, resolutions[currentResolutionIndex].Height, FullScreenMode.FullScreenWindow);
                    currentResolutionText.text = "Resolution: " + resolutions[currentResolutionIndex].Width + "x" + resolutions[currentResolutionIndex].Height;

                    leftButtonIcon.SetActive(true);
                    leftButtonText.text = resolutions[currentResolutionIndex - 1].Width + "x" + resolutions[currentResolutionIndex - 1].Height;

                    if(currentResolutionIndex < resolutions.Length - 1)
                    {
                        rightButtonIcon.SetActive(true);
                        rightButtonText.text = resolutions[currentResolutionIndex + 1].Width + "x" + resolutions[currentResolutionIndex + 1].Height;
                    }
                    else
                    {
                        rightButtonIcon.SetActive(false);
                        rightButtonText.text = "";
                    }
                }
            }

            if (Input.GetKey(KeyCode.D) || Input.GetButtonDown("Debug Previous"))
            {
                if (currentResolutionIndex > 0)
                {
                    --currentResolutionIndex;
                    Screen.SetResolution(resolutions[currentResolutionIndex].Width, resolutions[currentResolutionIndex].Height, FullScreenMode.FullScreenWindow);
                    currentResolutionText.text = "Resolution: " + resolutions[currentResolutionIndex].Width + "x" + resolutions[currentResolutionIndex].Height;

                    rightButtonIcon.SetActive(true);
                    rightButtonText.text = resolutions[currentResolutionIndex + 1].Width + "x" + resolutions[currentResolutionIndex + 1].Height;

                    if (currentResolutionIndex > 0)
                    {
                        leftButtonIcon.SetActive(true);
                        leftButtonText.text = resolutions[currentResolutionIndex - 1].Width + "x" + resolutions[currentResolutionIndex - 1].Height;
                    }
                    else
                    {
                        leftButtonIcon.SetActive(false);
                        leftButtonText.text = "";
                    }
                }
            }
        }
    }
}
