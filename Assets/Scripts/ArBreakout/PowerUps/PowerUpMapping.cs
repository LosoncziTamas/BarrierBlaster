using System;
using ArBreakout.Game;
using UnityEngine;
using UnityEngine.Assertions;

namespace ArBreakout.PowerUps
{
    public class PowerUpMapping : ScriptableObject
    {
        public PowerUpScriptableObject[] mappings;

        public PowerUpScriptableObject GetPowerUpDescriptor(PowerUp powerUp)
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