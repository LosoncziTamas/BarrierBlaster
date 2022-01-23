using System.Collections.Generic;
using System.Linq;
using Possible;
using UnityEngine;

namespace ArBreakout.Levels
{
    public class Level
    {
        public readonly LevelLoader.ParsedLevel parsedLevel;
        public readonly string displayedName;
        
        public bool completed;
        public bool unlocked;

        public Level(bool completed, bool unlocked, LevelLoader.ParsedLevel parsedLevel)
        {
            this.completed = completed;
            this.unlocked = unlocked;
            this.parsedLevel = parsedLevel;
            // {parsedLevel.levelIndex + 1} 
            displayedName = $"{parsedLevel.LevelName}";
        }
    }

    public class LevelProgression : SingletonBehaviour<LevelProgression>
    {
        private const string LevelIndexKey = "level_index_key";
        
        private int _unlockedLevelIndex;

        public List<Level> Levels { get; private set; }

        public bool LevelsCompleted
        {
            get
            {
                var last = Levels.Last();
                return last.parsedLevel.LevelIndex == _unlockedLevelIndex && last.completed;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _unlockedLevelIndex = PlayerPrefs.HasKey(LevelIndexKey) ? PlayerPrefs.GetInt(LevelIndexKey) : 0;
            Levels = new List<Level>();
            var parsedLevels = LevelLoader.LoadLevels();
            foreach (var level in parsedLevels)
            {
                var completed = level.LevelIndex < _unlockedLevelIndex;
                var unlocked = level.LevelIndex <= _unlockedLevelIndex;
                Levels.Add(new Level(completed, unlocked, level));
            }
        }
                
        private void OnApplicationQuit() 
        {
            PlayerPrefs.SetInt(LevelIndexKey, _unlockedLevelIndex);      
        }
        
        public void UnlockNextLevel(int levelIndex)
        {
            if (levelIndex == _unlockedLevelIndex)
            {
                Levels[_unlockedLevelIndex].completed = true;
                if (_unlockedLevelIndex < Levels.Count - 1)
                {
                    Levels[++_unlockedLevelIndex].unlocked = true;
                }
            }
        }

        public void UnlockAllLevels()
        {
            for (var i = 0; i < Levels.Count; i++)
            {
                Levels[i].completed = true;
                Levels[i].unlocked = true;
            }
            _unlockedLevelIndex = Levels.Count - 1;
            PlayerPrefs.SetInt(LevelIndexKey, _unlockedLevelIndex);
        }

        public void ClearProgress()
        {
            for (var i = 0; i < Levels.Count; i++)
            {
                Levels[i].completed = false;
                Levels[i].unlocked = false;
            }
            _unlockedLevelIndex = 0;
            Levels[_unlockedLevelIndex].unlocked = true;
            PlayerPrefs.SetInt(LevelIndexKey, _unlockedLevelIndex);
        }

        public Level GetDefaultLevel()
        {
            return Levels[0];
        }     
    }
}