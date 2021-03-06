using UnityEngine;

namespace BarrierBlaster.Game
{
    public class ChangeMeshColor : MonoBehaviour
    {
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");

        [SerializeField] private MeshRenderer _meshRenderer;

        private MaterialPropertyBlock _block;

        private void Awake()
        {
            _block = new MaterialPropertyBlock();
        }

        public void SetColor(Color color)
        {
            _meshRenderer.GetPropertyBlock(_block);
            _block.SetColor(ColorProperty, color);
            _meshRenderer.SetPropertyBlock(_block);
        }
        
        public void SetColorForMaterial(Color color, int materialIdx)
        {
            _meshRenderer.GetPropertyBlock(_block, materialIdx);
            _block.SetColor(ColorProperty, color);
            _meshRenderer.SetPropertyBlock(_block, materialIdx);
        }
    }
}