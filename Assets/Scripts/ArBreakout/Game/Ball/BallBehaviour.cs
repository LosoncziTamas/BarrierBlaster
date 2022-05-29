using ArBreakout.Common;
using ArBreakout.Game.Bricks;
using ArBreakout.Game.Obstacles;
using ArBreakout.Game.Paddle;
using ArBreakout.Game.Stage;
using ArBreakout.GamePhysics;
using ArBreakout.PowerUps;
using DG.Tweening;
using UnityEngine;
using static System.Int32;

namespace ArBreakout.Game.Ball
{
    public class BallBehaviour : MonoBehaviour
    {
        public const string GameObjectTag = "Ball";

        private static readonly float NegativeMaxZ = Mathf.Sin(Mathf.Deg2Rad * -15.0f);
        private static readonly float PositiveMinZ = Mathf.Sin(Mathf.Deg2Rad * 15.0f);
        private static readonly float NegativeMaxX = Mathf.Cos(Mathf.Deg2Rad * 165.0f);
        private static readonly float PositiveMinX = Mathf.Cos(Mathf.Deg2Rad * 15.0f);

        [SerializeField] private BallProperties _ballProperties;
        [SerializeField] private Bobbing _bobbing;
        [SerializeField] private GameEntities _gameEntities;
        [SerializeField] private BobbingProperties _bobbingProperties;
        [SerializeField] private RotationProperties _rotationProperties;

        private Vector3 _localVelocity;
        private Vector3 _localAcceleration;
        private GameObject _gameWorldRoot;
        private PowerUpActivator _powerUpActivator;

        private bool _released;
        private bool _collidedInFrame;

        /*
         * Property for the velocity component. Y value is intentionally not set, so the ball doesn't bounce out of the level due to the physics calculations.
         */
        public Vector3 LocalVelocity
        {
            set
            {
                _localVelocity.x = value.x;
                _localVelocity.z = value.z;
            }
            get => _localVelocity;
        }
        
        public Vector3 LaunchDirection { get; set; } = Vector3.forward;
        
        public Vector3 DefaultScale { get; private set; }
        
        public bool IsMissed { get; set; }

        private void Awake()
        {
            _gameWorldRoot = GameObject.Find(LevelRoot.ObjectName);
            _gameEntities.Add(this);
            DefaultScale = transform.localScale;
            _powerUpActivator = FindObjectOfType<PowerUpActivator>();
        }

        private void OnDestroy()
        {
            _gameEntities.Remove(this);
        }
        
        public void Release(float additionalForce, Vector3 direction)
        {
            transform.SetParent(_gameWorldRoot.transform);
            _released = true;
            LocalVelocity = direction * (_ballProperties.DefaultSpeed + additionalForce);
            _bobbing.Disable();
            _rotationProperties.RotationValue = _rotationProperties.DefaultRotationValue * 2.0f;
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Launch);
        }

        public void ScaleUp()
        {
            transform.DOScale(Vector3.one * 1.5f, 0.6f);
            if (!_released)
            {
                var paddle = _gameEntities.Paddle;
                _bobbing.Disable();
                var target = paddle.transform.position.z + 1.5f + _bobbingProperties.startOffsetZ;
                transform.DOMoveZ(target, 0.6f).OnComplete(() =>
                {
                    if (!_released)
                    {
                        _bobbing.Enable(addOffset: false);
                    }
                });
            }
        }

        public void ScaleDown()
        {
            transform.DOScale(DefaultScale, 0.6f);
            if (!_released)
            {
                var paddle = _gameEntities.Paddle;
                _bobbing.Disable();
                var target = paddle.transform.position.z + DefaultScale.z + _bobbingProperties.startOffsetZ;
                transform.DOMoveZ(target, 0.6f).OnComplete(() =>
                {
                    if (!_released)
                    {
                        _bobbing.Enable(addOffset: false);
                    }
                });
            }
        }

        private void DrawContactLine(BreakoutPhysics.Contact contact, Color color)
        {
            Debug.DrawRay(contact.Point, contact.Normal, color, 2, false);
        }

        private void FixedUpdate()
        {
            ApplyMouseControl();
            
            if (!_released)
            {
                return;
            }

            _localAcceleration = LocalVelocity.normalized;
            _localAcceleration *= _ballProperties.DefaultSpeed;
            _localAcceleration += -_ballProperties.Drag * LocalVelocity;

            LocalVelocity += BreakoutPhysics.CalculateVelocityDelta(_localAcceleration);
            transform.localPosition += BreakoutPhysics.CalculateMovementDelta(_localAcceleration, LocalVelocity);
            _collidedInFrame = false;
        }
        
