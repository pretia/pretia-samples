using PretiaArCloud;
using UnityEngine;

namespace Pretia.Samples.Relocalization
{

    /// <summary>
    /// 
    /// The Reloc Manager class handles OnMapRelocalized events on the 
    /// ARSharedAnchorManager.
    /// 
    /// </summary>
    public class RelocManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the ARSharedAnchorManager to listen to events on.
        /// </summary>
        [SerializeField]
        private ARSharedAnchorManager sharedAnchorManager;

        /// <summary>
        /// Event called when finished to relocalize.
        /// </summary>
        public static System.Action OnRelocalized { get; set; }
        
        /// <summary>
        /// Event called when finished to relocalize.
        /// </summary>
        public static System.Action<string> OnMapRelocalized { get; set; }
        
        /// <summary>
        /// Event for displaying relocalization score and content
        /// loading status.
        /// </summary>
        public static System.Action<string> OnStatusChanged { get; set; }
        
        /// <summary>
        /// Event for update score.
        /// </summary>
        public static System.Action<float> OnScoreUpdated { get; set; }
        
        /// <summary>
        /// Event called on exception detected.
        /// </summary>
        public static System.Action<System.Exception> OnException { get; set; }

        
        private void OnEnable()
        {
            // Subscribe to OnMapRelocalized and OnScoreUpdated events.
            sharedAnchorManager.OnMapRelocalized += InvokeOnMapRelocalized;
            sharedAnchorManager.OnScoreUpdated += InvokeOnScoreUpdated;
            sharedAnchorManager.OnSharedAnchorStateChanged += InvokeOnStateChange;
            sharedAnchorManager.OnRelocalized += InvokeOnRelocalized;
        }

        private void OnDisable()
        {
            // Un-subscribe from OnMapRelocalized and OnScoreUpdated events.
            sharedAnchorManager.OnMapRelocalized -= InvokeOnMapRelocalized;
            sharedAnchorManager.OnScoreUpdated -= InvokeOnScoreUpdated;
            sharedAnchorManager.OnSharedAnchorStateChanged -= InvokeOnStateChange;
            sharedAnchorManager.OnRelocalized -= InvokeOnRelocalized;
        }


        /// <summary>
        /// Cancel any existing relocalization attempts and start 
        /// relocalization process.
        /// </summary>
        public void StartRelocalization()
        {
            if (sharedAnchorManager.enabled == false)
            {
                sharedAnchorManager.enabled = true;
            }
            
            if (sharedAnchorManager.CurrentState != SharedAnchorState.Stopped)
            {
                sharedAnchorManager.ResetSharedAnchor();
            }

            sharedAnchorManager.StartCloudMapRelocalization();
        }

        public void StopRelocalization()
        {
            sharedAnchorManager.enabled = false;
        }
        
        // Registered method to ARSharedAnchorManager.
        private void InvokeOnRelocalized()
        {
            OnRelocalized?.Invoke();
        }
        
        // Registered method to ARSharedAnchorManager.
        private void InvokeOnMapRelocalized(string mapKey)
        {
            OnMapRelocalized?.Invoke(mapKey);
        }

        // Registered method to ARSharedAnchorManager.
        private void InvokeOnScoreUpdated(float score)
        {
            OnScoreUpdated?.Invoke(score);
        }
        
        // Called when the state changes.
        private void InvokeOnStateChange(SharedAnchorState newState)
        {
            OnStatusChanged?.Invoke(newState.ToString());
        }
    }
}