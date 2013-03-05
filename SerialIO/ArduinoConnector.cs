using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace SerialIO
{
    public class EventArgsWithPort:EventArgs{
        public int Port{get;set;}
        public EventArgsWithPort(int port) { Port = port; }
    }
    public class ArduinoConnector :IDisposable
    {
        const int KeyTurnThreshhold = 2600;

        public event EventHandler<EventArgsWithPort> SwitchPressed;
        public event EventHandler KeysTurned;
        public event EventHandler<EventArgsWithPort> PatchCompleted;
        SerialPort _port;
        public ArduinoConnector() 
        {
            _port = getSerialPort(115200);            
        }

        public ArduinoConnector(string serialPortHint)
        {
            _port = getSerialPort(115200, serialPortHint);
        }

        public void Init() 
        {
            _port.DataReceived += (_port_DataReceived);
            _port.Open();
        }

        public void SetLight(int index, bool state)
        { 
            var buffer = new byte[]{(byte)index,(byte)(state?0xFF:0)};
            _port.Write(buffer, 0, 2);
        }

        void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                string content = _port.ReadLine();
                int port;
                if (int.TryParse(content, out port)) 
                {
                    if (port >= 0 && port < 12)
                    {
                        if (SwitchPressed != null)
                            SwitchPressed(this, new EventArgsWithPort(port));
                    }
                    else if(port >= 12 && port < 14)
                    {
                        if(PatchCompleted != null)
                            PatchCompleted(this,new EventArgsWithPort(port));
                    }
                    else if (port >= 14 && port < 16) 
                    {
                        keyTurned(port);
                    }
                }
            }
        }

        int lastKey= -1;
        DateTime turnTime = DateTime.MaxValue;
        private void keyTurned(int port)
        {
            if (lastKey != -1 && port != lastKey && turnTime < DateTime.Now) 
            {
                if (KeysTurned != null)
                    KeysTurned(this, new EventArgs());
                lastKey = -1;
                turnTime = DateTime.MaxValue;
            }
            else
            {
                lastKey = port;
                turnTime = DateTime.Now.AddMilliseconds(KeyTurnThreshhold);
            }
        }

        SerialPort getSerialPort(int baudRate, string portHint = "")
        {
            if (!string.IsNullOrEmpty(portHint))
                try
                {
                    return new SerialPort(portHint, baudRate);
                }
                catch
                {
                    //round one: wait and try again

                    try
                    {
                        Thread.Sleep(50);
                        return new SerialPort(portHint, baudRate);
                    }
                    catch //round two failed? desired port wont work
                    { ;}
                }

            var ports = SerialPort.GetPortNames().ToList();
            ports.Remove(portHint);

            foreach (string port in ports)
            {
                try
                {
                    return new SerialPort(port, baudRate);
                }
                catch { ;}
            }
            //if we get here, there was no valid port
            throw new System.IO.IOException("no accessible port found");
        }

        public void Dispose() 
        {

            _port.Dispose();

        }
    }
}
