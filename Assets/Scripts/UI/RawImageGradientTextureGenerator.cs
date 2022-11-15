using UnityEngine;
using UnityEngine.UI;

namespace Pretia.Samples.UI
{

    /// <summary>
    /// Generates a Texture based on a gradient and applies it to the
    /// RawImage component.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class RawImageGradientTextureGenerator : MonoBehaviour
    {

        [SerializeField] 
        private Gradient colorGradient;

        [SerializeField] 
        private int width = 1;

        [SerializeField] 
        private int height = 256;

        [SerializeField] 
        [HideInInspector] 
        private RawImage rawImage;

        private Texture2D _gradientTexture;

#if UNITY_EDITOR
        private void Reset()
        {
            rawImage = GetComponent<RawImage>();
        }
#endif

        private void OnEnable()
        {
            if (_gradientTexture) return;
            _gradientTexture =
                new Texture2D(width, height, TextureFormat.ARGB32, false);
            FillTextureWithGradient(_gradientTexture);
            rawImage.texture = _gradientTexture;
        }

        private void FillTextureWithGradient(Texture2D texture)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y,
                        colorGradient.Evaluate((float)y / height));
                }
            }

            texture.Apply();
        }
    }
}