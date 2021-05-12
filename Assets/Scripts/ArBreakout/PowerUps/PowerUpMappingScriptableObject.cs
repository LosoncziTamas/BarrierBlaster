using System;
using ArBreakout.Game;
using UnityEngine;
using UnityEngine.Assertions;

namespace ArBreakout.PowerUps
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PowerUpMappingScriptableObject")]
    public class PowerUpMappingScriptableObject : ScriptableObject
    {
        public static PowerUpMappingScriptableObject Instance
        {
            get
            {
                if (_instance == null)
                {
                    var mappings = Resources.LoadAll<PowerUpMappingScriptableObject>("ScriptableObjects/");
                    Assert.IsTrue(mappings.Length == 1);
                    _instance = mappings[0];
                }

                return _instance;
            }
        }

        private static PowerUpMappingScriptableObject _instance;
        
        public PowerUpScriptableObject[] mappings;

        public PowerUpScriptableObject GetPowerUpSO(PowerUp powerUp)
        {
            foreach (var item in mappings)
            {
                if (item.powerUp == powerUp)
                {
                    return item;
                }
            }
            
            throw new ArgumentException($"No registered mapping for {powerUp}.");
        }
    }
}