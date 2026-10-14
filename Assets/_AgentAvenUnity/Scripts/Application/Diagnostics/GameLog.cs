
using UnityEngine;

namespace AgentAvenUnity.Application.Diagnostics
{
    /// <summary>
    /// [APPLICATION / DIAGNOSTICS]
    /// Writes categorized application messages to the Unity Console.
    ///
    /// HOW:
    /// - Implements the IGameLogger contract.
    /// - Uses DevMode to filter optional Info and Verbose logs.
    /// - Always reports warnings and errors.
    ///
    /// WHY:
    /// Keeps Unity's Debug API out of application services,
    /// making logging replaceable and easier to test.
    ///
    /// DESIGN:
    /// Interface implementation / Dependency Inversion.
    /// </summary>
    public sealed class GameLog : IGameLogger
    {
        #region Public Logging API

        public void Info(string category, string message)
        {
            if (!DevMode.Allows(DevLogLevel.Info))
                return;

            Debug.Log(Format(category, message));
        }

        public void Verbose(string category, string message)
        {
            if (!DevMode.Allows(DevLogLevel.Verbose))
                return;

            Debug.Log(Format(category, message));
        }

        public void Warning(string category, string message)
        {
            Debug.LogWarning(Format(category, message));
        }

        public void Error(string category, string message)
        {
            Debug.LogError(Format(category, message));
        }

        #endregion

        #region Internal Helpers

        private static string Format(string category, string message)
        {
            return $"[{category}] {message}";
        }

        #endregion
    }
}
