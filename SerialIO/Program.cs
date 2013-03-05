using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SerialIO
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = new ArduinoConnector("COM4");
            conn.Init();

            conn.SwitchPressed += (o, e) => {
                conn.SetLight(e.Port, true);
                Thread.Sleep(500);
            };

            conn.PatchCompleted += (o, i) => { Console.WriteLine("patch "+i+" completed");};
            conn.KeysTurned += (o, e) => { Console.WriteLine("keys were turned."); };

            while (true)
                Thread.Sleep(300);
        }
    }
}
