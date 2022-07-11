using UnityEngine;

namespace BarrierBlaster.Common
{
    public class SetFps : MonoBehaviour
    {
        [SerializeField] private int _fps = 60;
        [SerializeField] private int _vSyncCount = 0;
        private void Awake()
        {
            Application.targetFrameRate = _fps;
            QualitySettings.vSyncCount = _vSyncCount;
        }
    }
}