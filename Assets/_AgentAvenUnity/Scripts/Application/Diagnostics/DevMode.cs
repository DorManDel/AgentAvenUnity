// CTRL[hold] + K[click] -> +D[click] = AUTO-INDENTATION
namespace AgentAvenUnity.Application.Diagnostics
{
    /// <summary>
    /// Defines how detailed developer logs may be.
    /// Higher values include all lower levels.
    /// </summary>
    public enum DevLogLevel
    {
        Error = 0,
        Warning = 1,
        Info = 2,
        Verbose = 3
    }
    // VERBOSE = very high level of details (extended error and logfs display) 

    /// <summary>
    /// Stores the current developer diagnostics configuration.
    ///
    /// This is a plain static C# class, not a MonoBehaviour.
    /// It does not need a GameObject or an Update method.
    ///
    /// GameLog will read these settings in the next step.
    /// </summary>
    public static class DevMode
    {
        #region Configuration

        // Enable diagnostics automatically inside Unity Editor.
        // Disable optional diagnostics in player builds by default.
#if UNITY_EDITOR
        private static bool _enabled = true;
#else
        private static bool _enabled = false;
#endif

        private static DevLogLevel _logLevel = DevLogLevel.Info;

        #endregion

        #region Public Properties

        public static bool Enabled => _enabled;

        public static DevLogLevel LogLevel => _logLevel;

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the developer diagnostics settings.
        /// AppRoot will call this during initialization later.
        /// </summary>
        public static void Configure(
            bool enabled,
            DevLogLevel level)
        {
            _enabled = enabled;
            _logLevel = level;
        }


        /// <summary>
        /// Toggles optional developer diagnostics.
        ///
        /// The confirmation is printed directly through Unity Debug
        /// so the developer can see it even when diagnostics
        /// have just been disabled.
        /// </summary>
        public static void Toggle()
        {
            _enabled = !_enabled;

            UnityEngine.Debug.Log(
                $"[DevMode] Diagnostics {(_enabled ? "ENABLED" : "DISABLED")}. " +
                $"Log level: {_logLevel}");
        }


        /// <summary>
        /// Checks whether an optional developer message
        /// should be printed at the requested level.
        /// </summary>
        public static bool Allows(DevLogLevel level)
        {
            return _enabled && level <= _logLevel;
        }

        #endregion
    }
}
