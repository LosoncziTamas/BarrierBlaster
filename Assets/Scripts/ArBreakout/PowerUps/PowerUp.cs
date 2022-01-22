namespace ArBreakout.PowerUps
{
    public enum PowerUp
    {
        None,
        Hard,
        Accelerator,
        Decelerator,
        ControlSwitch,
        Magnifier,
        Minifier,
        Magnet
    }

    static class PowerUpUtils
    {
        public static PowerUp ParseLevelElement(string element)
        {
            if (element.Equals("h"))
            {
                return PowerUp.Hard;
            }

            if (element.Equals("a"))
            {
                return PowerUp.Decelerator;
            }

            if (element.Equals("c"))
            {
                return PowerUp.Accelerator;
            }

            if (element.Equals("r"))
            {
                return PowerUp.Minifier;
            }

            if (element.Equals("e"))
            {
                return PowerUp.Magnifier;
            }

            if (element.Equals("s"))
            {
                return PowerUp.ControlSwitch;
            }

            if (element.Equals("m"))
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