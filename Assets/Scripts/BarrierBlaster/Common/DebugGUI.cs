#undef DEBUG
// #define DEBUG
#if DEBUG

using BarrierBlaster.Game.Scoring;
using UnityEngine;

namespace BarrierBlaster.Gui.Modal
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

namespace BarrierBlaster.Gui.Modal
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

namespace BarrierBlaster.PowerUps
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

            if (GUILayout.Button("Lock all"))
            {
                PlayerPrefs.DeleteAll();
            }
        }
    }
}

#endif