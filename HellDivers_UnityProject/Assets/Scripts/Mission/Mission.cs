using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eMissionType
{
    Tower,
    KillMob,
}

public enum eMissionPriority
{
    Main, Side
}

[System.Serializable]
public struct MissionReward
{
    public int EXP;
    public int Money;
}

public delegate void MissionEventHolder();

public delegate void MissionEventTracker(Mission mission);

public class Mission : MonoBehaviour
{
    public eMissionType Type { get { return m_Type; } }
    public eMissionPriority Priority { get { return m_Priority; } }
    public bool IsFinished { get { return m_bFinished; } }
    public MissionReward Reward { get { return m_Reward; } }

    protected eMissionType m_Type;
    protected bool m_bFinished;
    protected eMissionPriority m_Priority;
    protected MissionReward m_Reward;

    public MissionEventHolder OnMissionStart;
    public MissionEventHolder OnMissionComplete;

    public void StartMission()
    {
        OnStart();
        if (OnMissionStart != null) OnMissionStart();
    }

    protected virtual void OnStart()
    {
    }

    protected void CompleteMission()
    {
        if (OnMissionComplete != null) OnMissionComplete();
    }
}