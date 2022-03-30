using ArBreakout.GamePhysics;
using ArBreakout.Misc;
using DG.Tweening;
using Michsky.UI.ModernUIPack;
using Possible.AppController;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using Plane = UnityEngine.Plane;
using Vector3 = UnityEngine.Vector3;

namespace ArBreakout.PlaneDetection
{
    public class PlaneDetectionAppState : AppState
    {
        private const string LevelParentName = "LevelParent";

#if UNITY_EDITOR
        private const float ScaleEasing = 0.02f;
        private const float AngularSpeed = 100.0f;
#else
        private const float ScaleEasing = 1.0f;
        private const float AngularSpeed = 200.0f;
#endif

        private struct HintState
        {
            public bool rotationAndScaleHintDisplayed;
            public bool movementHintDisplayed;
            public bool doubleTapHintDisplayed;
        }

        [SerializeField] private GameObject _levelParentPrefab;
        [SerializeField] private Button _backButton;
        [SerializeField] private PlaneDetectionHint _planeDetectionHint;
        [SerializeField] private CanvasGroup _buttonsOverlay;
        [SerializeField] private NotificationManager _worldTrackingNotification;
        [SerializeField] private LevelGhost _levelGhostPrefab;

        private ARService _arService;
        private TKAnyTouchRecognizer _touchRecognizer;
        private TKPinchRecognizer _pinchRecognizer;
        private TKTapRecognizer _tapRecognizer;
        private TKOneFingerRotationRecognizer _oneFingerRotationRecognizer;
        private Transform _levelParent;
        private HintState _hintState;
        private LevelGhost _levelGhost;
        private Plane _raycastPlane;

        private float _initialScale;
        private bool _planeDetected;

        protected override void Awake()
        {
            base.Awake();
            _arService = ARService.Instance;
            _arService.ARStateChange += OnARStateChange;
        }

        private void OnARStateChange(object sender, ARService.TrackingStateEventArgs args)
        {
            if (!_planeDetectionHint.GuideCompleted)
            {
                return;
            }

            var notTrackingReason = args.notTrackingReason;
            if (notTrackingReason != NotTrackingReason.None)
            {
                string title = default;
                string description = default;

                if (notTrackingReason == NotTrackingReason.ExcessiveMotion)
                {
                    title = "Unable to track the environment";
                    description = "Please hold your phone steady.";
                }
                else if (notTrackingReason == NotTrackingReason.InsufficientLight)
                {
                    title = "Unable to track the environment";
                    description = "Please find an place with sufficient light.";
                }
                else if (notTrackingReason == NotTrackingReason.InsufficientFeatures)
                {
                    title = "Unable to track the environment";
                    description = "Please make sure your environment is suitable for AR.";
                }
                else if (notTrackingReason == NotTrackingReason.Relocalizing)
                {
                    title = "Stabilizing";
                    description = "Please wait a few seconds until AR tracking recovers.";
                }

                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
                {
                    _worldTrackingNotification.title = title;
                    _worldTrackingNotification.description = description;
                    _worldTrackingNotification.UpdateUI();
                    _worldTrackingNotification.OpenNotification();
                }
            }
        }

        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);

            _levelParent = GetLevelParent().transform;

            _touchRecognizer = new TKAnyTouchRecognizer(new TKRect(0f, 0f, Screen.width, Screen.height));
            _pinchRecognizer = new TKPinchRecognizer();
            _oneFingerRotationRecognizer = new TKOneFingerRotationRecognizer();
            _tapRecognizer = new TKTapRecognizer {numberOfTapsRequired = 2};

            TouchKit.addGestureRecognizer(_tapRecognizer);
            // This has to be registered in order to capture double taps. 
            _tapRecognizer.gestureRecognizedEvent += OnDoubleTapRecognized;

            TouchKit.addGestureRecognizer(_oneFingerRotationRecognizer);
            TouchKit.addGestureRecognizer(_touchRecognizer);
            TouchKit.addGestureRecognizer(_pinchRecognizer);
            _backButton.onClick.AddListener(OnBackButtonClicked);

            if (!_planeDetectionHint.GuideCompleted)
            {
                _planeDetectionHint.StartGuide();
            }

