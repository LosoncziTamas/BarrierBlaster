#define DEBUG
#if false

using ArBreakout.Game.Scoring;
using UnityEngine;

namespace ArBreakout.Gui.Modal
{
    public partial class PauseModal
    {
        private async void OnGUI()
        {
            if (GUILayout.Button("Show Pause"))
            {
                await Show("I");
            }
        }
    }
}

namespace ArBreakout.Gui.Modal
{
    public partial class LevelCompleteModal
    {
        private async void OnGUI()
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Show Level Complete"))
            {
                await Show("I", new StagePerformance {Stars = 3});
            }
        }
    }
}

namespace ArBreakout.PowerUps
{
    public partial class PowerUpActivator
    {
        private void OnGUI()
        {
            GUILayout.Space(40);
            if (GUILayout.Button("Magnetize"))
            {
                MagnetizePaddle();
            }

            if (GUILayout.Button("Spawn"))
            {
                SpawnBall();
            }

            if (GUILayout.Button("Scale"))
            {
                ScaleUpBall();
            }
            
            if (GUILayout.Button("Laser Beam"))
            {
                ActivateLaserBeam();
            }
        }
    }
}

#endif