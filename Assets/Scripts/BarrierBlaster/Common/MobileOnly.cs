using UnityEngine;

namespace BarrierBlaster.Common
{
    public class MobileOnly : MonoBehaviour
    {
        private void Awake()
        {
           gameObject.SetActive(Application.platform is RuntimePlatform.Android or RuntimePlatform.IPhonePlayer);
        }
    }
}