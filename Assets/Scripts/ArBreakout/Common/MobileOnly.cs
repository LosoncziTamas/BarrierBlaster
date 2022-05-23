using UnityEngine;

namespace ArBreakout.Common
{
    public class MobileOnly : MonoBehaviour
    {
        private void Awake()
        {
           gameObject.SetActive(Application.isMobilePlatform);
        }
    }
}