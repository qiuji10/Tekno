using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
[DisallowMultipleComponent]
public class AnimateOnPath : MonoBehaviour
{
    public CinemachinePathBase m_Path;

    public enum MovingUpdateMethod
    {
        Update,
        FixedUpdate,
        LateUpdate
    };

    public MovingUpdateMethod m_UpdateMethod = MovingUpdateMethod.Update;
    public CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
    public float m_Speed;
    public float m_Position;

    void FixedUpdate()
    {
        if (m_UpdateMethod == MovingUpdateMethod.FixedUpdate)
            SetCartPosition(m_Position + m_Speed * Time.deltaTime);
    }

    void Update()
    {
        float speed = Application.isPlaying ? m_Speed : 0;
        if (m_UpdateMethod == MovingUpdateMethod.Update)
            SetCartPosition(m_Position + speed * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (!Application.isPlaying)
            SetCartPosition(m_Position);
        else if (m_UpdateMethod == MovingUpdateMethod.LateUpdate)
            SetCartPosition(m_Position + m_Speed * Time.deltaTime);
    }

    void SetCartPosition(float distanceAlongPath)
    {
        if (m_Path != null)
        {
            m_Position = m_Path.StandardizeUnit(distanceAlongPath, m_PositionUnits);
            transform.position = m_Path.EvaluatePositionAtUnit(m_Position, m_PositionUnits);
            transform.rotation = m_Path.EvaluateOrientationAtUnit(m_Position, m_PositionUnits);
        }
    }

    public float GetPathLength() { return m_Path.PathLength; }

    public void StartAnimate(float speed)
    {
        m_Position = 0;
        m_Speed = speed;
    }
}