using ArBreakout.Levels;

namespace ArBreakout.Gui.LevelSelector
{
    public class LevelModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool Unlocked { get; set; }

        public override string ToString()
        {
            return $"{nameof(Text)}: {Text}";
        }

        public static LevelModel Create(LevelData levelData)
        {
            return new LevelModel
            {
                Text = levelData.Name,
                Unlocked = levelData.Unlocked,
                Id = levelData.Id
            };
        }
    }
}