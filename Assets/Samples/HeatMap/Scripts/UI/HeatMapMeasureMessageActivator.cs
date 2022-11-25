using Pretia.Samples.Relocalization;
using UnityEngine;

namespace Pretia.Samples.HeatMap
{
    public class HeatMapMeasureMessageActivator : MonoBehaviour
    {
        [SerializeField]
        private float minActivationThreshold;
        
        [SerializeField]
        private float maxActivationThreshold;

        [SerializeField]
        private TutorialMessage messageToActivate;

        public float MaxActivationThreshold => maxActivationThreshold;

        private void OnEnable()
        {
            RelocManager.OnScoreUpdated += OnTrackingScoreUpdate;
        }

        private void OnDisable()
        {
            RelocManager.OnScoreUpdated -= OnTrackingScoreUpdate;
        }
        
        private void OnTrackingScoreUpdate(float score)
        {
            if (score > minActivationThreshold &&
                score < maxActivationThreshold)
            {
                if (!messageToActivate.gameObject.activeSelf)
                {
                    messageToActivate.Show();
                }
            }
            else
            {
                if (messageToActivate.gameObject.activeSelf)
                {
                    messageToActivate.Hide();
                }
            }
        }
    }
}