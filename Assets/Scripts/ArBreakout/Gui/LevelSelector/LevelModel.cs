using System.Collections.Generic;

namespace ArBreakout.Gui
{
    public class LevelModel
    {
        public string Text { get; set; }
        public bool Unlocked { get; set; }

        public override string ToString()
        {
            return $"{nameof(Text)}: {Text}";
        }

        public static List<LevelModel> CreateDebugData()
        {
            return new List<LevelModel>
            {
                new LevelModel
                {
                    Text = "I",
                    Unlocked = true
                },
                new LevelModel
                {
                    Text = "II",
                    Unlocked = true
                },
                new LevelModel
                {
                    Text = "III",
                    Unlocked = true
                },
                new LevelModel
                {
                    Text = "IV",
                    Unlocked = true
                },
                new LevelModel
                {
                    Text = "V",
                    Unlocked = false
                },
                new LevelModel
                {
                    Text = "VI",
                    Unlocked = false
                },
                new LevelModel
                {
                    Text = "VII",
                    Unlocked = false
                },
                new LevelModel
                {
                    Text = "VIII",
                    Unlocked = false
                },
                new LevelModel
                {
                    Text = "IX",
                    Unlocked = false
                }
            };
        }
    }
}