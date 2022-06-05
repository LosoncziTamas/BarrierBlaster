using System;

namespace BarrierBlaster.Game.Scoring
{
    [Serializable]
    public class StagePerformance
    {
        public int Stars;
        public bool NewRecord;
        public float Time;
        // TODO: add other metrics
    }
}