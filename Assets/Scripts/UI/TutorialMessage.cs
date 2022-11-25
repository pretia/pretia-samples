using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Pretia.Samples.HeatMap
{
    /// <summary>
    /// Implements a tutorial message shown at the UI.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class TutorialMessage : MonoBehaviour
    {
        private const float TUTORIAL_MESSAGE_FADE_DURATION = 0.125f;
        
        public float readingTime = 3;

        [SerializeField] 
        private UnityEvent onEnable;
        
        [SerializeField] 
        private UnityEvent onDisable;

        [SerializeField]
        [HideInInspector]
        private CanvasGroup canvasGroup;

#if UNITY_EDITOR
        private void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
#endif

        public void Show(bool fade = true)
        {
            gameObject.SetActive(true);
            onEnable.Invoke();
            
            if (fade)
            {
                StartCoroutine(FadeCoroutine(
                    start: 0, 
                    end: 1, 
                    TUTORIAL_MESSAGE_FADE_DURATION)
                );
            }
        }
        
        public void Hide(bool fade = true)
        {
            if (fade)
            {
                StartCoroutine(FadeCoroutine(
                    start: 1, 
                    end: 0,
                    TUTORIAL_MESSAGE_FADE_DURATION,
                    () =>
                    {
                        gameObject.SetActive(false);
                        onDisable.Invoke();
                    })
                );
            }
            else
            {
                gameObject.SetActive(false);
                onDisable.Invoke();
            }
        }

        private IEnumerator FadeCoroutine(
            float start, float end, 
            float duration, 
            Action endAction = null)
        {
            for (float t = 0; t <= duration; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(start, end, t / duration);
                yield return null;
            }

            canvasGroup.alpha = end;
            endAction?.Invoke();
        }
    }
}
