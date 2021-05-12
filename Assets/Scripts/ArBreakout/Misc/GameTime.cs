using UnityEngine;

namespace ArBreakout.Misc
{
    public static class GameTime
    {
        public static bool paused = false;
        
        public static float fixedDelta => paused ? 0 : Time.fixedDeltaTime;

        public static float delta => paused ? 0 : Time.deltaTime;
    }
}