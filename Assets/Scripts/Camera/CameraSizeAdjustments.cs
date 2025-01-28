using System;
using UnityEngine;

namespace Camera
{
    public class CameraSizeAdjustments : MonoBehaviour
    {
        private const float RatioChangeThreshold = 0.01f;

        [SerializeField] private UnityEngine.Camera cam;
        [Header("How many world Unity units fit into the screen height")]
        [SerializeField] private float height = 5f;  // Desired world height
        private float _currRatio;

        private void Awake()
        {
            if (cam == null)
                cam = UnityEngine.Camera.main;

            if (!cam.orthographic)
                Debug.LogWarning("Camera is not orthographic, this script is designed for orthographic cameras");
        }

        private void Start()
        {
            _currRatio = (float)Screen.width / Screen.height;
            FitToHeight();
        }

        private void Update()
        {
            var newRatio = (float)Screen.width / Screen.height;
            if (Math.Abs(newRatio - _currRatio) > RatioChangeThreshold)
            {
                _currRatio = newRatio;
                FitToHeight();
            }
        }

        private void FitToHeight()
        {
            // Calculate the camera's current height (in world units)
            var currentHeight = cam.orthographicSize * 2f;
        
            // Calculate the ratio of the desired height to the current height
            var ratioChange = height / currentHeight;
        
            // Adjust the orthographic size to match the desired height
            cam.orthographicSize *= ratioChange;
        }
    }
}