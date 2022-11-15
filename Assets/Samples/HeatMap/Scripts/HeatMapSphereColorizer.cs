using UnityEngine;
using System.Linq;
using Pretia.Samples.Relocalization;

namespace Pretia.Samples.HeatMap
{
    /// <summary>
    /// Implements vertex coloring on mesh object based on heatmap.
    /// </summary>
    [RequireComponent(typeof(HeatMapSphereLocator))]
    [RequireComponent(typeof(MeshFilter))]
    public class HeatMapSphereColorizer : MonoBehaviour
    {
        private const float DAMP_PER_FRAME = 0.0005f;
        private const float DAMP_PER_TRANSLATION = 0.5f;
    
        [SerializeField] 
        private Gradient colorGradient;

        [SerializeField] 
        [Range(0, 0.5f)] 
        private float effectAreaSize = 0.15f;

        [SerializeField] 
        private int weightAttenuation = 2;
        
        [SerializeField]
        [HideInInspector]
        private HeatMapSphereLocator sphereLocator;

        private Vector3 _hitPosition;
        private Camera _arCamera;
        private Mesh _mesh;
        private Vector3[] _normals;
        private Color32[] _colors;
        private float[] _weights;

        private float EffectAreaThreshold => 1 - effectAreaSize;

#if UNITY_EDITOR
        private void Reset()
        {
            sphereLocator = GetComponent<HeatMapSphereLocator>();
        }
#endif

        private void Awake()
        {
            _arCamera = Camera.main;
            _mesh = GetComponent<MeshFilter>().mesh;
        }

        private void Start()
        {
            InitializeVariables();
            UpdateColors();
        }

        private void OnEnable()
        {
            sphereLocator.OnSphereTranslate += DecreaseWeightsTranslate;
            RelocManager.OnScoreUpdated += UpdateWeights;
        }

        private void LateUpdate()
        {
            DecreaseWeightsFrame();
            UpdateColors();
        }

        private void OnDisable()
        {
            sphereLocator.OnSphereTranslate -= DecreaseWeightsTranslate;
            RelocManager.OnScoreUpdated -= UpdateWeights;
        }

        public void InitializeVariables()
        {
            _mesh.RecalculateNormals();
            _normals = _mesh.normals;
            _weights = Enumerable.Repeat(0f, _mesh.vertexCount).ToArray();
            _colors = new Color32[_mesh.vertexCount];
        }

        private void UpdateWeights(float score)
        {
            if (!GetLocalHitPosition(out _hitPosition))
            {
                return; //out of sphere
            }

            for (var i = 0; i < _weights.Length; i++)
            {
                var weight = -Vector3.Dot(_hitPosition.normalized, _normals[i]);

                if (!(weight >= EffectAreaThreshold)) continue;

                weight = Mathf.Pow(weight, weightAttenuation) * score;
                _weights[i] = Mathf.Max(_weights[i], weight);
            }
        }

        private bool GetLocalHitPosition(out Vector3 hitPos)
        {
            var screenCenter = new Vector2(
                (float)Screen.width / 2,
                (float)Screen.height / 2
            );

            var sphereCenter = transform.position;

            var ray = _arCamera.ScreenPointToRay(screenCenter);

            // Cast ray from the screen center to sphere and get hit point's
            // position in object space.
            if (Physics.Raycast(ray, out var hit))
            {
                hitPos = Quaternion.Inverse(transform.rotation) *
                         (hit.point - sphereCenter);

                return true;
            }

            // Hit
            hitPos = Vector3.zero;
            Debug.Log("device is out of the sphere");

            return false;
        }

        private void DecreaseWeightsFrame()
        {
            for (var i = 0; i < _weights.Length; i++)
            {
                _weights[i] *= 1 - DAMP_PER_FRAME;
            }
        }

        private void DecreaseWeightsTranslate(float translation)
        {
            for (var i = 0; i < _weights.Length; i++)
            {
                _weights[i] *= 1 - translation * DAMP_PER_TRANSLATION;
            }
        }

        // Update vertex colors according to weights.
        private void UpdateColors()
        {
            for (var i = 0; i < _mesh.vertexCount; i++)
            {
                _colors[i] = colorGradient.Evaluate(_weights[i]);
            }

            _mesh.colors32 = _colors;
        }
    }
}