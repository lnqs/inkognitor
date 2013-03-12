using System;

namespace Hacking
{
    public class Difficulty
    {
        private static int MaxLevel = 6;
        private static double MaxSpeed = 300.0;
        private static double MaxErrorProbability = 0.5;

        private double errorCodeBlockProbability;
        private double scrollingSpeed;

        public Difficulty()
        {
            SetForLevel(1);
        }

        public double ErrorCodeBlockProbability { get { return errorCodeBlockProbability; } }
        public double ScrollingSpeed { get { return scrollingSpeed; } }

        public void SetForLevel(int level)
        {
            level = Math.Max(level, 1);
            errorCodeBlockProbability = MaxErrorProbability * level / MaxLevel;
            scrollingSpeed = MaxSpeed * level / MaxLevel;
        }
    }
}
