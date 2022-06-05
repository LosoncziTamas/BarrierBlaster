using UnityEngine;

namespace BarrierBlaster.Common
{
    public static class GameTime
    {
        public static bool Paused = false;
        public static float FixedDelta => Paused ? 0 : Time.fixedDeltaTime;
        public static float Delta => Paused ? 0 : Time.deltaTime;
    }
}