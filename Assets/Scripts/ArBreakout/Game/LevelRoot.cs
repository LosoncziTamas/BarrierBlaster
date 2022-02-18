using System.Collections.Generic;
using ArBreakout.Levels;
using ArBreakout.Misc;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;
using static ArBreakout.GamePhysics.BreakoutPhysics;

namespace ArBreakout.Game
{
    public class LevelRoot : MonoBehaviour
    {
        public const string ObjectName = "LevelRoot";

        [SerializeField] private BrickPool _brickPoolPrefab;
        [SerializeField] private BallBehaviour _ballPrefab;
        [SerializeField] private GameObject _paddleParentPrefab;
        [SerializeField] private WallBehaviour _wallBehaviourPrefab;
        [SerializeField] private Gap _gapPrefab;
        [SerializeField] private ColorPalette _colorPalette;

        private GameObject _gameWorldRoot;
        private BrickPool _brickPool;

        private readonly List<BrickBehaviour> _brickReferences = new List<BrickBehaviour>();

        public BallBehaviour BallBehaviour { get; private set; }
        public PaddleBehaviour Paddle { get; private set; }
        public int InitialBrickCount => _brickReferences.Count;

        public bool Initialized { get; private set; }

        public void InitWithLevel(Transform levelParent, LevelLoader.ParsedLevel level)
        {
            Assert.IsNull(_gameWorldRoot);

            _gameWorldRoot = new GameObject(ObjectName);
            _gameWorldRoot.transform.SetParent(levelParent);
            _gameWorldRoot.transform.localScale = Vector3.one;
            _gameWorldRoot.transform.localRotation = Quaternion.identity;
            _gameWorldRoot.transform.localPosition = Vector3.zero;

            _brickPool = Instantiate(_brickPoolPrefab, levelParent);
            _brickPool.gameObject.SetActive(false);

            InitWallsAndGap();
            Paddle = InitPaddle();
            BallBehaviour = InitBall(Paddle.transform);
            InitBricks(level);

            Initialized = true;
        }

        public void DestroySelf()
        {
            Assert.IsNotNull(_gameWorldRoot, "Trying to destroy a non-existing game world.");

            _brickReferences.Clear();
            Initialized = false;
            Paddle = null;
            BallBehaviour = null;

            Destroy(_gameWorldRoot);
            Destroy(_brickPool);
        }

        public void SetupLevel(LevelLoader.ParsedLevel level)
        {
            Assert.IsTrue(Initialized);
            Assert.IsNotNull(Paddle);
            Assert.IsNotNull(BallBehaviour);

            foreach (var brick in _brickReferences)
            {
                if (brick)
                {
                    _brickPool.ReturnBrick(brick);
                }
            }

            _brickReferences.Clear();
            InitBricks(level);
            Paddle.ResetToDefaults();
            GamePlayUtils.AnchorBallToPaddle(BallBehaviour, Paddle);
        }

        private void InitBricks(LevelLoader.ParsedLevel parsedLevel)
        {
            var count = parsedLevel.bricksProps.Count;
            var paletteColorCount = _colorPalette.Colors.Length;
            foreach (var bricksProp in parsedLevel.bricksProps)
            {
                var brick = _brickPool.GetBrick();
                brick.gameObject.name = $"Brick {bricksProp.Location}";
                var brickTransform = brick.transform;
                brickTransform.SetParent(_gameWorldRoot.transform, false);
                brickTransform.localPosition = bricksProp.Location;
                brickTransform.localRotation = Quaternion.identity;
                // Scale of the brick is set with the animation.
                var lineIndex = bricksProp.LineIdx;
                var color = _colorPalette.Colors[lineIndex % paletteColorCount];
                brick.Init(bricksProp, color, count);
                _brickReferences.Add(brick);
            }
        }

        private BallBehaviour InitBall(Transform paddleTransform)
        {
            var ballInstance = Instantiate(_ballPrefab, _gameWorldRoot.transform);
            GamePlayUtils.CenterAboveObject(ballInstance.transform, paddleTransform);
            ballInstance.transform.SetParent(paddleTransform.parent);

            return ballInstance;
        }

        private PaddleBehaviour InitPaddle()
        {
            var paddleParent = Instantiate(_paddleParentPrefab, _gameWorldRoot.transform);
            var playerInstance = paddleParent.GetComponentInChildren<PaddleBehaviour>();

            // Placing player at the bottom of the scene.
            var playerOffset = Vector3.forward * Mathf.Floor(LevelDimY * 0.5f) + Vector3.down * 0.5f;
            playerInstance.transform.Translate(playerInstance.transform.TransformVector(playerOffset), Space.Self);
            playerInstance.StoreCurrentPositionAsStartPosition();

            return playerInstance;
        }

        private void InitWallsAndGap()
        {
            var wall = Instantiate(_wallBehaviourPrefab, _gameWorldRoot.transform);
            var gap = Instantiate(_gapPrefab, _gameWorldRoot.transform);
        }
    }
}