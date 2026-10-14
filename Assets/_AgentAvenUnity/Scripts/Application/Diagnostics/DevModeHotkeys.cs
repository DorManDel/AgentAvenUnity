
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace AgentAvenUnity.Application.Diagnostics
{
    /// <summary>
    /// (IDENTIFIER)
    /// Handles the developer keyboard shortcut. 
    ///
    /// Attach this component to the persistent AppRoot
    /// GameObject in Boot_00.
    ///
    /// Shortcut: Ctrl + Shift + F8.
    /// Enabled only in Editor and Development Builds.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DevModeHotkeys : MonoBehaviour
    {
        #region Unity Lifecycle

        private void Update()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

#if ENABLE_INPUT_SYSTEM

            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
                return;

            bool ctrl =
                keyboard.leftCtrlKey.isPressed ||
                keyboard.rightCtrlKey.isPressed;

            bool shift =
                keyboard.leftShiftKey.isPressed ||
                keyboard.rightShiftKey.isPressed;

            bool shortcutPressed =
                ctrl && shift && keyboard.f8Key.wasPressedThisFrame;

            if (shortcutPressed)
                DevMode.Toggle();

#elif ENABLE_LEGACY_INPUT_MANAGER

            bool ctrl =
                Input.GetKey(KeyCode.LeftControl) ||
                Input.GetKey(KeyCode.RightControl);

            bool shift =
                Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift);

            if (ctrl && shift && Input.GetKeyDown(KeyCode.F8))
                DevMode.Toggle();

#endif

#endif
        }

        #endregion
    }
}
