using UnityEngine;

namespace ArBreakout.Levels.Builder
{
    public class SaveBrickAttributes : MonoBehaviour
    {
        [SerializeField] private LevelData _destLevel;

        private void OnGUI()
        {
            GUILayout.Space(100);
            if (GUILayout.Button("Save Brick Attributes"))
            {
                var bricks = FindObjectsOfType<LevelBuilderBrick>();
                _destLevel.BrickAttributes.Clear();
                foreach (var brick in bricks)
                {
                    var attribute = brick.GetBrickAttributes();
                    _destLevel.BrickAttributes.Add(attribute);
                }
            }
        }
    }
}