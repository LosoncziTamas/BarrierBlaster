using ArBreakout.Misc;
using UnityEngine;

namespace ArBreakout.Game
{
    public class BrickNew : MonoBehaviour
    {
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private ColorPalette _colorPalette;

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
    }
}