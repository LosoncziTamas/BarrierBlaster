using DG.Tweening;
using Possible.AppController;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ArBreakout.PlaneDetection
{
    public class CheckARAvailabilityState : AppState
    {
        [SerializeField] private CanvasGroup _deviceNotSupported;
        [SerializeField] private GameObject _circularProgress;
        
        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);
            if (Application.isEditor || ARIsReady(ARService.Instance.LastKnownState))
            {
                Controller.TransitionTo(typeof(PlaneDetectionAppState));
            }
            else
            {
                ARService.Instance.ARStateChange += OnARStateChange;
            }
        }

        public override void OnExit(AppState toState)
        {
            base.OnExit(toState);
            ARService.Instance.ARStateChange -= OnARStateChange;
        }

        private void OnARStateChange(object sender, ARService.TrackingStateEventArgs args)
        {
            // AR session initialization automatically kicks off.
            // We'll need to check the state to determine if device is not supported.
            if (args.newState == ARSessionState.Unsupported)
            {
                _circularProgress.SetActive(false);
                _deviceNotSupported.DOFade(1.0f, 0.6f);
            }
            else if (args.newState == ARSessionState.Ready || args.newState == ARSessionState.SessionTracking || args.newState == ARSessionState.SessionInitializing)
            {
                Controller.TransitionTo(typeof(PlaneDetectionAppState));
            }
        }

        private static bool ARIsReady(ARSessionState state)
        {
            return state == ARSessionState.Ready || state == ARSessionState.SessionTracking || state == ARSessionState.SessionInitializing;
        }
    }
}