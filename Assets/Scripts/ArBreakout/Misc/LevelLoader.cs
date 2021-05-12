using System.Collections.Generic;
using System.IO;
using ArBreakout.PowerUps;
using UnityEngine;
using UnityEngine.Assertions;

namespace ArBreakout.Misc
{
    public static class LevelLoader 
    {
        private const string LevelRootPath = "Levels";
        
        private const int LevelDimension = 9;

        private static List<ParsedLevel> _cachedLevels;
        public class ParsedLevel
        {
            public readonly List<Vector3> brickLocations;
            public readonly List<PowerUp> brickTypes;
               
            public readonly float timeLimitInSeconds;
            
            public string levelName;
            public int levelIndex; 

            public ParsedLevel(float timeLimitInSeconds, List<Vector3> brickLocations, List<PowerUp> brickTypes)
            {
                this.timeLimitInSeconds = timeLimitInSeconds;
                this.brickLocations = brickLocations;
                this.brickTypes = brickTypes;
            }
        }
        
        public static List<ParsedLevel> LoadLevels()
        {
            if (_cachedLevels != null)
            {
                return _cachedLevels;
            }
            
            _cachedLevels = new List<ParsedLevel>();
            // TODO: Switch to asset bundles in production
            var loadedLevels = Resources.LoadAll<TextAsset>(LevelRootPath);
            foreach (var levelCSV in loadedLevels)
            {
                var content =  levelCSV.text.Trim();
                var level = ParseLevelFileContent(content);
                    
                var splitName = levelCSV.name.Split('_');
                level.levelIndex = int.Parse(splitName[0]) - 1;
                level.levelName = splitName[1];
                    
                _cachedLevels.Add(level); 
            }

            return _cachedLevels;
        } 
        
        private static ParsedLevel ParseLevelFileContent(string content)
        {
            var lines = content.Split('\n');
            var brickLocations = new List<Vector3>();
            var brickTypes = new List<PowerUp>();
            var metaDataLineIndex = lines.Length - 1;
            
            Assert.IsTrue(lines.Length == LevelDimension + 1);
            
            for (var lineIndex = 0; lineIndex < metaDataLineIndex; lineIndex++)
            {
                var lineElements = lines[lineIndex].Split(',');
                
                Assert.IsTrue(lineElements.Length <= LevelDimension + 1);

                for (var elementIndex = 0; elementIndex < lineElements.Length; elementIndex++)
                {
                    var levelElement = lineElements[elementIndex];
                    if (string.IsNullOrWhiteSpace(levelElement))
                    {
                        continue;
                    }
                    if (!levelElement.Equals("0"))
                    {
                        brickLocations.Add(new Vector3
                        {
                            x = -0.5f * LevelDimension + elementIndex + 0.5f,
                            y =  0.5f,
                            z =  0.5f * LevelDimension - lineIndex + 3.0f
                        });
                        brickTypes.Add(PowerUpUtils.ParseLevelElement(levelElement));
                    }
                }
            }

            var metaData = lines[metaDataLineIndex].Split(',');
            Assert.IsTrue(metaData.Length > 0);

            var timeLimit = float.Parse(metaData[0]);
            
            return new ParsedLevel(timeLimit, brickLocations, brickTypes);
        }
    }
}