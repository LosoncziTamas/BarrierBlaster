using UnityEngine;

namespace ArBreakout.Misc
{
    [CreateAssetMenu]
    public class ColorPalette : ScriptableObject
    {
        public Color Color1;
        public Color Color2;
        public Color Color3;
        public Color Color4;
        public Color Color5;

        public Color[] Colors => new[] { Color1, Color2, Color3, Color4, Color5 };
    }
}