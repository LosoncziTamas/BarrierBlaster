using System.Collections.Generic;
using UnityEngine;

namespace ArBreakout.Game
{
    [CreateAssetMenu]
    public class GameEntities : ScriptableObject
    {
        public List<BrickBehaviour> Bricks { get; } = new();
        public List<BallBehaviour> Balls { get; } = new();
        public List<WallBehaviour> Walls { get; } = new();
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

        public void Remove(Gap gap)
        {
            Debug.Assert(gap == Gap);
            Gap = null;
        }
        
        public void Add(PaddleBehaviour paddleBehaviour)
        {
            Debug.Assert(Paddle == null);
            Paddle = paddleBehaviour;
        }

        public void Remove(PaddleBehaviour paddleBehaviour)
        {
            Debug.Assert(paddleBehaviour == Paddle);
            Paddle = null;
        }
        
        public void Remove(WallBehaviour wallBehaviour)
        {
            Walls.Remove(wallBehaviour);
        }
        
        public void Remove(BallBehaviour ballBehaviour)
        {
            Balls.Remove(ballBehaviour);
        }
        
        public void Remove(BrickBehaviour brickBehaviour)
        {
            Bricks.Remove(brickBehaviour);
        }
    }
}