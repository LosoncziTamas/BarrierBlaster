using System;
using UnityEditor;
using UnityEngine;

namespace BarrierBlaster.Levels.Builder
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

                var obstacles = FindObjectsOfType<LevelBuilderObstacle>();
                _destLevel.ObstacleAttributes.Clear();
                foreach (var obstacle in obstacles)
                {
                    var attribute = obstacle.GetObstacleAttributes();
                    _destLevel.ObstacleAttributes.Add(attribute);
                }
#if UNITY_EDITOR
                EditorUtility.SetDirty(_destLevel);
                if (string.IsNullOrEmpty(_destLevel.Id))
                {
                    _destLevel.Id = Guid.NewGuid().ToString();
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }
        }
    }
}