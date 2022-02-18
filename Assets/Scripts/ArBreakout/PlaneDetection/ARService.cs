using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ArBreakout.PlaneDetection
{
    public class ARService : MonoBehaviour
    {
        public static ARService Instance { get; private set; }

        [SerializeField] private Camera _arCamera;
        [SerializeField] private ARRaycastManager _raycastManager;
        [SerializeField] private ARPlaneManager _arPlaneManager;
        [SerializeField] private ARSession _arSession;
        [SerializeField] private ARPointCloudManager _pointCloudManager;

        private readonly Vector2 _screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
        private readonly List<ARRaycastHit> _hits = new List<ARRaycastHit>();
        private readonly List<ARPlane> _trackedPlanes = new List<ARPlane>();

        private ARPointCloudParticleVisualizer _pointCloud;
        private ARSessionState _lastKnownState;

        public ARSessionState LastKnownState => _lastKnownState;

        public Camera ARCamera => _arCamera;

        public int TrackedPlanesCount
        {
            get
            {
#if UNITY_EDITOR
                return 1;
#endif
                return _trackedPlanes.Count;
            }
        }

        public class TrackingStateEventArgs : EventArgs
        {
            public readonly ARSessionState newState;
            public readonly NotTrackingReason notTrackingReason;

            public TrackingStateEventArgs(ARSessionState newState, NotTrackingReason notTrackingReason)
            {
                this.newState = newState;
                this.notTrackingReason = notTrackingReason;
            }
        }

        public event EventHandler<TrackingStateEventArgs> ARStateChange;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnEnable()
        {
            ARSession.stateChanged += OnARStateChange;
            _arPlaneManager.planesChanged += OnPlanesChanged;
        }

        private void OnDisable()
        {
            ARSession.stateChanged -= OnARStateChange;
            _arPlaneManager.planesChanged -= OnPlanesChanged;
        }

        public bool HitTestFromScreenCenter(out Pose pose, out float distance)
        {
#if UNITY_EDITOR
            pose = new Pose(Vector3.zero, Quaternion.identity);
            distance = 1.0f;
            return true;
#endif
            pose = default;
            distance = default;

            if (_raycastManager.Raycast(_screenCenter, _hits, TrackableType.Planes))
            {
                pose = _hits[0].pose;
                distance = _hits[0].distance;
                return true;
            }

            return false;
        }

        public void TogglePointCloud(bool enable)
        {
            if (_pointCloud == null)
            {
                _pointCloud = _pointCloudManager.GetComponentInChildren<ARPointCloudParticleVisualizer>();
            }

            if (_pointCloud)
            {
                _pointCloud.enabled = enable;
            }
        }

        public void ResetAR()
        {
            _arSession.Reset();
            _trackedPlanes.Clear();
            TogglePointCloud(true);
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            var removedIDs = args.removed.Select(p => p.trackableId);
            _trackedPlanes.RemoveAll(plane => removedIDs.Contains(plane.trackableId));
            _trackedPlanes.AddRange(args.added);
            // We ignore updated planes, since we are only interested in the number of available planes.
        }

        private void OnARStateChange(ARSessionStateChangedEventArgs args)
        {
            PublishEvent(args.state, ARSession.notTrackingReason);
        }

        private void PublishEvent(ARSessionState newState, NotTrackingReason notTrackingReason)
        {
            Debug.Log($"last state {_lastKnownState} new state {newState} not tracking reason {notTrackingReason}");
            ARStateChange?.Invoke(this, new TrackingStateEventArgs(newState, notTrackingReason));
            _lastKnownState = newState;
        }
    }
}