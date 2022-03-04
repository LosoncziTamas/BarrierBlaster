using System.Collections.Generic;
using ArBreakout.PowerUps;
using UnityEngine;

namespace ArBreakout.Game
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

        public void Add(BrickBehaviour brickBehaviour)
        {
            Bricks.Add(brickBehaviour);
        }

        public void Add(BallBehaviour ballBehaviour)
        {
            Balls.Add(ballBehaviour);
        }

        public void Add(WallBehaviour wallBehaviour)
        {
            Walls.Add(wallBehaviour);
        }

        public void Add(Gap gap)
        {
            Debug.Assert(Gap == null);
            Gap = gap;
        }
        
        public void Add(PaddleBehaviour paddleBehaviour)
        {
            Debug.Assert(Paddle == null);
            Paddle = paddleBehaviour;
        }
        
        public void Add(Collectable collectable)
        {
            Collectables.Add(collectable);
        }
        
        public void Remove(Collectable collectable)
        {
            Debug.Log($"[GameEntities] remove {collectable.name}");
            Collectables.Remove(collectable);
        }

        public void Remove(Gap gap)
        {
            Debug.Log($"[GameEntities] remove {gap.name}");
            Debug.Assert(gap == Gap);
            Gap = null;
        }

        public void Remove(PaddleBehaviour paddleBehaviour)
        {
            Debug.Log($"[GameEntities] remove {paddleBehaviour.name}");
            Debug.Assert(paddleBehaviour == Paddle);
            Paddle = null;
        }
        
        public void Remove(WallBehaviour wallBehaviour)
        {
            Debug.Log($"[GameEntities] remove {wallBehaviour.name}");
            Walls.Remove(wallBehaviour);
        }
        
        public void Remove(BallBehaviour ballBehaviour)
        {
            Debug.Log($"[GameEntities] remove {ballBehaviour.name}");
            Balls.Remove(ballBehaviour);
        }
        
        public void Remove(BrickBehaviour brickBehaviour)
        {
            Debug.Log($"[GameEntities] remove {brickBehaviour.name}");
            Bricks.Remove(brickBehaviour);
        }
    }
}