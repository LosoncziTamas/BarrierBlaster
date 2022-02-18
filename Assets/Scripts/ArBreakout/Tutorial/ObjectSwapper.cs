using System.Collections.Generic;
using ArBreakout.PowerUps;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Tutorial
{
    public class ObjectSwapper : MonoBehaviour
    {
        private readonly Dictionary<PowerUp, GameObject> _powerUpObjects = new Dictionary<PowerUp, GameObject>();

        [SerializeField] private GameObject _magnet;
        [SerializeField] private GameObject _arrow;
        [SerializeField] private GameObject _mushroom;
        [SerializeField] private GameObject _anvil;
        [SerializeField] private GameObject _scissor;
        [SerializeField] private GameObject _beer;

        private GameObject _visibleObject;
        private PowerUp _visiblePowerUp;
        private bool _canSwap = true;

        private void Awake()
        {
            _powerUpObjects.Add(PowerUp.Accelerator, _arrow);
            _powerUpObjects.Add(PowerUp.Magnet, _magnet);
            _powerUpObjects.Add(PowerUp.Magnifier, _mushroom);
            _powerUpObjects.Add(PowerUp.Decelerator, _anvil);
            _powerUpObjects.Add(PowerUp.Minifier, _scissor);
            _powerUpObjects.Add(PowerUp.ControlSwitch, _beer);
        }

        public void SwapToPowerUpObject(PowerUp powerUp, float rotationDegree)
        {
            if (_visibleObject != null && _visiblePowerUp == powerUp || _visiblePowerUp == powerUp || !_canSwap)
            {
                return;
            }

            _canSwap = false;

            var prevObject = _visibleObject;
            _visibleObject = _powerUpObjects[powerUp];
            if (prevObject)
            {
                prevObject.transform.DOPunchRotation(new Vector3(0.0f, rotationDegree, 0.0f), 0.6f, 1, 0.5f);
                prevObject.transform.DOScale(Vector3.zero, 0.6f)
                    .OnComplete(() => prevObject.gameObject.SetActive(false));
            }

            _visibleObject.SetActive(true);
            _visibleObject.transform.localScale = Vector3.zero;
            _visibleObject.transform.DOPunchRotation(new Vector3(0.0f, -rotationDegree, 0.0f), 0.6f, 1, 0.5f);
            _visibleObject.transform.DOScale(Vector3.one, 0.6f).OnComplete(() => _canSwap = true);
            _visiblePowerUp = powerUp;
        }
    }
}