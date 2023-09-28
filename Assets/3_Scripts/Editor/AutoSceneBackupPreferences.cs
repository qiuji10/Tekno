using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// Register a SettingsProvider using UIElements for the drawing framework:
static class AutoSceneBackupSettingsUIElementsRegister
{
    private static string styleSheetPath => "Assets/Editor/settings_ui.uss";
    private static string defaultBackupFolderPath => "AutoSceneBackups";
    private static string defaultDeveloperFolder => "Default";

    [SettingsProvider]
    public static SettingsProvider CreateAutoSceneBackupSettingsProvider()
    {
        // Check if the style sheet file exists, and create it if it doesn't
        if (!File.Exists(styleSheetPath))
        {
            CreateDefaultStyleSheet();
        }

        var provider = new SettingsProvider("Preferences/AutoSceneBackupSettings", SettingsScope.User)
        {
            label = "Auto Scene Backup",
            activateHandler = (searchContext, rootElement) =>
            {
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
                rootElement.styleSheets.Add(styleSheet);

                var title = new Label()
                {
                    text = "Auto Scene Backup Settings",
                    style =
                    {
                        fontSize = 16,
                        marginBottom = 10,
                        unityFontStyleAndWeight = FontStyle.Bold
                    }
                };
                title.AddToClassList("title");
                rootElement.Add(title);

                var properties = new VisualElement()
                {
                    style =
                    {
                        flexDirection = FlexDirection.Column
                    }
                };
                properties.AddToClassList("property-list");
                rootElement.Add(properties);

                var backupFolderPathField = new TextField("Backup Folder Path");
                backupFolderPathField.SetValueWithoutNotify(EditorPrefs.GetString("backup-folder-path", defaultBackupFolderPath));
                backupFolderPathField.RegisterValueChangedCallback(e =>
                {
                    EditorPrefs.SetString("backup-folder-path", e.newValue);
                });
                properties.Add(backupFolderPathField);

                var developerPathField = new TextField("Developer Path");
                developerPathField.SetValueWithoutNotify(EditorPrefs.GetString("developer-path", defaultDeveloperFolder));
                developerPathField.RegisterValueChangedCallback(e =>
                {
                    EditorPrefs.SetString("developer-path", e.newValue);
                });
                properties.Add(developerPathField);

                var autoBackupToggle = new Toggle("Enable Auto Backup");
                autoBackupToggle.SetValueWithoutNotify(EditorPrefs.GetBool("auto-backup-enabled", true));
                autoBackupToggle.RegisterValueChangedCallback(e =>
                {
                    EditorPrefs.SetBool("auto-backup-enabled", e.newValue);
                });
                properties.Add(autoBackupToggle);

                var sceneBeingBackedUpLabel = new Label();
                properties.Add(sceneBeingBackedUpLabel);

                // Create a button to open the backup folder in the file explorer (Windows) or Finder (Mac)
                var openBackupFolderButton = new Button(() =>
                {
                    string backupFolderPath = EditorPrefs.GetString("backup-folder-path", defaultBackupFolderPath);
                    string developerPath = EditorPrefs.GetString("developer-path", defaultDeveloperFolder);
                    string backupPath = $"{Application.persistentDataPath}/{backupFolderPath}/{developerPath}/";

                    // Make sure the backup folder exists before attempting to open it
                    if (Directory.Exists(backupPath))
                    {
                        // Open the backup folder with the default file explorer (Windows) or Finder (Mac)
                        EditorUtility.RevealInFinder(backupPath);
                    }
                    else
                    {
                        Debug.LogWarning($"Backup folder ({backupPath}) does not exist.");
                    }
                });
                openBackupFolderButton.text = "Open Backup Folder";
                properties.Add(openBackupFolderButton);


                // Subscribe to the EditorSceneManager.sceneSaved event to update the displayed scene
                EditorSceneManager.sceneSaved += (scene) =>
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    sceneBeingBackedUpLabel.text = "Scene Being Backed Up: " + sceneName;
                };

            },
            keywords = new HashSet<string>(new[] { "Backup Folder Path", "Developer Path" })
        };

        return provider;
    }

    // Helper function to create the default style sheet
    private static void CreateDefaultStyleSheet()
    {
        var styleSheetContent = "VisualElement {}";

        // Create the style sheet file
        File.WriteAllText(styleSheetPath, styleSheetContent);
    }
}
