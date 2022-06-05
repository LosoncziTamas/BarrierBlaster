using System.Collections.Generic;
using BarrierBlaster.Game.Ball;
using BarrierBlaster.Game.Bricks;
using BarrierBlaster.Game.Obstacles;
using BarrierBlaster.Game.Paddle;
using BarrierBlaster.Game.Stage;
using BarrierBlaster.PowerUps;
using UnityEngine;

namespace BarrierBlaster.Game
{
    [CreateAssetMenu]
    public class GameEntities : ScriptableObject
    {
        public List<BrickBehaviour> Bricks { get; } = new();
        public List<BallBehaviour> Balls { get; } = new();
        public List<WallBehaviour> Walls { get; } = new();
        public List<Collectable> Collectables { get; } = new();
        public Gap Gap { get; private set;}
        public PaddleBehaviour Paddle { get; private set;}
        public List<Obstacle> Obstacles { get; } = new();

        private int _entityCount;

        public void Add(BrickBehaviour brickBehaviour)
        {
            Bricks.Add(brickBehaviour);
            _entityCount++;
        }

        public void Add(BallBehaviour ballBehaviour)
        {
            Balls.Add(ballBehaviour);
            _entityCount++;
        }

        public void Add(WallBehaviour wallBehaviour)
        {
            Walls.Add(wallBehaviour);
            _entityCount++;
        }

        public void Add(Gap gap)
        {
            Debug.Assert(Gap == null);
            Gap = gap;
            _entityCount++;
        }
        
        public void Add(PaddleBehaviour paddleBehaviour)
        {
            Debug.Assert(Paddle == null);
            Paddle = paddleBehaviour;
            _entityCount++;
        }
        
        public void Add(Collectable collectable)
        {
            Collectables.Add(collectable);
            _entityCount++;
        }

        public void Add(Obstacle obstacle)
        {
            Obstacles.Add(obstacle);
            _entityCount++;
        }
        
        public void Remove(Obstacle obstacle)
        {
            if (Obstacles.Remove(obstacle))
            {
                _entityCount--;
            }
        }
        
        public void Remove(Collectable collectable)
        {
            Debug.Log($"[GameEntities] remove {collectable.name}");
            if (Collectables.Remove(collectable))
            {
                _entityCount--;
            }
        }

        public void Remove(Gap gap)
        {
            Debug.Log($"[GameEntities] remove {gap.name}");
            Debug.Assert(gap == Gap);
            Gap = null;
            _entityCount--;
        }

        public void Remove(PaddleBehaviour paddleBehaviour)
        {
            Debug.Log($"[GameEntities] remove {paddleBehaviour.name}");
            Debug.Assert(paddleBehaviour == Paddle);
            Paddle = null;
            _entityCount--;
        }
        
        public void Remove(WallBehaviour wallBehaviour)
        {
            Debug.Log($"[GameEntities] remove {wallBehaviour.name}");
            if (Walls.Remove(wallBehaviour))
            {
                _entityCount--;
            }
        }
        
        public void Remove(BallBehaviour ballBehaviour)
        {
            Debug.Log($"[GameEntities] remove {ballBehaviour.name}");
            if (Balls.Remove(ballBehaviour))
            {
                _entityCount--;
            }
        }
        
        public void Remove(BrickBehaviour brickBehaviour)
        {
            Debug.Log($"[GameEntities] remove {brickBehaviour.name}");
            if (Bricks.Remove(brickBehaviour))
            {
                _entityCount--;
            }
        }
    }
}