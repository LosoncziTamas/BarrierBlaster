using UnityEngine;

namespace ArBreakout.Game.Stage
{
    public class WallBehaviour : MonoBehaviour
    {
        public const string GameObjectTag = "Wall";
        
        [SerializeField] private GameEntities _gameEntities;

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