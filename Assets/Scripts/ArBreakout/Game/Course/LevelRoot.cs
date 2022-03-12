using System.Linq;
using ArBreakout.Levels;
using ArBreakout.Misc;
using UnityEngine;
using static ArBreakout.GamePhysics.BreakoutPhysics;

namespace ArBreakout.Game.Course
{
    public class LevelRoot : MonoBehaviour
    {
        public const string ObjectName = "Level Root";

        [SerializeField] private BallBehaviour _ballPrefab;
        [SerializeField] private GameObject _paddleParentPrefab;
        [SerializeField] private WallBehaviour _wallBehaviourPrefab;
        [SerializeField] private Gap _gapPrefab;
        [SerializeField] private ColorPalette _colorPalette;
        [SerializeField] private BrickPool _brickPool;
        [SerializeField] private GameEntities _gameEntities;
        
        private void Awake()
        {
            gameObject.name = ObjectName;
        }

        public void InitLevel(LevelData selected)
        {
            InitWallsAndGap();
            var paddle = InitPaddle();
            InitBall(paddle.transform);
            InitBricks(selected);
        }

        private void InitBricks(LevelData selected)
        {
            var idx = 0;
            foreach (var brickAttribute in selected.BrickAttributes)
            {
                ++idx;
                var brick = _brickPool.GetBrick();
                brick.gameObject.name = $"Brick [{idx}]";
                var brickTransform = brick.transform;
                brickTransform.SetParent(transform, false);
                brickTransform.localPosition = brickAttribute.Position;
                brickTransform.localRotation = brickAttribute.Rotation;
                // Scale of the brick is set with the animation.
                brick.Init(brickAttribute, selected.BrickAttributes.Count);
            }
        }

        public void ContinueWithLevel(LevelData levelData, bool reset)
        {
            foreach (var brick in _gameEntities.Bricks)
            {
                _brickPool.ReturnBrick(brick);
            }

            foreach (var collectable in _gameEntities.Collectables)
            {
                collectable.Destroy();
            }

            InitBricks(levelData);
            var ball = _gameEntities.Balls.First();
            GamePlayUtils.AnchorBallToPaddle(ball, _gameEntities.Paddle);
            if (reset)
            {
                _gameEntities.Paddle.ResetToDefaults();
            }
        }

        public void ClearLevel()
        {
            foreach (var brick in _gameEntities.Bricks)
            {
                _brickPool.ReturnBrick(brick);
            }
            
            foreach (var collectable in _gameEntities.Collectables)
            {
                collectable.Destroy();
            }

            // TODO: consider polling these as well
            foreach (var ball in _gameEntities.Balls)
            {
                Destroy(ball.gameObject);
            }
            
            foreach (var wall in _gameEntities.Walls)
            {
                Destroy(wall.gameObject);
            }
            
            Destroy(_gameEntities.Gap.gameObject);
            // Paddle has a parent GO.
            Destroy(_gameEntities.Paddle.transform.parent.gameObject);
        }
        
        private BallBehaviour InitBall(Transform paddleTransform)
        {
            var ballInstance = Instantiate(_ballPrefab, transform);
            GamePlayUtils.CenterAboveObject(ballInstance.transform, paddleTransform);
            ballInstance.transform.SetParent(paddleTransform.parent);

            return ballInstance;
        }

        private PaddleBehaviour InitPaddle()
        {
            var paddleParent = Instantiate(_paddleParentPrefab, transform);
            var playerInstance = paddleParent.GetComponentInChildren<PaddleBehaviour>();

            // Placing player at the bottom of the scene.
            var playerOffset = Vector3.forward * Mathf.Floor(LevelDimY * 0.5f) + Vector3.down * 0.5f;
            playerInstance.transform.Translate(playerInstance.transform.TransformVector(playerOffset), Space.Self);
            playerInstance.StoreCurrentPositionAsStartPosition();

            return playerInstance;
        }

        private void InitWallsAndGap()
        {
            Instantiate(_wallBehaviourPrefab, transform);
            Instantiate(_gapPrefab, transform);
        }
        
#if false
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
#endif
    }
}