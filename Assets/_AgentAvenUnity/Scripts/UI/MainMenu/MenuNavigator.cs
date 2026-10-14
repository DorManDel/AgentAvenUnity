
using System;
using System.Collections.Generic;
using AgentAvenUnity.Application.Diagnostics;
using UnityEngine;

namespace AgentAvenUnity.UI.MainMenu
{
    /// <summary>
    /// [UI / MENU NAVIGATION]
    /// Controls visible menu pages and Back navigation.
    ///
    /// HOW:
    /// - Receives IGameLogger through Initialize().
    /// - Builds a Dictionary from Inspector entries.
    /// - Uses a Stack to remember previously visited pages.
    /// - Keeps only one registered menu page visible.
    ///
    /// WHY:
    /// Navigation logic belongs in one component rather than
    /// being scattered across individual UI buttons.
    ///
    /// DESIGN:
    /// Single Responsibility + Method Dependency Injection.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MenuNavigator : MonoBehaviour
    {
        #region Inspector Configuration

        /// <summary>
        /// Connects a MenuPage identifier to a Unity panel.
        ///
        /// One Page-to-Panel mapping, saved by Unity serialization.
        /// [pairing through inspector]
        /// </summary>
        [Serializable]
        private sealed class MenuPanelEntry
        {
            // [SerializeField] = save + expose private var on inspector (prevert public)
            [SerializeField] private MenuPage page;
            [SerializeField] private GameObject panel;

            public MenuPage Page => page;
            public GameObject Panel => panel;
        }

        // list of couples ( pairing page  + panel )
        [SerializeField]
        private List<MenuPanelEntry> menuPanels = new();

        #endregion

        #region Dependencies

        // Injected during initialization, before navigation starts.
        private IGameLogger _logger;

        #endregion

        #region Navigation State

        // Fast page-to-panel lookup, created at runtime. (used for finding panel by page [pairs]- in hirarchy)
        private readonly Dictionary<MenuPage, GameObject> _panels = new();

        // Previous pages; the most recent page is on top.(readonly - prevent change[referral] of field after init)
        private readonly Stack<MenuPage> _history = new();

        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public MenuPage CurrentPage { get; private set; } = MenuPage.Main;

        #endregion

        #region Initialization

        /// <summary>
        /// Receives the logger, validates configuration and opens Main.
        ///
        /// Called by MainMenuController after AppRoot is ready.
        /// </summary>
        public void Initialize(IGameLogger logger)
        {
            if (_isInitialized)
                return;

            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            _panels.Clear();
            _history.Clear();

            HashSet<GameObject> assignedPanels = new();

            foreach (MenuPanelEntry entry in menuPanels)
            {
                if (entry == null || entry.Panel == null)
                {
                    _logger.Error(
                        "MenuNav",
                        "A menu panel reference is missing.");
                    return;
                }

                if (!Enum.IsDefined(typeof(MenuPage), entry.Page))
                {
                    _logger.Error(
                        "MenuNav",
                        $"Invalid menu page: {entry.Page}");
                    return;
                }

                if (!_panels.TryAdd(entry.Page, entry.Panel))
                {
                    _logger.Error(
                        "MenuNav",
                        $"Duplicate page: {entry.Page}");
                    return;
                }

                if (!assignedPanels.Add(entry.Panel))
                {
                    _logger.Error(
                        "MenuNav",
                        $"Panel assigned to more than one page: " +
                        entry.Panel.name);
                    return;
                }
            }

            // Every enum value must have one configured panel.
            foreach (MenuPage page in Enum.GetValues(typeof(MenuPage)))
            {
                if (!_panels.ContainsKey(page))
                {
                    _logger.Error(
                        "MenuNav",
                        $"Missing panel for page: {page}");
                    return;
                }
            }

            _isInitialized = true;

            ShowPage(MenuPage.Main);

            _logger.Info(
                "MenuNav",
                $"Initialized {_panels.Count} menu pages.");
        }

        #endregion

        #region Public Navigation API

        /// <summary>
        /// Opens a page and pushes the current page onto history.
        /// </summary>
        public void Open(MenuPage page)
        {
            if (!CanNavigate(page))
                return;

            if (page == CurrentPage)
                return;

            _history.Push(CurrentPage);

            ShowPage(page);

            _logger.Info(
                "MenuNav",
                $"Opened {page}. History: {_history.Count}");
        }

        /// <summary>
        /// Pops and displays the most recently visited page.
        /// </summary>
        public void Back()
        {
            if (!_isInitialized)
                return;

            if (_history.Count == 0)
            {
                _logger.Verbose(
                    "MenuNav",
                    "Back ignored: history is empty.");
                return;
            }

            MenuPage previousPage = _history.Pop();

            ShowPage(previousPage);

            _logger.Info(
                "MenuNav",
                $"Back to {previousPage}. History: {_history.Count}");
        }

        /// <summary>
        /// Clears navigation history and returns directly to Main.
        /// </summary>
        public void GoHome()
        {
            if (!_isInitialized)
                return;

            _history.Clear();
            ShowPage(MenuPage.Main);

            _logger.Info("MenuNav", "Returned to Main.");
        }

        #endregion

        #region Internal Helpers

        private bool CanNavigate(MenuPage page)
        {
            if (!_isInitialized)
            {
                _logger?.Error(
                    "MenuNav",
                    "Navigation requested before initialization.");
                return false;
            }

            if (!_panels.ContainsKey(page))
            {
                _logger.Error(
                    "MenuNav",
                    $"Unknown page: {page}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Changes visibility without modifying navigation history.
        /// </summary>
        private void ShowPage(MenuPage page)
        {
            CurrentPage = page;

            foreach (KeyValuePair<MenuPage, GameObject> entry in _panels)
            {
                entry.Value.SetActive(entry.Key == page);
            }

            _logger.Verbose(
                "MenuNav",
                $"Visible page: {page}");
        }

        #endregion

    }
}
