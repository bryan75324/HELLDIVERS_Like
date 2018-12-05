using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HELLDIVERS.UI.InGame;

public class MissionManager
{
    public static MissionManager Instance { get; private set; }
    public int MainMissionCount { get; private set; }
    public Dictionary<eMissionPriority, List<Mission>> Missions { get { return m_Missions; } }

    private MissionFactory m_Factory = new MissionFactory();
    private Dictionary<eMissionPriority, List<Mission>> m_Missions = new Dictionary<eMissionPriority, List<Mission>>();
    private List<Transform> m_TowerPositions;

    public void Init()
    {
        Instance = this;

        if (MapInfo.Instance.TowerPos != null && MapInfo.Instance.TowerPos.Count > 0)
        {
            var MapTowerPositions = MapInfo.Instance.TowerPos;
            Transform[] towerPositions = new Transform[MapTowerPositions.Count];
            MapInfo.Instance.TowerPos.CopyTo(towerPositions);

            m_TowerPositions = new List<Transform>();
            foreach (var t in towerPositions)
            {
                m_TowerPositions.Add(t);
            }
        }
    }

    public void StartAllMission()
    {
        foreach (var missions in m_Missions)
        {
            foreach (Mission m in missions.Value)
            {
                m.StartMission();
            }
        }
    }

    public void CreateTowerMissions(uint num, uint level = 0)
    {
        if (m_TowerPositions == null || m_TowerPositions.Count < 0) return;

        num = (num > m_TowerPositions.Count) ? (uint)m_TowerPositions.Count : num;
        for (int i = 0; i < num; i++)
        {
            int index = Random.Range(0, m_TowerPositions.Count - 1);
            CreateMission(eMissionType.Tower, m_TowerPositions[index], level);
            m_TowerPositions.RemoveAt(index);
        }
    }

    public void CreateMission(eMissionType type, Transform pos = null, uint level = 0)
    {
        Mission mission = m_Factory.CreateMission(type, pos, level);

        List<Mission> pList;
        if (m_Missions.ContainsKey(mission.Priority))
        {
            pList = m_Missions[mission.Priority];
        }
        else
        {
            pList = new List<Mission>();
            m_Missions.Add(mission.Priority, pList);
        }

        if (mission.Priority == eMissionPriority.Main)
        {
            MainMissionCount++;
            mission.OnMissionComplete += () => { MainMissionCount--; };
        }

        pList.Add(mission);
    }
}