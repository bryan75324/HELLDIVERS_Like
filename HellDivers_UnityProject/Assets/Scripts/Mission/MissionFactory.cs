using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HELLDIVERS.UI.InGame;

public class MissionFactory
{
    public Mission CreateMission(eMissionType type, Transform pos = null, uint level = 0)
    {
        Mission mission = null;

        switch (type)
        {
            case eMissionType.Tower:
                GameObject tower = new GameObject("MissionTower");
                MissionTower missionTower = tower.AddComponent<MissionTower>();
                string towerDataFileName = string.Format("MissionTowerSetting_{0}", level);
                MissionTowerData data = ResourceManager.m_Instance.LoadData(typeof(MissionTowerData), "Mission/Settings", towerDataFileName) as MissionTowerData;
                missionTower.Initialize(data);
                mission = missionTower;

                if (pos != null)
                {
                    tower.transform.SetParent(pos);
                    tower.transform.position = pos.position;
                }

                UIInGameMain.Instance.AddMissionInfo(mission);
                UIInGameMain.Instance.AddDynamicMissionMsg(missionTower);
                UIInGameMain.Instance.AddMapPoint(tower, eMapPointType.MISSIONTOWER);
                break;

            case eMissionType.KillMob:
                GameObject killMob = new GameObject("MissionKillMob");
                MissionKillMob missionKill = killMob.AddComponent<MissionKillMob>();
                string killMobDataFileName = string.Format("MissionKillMobSetting_{0}", level);
                MissionKillMobData killData = ResourceManager.m_Instance.LoadData(typeof(MissionKillMobData), "Mission/Settings", killMobDataFileName) as MissionKillMobData;
                missionKill.Initialize(killData);
                mission = missionKill;

                UIInGameMain.Instance.AddMissionInfo(mission);
                break;
        }

        return mission;
    }
}