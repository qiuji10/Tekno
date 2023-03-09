using UnityEngine;
using System.Collections;

public class CustomLog : MonoBehaviour
{
    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 20;
        GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height));
        if (GUILayout.Button("ClearLog"))
        {
            myLogQueue.Clear();
        }
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()), style);
        GUILayout.EndArea();
    }
}