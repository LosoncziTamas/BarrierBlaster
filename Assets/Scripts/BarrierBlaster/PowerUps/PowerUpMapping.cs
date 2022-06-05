using System;
using UnityEngine;

namespace BarrierBlaster.PowerUps
{
    public class PowerUpMapping : ScriptableObject
    {
        public PowerUpDescriptor[] mappings;

        public PowerUpDescriptor GetPowerUpDescriptor(PowerUp powerUp)
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