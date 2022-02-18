using ArBreakout.Misc;
using Possible.AppController;
using UnityEngine;
using System.Linq;
using ArBreakout.Levels;
using ArBreakout.Main;
using ArBreakout.PlaneDetection;
using Michsky.UI.ModernUIPack;
using Possible.NativePermissions.Managed;
using Permission = Possible.NativePermissions.Managed.Permission;

namespace ArBreakout.SinglePlayer
{
    public class LegacyLevelSelectorAppState : AppState
    {
        [SerializeField] private LevelSelector _levelSelector;
        [SerializeField] private NotificationManager _permissionNotification;
        [SerializeField] private CanvasGroup _canvas;

        private static Level _selectedLevel;
        public static Level SelectedLevel => _selectedLevel;

        private LevelProgression _levelProgression;

        protected override void Awake()
        {
            base.Awake();
            _levelProgression = LevelProgression.Instance;
            GameTime.paused = false;
            if (Application.platform == RuntimePlatform.Android)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            _canvas.SetVisibility(false);
        }

        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);
            _canvas.SetVisibility(true);
            ResetLevelSelector();
        }

        public override void OnExit(AppState toState)
        {
            _canvas.SetVisibility(false);
            base.OnExit(toState);
        }

        private void ResetLevelSelector()
        {
            var levelSelectorItems = _levelProgression.Levels
                .Select((info => new LevelSelector.ItemData(OnLevelSelected, info.displayedName, info.unlocked, info)))
                .ToList();
            _levelSelector.SetItems(levelSelectorItems);
        }

        private void OnEnable()
        {
            NativePermissions.OnRequestResult += OnPermissionRequestResult;
        }

        private void OnDisable()
        {
            NativePermissions.OnRequestResult -= OnPermissionRequestResult;
        }

        private void OnGUI()
        {
            GUILayout.Space(100);
            if (_levelProgression.LevelsCompleted)
            {
                if (GUILayout.Button("Clear level progress"))
                {
                    _levelProgression.ClearProgress();
                    ResetLevelSelector();
                }
            }
            else if (GUILayout.Button("Unlock all levels"))
            {
                _levelProgression.UnlockAllLevels();
                ResetLevelSelector();
            }
        }

        private void OnLevelSelected(Main.LevelSelector.ItemData level)
        {
            _selectedLevel = (Level) level.dataRef;
            if (NativePermissions.IsPermissionGranted(Permission.Camera))
            {
                Controller.TransitionTo(typeof(CheckARAvailabilityState));
            }
            else
            {
                NativePermissions.RequestPermission(Permission.Camera);
            }
        }

        private void OnPermissionRequestResult(object sender, RequestResultArgs requestResult)
        {
            if (requestResult.isGranted)
            {
                // TODO: fix this issue on Android
                if (Application.platform == RuntimePlatform.Android)
                {
                    _permissionNotification.title = "Camera permission granted.";
                    _permissionNotification.description = "Please restart the app for the changes to take effect.";
                    _permissionNotification.UpdateUI();
                    _permissionNotification.OpenNotification();
                }
                else
                {
                    Controller.TransitionTo(typeof(CheckARAvailabilityState));
                }
            }
            else
            {
                _permissionNotification.title = "Camera permission denied";
                _permissionNotification.description = "Please open the settings and enable camera usage.";
                _permissionNotification.UpdateUI();
                _permissionNotification.OpenNotification();
            }
        }
    }
}