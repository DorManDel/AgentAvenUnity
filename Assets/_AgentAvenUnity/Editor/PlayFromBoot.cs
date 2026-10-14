
#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AgentAvenUnity.EditorTools
{
    /// <summary>
    /// [EDITOR TOOL / PLAY FROM BOOT]
    ///
    /// WHAT:
    /// Starts Play Mode from Boot_00 regardless of the
    /// Scene currently opened in Unity.
    ///
    /// HOW:
    /// Assigns Boot_00 to Unity's playModeStartScene.
    /// A menu toggle allows temporarily disabling it.
    ///
    /// WHY:
    /// Ensures AppRoot and application services are
    /// initialized before entering the Main Menu.
    ///
    /// DESIGN:
    /// Editor automation; no runtime dependencies.
    /// </summary>
    [InitializeOnLoad]
    internal static class PlayFromBoot
    {
        #region Configuration

        private const string BootScenePath =
            "Assets/_AgentAvenUnity/Scenes/Boot_00.unity";

        private const string MenuPath =
            "AgentAvenUnity/Play Mode/Start From Boot";

        private const string SessionKey =
            "AgentAvenUnity.PlayFromBoot.Enabled";

        #endregion

        #region Editor Lifecycle

        static PlayFromBoot()
        {
            // Apply after Unity finishes its Editor reload.
            EditorApplication.delayCall += Apply;
        }

        #endregion

        #region Menu Commands

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            bool enabled = !SessionState.GetBool(SessionKey, true);

            SessionState.SetBool(SessionKey, enabled);
            Apply();

            Debug.Log(
                $"[EditorBoot] Play From Boot: " +
                $"{(enabled ? "ENABLED" : "DISABLED")}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateToggle()
        {
            Menu.SetChecked(
                MenuPath,
                SessionState.GetBool(SessionKey, true));

            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        #endregion

        #region Configuration Application

        private static void Apply()
        {
            // Default: start from Boot.
            // Disabled: start from the currently open Scene.
            if (!SessionState.GetBool(SessionKey, true))
            {
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            SceneAsset bootScene =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    BootScenePath);

            if (bootScene == null)
            {
                EditorSceneManager.playModeStartScene = null;

                Debug.LogError(
                    "[EditorBoot] Boot_00 scene not found!");
                return;
            }

            EditorSceneManager.playModeStartScene = bootScene;
        }

        #endregion
    }
}

#endif
