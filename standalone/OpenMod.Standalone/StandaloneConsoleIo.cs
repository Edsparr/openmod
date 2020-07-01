﻿using System;
using System.Diagnostics;
using System.Drawing;

namespace OpenMod.Standalone
{
    public static class StandaloneConsoleIo
    {
        public delegate void CommandExecute(string commandLine);
        public static event CommandExecute OnCommandExecute;
        private static volatile bool s_IsRunning;

        public static void StartListening()
        {
            s_IsRunning = true;
            do
            {
                var line = ReadLine.Read().Trim();
                if (!string.IsNullOrEmpty(line))
                {
                    if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    try
                    {
                        OnCommandExecute?.Invoke(line);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString(), Color.DarkRed);
                        Debugger.Break();
                    }
                }
            } while (s_IsRunning);
        }

        public static void StopListening()
        {
            s_IsRunning = false;
        }
    }
}