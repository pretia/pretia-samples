using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pretia.Samples.UI{

    [RequireComponent(typeof(Button))]
    public class ChangeSceneButton : MonoBehaviour
    {
        [SerializeField]
        private string sceneName;
        
        [SerializeField]
        [HideInInspector] 
        private Button button;

#if UNITY_EDITOR
        private void Reset()
        {
            button = GetComponent<Button>();
        }
#endif

        private void OnEnable()
        {
            button.onClick.AddListener(ChangeScene);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ChangeScene);
        }

        private void ChangeScene()
        {
            if (string.IsNullOrEmpty(sceneName)) { return; }
            SceneManager.LoadScene(sceneName);
        }
    }
}
