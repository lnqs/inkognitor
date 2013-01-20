using System;
using System.Collections.Generic;
using System.Reflection;

namespace Inkognitor
{
    public class CommandDispatcher
    {
        private ICommandProvider provider;
        private Dictionary<string, Listener> listeners = new Dictionary<string, Listener>();

        public CommandDispatcher(ICommandProvider commandProvider)
        {
            Provider = commandProvider;
            AddListener(this);
        }

        public ICommandProvider Provider
        {
            get { return provider; }
            set
            {
                if (provider != null)
                {
                    provider.Dispatch = null;
                }

                provider = value;
                provider.Dispatch = Dispatch;
            }
        }

        public void AddListener(object listener)
        {
            foreach (MethodInfo method in listener.GetType().GetMethods(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                CommandListener attribute = (CommandListener)Attribute
                        .GetCustomAttribute(method, typeof(CommandListener), true);
                if (attribute != null)
                {
                    listeners.Add(attribute.Identifier,
                            new Listener(listener, method, attribute.Description));
                }
            }
        }

        public string Dispatch(string[] command)
        {
            if (listeners.ContainsKey(command[0]))
            {
                return listeners[command[0]].Invoke(command);
            }
            else
            {
                return "command not found";
            }
        }

        [CommandListener("help", Description="returns a list of all commands and parameters")]
        private string GetHelp()
        {
            string help = "";

            foreach (var listener in listeners)
            {
                help += listener.Key + " ";
                foreach (ParameterInfo parameter in listener.Value.Method.GetParameters())
                {
                    help += "(" + parameter.ParameterType.Name + ")";
                    help += parameter.Name + " ";
                }

                if (listener.Value.Description != null)
                {
                    help +=  "-- " + listener.Value.Description;
                }

                help += "\n";
            }

            return help;
        }

        private class Listener
        {
            private object instance;
            private MethodInfo method;
            private string description;

            public Listener(object instance_, MethodInfo method_, string description_)
            {
                instance = instance_;
                method = method_;
                description = description_;
            }

            public MethodInfo Method { get { return method; } }
            public string Description { get { return description; } }

            public string Invoke(string[] command)
            {
                object[] arguments = new object[command.Length - 1];
                ParameterInfo[] parameters = method.GetParameters();

                if (arguments.Length != parameters.Length)
                {
                    return "incorrect number of parameters";
                }

                for (int i = 0; i < parameters.Length; i++)
                {
                    try
                    {
                        arguments[i] = Convert.ChangeType(command[i + 1], parameters[i].ParameterType);
                    }
                    catch (Exception)
                    {
                        return "couldn't convert argument " + Convert.ToString(i + 1)
                                + " to " + parameters[i].ParameterType.Name;
                    }
                }

                object ret = method.Invoke(instance, arguments);

                if (method.ReturnType != typeof(void))
                {
                    return ret.ToString();
                }

                return "";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    class CommandListener : Attribute
    {
        public CommandListener(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; set; }
        public string Description { get; set; }
    }
}