            _buttonsOverlay.alpha = 0.0f;
            _planeDetected = false;
        }

        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape))
            {
                OnBackButtonClicked();
            }
        }

        private void OnDoubleTapRecognized(TKTapRecognizer recognizer)
        {
            UIMessageController.Instance.ClearMessages();
            // _levelGhost.SwapToLevelBase((() => { Controller.TransitionTo(typeof(GameAppState)); }));
        }

        private GameObject GetLevelParent()
        {
            var parent = GameObject.Find(LevelParentName);
            if (parent)
            {
                return parent;
            }

            parent = Instantiate(_levelParentPrefab, _arService.transform);
            parent.name = LevelParentName;
            return parent;
        }

        public override void OnExit(AppState toState)
        {
            base.OnExit(toState);
            TouchKit.removeGestureRecognizer(_oneFingerRotationRecognizer);
            TouchKit.removeGestureRecognizer(_touchRecognizer);
            TouchKit.removeGestureRecognizer(_pinchRecognizer);
            _tapRecognizer.gestureRecognizedEvent -= OnDoubleTapRecognized;
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        private void OnBackButtonClicked()
        {
            Destroy(_levelGhost.gameObject);
            UIMessageController.Instance.ClearMessages();
            // Controller.TransitionTo(typeof(LegacyLevelSelectorAppState));
            ARService.Instance.ResetAR();
        }

        public override void OnUpdate()
        {
            if (!_planeDetectionHint.GuideCompleted)
            {
                return;
            }

            if (!_planeDetected && _arService.TrackedPlanesCount > 0)
            {
                _planeDetected = PerformHitTest();
                if (_planeDetected)
                {
#if UNITY_IOS || UNITY_ANDROID
                    Handheld.Vibrate();
#endif
                    _planeDetectionHint.HideGuide();
                    _buttonsOverlay.DOFade(1.0f, 0.6f);
                    _arService.TogglePointCloud(false);
                    if (!_hintState.movementHintDisplayed)
                    {
                        _hintState.movementHintDisplayed = true;
                        UIMessageController.Instance.DisplayMessage("Select and move the level using your finger.");
                    }
                }
            }

            if (_planeDetected)
            {
                if (_pinchRecognizer.state == TKGestureRecognizerState.RecognizedAndStillRecognizing &&
                    !_levelGhost.IsDragging)
                {
                    DisplayGestureHint();
                    var newScale =
                        Mathf.Clamp(
                            _pinchRecognizer.deltaScale * ScaleEasing * Time.deltaTime +
                            _levelParent.localScale.x, _initialScale * 0.75f, _initialScale * 1.5f);
                    _levelParent.localScale = Vector3.one * newScale;
                }
                else if (_oneFingerRotationRecognizer.state == TKGestureRecognizerState.RecognizedAndStillRecognizing &&
                         !_levelGhost.IsDragging)
                {
                    DisplayGestureHint();
                    // TODO: improve rotation
                    var normalizedRotation = _oneFingerRotationRecognizer.deltaRotation < 0.0f ? -1.0f : 1.0f;
                    var rotationDelta = Mathf.Clamp(_oneFingerRotationRecognizer.deltaRotation, -1.0f, 1.0f) *
                                        Time.deltaTime * AngularSpeed;
                    _levelParent.Rotate(Vector3.up, _oneFingerRotationRecognizer.deltaRotation);
                }
                else if (_touchRecognizer.state == TKGestureRecognizerState.RecognizedAndStillRecognizing)
                {
                    DragObject();
                }
                else
                {
                    if (_levelGhost.IsDragging)
                    {
                        DisplayGestureHint();
                        _levelGhost.Release();
                    }
                }
            }
        }

        private void DisplayGestureHint()
        {
            if (!_hintState.rotationAndScaleHintDisplayed)
            {
                UIMessageController.Instance.DisplayMessage(
                    "You can also use single finger rotation and pinch to scale.", 4.0f);
                _hintState.rotationAndScaleHintDisplayed = true;
            }
            else if (!_hintState.doubleTapHintDisplayed)
            {
                UIMessageController.Instance.DisplayMessage("Use double tap to place the level.", 2.0f);
                _hintState.doubleTapHintDisplayed = true;
            }
        }

        private bool PerformHitTest()
        {
            if (_arService.HitTestFromScreenCenter(out var pose, out var distance))
            {
                // The distance from the raycast hit to the top of the view frustum
                var fromHitToFrustumTop = distance * Mathf.Tan(_arService.ARCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                var pixelToMeter = fromHitToFrustumTop / (Screen.height * 0.5f);

                // We want to make sure the initial scale of the placed object fits into the frustum
                var largestLevelDim = Mathf.Max(BreakoutPhysics.LevelDimX, BreakoutPhysics.LevelDimY);
                _initialScale = Screen.height * (1.0f / largestLevelDim) * pixelToMeter *
                                BreakoutPhysics.LevelSizeInMeter * 0.75f;

                _levelParent.localScale = new Vector3(_initialScale, _initialScale, _initialScale);
                _levelParent.position = pose.position;
                _levelParent.rotation = pose.rotation;

                _levelGhost = Instantiate(_levelGhostPrefab, _levelParent);

                // Create a plane aligned with the hit
                _raycastPlane = new Plane(_levelParent.up, pose.position);
                return true;
            }

            return false;
        }

        private void DragObject()
        {
            var ray = _arService.ARCamera.ScreenPointToRay(_touchRecognizer.touchLocation());
            if (!_levelGhost.IsDragging)
            {
                if (Physics.Raycast(ray, out var hitInfo))
                {
                    if (hitInfo.transform.gameObject.CompareTag("LevelGhost"))
                    {
                        _levelGhost.Drag();
                    }
                }
            }
            else if (_raycastPlane.Raycast(ray, out var enter))
            {
                var hitPoint = ray.GetPoint(enter);
                _levelParent.DOMove(hitPoint, 0.4f);
            }
        }

        private void OnDestroy()
        {
            _arService.ARStateChange -= OnARStateChange;
        }
    }
}