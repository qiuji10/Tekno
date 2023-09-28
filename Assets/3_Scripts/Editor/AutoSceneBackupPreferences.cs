using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// Create a new type of Settings Asset.

[System.Serializable]
class AutoSceneBackupSettings : ScriptableObject
{
    public const string k_AutoSceneBackupSettingsPath = "Assets/Editor/AutoSceneBackupSettings.asset";

    [SerializeField]
    private string backupFolderPath = "Assets/SceneBackups";
    [SerializeField]
    private string developerPath = "default";

    internal static AutoSceneBackupSettings GetOrCreateSettings()
    {
        var settings = AssetDatabase.LoadAssetAtPath<AutoSceneBackupSettings>(k_AutoSceneBackupSettingsPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<AutoSceneBackupSettings>();
            AssetDatabase.CreateAsset(settings, k_AutoSceneBackupSettingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}

// Register a SettingsProvider using UIElements for the drawing framework:
static class AutoSceneBackupSettingsUIElementsRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateAutoSceneBackupSettingsProvider()
    {
        var provider = new SettingsProvider("Preferences/AutoSceneBackupSettings", SettingsScope.User)
        {
            label = "Auto Scene Backup",
            activateHandler = (searchContext, rootElement) =>
            {
                var settings = AutoSceneBackupSettings.GetSerializedSettings();

                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/settings_ui.uss");
                rootElement.styleSheets.Add(styleSheet);

                var title = new Label()
                {
                    text = "Auto Scene Backup Settings"
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

                properties.Add(new PropertyField(settings.FindProperty("backupFolderPath")));
                properties.Add(new PropertyField(settings.FindProperty("developerPath")));

                rootElement.Bind(settings);
            },
            keywords = new HashSet<string>(new[] { "Backup Folder Path", "Developer Path" })
        };

        return provider;
    }
}
