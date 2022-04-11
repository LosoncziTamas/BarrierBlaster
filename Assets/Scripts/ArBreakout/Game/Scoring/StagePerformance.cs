using System;

namespace ArBreakout.Game.Scoring
{
    [Serializable]
    public class StagePerformance
    {
        public int Stars { get; set; }
        public bool NewRecord { get; set; }
        public float Time { get; set; }
        // TODO: add other metrics
    }
}