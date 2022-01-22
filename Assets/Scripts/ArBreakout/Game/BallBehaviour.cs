using ArBreakout.GamePhysics;
using ArBreakout.Misc;
using ArBreakout.PowerUps;
using UnityEngine;

namespace ArBreakout.Game
{
    [RequireComponent(typeof(Bobbing))]
    [RequireComponent(typeof(MeshRenderer))]
    public class BallBehaviour : MonoBehaviour
    {
        public const string GameObjectTag = "Ball";

        private const float DefaultSpeed = 18.0f;
        private const float Drag = 2.0f;
        
        private static readonly float NegativeMaxZ = Mathf.Sin(Mathf.Deg2Rad * -15.0f);
        private static readonly float PositiveMinZ = Mathf.Sin(Mathf.Deg2Rad *  15.0f);
        private static readonly float NegativeMaxX = Mathf.Cos(Mathf.Deg2Rad *  165.0f);
        private static readonly float PositiveMinX = Mathf.Cos(Mathf.Deg2Rad *  15.0f);

        [SerializeField] private Color _defaultColor;
        
        private Vector3 _launchLocalDirection = Vector3.forward;
        private Vector3 _localVelocity;
        private Vector3 _localAcceleration;
        private Bobbing _bobbing;
        private MeshRenderer _renderer;
        private GameObject _gameWorldRoot;

        private bool _released;
        private bool _collidedWithBrickInFrame;
        private static readonly int ColorPropertyID = Shader.PropertyToID("_Color");

        /*
         * Property for the velocity component. Y value is intentionally not set, so the ball doesn't bounce out of the level due to the physics calculations.
         */
        private Vector3 LocalVelocity
        {
            set
            {
                _localVelocity.x = value.x;
                _localVelocity.z = value.z;
            }
            get => _localVelocity;
        }

        private void Awake()
        {
            _bobbing = GetComponent<Bobbing>();
            _renderer = GetComponent<MeshRenderer>();
            _gameWorldRoot = GameObject.Find(GameWorld.WorldRootName);
        }

        private void Start()
        {
            _bobbing.Enable();
        }

        public void Release(float additionalForce)
        {
            transform.SetParent(_gameWorldRoot.transform);
            _released = true;
            LocalVelocity = _launchLocalDirection.normalized * (DefaultSpeed + additionalForce);
            _bobbing.Disable();
        }
        
        public void ResetPowerUpToDefaults()
        {
            _renderer.material.SetColor(ColorPropertyID, _defaultColor);
        }

        private void ActivatePowerUp(BrickBehaviour smashedBrick)
        {
            var powerUp = smashedBrick.PowerUp;
            if (powerUp.EffectsPaddle())
            {
                var powerUpSO = PowerUpMappingScriptableObject.Instance.GetPowerUpSO(powerUp);
                _renderer.material.SetColor(ColorPropertyID, powerUpSO.color);
            }
        }
        
        private void DrawContactLine(BreakoutPhysics.Contact contact, Color color)
        {
            Debug.DrawRay(contact.Point, contact.Normal, color, 2, false);
        }

        private bool _mouseControlEnabled = false;
        
        private void OnGUI()
        {
            if (!_mouseControlEnabled)
            {
                if (GUILayout.Button("Enable ball control"))
                {
                    _mouseControlEnabled = true;
                }
            }
            else
            {
                if (GUILayout.Button("Disable ball control"))
                {
                    _mouseControlEnabled = false;
                }
            }
        }

        private void ApplyMouseControl()
        {
            if (!_mouseControlEnabled)
            {
                return;
            }
            
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
        
        private void FixedUpdate()
        {
            ApplyMouseControl();
            
            if (!_released)
            {
                return;
            }
            
            _localAcceleration = LocalVelocity.normalized;
            _localAcceleration *= DefaultSpeed;
            _localAcceleration += -Drag * LocalVelocity;

            LocalVelocity += BreakoutPhysics.CalculateVelocityDelta(_localAcceleration);
            transform.localPosition += BreakoutPhysics.CalculateMovementDelta(_localAcceleration, LocalVelocity);
            _collidedWithBrickInFrame = false;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(WallBehaviour.GameObjectTag))
            {
                ResolveWallCollision(other);
            }           
            else if (other.gameObject.CompareTag(PaddleBehaviour.GameObjectTag) && _released)
            {
                ResolvePaddleCollision(other);
            }
            else if (other.gameObject.CompareTag(BrickBehaviour.GameObjectTag) && !_collidedWithBrickInFrame)
            {
                // It is possible that the ball collides with multiple objects (bricks) in a frame. But we want to resolve only one at a time.
                _collidedWithBrickInFrame = true;
                ResolveBrickCollision(other);
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
                Debug.LogFormat("Correcting ball reflection from wall. Orig: {0} New: {1}", velocityNormal, newVelocity);
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
            
            if (paddle.Magnetized)
            {
                GamePlayUtils.ApplyMagnet(this, paddle);
            }
        }

        private void ResolveBrickCollision(Collision brickCollision)
        {
            var contact = BreakoutPhysics.ExtractContactPoint(brickCollision);
            
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
            
            var brick = brickCollision.gameObject.GetComponent<BrickBehaviour>();
            brick.Smash();
            ActivatePowerUp(brick);
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
            _released = false;
            _launchLocalDirection = Vector3.forward;
            _localVelocity = new Vector3();
            _localAcceleration = new Vector3();
        }
    }
}