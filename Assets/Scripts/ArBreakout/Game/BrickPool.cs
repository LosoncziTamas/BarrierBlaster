using System.Collections.Generic;
using UnityEngine;

namespace ArBreakout.Game
{
    public class BrickPool : MonoBehaviour
    {
        [SerializeField] private BrickBehaviour _brickPrefab;
        
        private readonly Stack<BrickBehaviour> _pooledBricks = new Stack<BrickBehaviour>();

        public BrickBehaviour GetBrick()
        {
            BrickBehaviour result;
            
            if (_pooledBricks.Count > 0)
            {
                result = _pooledBricks.Pop();
            }
            else
            {
                result = Instantiate(_brickPrefab);
            }
            result.transform.SetParent(null);
            result.gameObject.SetActive(true);
            result.Pool = this;
            
            return result;
        }

        public void ReturnBrick(BrickBehaviour toReturn)
        {
            if (toReturn.Pool != null)
            {
                toReturn.Pool = null;
                toReturn.gameObject.SetActive(false);
                toReturn.transform.SetParent(transform);
                _pooledBricks.Push(toReturn);
            }
        }

        private void OnDestroy()
        {
            _pooledBricks.Clear();
            foreach (var item in _pooledBricks)
            {
                Destroy(item);
            }
        }
    }
}