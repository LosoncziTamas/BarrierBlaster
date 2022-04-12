using System.Collections.Generic;
using UnityEngine;

namespace ArBreakout.Common
{
    public class ItemPool : MonoBehaviour
    {
        [SerializeField] private GameObject _itemPrefab;

        private readonly Stack<GameObject> _pooledObjects = new Stack<GameObject>();

        public GameObject GetItem()
        {
            GameObject result;

            if (_pooledObjects.Count > 0)
            {
                result = _pooledObjects.Pop();
            }
            else
            {
                result = Instantiate(_itemPrefab);
            }

            result.transform.SetParent(null);
            result.gameObject.SetActive(true);

            return result;
        }

        public void ReturnItem(GameObject toReturn)
        {
            toReturn.gameObject.SetActive(false);
            toReturn.transform.SetParent(transform);
            _pooledObjects.Push(toReturn);
        }

        public void Clear()
        {
            foreach (var item in _pooledObjects)
            {
                Destroy(item);
            }

            _pooledObjects.Clear();
        }
    }
}