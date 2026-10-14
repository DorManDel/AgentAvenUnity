
namespace AgentAvenUnity.Application.Diagnostics
{
    /// <summary>
    /// [INTERFACE]
    /// Defines the logging operations available to application services.
    ///
    /// WHY AN INTERFACE?
    /// Consumers depend on this contract rather than a concrete logger.
    /// This lets AppRoot inject different implementations when needed.
    ///
    /// Examples:
    /// - GameLog: writes messages to the Unity Console.
    /// - FakeGameLogger: collects messages for automated tests.
    /// </summary>
    public interface IGameLogger
    {
        #region Logging Contract

        /// <summary>
        /// Records an important application event.
        /// Displayed when developer diagnostics allow Info logs.
        /// 
        /// * string category = who sends the msg : ex: "SceneFlow"
        /// * string message = the message itself
        /// 
        /// </summary>
        void Info(string category, string message);

        /// <summary>
        /// Records detailed execution information.
        /// Useful when investigating bugs or tracing program flow.
        /// </summary>
        void Verbose(string category, string message);

        /// <summary>
        /// Reports a potentially problematic situation.
        /// Warnings remain visible when DevMode is disabled.
        /// </summary>
        void Warning(string category, string message);

        /// <summary>
        /// Reports a failure that requires attention.
        /// Errors remain visible when DevMode is disabled.
        /// </summary>
        void Error(string category, string message);

        #endregion
    }
}
