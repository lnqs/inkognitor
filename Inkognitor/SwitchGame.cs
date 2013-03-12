using System;
using System.Threading;
using SerialIO;

namespace Inkognitor
{
    public class SwitchGame
    {
        const int SleepTime = 1000; // ms
        const double LevelLightsMultiplier = 1.0;
        const double LevelTimeMultiplier = 1.0;

        // 12 is the max port passed from ArduinoConnector. Should be a constant.
        private Switch[] switches = new Switch[12];
        ArduinoConnector arduino;
        private bool running = true;
        private Random random = new Random();
        private int maxLevel;

        public SwitchGame(ArduinoConnector arduino_, int maxLevel_)
        {
            arduino = arduino_;
            arduino.SwitchPressed += HandleSwitchPressed;

            Level = 1;
            maxLevel = maxLevel_;
        }

        public int Level { get; set; }

        public event EventHandler MistakeMade;

        public void Run()
        {
            while (running)
            {
                Thread.Sleep(SleepTime); // I feel dirty for this :/

                CheckSwitches();
                EnableRandomSwitches();
            }
        }

        public void Stop()
        {
            running = false;
        }

        private void InitializeSwitches()
        {
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i].Active = false;
            }
        }

        private void CheckSwitches()
        {
            for (int i = 0; i < switches.Length; i++)
            {
                if (switches[i].Active)
                {
                    switches[i].TimeLeft -= SleepTime;

                    if (switches[i].TimeLeft <= 0)
                    {
                        DisableSwitch(i);
                        MistakeMade(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void EnableRandomSwitches()
        {
            for (int i = 0; i < (int)(Level * LevelLightsMultiplier); i++)
            {
                int r = random.Next(switches.Length);

                if (!switches[r].Active)
                {
                    EnableSwitch(r);
                }
            }
        }

        private void EnableSwitch(int i)
        {
            switches[i].Active = true;
            arduino.SetLight(i, true);
            switches[i].TimeLeft = (int)((maxLevel - Level) * LevelTimeMultiplier);
        }

        private void DisableSwitch(int i)
        {
            switches[i].Active = false;
            arduino.SetLight(i, false);
        }

        private void HandleSwitchPressed(object sender, EventArgsWithPort e)
        {
            if (switches[e.Port].Active)
            {
                DisableSwitch(e.Port);
            }
            else
            {
                MistakeMade(this, EventArgs.Empty);
            }
        }

        private struct Switch
        {
            public bool Active;
            public int TimeLeft;
        }
    }
}
