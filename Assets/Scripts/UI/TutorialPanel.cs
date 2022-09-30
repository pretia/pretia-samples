using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pretia.Samples.HeatMap
{
    /// <summary>
    /// Manages visibility for tutorial messages.
    /// </summary>
    public class TutorialPanel : MonoBehaviour
    {
        
        [SerializeField] 
        private Button skipButton;
        
        [SerializeField] 
        private TutorialMessage[] tutorialMessages;

        [SerializeField] 
        private UnityEvent onEnable;
        
        [SerializeField] 
        private UnityEvent onDisable;

        private bool _shouldStop;
        
        public bool DisplayingTutorial { get; private set; }

        private void Awake()
        {
            onDisable.Invoke();
            HideAllTutorialMessages();
        }

        private void OnEnable()
        {
            if (skipButton)
            {
                skipButton.onClick.AddListener(StopTutorial);
            }
        }

        private void OnDisable()
        {
            if (skipButton)
            {
                skipButton.onClick.RemoveListener(StopTutorial);
            }
        }

        public void ShowTutorial()
        {
            StartCoroutine(ShowTutorialCoroutine());
        }

        private IEnumerator ShowTutorialCoroutine()
        {
            DisplayingTutorial = true;
            onEnable.Invoke();
            _shouldStop = false;
            
            yield return new WaitForSeconds(0.5f);

            for (var i = 0; i < tutorialMessages.Length; i++)
            {
                var message = tutorialMessages[i];
                message.Show();

                var waitingTime = message.readingTime;
                while (waitingTime > 0 && !_shouldStop)
                {
                    yield return null;
                    waitingTime -= Time.deltaTime;
                }

                if (message.isActiveAndEnabled)
                {
                    message.Hide();
                }
                
                if (_shouldStop)
                {
                    break;
                }
            }

            onDisable.Invoke();
            
            HideAllTutorialMessages();
            DisplayingTutorial = false;
        }

        private void StopTutorial()
        {
            _shouldStop = true;
        }

        private void HideAllTutorialMessages()
        {
            for (var i = 0; i < tutorialMessages.Length; i++)
            {
                tutorialMessages[i].Hide(false);
            }
        }

        private void OnReset()
        {
            StopAllCoroutines();
            HideAllTutorialMessages();
        }
    }
}