using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public struct CutsceneData
{
    public string cutsceneName;
    public PlayableDirector director;
}

public class BossStageManager : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    
    private PlayerController playerController;
    private StanceManager stanceManager;

    public List<CutsceneData> datas = new List<CutsceneData>();

    private void Awake()
    {
        playerController = playerData.controller;
        stanceManager = FindObjectOfType<StanceManager>();
    }

    public void PlayCutscene(string cutsceneName)
    {
        datas.Find((data) => data.cutsceneName == cutsceneName).director.Play();
    }
}
