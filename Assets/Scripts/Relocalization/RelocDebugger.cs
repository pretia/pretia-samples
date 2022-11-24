using System;
using UnityEngine;
using UnityEngine.Events;

 namespace Pretia.Samples.Relocalization
{
    /// <summary>
    /// Implements a panel for showing exception.
    /// </summary>
    public class RelocDebugger : MonoBehaviour
    {
        /// <summary>
        /// Event for displaying relocalization score and content
        /// loading status.
        /// </summary>
        [Tooltip("Event for displaying relocalization score and content " +
                 "loading status.")]
        [SerializeField]
        private UnityEvent<string> onStatusChanged;

        private void Awake()
        {
            RelocManager.OnStatusChanged += OnStatusChanged;
            RelocManager.OnScoreUpdated += OnScoreUpdated;
            RelocManager.OnMapRelocalized += OnMapRelocalized;
        }

        private void OnDestroy()
        {
            RelocManager.OnStatusChanged -= OnStatusChanged;
            RelocManager.OnScoreUpdated -= OnScoreUpdated;
            RelocManager.OnMapRelocalized -= OnMapRelocalized;
        }
        
        private void OnStatusChanged(string status)
        {
            UpdateStatus(status);
        }

        private void OnMapRelocalized(string mapKey)
        {
            UpdateStatus("Relocalization success: " + mapKey);
        }

        private void OnScoreUpdated(float score)
        {
            // Display the current reloc score, so the user knows if they are
            // scanning the correct part of the environment.
            UpdateStatus($"Relocalizing... ({(int)(score * 100.0f)}/100)");
        }

        // Log an error and call OnStatusChanged callback to propagate 
        // the message to any listeners.
        private void UpdateStatus(string statusMessage)
        {
            //Debug.Log(statusMessage);
            onStatusChanged.Invoke(statusMessage);
        }
    }
}