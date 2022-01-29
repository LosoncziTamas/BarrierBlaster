using ArBreakout.Game;
using ArBreakout.GamePhysics;
using ArBreakout.Levels;
using ArBreakout.Misc;
using ArBreakout.PlaneDetection;
using ArBreakout.PowerUps;
using ArBreakout.Tutorial;
using Michsky.UI.ModernUIPack;
using Possible.AppController;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArBreakout.SinglePlayer
{
    public class GameAppState : AppState
    {
        private const int InitialLifeCount = 3;
        
        [SerializeField] private GameWorld _gameWorld;

        [SerializeField] private LifeCounter _lifeCounter;
        [SerializeField] private Text _timeLeftText;
        [SerializeField] private PointerDetector _leftButton;
        [SerializeField] private PointerDetector _rightButton;
        [SerializeField] private PointerDetector _fireButton;
        [SerializeField] private ModalWindowManager _gameOverModal;
        [SerializeField] private ModalWindowManager _levelCompleteModal;
        [SerializeField] private ModalWindowManager _allLevelCompleteModal;        
        [SerializeField] private TextMeshProUGUI _completionModalText;
        [SerializeField] private Button _backButton;
        [SerializeField] private PowerUpPanel _powerUpPanel;
        [SerializeField] private TutorialOverlay _tutorialOverlay;
        
        private GameObject _levelParent;
        private LevelLoader.ParsedLevel _currLevel;
        private LevelProgression _levelProgression;
        
        private int _totalLives;
        private float _timeLeftInSeconds;
        private int _brickCount;

        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);
            _levelProgression = LevelProgression.Instance;
            _levelParent = GameObject.Find("LevelParent");
            Assert.IsNotNull(_levelParent,"No level parent was found");

#if UNITY_EDITOR
            // View the level form the top in the editor.
            // var editorCam = Camera.main.transform;
            // editorCam.position = _levelParent.transform.position + Vector3.up * BreakoutPhysics.LevelSizeInMeter * 2.0f;
            // editorCam.localRotation =  Quaternion.Euler(new Vector3(90.0f, 0.0f, 0f));            
#endif
            _currLevel = LevelSelectorAppState.SelectedLevel.parsedLevel;
            Assert.IsNotNull(_currLevel, "No level selected.");
            InitializeLevel();
        }

        private void OnEnable()
        {
            Gap.BallHasLeftTheGameEvent += OnBallHasLeftTheGameEvent;
            BrickBehaviour.BrickDestroyedEvent += OnBrickDestroyedEvent;
            PaddleBehaviour.PowerUpStateChangeEvent += OnPowerUpStateChangeEvent;
            
            _gameOverModal.onCancel.AddListener(OnBackToMain);
            _gameOverModal.onConfirm.AddListener(OnRetry);
            _levelCompleteModal.onConfirm.AddListener(OnLevelComplete);
            _allLevelCompleteModal.onCancel.AddListener(OnBackToMain);
            _backButton.onClick.AddListener(OnPause);
        }
        
        private void OnDisable()
        {
            Gap.BallHasLeftTheGameEvent -= OnBallHasLeftTheGameEvent;
            BrickBehaviour.BrickDestroyedEvent -= OnBrickDestroyedEvent;
            PaddleBehaviour.PowerUpStateChangeEvent -= OnPowerUpStateChangeEvent;
            
            _gameOverModal.onCancel.RemoveListener(OnBackToMain);
            _gameOverModal.onConfirm.RemoveListener(OnRetry);
            _levelCompleteModal.onConfirm.RemoveListener(OnLevelComplete);
            _allLevelCompleteModal.onCancel.RemoveListener(OnBackToMain);
            _backButton.onClick.RemoveListener(OnPause);
            
            _powerUpPanel.SetVisibility(false);   
        }

        private void OnRetry()
        {            
            _gameWorld.SetupLevel(_currLevel);
            
            _timeLeftInSeconds = _currLevel.timeLimitInSeconds;
            _timeLeftText.text = GamePlayUtils.FormatTime(_timeLeftInSeconds);
            _totalLives = InitialLifeCount;
            _brickCount = _gameWorld.InitialBrickCount;
            _lifeCounter.UpdateLives(_totalLives);
        }

        private void OnPause()
        {
            _tutorialOverlay.Show(
                () => GameTime.paused = false,
                OnBackToMain);
            GameTime.paused = true;
        }
        
        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape))
            {
                if (!GameTime.paused)
                {
                    OnPause();
                }
            }
        }

        private void OnBackToMain()
        {
            GameTime.paused = false;
            _gameWorld.DestroySelf();
           Destroy(_levelParent);
           Controller.TransitionTo(typeof(LevelSelectorAppState));
           ARService.Instance.ResetAR();
        }

        private void OnLevelComplete()
        {
            SetupNextLevel();
            GameTime.paused = false;
        }

        public override void OnUpdate()
        {
            if (GameTime.paused)
            {
                return;
            }
            
            _timeLeftInSeconds -= GameTime.delta;

            if (_totalLives == 0 || _timeLeftInSeconds <= 0.0f)
            {
                _gameOverModal.OpenWindow();
                return;
            }

            _timeLeftText.text = GamePlayUtils.FormatTime(_timeLeftInSeconds);
            
            if (_leftButton.PointerDown || Input.GetAxis("Horizontal") < 0)
            {
                _gameWorld.Paddle.MoveLeft();
            }
            if (_rightButton.PointerDown || Input.GetAxis("Horizontal") > 0)
            {
                _gameWorld.Paddle.MoveRight();
            }
            if (_fireButton.PointerDown || Input.GetButton("Jump"))
            {
                _gameWorld.Paddle.Fire();
            }
        }

        private void InitializeLevel()
        {
            Assert.IsFalse(_gameWorld.Initialized);
#if UNITY_EDITOR
            if (_currLevel == null)
            {
                _currLevel = _levelProgression.GetDefaultLevel().parsedLevel;
            }
#endif
            _gameWorld.InitWithLevel(_levelParent.transform, _currLevel);

            _timeLeftInSeconds = _currLevel.timeLimitInSeconds;
            _timeLeftText.text = GamePlayUtils.FormatTime(_timeLeftInSeconds);
            _totalLives = InitialLifeCount;
            _brickCount = _gameWorld.InitialBrickCount;
            _lifeCounter.UpdateLives(_totalLives);
        }

        private void SetupNextLevel()
        {
            var allLevels = _levelProgression.Levels;
            
            Assert.IsTrue(_currLevel.LevelIndex + 1 < allLevels.Count);
            Assert.IsTrue(_totalLives > 0);

            _currLevel = allLevels[_currLevel.LevelIndex + 1].parsedLevel;
            _gameWorld.SetupLevel(_currLevel);
            
            _timeLeftInSeconds = _currLevel.timeLimitInSeconds;
            _timeLeftText.text = GamePlayUtils.FormatTime(_timeLeftInSeconds);
            _brickCount = _gameWorld.InitialBrickCount;
        }

        private void OnPowerUpStateChangeEvent(object sender, PaddleBehaviour.PowerUpState e)
        {
            _powerUpPanel.Refresh(e.ActivePowerUps, e.ActivePowerUpTimes);
        }
        
        private void OnBrickDestroyedEvent(object sender, BrickBehaviour.BrickDestroyedArgs e)
        {
            Assert.IsTrue(_brickCount > 0);
            
            _brickCount--;
            if (_brickCount == 0)
            {
                _levelProgression.UnlockNextLevel(_currLevel.LevelIndex);
                
                if (_currLevel.LevelIndex == _levelProgression.Levels.Count - 1)
                {
                    _allLevelCompleteModal.OpenWindow();
                }
                else
                {
                    _completionModalText.text = $"Time: {GamePlayUtils.FormatTime(_timeLeftInSeconds)}";
                    _levelCompleteModal.OpenWindow();
                }

                GamePlayUtils.AnchorBallToPaddle(_gameWorld.BallBehaviour, _gameWorld.Paddle);
                GameTime.paused = true;
            }
        }

        private void OnBallHasLeftTheGameEvent(object sender, Gap.BallHasLeftTheGameArgs e)
        {
            _lifeCounter.UpdateLives(--_totalLives);
            if (_totalLives > 0)
            {
                GamePlayUtils.AnchorBallToPaddle(_gameWorld.BallBehaviour, _gameWorld.Paddle);
            }
        }
    }
}