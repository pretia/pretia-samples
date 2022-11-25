using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Pretia.Samples.Relocalization
{
    /// <summary>
    /// Will instantiate content from the Resources path when relocalization
    /// succeeds. Prefabs should be named using Map Keys of the maps associated 
    /// with your application, for which you want to display content.
    /// 
    /// This system is designed to work hand in hand with the 
    /// Content Authoring System, in order to align content, and use 
    /// as a basis for these prefabs: 
    /// https://docs.developers.pretiaar.com/tutorials/content-authoring/
    /// 
    /// This class also handles anchoring of the content after 
    /// instantiation, so that content will remain in place when 
    /// relocalizing to a different location.
    /// </summary>
    [RequireComponent(typeof(RelocManager))]
    public class MapContentDownloader : MonoBehaviour
    {
        private void OnEnable()
        {
            // Subscribe to OnMapRelocalized and OnScoreUpdated events.
            RelocManager.OnMapRelocalized += OnMapRelocalized;
        }

        private void OnDisable()
        {
            // Un-subscribe from OnMapRelocalized and OnScoreUpdated events.
            RelocManager.OnMapRelocalized -= OnMapRelocalized;
        }
        
        /// <summary>
        /// Root under which all content should be loaded.
        /// </summary>
        [Tooltip("Root under which all content should be loaded")]
        [SerializeField]
        private Transform contentRoot;
        
        /// <summary>
        /// Dictionary to keep track of the instantiated content.
        /// </summary>
        private readonly Dictionary<string, GameObject> _contentInstances =
            new Dictionary<string, GameObject>();
        
        // Registered method to ARSharedAnchorManager.
        private void OnMapRelocalized(string mapKey)
        {
            if (string.IsNullOrEmpty(mapKey)) return;
            StartCoroutine(LoadMapContent(mapKey));
        }
        
        /// <summary>
        /// Instantiates prefab, loaded Asynchronously from the resource path.
        /// </summary>
        /// <param name="mapKey">A prefab should be created in the resource
        /// path, named the same as the mapKey.</param>
        private IEnumerator LoadMapContent(string mapKey)
        {
            // Attempt to async load the prefab containing this maps content.
            var request = Resources.LoadAsync<GameObject>(mapKey);

            // Wait until loading is completed.
            yield return request;

            var prefab = request.asset as GameObject;
            if ((object)prefab != null)
            {
                // If we already had content for this map, then destroy the old
                // one and re-instantiate it.
                if (_contentInstances.ContainsKey(mapKey))
                    Destroy(_contentInstances[mapKey]);

                // If loaded successfully then instantiate the content.
                var contentObject = Instantiate(prefab, contentRoot);

                // Save a reference in case we need to clean it up later.
                _contentInstances[mapKey] = contentObject;

                // Add an anchor so that the content will stay in place even if
                // we relocalize to another map.
                contentObject.AddComponent<ARAnchor>();

                UpdateStatus("Loaded content for: " + mapKey);
            }
            else
            {
                // Prefab failed to load - ensure the prefab has been added
                // to a Resources directory!
                UpdateStatus("Error loading prefab: " + mapKey);
            }
        }
        
        /// <summary>
        /// Log an error and call OnStatusChanged callback to propagate 
        /// the message to any listeners.
        /// </summary>
        /// <param name="statusMessage">Message to be logged.</param>
        private void UpdateStatus(string statusMessage)
        {
            // Debug.Log(statusMessage);
        }
    }
}