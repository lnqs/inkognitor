using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Inkognitor
{
    public class PrintingTimer
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private int ticksLeft;
        private TextBlock output;
        private string tickText;
        private EventHandler finishingHandler;

        public PrintingTimer()
        {
            dispatcherTimer.Tick += HandleTick;
        }

        public bool IsEnabled { get { return dispatcherTimer.IsEnabled; } }

        public void Start(TextBlock output_, int ticks, int delay, string tickText_, EventHandler finishingHandler_)
        {
            output = output_;
            ticksLeft = ticks;
            tickText = tickText_;
            finishingHandler = finishingHandler_;
            dispatcherTimer.Interval = new TimeSpan(0, 0, delay);
            dispatcherTimer.Start();
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
        }

        private void HandleTick(object sender, EventArgs e)
        {
            ticksLeft -= 1;

            if (ticksLeft > 0)
            {
                output.Text += tickText;
            }
            else
            {
                dispatcherTimer.Stop();
                finishingHandler(this, EventArgs.Empty);
            }
        }
    }
}
