
using System;
using System.IO;
using AgentAvenUnity.Application.Diagnostics;
using UnityEngine.SceneManagement;

namespace AgentAvenUnity.Application.SceneFlow
{
    /// <summary>
    /// [APPLICATION / SCENE FLOW]
    /// Validates and loads scenes registered in Unity Build Profiles.
    ///
    /// HOW:
    /// - Receives a logger through its constructor.
    /// - Searches for an exact scene-name match.
    /// - Loads the matching scene using its Build Index.
    ///
    /// WHY:
    /// Centralizes scene loading and diagnostic reporting.
    ///
    /// DESIGN:
    /// Constructor Dependency Injection + Dependency Inversion.
    /// </summary>
    public sealed class SceneFlowService
    {
        #region Dependencies

        // readonly: the reference cannot be replaced after construction.
        // We depend on a logging contract, not on GameLog directly.
        private readonly IGameLogger _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Receives the logging dependency from AppRoot.
        /// A valid logger is required for this service.
        /// </summary>
        public SceneFlowService(IGameLogger logger)
        {
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public API

        /// <summary>
        /// Loads a scene by its exact name in the Build Profile.
        /// </summary>
        public void Load(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                _logger.Error("SceneFlow", "Scene name is empty.");
                return;
            }

            _logger.Info(
                "SceneFlow",
                $"Requested scene: '{sceneName}'");

            // Search scenes registered in the active Build Profile.
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);

                string registeredName =
                    Path.GetFileNameWithoutExtension(path);

                bool isMatch = string.Equals(
                    sceneName,
                    registeredName,
                    StringComparison.Ordinal);

                // Verbose: detailed information useful during debugging.
                _logger.Verbose(
                    "SceneFlow",
                    $"Index {i}: '{registeredName}' | Match: {isMatch}");

                if (!isMatch)
                    continue;

                _logger.Info(
                    "SceneFlow",
                    $"Loading build index {i}: '{registeredName}'");

                SceneManager.LoadScene(i, LoadSceneMode.Single);
                return;
            }

            // Errors must remain visible even when DevMode is disabled.
            _logger.Error(
                "SceneFlow",
                $"Scene not found in Build Profile: '{sceneName}'");
        }

        #endregion
    }
}
