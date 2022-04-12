using ArBreakout.Common;
using ArBreakout.Common.Variables;
using ArBreakout.Levels;
using UnityEngine;

namespace ArBreakout.Game.Scoring
{
    public class StagePerformanceTracker : MonoBehaviour
    {
        [SerializeField] private IntVariable _lifeCount;

        private int _startLifeCount;
        private float _startTime;
        private LevelData _levelData;

        public void BeginTracking(LevelData levelData)
        {
            _levelData = levelData;
            _startLifeCount = _lifeCount.Value;
            _startTime = Time.time;
        }

        public StagePerformance EndTracking()
        {
            var lifeDiff = _startLifeCount - _lifeCount.Value;
            // TODO: is paused state taken into account?
            var timeDiff = _startTime - Time.time;
            var starCount = lifeDiff switch
            {
                0 => 3,
                1 => 2,
                _ => 1
            };

            var lastStarCount = GetStarCountForStage(_levelData.Id);
            var newRecord = lastStarCount < starCount;
            
            var result = new StagePerformance
            {
                Stars = starCount,
                Time = timeDiff,
                NewRecord = newRecord
            };

            if (newRecord)
            {
                PlayerPrefs.SetString(_levelData.Id, JsonUtility.ToJson(result));
            }
            
            return result;
        }

        public static int GetStarCountForStage(string levelId) 
        {
            if (string.IsNullOrEmpty(levelId))
            {
                Debug.LogError("Empty level id.");
                return 0;
            }
            var jsonPerformance = PlayerPrefs.GetString(levelId, null);
            if (string.IsNullOrEmpty(jsonPerformance))
            {
                return 0;
            }
            var stagePerf = JsonUtility.FromJson<StagePerformance>(jsonPerformance);
            return stagePerf.Stars;
        }
    }
}