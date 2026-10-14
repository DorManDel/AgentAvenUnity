
namespace AgentAvenUnity.UI.MainMenu
{
    /// <summary>
    /// [UI / MENU]
    /// Identifies the available Main Menu pages [ENUM].
    ///
    /// WHY:
    /// Avoids hardcoded string names when navigating between panels.
    ///
    /// USED BY:
    /// MenuNavigator and MainMenuController.
    ///
    /// NOTE:
    /// Exit is an action, not a page.
    /// </summary>
    public enum MenuPage
    {
        Main,
        SinglePlayer,
        Multiplayer,
        HowToPlay,
        Settings,
        AboutDev
    }

}
