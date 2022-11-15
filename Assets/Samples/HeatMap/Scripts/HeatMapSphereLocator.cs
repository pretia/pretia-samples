using UnityEngine;
using System;

namespace Pretia.Samples.HeatMap
{
    public class HeatMapSphereLocator : MonoBehaviour
    {

        [SerializeField] 
        private Transform cameraTransform;
        
        [SerializeField] 
        private float safeZoneRadius = 0.5f;
        
        [SerializeField]
        [Range(0.01f, 0.1f)] 
        private float speed = 0.03f;

        public event Action<float> OnSphereTranslate;

        private Transform _cachedTransform;
        
        private bool _dragging;

        private void Awake()
        {
            _cachedTransform = transform;
        }

        private void OnEnable()
        {
            _cachedTransform.position = cameraTransform.position;
        }

        private void LateUpdate()
        {
            var distanceToCamera = Vector3.Distance(
                _cachedTransform.position,
                cameraTransform.position
            );

            if (distanceToCamera >= safeZoneRadius)
            {
                _dragging = true;
            }

            if (_dragging)
            {
                FollowARCamera();
            }
        }

        private void FollowARCamera()
        {
            var position = _cachedTransform.position;
            var nextPosition = Vector3.Lerp(
                position, 
                cameraTransform.position, 
                speed
            );
            
            var translateDist = Vector3.Distance(position, nextPosition);
            position = nextPosition;
            _cachedTransform.position = position;

            OnSphereTranslate?.Invoke(translateDist);

            if (translateDist <= 0.005f)
            {
                _dragging = false;
            }
        }
    }
}