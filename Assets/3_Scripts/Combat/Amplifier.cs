using SonicBloom.Koreo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amplifier : MonoBehaviour, IDamagaeble
{
    public bool IsAlive => throw new System.NotImplementedException();
    
    public void Damage(int damage)
    {
        throw new System.NotImplementedException();
    }

    private List<KoreographyEvent> koreographyEvents = new List<KoreographyEvent>();
    private int startSample, endSample;
    int counter, curCounter;


    private void Start()
    {
        //Koreographer.Instance.GetAllEventsInRange("[Electro]PIXL-Buzz Kill", "BeatTrack", 0, 15681960, koreographyEvents);
        Koreographer.Instance.RegisterForEvents("BeatTrack", OnEventCall);
    }

    private void OnApplicationQuit()
    {
        Koreographer.Instance.UnregisterForEvents("BeatTrack", OnEventCall);
    }

    [SerializeField] int backBuffer = 500, frontBuffer;
    private bool tick, streak;

    [SerializeField] KeyCode key;
    private void Update()
    {
        //int sample = Koreographer.GetSampleTime();

        //Debug.Log($"cur:{sample}, start:{startSample}, end:{endSample}");
        if (Input.GetKeyDown(key))
        {
            int sample = Koreographer.GetSampleTime();

            if (startSample <= sample && endSample >= sample && !tick)
            {
                Debug.Log($"sample:{sample}\tstar:{startSample}\tend:{endSample}\n<color=green>perfect</color>");
            }
            else if (sample < startSample)
            {
                streak = false;
                counter = 0;
                Debug.Log($"sample:{sample}\tstar:{startSample}\tend:{endSample}\n<color=red>too early</color>");
            }
            else if (sample > endSample)
            {
                streak = false;
                counter = 0;
                Debug.Log($"sample:{sample}\tstar:{startSample}\tend:{endSample}\n<color=red>too late</color>");
            }
            

            if (counter == 4)
            {
                counter = 0;
                Debug.Log("<color=yellow>Success!</color>");
            }
        }       
    }

    private void OnEventCall(KoreographyEvent koreoEvent)
    {
        tick = false;
        Debug.Log($"start: {koreoEvent.StartSample}, end: {koreoEvent.EndSample}");
        startSample = koreoEvent.StartSample - frontBuffer;
        endSample = koreoEvent.EndSample + backBuffer;
    }


}
