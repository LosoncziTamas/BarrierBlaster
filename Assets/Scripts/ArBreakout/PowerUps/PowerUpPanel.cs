﻿using System.Collections.Generic;
using ArBreakout.Misc;
using UnityEngine;
using UnityEngine.Assertions;

namespace ArBreakout.PowerUps
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PowerUpPanel : MonoBehaviour
    {
        [SerializeField] private ItemPool _pool;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            SetVisibility(false);
        }

        private void Clear()
        {
            while (transform.childCount > 0)
            {
                var child = transform.GetChild(0);
                _pool.ReturnItem(child.gameObject);
            }   
        }

        public void SetVisibility(bool visible)
        {
            _canvasGroup.alpha = visible ? 1.0f : 0.0f;
        }

        public void Refresh(List<PowerUp> activePowerUps, List<float> activePowerUpTimes)
        {
            Assert.IsTrue(activePowerUps.Count == activePowerUpTimes.Count);
            
            Clear();
            SetVisibility(activePowerUps.Count > 0);
            
            for (var i = 0; i < activePowerUps.Count; i++)
            {
                var powerUp = activePowerUps[i];
                var powerUpTime = activePowerUpTimes[i];
                var data = PowerUpMappingScriptableObject.Instance.GetPowerUpSO(powerUp);
                var item = _pool.GetItem().GetComponent<PowerUpItem>();
                
                item.transform.SetParent(transform);
                item.transform.localScale = Vector3.one;
                item.Init(data.icon, powerUpTime);
            }
        }
    }
}
