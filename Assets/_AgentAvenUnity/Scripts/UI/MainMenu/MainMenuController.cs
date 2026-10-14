
using AgentAvenUnity.Application.Bootstrap;
using AgentAvenUnity.Application.Diagnostics;
using UnityEngine;

namespace AgentAvenUnity.UI.MainMenu
{
    /// <summary>
    /// [UI / MAIN MENU CONTROLLER]
    /// Receives user actions from Main Menu buttons.
    ///
    /// HOW:
    /// - Connects the shared application logger to MenuNavigator.
    /// - Translates button clicks into navigation requests.
    /// - Handles application-level actions such as Exit.
    ///
    /// WHY:
    /// Keeps UI button handling separate from navigation history.
    ///
    /// DESIGN:
    /// Controller / Separation of Concerns.
    /// Acts as a small scene-level composition point.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MenuNavigator))]
    public sealed class MainMenuController : MonoBehaviour
    {
        #region Inspector References

        [SerializeField]
        private MenuNavigator navigator;

        #endregion

        #region Dependencies

        private IGameLogger _logger;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (navigator == null)
            {
                Debug.LogError(
                    "[MainMenu] Missing MenuNavigator reference.",
                    this);
                enabled = false;
                return;
            }

            AppRoot app = AppRoot.Instance;

            if (app == null || app.Logger == null)
            {
                // Emergency startup logging:
                // The shared logger is not available yet.
                Debug.LogError(
                    "[MainMenu] AppRoot/Logger unavailable. " +
                    "Start through Boot_00.",
                    this);
                enabled = false;
                return;
            }

            _logger = app.Logger;

            // Method Injection: supply the dependency.
            navigator.Initialize(_logger);

            if (!navigator.IsInitialized)
            {
                _logger.Error(
                    "MainMenu",
                    "MenuNavigator initialization failed.");
                enabled = false;
                return;
            }

            _logger.Info("MainMenu", "Controller initialized.");
        }

        #endregion

        #region Main Menu Button Actions

        public void OpenSinglePlayer()
        {
            navigator.Open(MenuPage.SinglePlayer);
        }

        public void OpenMultiplayer()
        {
            navigator.Open(MenuPage.Multiplayer);
        }

        public void OpenHowToPlay()
        {
            navigator.Open(MenuPage.HowToPlay);
        }

        public void OpenSettings()
        {
            navigator.Open(MenuPage.Settings);
        }

        public void OpenAboutDev()
        {
            navigator.Open(MenuPage.AboutDev);
        }

        public void ExitGame()
        {
            _logger.Info("MainMenu", "Exit requested.");

#if UNITY_EDITOR
            // Close Play Mode instead of the Unity Editor.
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // Application.Quit();
            global::UnityEngine.Application.Quit();
#endif
        }

        #endregion

        #region Shared Navigation Actions

        /// <summary>
        /// Shared entry point for all subpanel Back buttons.
        /// </summary>
        public void Back()
        {
            navigator.Back();
        }

        public void GoHome()
        {
            navigator.GoHome();
        }

        #endregion
    }
}
