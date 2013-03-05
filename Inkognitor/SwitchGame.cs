using System;
using SerialIO;

namespace Inkognitor
{
    public class SwitchGame
    {
        public SwitchGame(ArduinoConnector arduino)
        {
            Level = 1;
        }

        public int Level { get; set; }

        public event EventHandler MistakeMade;

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
