using UnityEngine;

namespace BarrierBlaster.Common
{
    public class MobileOnly : MonoBehaviour
    {
        private void Awake()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            gameObject.SetActive(true);
#endif
        }
    }
}