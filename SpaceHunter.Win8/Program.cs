using ScribbleHunter;
using System;

namespace ScribbleHunterStart
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var factory = new global::MonoGame.Framework.GameFrameworkViewSource<ScribbleHunterWindows>();
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);
        }
    }
}
