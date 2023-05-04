using UnityEngine;
using System;
using NaughtyAttributes;

public class TempoManager : MonoBehaviour
{
    public float delay;
    public float BPM = 120;
    [SerializeField] float bpmChnager;
    public static float _lastBeatTime;
    public static float staticBPM;

    private float _lastSyncTime = 0;
    private uint _beatsSinceSync = 0;
    private bool hasStarted;

    public static event Action OnBeat;

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track track)
    {
        bpmChnager = track.bpm;
        SyncBPM();
    }

    void Update()
    {
        //Check beat timer and trigger beat if neccessary
        if (!hasStarted)
        {
            if (Time.time >= delay)
            {
                hasStarted = true;
                SyncBPM();
            }
        }
        else
        {
            if (Time.time > _lastSyncTime + (BeatsPerMinuteToDelay(BPM) * _beatsSinceSync))
            {
                Beat();
            }
        }
    }

    public static float BeatsPerMinuteToDelay(float beatsPerMinute)
    {
        //beats per second = beatsPerMinute / 60
        return 1.0f / (beatsPerMinute / 60.0f);
    }

    public float TimeSinceLastBeat()
    {
        return Time.time - (_lastSyncTime + BeatsPerMinuteToDelay(staticBPM) * _beatsSinceSync);
    }

    public static float GetTimeToBeatCount(float beatFraction)
    {
        return (BeatsPerMinuteToDelay(staticBPM) * beatFraction) - (BeatsPerMinuteToDelay(staticBPM) / 2.0f);
    }

    private void Beat()
    {
        _beatsSinceSync++;
        OnBeat?.Invoke();
        _lastBeatTime = Time.time;
    }

    [Button]
    /// <summary>
    /// Restart BPM timer
    /// </summary>
    public void SyncBPM()
    {
        staticBPM = BPM = bpmChnager;
        _lastSyncTime = Time.time;
        _beatsSinceSync = 0;
        Beat(); //NB: beat is now synced immedately instead of after a 1 beat delay
    }

}