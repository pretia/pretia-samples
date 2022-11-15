using Pretia.Samples.Relocalization;
using UnityEngine;
using UnityEngine.UI;

 namespace Pretia.Samples.HeatMap
{
    /// <summary>
    /// Implements Heads Up Display panel.
    /// </summary>
    public class HeatMapHUDPanel : MonoBehaviour
    {
        public const float SCORE_SMOOTHING = 0.125f;
        
        [SerializeField] 
        private Text statusLabel;
        
        [SerializeField] 
        private Image relocalizationScoreBarFill;
        
        private float _score;
        private float _scoreTarget;
        private float _scoreVelocity;

        private void OnEnable()
        {
            RelocManager.OnStatusChanged += UpdateStateLabel;
            RelocManager.OnScoreUpdated += UpdateScoreBarTarget;
            HeatMapAppController.OnReset += OnReset;
        }

        private void Update()
        {
            _score = Mathf.SmoothDamp(
                _score,
                _scoreTarget, 
                ref _scoreVelocity, 
                SCORE_SMOOTHING
            );
            
            relocalizationScoreBarFill.fillAmount = _score;
        }

        private void OnDisable()
        {
            RelocManager.OnStatusChanged -= UpdateStateLabel;
            RelocManager.OnScoreUpdated -= UpdateScoreBarTarget;
            HeatMapAppController.OnReset -= OnReset;
        }

        private void UpdateStateLabel(string state)
        {
            statusLabel.text = state;
        }

        public void UpdateScoreBarTarget(float score)
        {
            _scoreTarget = score;
        }
        
        private void OnReset()
        {
            UpdateStateLabel(string.Empty);
            UpdateScoreBarTarget(0);
        }
    }
}