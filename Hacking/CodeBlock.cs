using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public class CodeBlock : Sprite
    {
        private CodeBlockPersonalities personalities;
        private int personality;

        public CodeBlock(CodeBlockPersonalities personalities_)
        {
            personalities = personalities_;
        }

        public int Personality
        {
            get { return personality; }
            set
            {
                personality = value;
                if (value == CodeBlockPersonalities.PersonalityError)
                {
                    Surface = personalities.ErrorSurface;
                }
                else
                {
                    Surface = personalities.Surfaces[value];
                }
            }
        }
    }
}
