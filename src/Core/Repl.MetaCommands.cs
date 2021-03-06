#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;

#if DEBUG
using System.Diagnostics;
#endif

namespace ICsi.Core
{
    internal sealed partial class Repl
    {
        private IDictionary<string, MethodInfo> _commands;

        [ReplMetaCommand(".cls", "Clears the screen")]
        private void ClsCommand()
        {
#if DEBUG
            Debug.WriteLine("Executing .cls command");
#endif

            Console.Clear();
        }

        [ReplMetaCommand(".quit", "Exits the REPL")]
        private void QuitCommand()
        {
#if DEBUG
            Debug.WriteLine("Quitting the REPL");
#endif

            Environment.Exit(0);
        }

        [ReplMetaCommand(".tree", "Enables/disables Syntax Visualizer")]
        private void TreeCommand()
        {
            if (_enableSyntaxVisualizer)
            {
#if DEBUG
                Debug.WriteLine("Disabling Syntax Visualizer...");
#endif

                _enableSyntaxVisualizer = false;
                Console.WriteLine("Disabled Syntax Visualizer");
            }
            
            else
            {
#if DEBUG
                Debug.WriteLine("Enabling Syntax Visualizer...");
#endif

                _enableSyntaxVisualizer = true;
                Console.WriteLine("Enabled Syntax Visualizer");
            }
        }

        [ReplMetaCommand(".help", "Prints this help message")]
        private void HelpCommand()
        {
#if DEBUG
            Debug.WriteLine("Printing help");
#endif

            Console.WriteLine("Keys:");
            Console.WriteLine("PageUp      Loads previous submission");
            Console.WriteLine("PageDown    Loads next submission");
            Console.WriteLine("Enter       Executes the given submission");
            Console.WriteLine("Ctrl+Enter  Inserts a new line");
            Console.WriteLine("Arrows      Browse through the code");

            Console.WriteLine();

            Console.WriteLine("C#-specific directives:");
            Console.WriteLine("#r\tReferences a metadata file");
            Console.WriteLine("#load\tLoads a C# script");

            Console.WriteLine();

            IEnumerable<MethodInfo> methods = GetType().GetMethods(BindingFlags.NonPublic
                                                                 | BindingFlags.Instance);
                
            IEnumerable<ReplMetaCommandAttribute> GetCommands()
            {
                foreach (MethodInfo method in methods)
                {
                    yield return (ReplMetaCommandAttribute)method.GetCustomAttribute(typeof(ReplMetaCommandAttribute))!;
                }
            }

            Console.WriteLine("REPL-specific commands");
            foreach (ReplMetaCommandAttribute command in GetCommands())
            {
                if (command != null)
                    Console.WriteLine($"{command.Command}\t{command.Description}");
            }
        }

        private void RegisterMetaCommands()
        {
            _commands = new Dictionary<string, MethodInfo>();

#if DEBUG
            Debug.WriteLine("Initializing REPL meta commands...");
#endif

            MethodInfo[] methods = GetType().GetMethods(BindingFlags.NonPublic
                                                      | BindingFlags.Instance);
            
            foreach (MethodInfo method in methods)
            {
                var attribute = (ReplMetaCommandAttribute)method.GetCustomAttribute(typeof(ReplMetaCommandAttribute))!;

                if (attribute != null)
                    _commands.Add(attribute.Command, method);
            }
        }

        private void ExecuteCommand(string command)
        {
#if DEBUG
            Debug.WriteLine("Executing command: " + command);
#endif

            if (_commands.ContainsKey(command))
                _commands[command].Invoke(this, new object[0]);
            else
            {
#if DEBUG
                Debug.WriteLine("Command execution failed: Unrecognized.");
#endif

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Error.WriteLine($"Unrecognized command: {command}. Type .help to know which commands are available.");
                Console.ResetColor();
            }
        }
    }
}