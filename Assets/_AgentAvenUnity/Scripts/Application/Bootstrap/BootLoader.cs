using UnityEngine;

namespace AgentAvenUnity.Application.Bootstrap
{
    /// <summary>Entry point for the Boot scene; opens the Main Menu after AppRoot.Awake.</summary>
    [DisallowMultipleComponent]
    public sealed class BootLoader : MonoBehaviour
    {
        #region Configuration

        [SerializeField] private string mainMenuSceneName = "MainMenu_01";

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // All active scene objects complete Awake before Start runs.
            if (AppRoot.Instance == null)
            {
                Debug.LogError("[BootLoader] AppRoot missing from Boot scene.");
                return;
            }

            AppRoot.Instance.SceneFlow.Load(mainMenuSceneName);
        }

        #endregion
    }
}