        private void ApplyMouseControl()
        {
            if (Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var info))
                {
                    var offset = info.point - transform.position;
                    Debug.DrawLine(transform.position, transform.position + offset);
                    ChangeDirection(offset);
                }            
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(WallBehaviour.GameObjectTag))
            {
                AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
                ResolveWallCollision(other);
            }
            else if (other.gameObject.CompareTag(PaddleBehaviour.GameObjectTag) && _released)
            {
                ResolvePaddleCollision(other);
            }
            else if (other.gameObject.CompareTag(BrickBehaviour.GameObjectTag) && !_collidedInFrame)
            {
                // It is possible that the ball collides with multiple objects (bricks) in a frame. But we want to resolve only one at a time.
                _collidedInFrame = true;
                ResolveBrickCollision(other);
            }
            else if (other.gameObject.CompareTag(HorizontalObstacle.Tag) && !_collidedInFrame)
            {
                AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
                _collidedInFrame = true;
                ReflectFromRectangularShape(other);
            }
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            var contact = BreakoutPhysics.ExtractContactPoint(collisionInfo);
            DrawContactLine(contact, Color.red);

            // Move ball out of other object, then fallback to default collision resolution.
            const float MinCorrection = 0.001f;
            transform.position += contact.Normal * Mathf.Max(MinCorrection, contact.Separation);
            OnCollisionEnter(collisionInfo);
        }

        private void ReflectFromRectangularShape(Collision collision)
        {
            var contact = BreakoutPhysics.ExtractContactPoint(collision);
            DrawContactLine(contact, Color.blue);
            Debug.DrawRay(transform.position, LocalVelocity.normalized, Color.cyan, 2, false);
            var normalInLocalSpace = transform.InverseTransformDirection(contact.Normal);
            var reflection = Vector3.Reflect(LocalVelocity, normalInLocalSpace);

            // If angle between velocity and reflection is very low, bouncing may seem unnatural in the game.            
            var angle = Vector3.Angle(LocalVelocity, reflection);
            Debug.Log(angle);
            if (angle < 30.0f)
            {
                LocalVelocity = contact.Normal * LocalVelocity.magnitude;
            }
            else
            {
                LocalVelocity = reflection;
            }
            Debug.DrawRay(transform.position, LocalVelocity.normalized, Color.magenta, 2, false);
        }

        private void ResolveWallCollision(Collision wallCollision)
        {
            var contact = BreakoutPhysics.ExtractContactPoint(wallCollision);

            DrawContactLine(contact, Color.blue);

            var reflection = Vector3.Reflect(LocalVelocity, transform.InverseTransformDirection(contact.Normal));
            var velocityNormal = reflection.normalized;

            // Take the average of the surface and the current velocity vector in case they are perpendicular.
            // Otherwise the ball would move the same direction, resulting in a tunnelling effect.
            if (reflection.Equals(LocalVelocity))
            {
                LocalVelocity = (LocalVelocity + transform.InverseTransformDirection(contact.Normal)) * 0.5f;
                Debug.DrawRay(transform.position, LocalVelocity.normalized, Color.magenta, 2, false);
                return;
            }

            // Correcting bounce off from wall
            if (velocityNormal.z < PositiveMinZ && velocityNormal.z > NegativeMaxZ)
            {
                var velocityLen = LocalVelocity.magnitude;
                var newVelocity = new Vector3
                {
                    x = velocityNormal.x < 0 ? NegativeMaxX : PositiveMinX,
                    y = 0,
                    z = velocityNormal.z < 0 ? NegativeMaxZ : PositiveMinZ
                };
                LocalVelocity = newVelocity * velocityLen;
                Debug.LogFormat("Correcting ball reflection from wall. Orig: {0} New: {1}", velocityNormal,
                    newVelocity);
            }
            else
            {
                LocalVelocity = reflection;
            }
        }

        private void ResolvePaddleCollision(Collision paddleCollision)
        {
            var newDir = transform.position - paddleCollision.gameObject.transform.position;
            ChangeDirection(newDir);

            var paddle = paddleCollision.gameObject.GetComponent<PaddleBehaviour>();
            if (!paddle)
            {
                return;
            }
            if (paddle.Magnetized)
            {
                GamePlayUtils.ApplyMagnet(this, paddle);
            }
            else
            {
                AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
            }
        }

        private void ResolveBrickCollision(Collision brickCollision)
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Hit);
            var amplified = transform.localScale != DefaultScale;
            var brick = brickCollision.gameObject.GetComponent<BrickBehaviour>();
            if (amplified)
            {
                brick.Smash(MaxValue);
                return;
            }

            ReflectFromRectangularShape(brickCollision);
            var hitTimes = transform.localScale != DefaultScale ? 2 : 1;
            brick.Smash(hitTimes);
        }

        private void ChangeDirection(Vector3 newDirInWorldSpace)
        {
            LocalVelocity = transform.InverseTransformDirection(newDirInWorldSpace.normalized) * LocalVelocity.magnitude;
        }

        /*
         * Reset the internal state of the ball to the default, ready-to-launch state.
         */
        public void ResetToDefaults()
        {
            _bobbing.Enable();
            _rotationProperties.RotationValue = _rotationProperties.DefaultRotationValue;
            _released = false;
            _localVelocity = new Vector3();
            _localAcceleration = new Vector3();
            LaunchDirection = _localVelocity.magnitude > 0.01 ? _localVelocity.normalized : Vector3.forward;
            IsMissed = false;
        }

        public void ResetPowerUpToDefaults()
        {
            _powerUpActivator.DeActivatePowerUp(PowerUp.Magnifier);
        }
    }
}