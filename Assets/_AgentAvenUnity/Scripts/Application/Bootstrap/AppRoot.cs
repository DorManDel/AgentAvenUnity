
using AgentAvenUnity.Application.Diagnostics;
using AgentAvenUnity.Application.SceneFlow;
using UnityEngine;

namespace AgentAvenUnity.Application.Bootstrap
{
    /// <summary>
    /// [APPLICATION / BOOTSTRAP]
    /// Persistent application root and service composition point.
    ///
    /// HOW:
    /// - Prevents duplicate AppRoot instances.
    /// - Survives scene changes.
    /// - Creates shared services and connects their dependencies.
    ///
    /// WHY:
    /// Initializes application services in one known location.
    ///
    /// PATTERNS:
    /// Singleton, Composition Root, Dependency Injection.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AppRoot : MonoBehaviour
    {
        #region Public API

        public static AppRoot Instance { get; private set; }

        public IGameLogger Logger { get; private set; }

        public SceneFlowService SceneFlow { get; private set; }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);

            InitializeServices();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        #endregion

        #region Service Composition

        /// <summary>
        /// [CONSTRUCTOR INJECTION]
        /// Creates application-wide services and supplies dependencies.
        /// Initialization order matters when one service uses another.
        /// </summary>
        private void InitializeServices()
        {
            // Create the shared logger first.
            Logger = new GameLog();

            // Constructor injection: SceneFlow receives IGameLogger.
            SceneFlow = new SceneFlowService(Logger);

            Logger.Info("AppRoot", "Core services initialized.");
        }

        #endregion
    }
}
