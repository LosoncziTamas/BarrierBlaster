using UnityEngine;

namespace ArBreakout.Game.Obstacles
{
    public abstract class Obstacle : MonoBehaviour
    {
        [SerializeField] protected GameEntities _gameEntities;

        private void Awake()
        {
            _gameEntities.Add(this);
        }

        private void OnDestroy()
        {
            _gameEntities.Remove(this);
        }
    }
}