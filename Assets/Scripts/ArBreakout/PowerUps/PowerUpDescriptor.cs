using UnityEngine;

namespace ArBreakout.PowerUps
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PowerUpScriptableObject")]
    public class PowerUpDescriptor : ScriptableObject
    {
        public Collectable collectablePrefab;
        public PowerUp powerUp;
        public string letter;
        public Color insideColor;
        public Color outsideColor;
    }
}