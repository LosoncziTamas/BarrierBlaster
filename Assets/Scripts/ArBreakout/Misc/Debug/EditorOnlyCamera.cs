using UnityEngine;

namespace ArBreakout
{
    [RequireComponent(typeof(Camera))]
    public class EditorOnlyCamera : MonoBehaviour
    {
        public Camera cameraRef;
        
        public static EditorOnlyCamera Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("No instance available.");
                }

                return _instance;
            }
            private set
            {
                if (_instance == null)
                {
                    _instance = value;
                }
                else
                {
                    Debug.LogError("Instance already set.");
                }
            }
        }

        private static EditorOnlyCamera _instance;
        
        private void Awake()
        {
            cameraRef = GetComponent<Camera>();
            if (Application.isEditor)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}
