using System.Collections.Generic;
using ArBreakout.Misc;
using UnityEngine;

namespace ArBreakout.Game.Bricks
{
    public class BrickPool : MonoBehaviour
    {
        [SerializeField] private BrickBehaviour _brickPrefab;
        [SerializeField] private GameEvent _activeBricksCleared;

        private int _activeBrickCount;
        
        private readonly Stack<BrickBehaviour> _pooledBricks = new();

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
            _activeBrickCount++;
            return result;
        }

        public void ReturnBrick(BrickBehaviour toReturn, bool raiseEvent = false)
        {
            if (toReturn.Pool != null)
            {
                toReturn.CancelInvoke();
                toReturn.Pool = null;
                toReturn.gameObject.SetActive(false);
                toReturn.transform.SetParent(transform);
                _pooledBricks.Push(toReturn);
                _activeBrickCount--;
            }

            if (raiseEvent && _activeBrickCount == 0)
            {
                _activeBricksCleared.Raise();
            }
        }

        private void OnDestroy()
        {
            _pooledBricks.Clear();
            foreach (var item in _pooledBricks)
            {
                Destroy(item);
                _activeBrickCount--;
            }
        }
    }
}