using System.Linq;
using ArBreakout.Levels;
using ArBreakout.Misc;
using UnityEngine;
using static ArBreakout.GamePhysics.BreakoutPhysics;

namespace ArBreakout.Game.Stage
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
                brickAttribute.RowIndex = idx;
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
            ballInstance.ResetToDefaults();
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
    }
}