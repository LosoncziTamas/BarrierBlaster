using UnityEngine;
using UnityEngine.Serialization;

namespace ArBreakout.PowerUps
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PowerUpScriptableObject")]
    public class PowerUpDescriptor : ScriptableObject
    {
        public Collectable collectablePrefab;
        public PowerUp powerUp;
        public string letter;
        public Material insideMaterial;
        public Material outsideMaterial;
        public Color accentColor;
    }
}