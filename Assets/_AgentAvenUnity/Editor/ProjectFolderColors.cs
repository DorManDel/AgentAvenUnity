#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AgentAvenUnity.EditorTools
{
    /// <summary>
    /// Tints folder icons in the Project window. Editor-only: no runtime or asset changes.
    /// A folder's NAME chooses its color; only our project's folders are affected.
    /// </summary>
    [InitializeOnLoad]
    internal static class ProjectFolderColors
    {
        #region Configuration

        private const string ProjectRoot = "Assets/_AgentAvenUnity";

        // Case-insensitive keys make "Scenes" and "scenes" equivalent.
        private static readonly Dictionary<string, Color> FolderColors =
            new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
            {
                { "_AgentAvenUnity", new Color(0.48f, 0.39f, 0.95f) },
                { "Art", new Color(0.95f, 0.60f, 0.25f) },
                { "Audio", new Color(0.68f, 0.49f, 0.96f) },
                { "Data", new Color(0.95f, 0.76f, 0.35f) },
                { "Prefabs", new Color(0.28f, 0.76f, 0.60f) },
                { "Scenes", new Color(0.32f, 0.65f, 0.97f) },
                { "Scripts", new Color(0.39f, 0.79f, 0.96f) },
                { "ScriptableObjects", new Color(0.92f, 0.56f, 0.78f) },
                { "Tests", new Color(0.68f, 0.71f, 0.76f) },
                { "Editor", new Color(0.94f, 0.56f, 0.31f) },
                { "Application", new Color(0.36f, 0.78f, 0.93f) },
                { "UI", new Color(0.44f, 0.76f, 0.97f) },
                { "Game", new Color(0.38f, 0.85f, 0.57f) },
                { "Infrastructure", new Color(0.85f, 0.70f, 0.43f) },
                { "Bootstrap", new Color(0.48f, 0.39f, 0.95f) },
                { "SceneFlow", new Color(0.32f, 0.65f, 0.97f) },
                { "MainMenu", new Color(0.44f, 0.76f, 0.97f) },
                { "Settings", new Color(0.68f, 0.71f, 0.76f) },
                { "Localization", new Color(0.68f, 0.49f, 0.96f) },
                { "Docs", new Color(0.25f, 0.78f, 0.64f) },
                { "Diagnostics", new Color(0.96f, 0.42f, 0.46f) },
                
                // ART: Visual resources used by the game.
                { "Fonts",       new Color(0.71f, 0.64f, 0.96f) },
                { "Source",      new Color(0.68f, 0.60f, 0.87f) },
                { "TMP",         new Color(0.75f, 0.68f, 0.96f) },
                { "Sprites",     new Color(0.95f, 0.60f, 0.35f) },
                { "Characters",  new Color(0.95f, 0.65f, 0.42f) },
                { "Cat",         new Color(0.94f, 0.76f, 0.45f) },
                { "Dog",         new Color(0.80f, 0.68f, 0.48f) },
                { "Fox",         new Color(0.98f, 0.52f, 0.31f) },
                { "Shared",      new Color(0.70f, 0.73f, 0.79f) },

                // UI: Reusable graphics and interface components.
                { "Background",  new Color(0.42f, 0.68f, 0.88f) },
                { "Buttons",     new Color(0.39f, 0.81f, 0.65f) },
                { "Cursor",      new Color(0.95f, 0.79f, 0.40f) },
                { "FX",          new Color(0.95f, 0.55f, 0.72f) },
                { "Icons",       new Color(0.48f, 0.85f, 0.78f) },
                { "Logos",       new Color(0.96f, 0.81f, 0.41f) },
                { "Panels",      new Color(0.48f, 0.73f, 0.91f) },
                { "Sliders",     new Color(0.92f, 0.67f, 0.42f) },
                { "Tabs",        new Color(0.73f, 0.65f, 0.93f) },

            };

        #endregion

        #region Editor Lifecycle

        static ProjectFolderColors()
        {
            // Runs for every Project-window item Unity draws.
            EditorApplication.projectWindowItemOnGUI += DrawFolderIcon;
        }

        #endregion

        #region Drawing

        private static void DrawFolderIcon(string guid, Rect itemRect)
        {
            // Draw only during repaint; don't interfere with clicks or selection.
            if (Event.current.type != EventType.Repaint)
                return;

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path) || !AssetDatabase.IsValidFolder(path))
                return;

            if (path != ProjectRoot &&
                !path.StartsWith(ProjectRoot + "/", StringComparison.Ordinal))
                return;

            string name = System.IO.Path.GetFileName(path);
            if (!FolderColors.TryGetValue(name, out Color tint))
                return;

            // Unity supplies the folder artwork; we tint it on top of the default icon.
            Texture icon = EditorGUIUtility.FindTexture("Folder Icon");
            if (icon == null)
                return;

            // Unity calls this callback for both list rows and grid thumbnails.
            bool isList = itemRect.height <= 22f;
            Rect iconRect;
            if (isList)
            {
                iconRect = new Rect(itemRect.x + 2f, itemRect.y + (itemRect.height - 16f) / 2f, 16f, 16f);
            }
            else
            {
                float size = Mathf.Min(64f, Mathf.Max(16f, itemRect.width - 16f));
                iconRect = new Rect(itemRect.x + (itemRect.width - size) / 2f, itemRect.y + 2f, size, size);
            }

            Color oldColor = GUI.color;
            GUI.color = tint;
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
            GUI.color = oldColor;
        }

        #endregion
    }
}
#endif
