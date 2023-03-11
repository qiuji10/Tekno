using UnityEngine;
using System.Collections;

public class CustomLog : MonoBehaviour
{
    Queue myLogQueue = new Queue();

    Vector2 scrollPosition = Vector2.zero;

    bool showLogWindow = false; // Flag to track whether to show log window or not

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showLogWindow = !showLogWindow;
        }
    }

    void OnGUI()
    {
        if (showLogWindow)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height));
            if (GUILayout.Button("ClearLog"))
            {
                myLogQueue.Clear();
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(400), GUILayout.Height(400));
            GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()), style);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            // Set scroll position to bottom
            if (Event.current.type == EventType.Repaint)
            {
                scrollPosition.y = Mathf.Infinity;
            }
        }
    }
}
