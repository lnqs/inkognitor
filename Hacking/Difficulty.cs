using System;

namespace Hacking
{
    public class Difficulty
    {
        private int maxErrorsPerCodeBlockRow;
        private float errorCodeBlockProbability;
        private float scrollingSpeed;
        private int levelCount;

        public Difficulty(int levelCount_)
        {
            levelCount = levelCount_;
            SetForLevel(1);
        }

        public int MaxErrorsPerCodeBlockRow { get { return maxErrorsPerCodeBlockRow; } }
        public float ErrorCodeBlockProbability { get { return errorCodeBlockProbability; } }
        public float ScrollingSpeed { get { return scrollingSpeed; } }

        public void SetForLevel(int level)
        {
            level = Math.Max(level, 1);
            maxErrorsPerCodeBlockRow = 3 * level / levelCount;
            errorCodeBlockProbability = 0.25f * level / levelCount;
            scrollingSpeed = 300.0f * level / levelCount;
        }
    }
}
