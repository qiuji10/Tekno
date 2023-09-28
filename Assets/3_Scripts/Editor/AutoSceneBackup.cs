using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class AutoSceneBackup : AssetPostprocessor
{
    private static string backupFolderPath = "Assets/SceneBackups";
    private static string developerPath = "default";

    static AutoSceneBackup()
    {
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;
    }

    private static void EditorSceneManager_sceneSaved(Scene scene)
    {
        BackupScene();
    }

    private static void BackupScene()
    {
        string scenePath = EditorSceneManager.GetActiveScene().path;
        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        string timestamp = DateTime.Now.ToString("[yyyy-MM-dd] HH-mm-ss");
        string newBackupFileName = $"{sceneName}_backup {timestamp}.unity";
        string newBackupPath = Path.Combine(backupFolderPath, newBackupFileName);

        string path = $"{backupFolderPath}/{developerPath}/";

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
}
