namespace ArBreakout.PowerUps
{
    public enum PowerUp
    {
        None,
        Accelerator,
        Decelerator,
        ControlSwitch,
        Magnifier,
        Minifier,
        Magnet
    }

    public static class PowerUpUtils
    {
        public static PowerUp ParseLevelElement(string element)
        {
            if (element.Contains("-"))
            {
                return PowerUp.Decelerator;
            }
            if (element.Contains("+"))
            {
                return PowerUp.Accelerator;
            }
            if (element.Contains("o"))
            {
                return PowerUp.Minifier;
            }
            if (element.Contains("O"))
            {
                return PowerUp.Magnifier;
            }
            if (element.Contains("%"))
            {
                return PowerUp.ControlSwitch;
            }
            if (element.Contains("="))
            {
                return PowerUp.Magnet;
            }

            return PowerUp.None;
        }

        public static bool EffectsPaddle(this PowerUp powerUp)
        {
            return false;
        }

        public static bool EffectsPaddle(int powerUpOrdinal)
        {
            var powerUp = (PowerUp) powerUpOrdinal;
            return EffectsPaddle(powerUp);
        }
    }
}