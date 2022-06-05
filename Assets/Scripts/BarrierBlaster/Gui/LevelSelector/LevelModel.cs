using BarrierBlaster.Levels;

namespace BarrierBlaster.Gui.LevelSelector
{
    public class LevelModel
    {
        public string Id { get; private set; }
        public string Text { get; set; }
        public bool Unlocked { get; private set; }

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