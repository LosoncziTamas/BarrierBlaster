using UnityEngine;

namespace ArBreakout.PowerUps
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PowerUpScriptableObject")]
    public class PowerUpScriptableObject : ScriptableObject
    {
        public Collectable collectablePrefab;
        public PowerUp powerUp;
        public Sprite icon;
        public string descriptionText;
        public Material material;
        public Color color;
    }
}