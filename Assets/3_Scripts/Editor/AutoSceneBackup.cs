using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class AutoSceneBackup : AssetPostprocessor
{
    private static string backupFolderPath;
    private static string developerPath;
    private static bool autoBackupEnabled; // Add this field

    static AutoSceneBackup()
    {
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;
    }

    private static void EditorSceneManager_sceneSaved(Scene scene)
    {
        LoadSettings();

        if (autoBackupEnabled) // Check if auto backup is enabled
        {
            BackupScene();
        }
    }

    private static void BackupScene()
    {
        //string path = $"{backupFolderPath}/{developerPath}/";
        string path = $"{Application.persistentDataPath}/{backupFolderPath}/{developerPath}/";

        string scenePath = EditorSceneManager.GetActiveScene().path;
        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        string timestamp = DateTime.Now.ToString("[yyyy-MM-dd] HH-mm-ss");
        string newBackupFileName = $"{sceneName}_backup {timestamp}.unity";
        string newBackupPath = Path.Combine(path, newBackupFileName);

        // Ensure the backup directory exists, and if not, create it
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Delete the old backups and their corresponding .meta files
        string[] allBackupFiles = Directory.GetFiles(path);
        foreach (var backupFile in allBackupFiles)
        {
            string fileName = Path.GetFileName(backupFile);
            if (fileName.StartsWith($"{sceneName}_backup ") && (fileName.EndsWith(".unity") || fileName.EndsWith(".meta")))
            {
                FileUtil.DeleteFileOrDirectory(backupFile);
            }
        }

        // Copy the scene to the updated backup path, overwriting if necessary
        File.Copy(scenePath, newBackupPath, true);

        AssetDatabase.Refresh();
    }

    private static void LoadSettings()
    {
        backupFolderPath = EditorPrefs.GetString("backup-folder-path", "Assets/SceneBackups");
        developerPath = EditorPrefs.GetString("developer-path", "default");
        autoBackupEnabled = EditorPrefs.GetBool("auto-backup-enabled", true); // Load auto backup toggle
    }

    private static void SaveSettings()
    {
        EditorPrefs.SetString("backup-folder-path", backupFolderPath);
        EditorPrefs.SetString("developer-path", developerPath);
        EditorPrefs.SetBool("auto-backup-enabled", autoBackupEnabled); // Save auto backup toggle
    }
}
