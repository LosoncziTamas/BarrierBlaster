using System.Collections.Generic;
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
            public readonly List<BrickProps> bricksProps;
            public readonly int lineCount;
            public readonly float timeLimitInSeconds;

            public string LevelName { get; set; }
            public int LevelIndex { get; set; }

            public ParsedLevel(float timeLimitInSeconds, List<BrickProps> bricksProps, int lineCount)
            {
                this.timeLimitInSeconds = timeLimitInSeconds;
                this.bricksProps = bricksProps;
                this.lineCount = lineCount;
            }
        }

        public class BrickProps
        {
            public Vector3 Location { get; set; }
            public int LineIdx { get; set; }
            public int HitPoints { get; set; }
            public PowerUp PowerUp { get; set; }
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
                level.LevelIndex = int.Parse(splitName[0]) - 1;
                level.LevelName = splitName[1];
                    
                _cachedLevels.Add(level); 
            }

            return _cachedLevels;
        } 
        
        private static ParsedLevel ParseLevelFileContent(string content)
        {
            var lines = content.Split('\n');
            var metaDataLineIndex = lines.Length - 1;
            var brickProps = new List<BrickProps>();
            
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
                        var pos = new Vector3
                        {
                            x = -0.5f * LevelDimension + elementIndex + 0.5f,
                            y =  0.5f,
                            z =  0.5f * LevelDimension - lineIndex + 3.0f
                        };

                        var powerUp = PowerUpUtils.ParseLevelElement(levelElement);
                        
                        var brickProp = new BrickProps
                        {
                            Location = pos,
                            PowerUp = powerUp,
                            LineIdx = lineIndex,
                            HitPoints = 1
                        };
                        brickProps.Add(brickProp);
                    }
                }
            }

            var metaData = lines[metaDataLineIndex].Split(',');
            Assert.IsTrue(metaData.Length > 0);

            var timeLimit = float.Parse(metaData[0]);
            
            return new ParsedLevel(timeLimit, brickProps, lines.Length);
        }
    }
}