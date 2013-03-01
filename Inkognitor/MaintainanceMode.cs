using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Inkognitor
{
    public class MaintainanceMode : MainWindowMode
    {
        private Files files;
        private Command activeCommand;

        public override string Name { get { return "Maintainance"; } }
        public override string DefaultText { get { return CommandNames; } }
        private string CommandNames
        {
            get { return String.Join("\n", files.MaintainanceCommandNames); }
        }

        public override void Enter(MainWindow window, Logger logger, Files files_)
        {
            files = files_;
            base.Enter(window, logger, files_);
        }

        protected override void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e)
        {
            if (activeCommand == null || activeCommand.Finished)
            {
                if (files.MaintainanceCommands.ContainsKey(e.Text.ToUpper()))
                {
                    activeCommand = files.MaintainanceCommands[e.Text.ToUpper()];
                    // It sucks to pass everything that may be needed by a subclass.
                    // This has to be done otherwise. But I'm too tired to think about a better way currently.
                    activeCommand.Execute(window.textBlock, CommandNames, FireFinishedEvent);
                    logger.ChatLog.Log("Maintainance command: {0}", e.Text.ToUpper());
                }
                else
                {
                    window.textBlock.Text = files.UnknownCommand;
                    logger.ChatLog.Log("Unknown maintainance command: {0}", e.Text.ToUpper());
                }
            }
        }

        public abstract class Command
        {
            public abstract void Execute(TextBlock target, string commandlist, Action nextMode);
            public abstract bool Finished { get; }
        }

        public class ShowCommandsCommand : Command
        {
            public override void Execute(TextBlock target, string commandlist, Action nextMode)
            {
                target.Text = commandlist;
            }

            public override bool Finished { get { return true; } }
        }

        public class PrintDottetCommand : Command
        {
            private string pre;
            private int delay;
            private string post;
            private PrintingTimer timer = new PrintingTimer();

            public PrintDottetCommand(string pre_, int delay_, string post_)
            {
                pre = pre_;
                delay = delay_;
                post = post_;
            }

            public override void Execute(TextBlock target, string commandlist, Action nextMode)
            {
                target.Text = pre;
                timer.Start(target, delay, 1, ".", (sender, e) => { target.Text += "\n" + post; });
            }

            public override bool Finished { get { return !timer.IsEnabled; } }
        }

        public class NextModeCommand : Command
        {
            private string pre;
            private int delay;
            private PrintingTimer timer = new PrintingTimer();

            public NextModeCommand(string pre_, int delay_)
            {
                pre = pre_;
                delay = delay_;
            }

            public override void Execute(TextBlock target, string commandlist, Action nextMode)
            {
                target.Text = pre;
                timer.Start(target, delay, 1, ".", (sender, e) => { nextMode(); });
            }

            public override bool Finished { get { return !timer.IsEnabled; } }
        }
    }
}
