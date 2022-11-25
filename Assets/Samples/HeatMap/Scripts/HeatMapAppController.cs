using System.Collections;
using Pretia.Samples.Relocalization;
using UnityEngine;

namespace Pretia.Samples.HeatMap
{
    /// <summary>
    /// Manages general application flow.
    /// </summary>
    public class HeatMapAppController : MonoBehaviour
    {
        [SerializeField] 
        private HeatMapHUDPanel hudPanel;
        
        [SerializeField]
        private TutorialPanel tutorialPanel;
        
        [SerializeField]
        private RelocManager relocManager;

        [SerializeField] 
        private HeatMapSphereColorizer sphereColorizer;

        private Coroutine _currentCoroutine;
        
        public static System.Action OnReset { get; set; }

        private void Awake()
        {
            RelocManager.OnRelocalized += HideSphere;
            RelocManager.OnException += OnException;
        }
        
        private void Start()
        {
            if (!sphereColorizer.gameObject.activeInHierarchy)
            {
                sphereColorizer.gameObject.SetActive(true);
            }
            
            relocManager.StartRelocalization();
        }

        private void OnDestroy()
        {
            RelocManager.OnRelocalized -= HideSphere;
            RelocManager.OnException -= OnException;
        }
        
        public void UpdateScore(HeatMapMeasureMessageActivator activator) =>
            UpdateScore(activator.MaxActivationThreshold);
        
        public void UpdateScore(float score)
        {
            if (tutorialPanel.DisplayingTutorial)
            {
                hudPanel.UpdateScoreBarTarget(score);
            }
        }

        private void HideSphere()
        {
            _currentCoroutine = StartCoroutine(HideSphereCoroutine());
        }

        private IEnumerator HideSphereCoroutine()
        {
            yield return new WaitForSeconds(3);
            sphereColorizer.gameObject.SetActive(false);
            _currentCoroutine = null;
        }

        private void OnException(System.Exception exception)
        {
            Debug.Log(exception);
            ResetRelocalization();
        }
        
        public void ResetRelocalization()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
            
            sphereColorizer.InitializeVariables();
            sphereColorizer.gameObject.SetActive(true);
            UpdateScore(0);
            relocManager.StartRelocalization();
            OnReset?.Invoke();
        }
    }
}