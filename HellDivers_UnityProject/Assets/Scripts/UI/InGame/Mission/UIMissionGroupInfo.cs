using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HELLDIVERS.UI;

namespace HELLDIVERS.UI.InGame
{
    public class UIMissionGroupInfo : UIMissionInfo
    {
        public event UIEventHolder OnFinished;

        protected LinkedList<Mission> m_Missions = new LinkedList<Mission>();

        protected override void OnInitialized(Mission mission)
        {
            if (m_Missions.Count > 0) m_Missions.Clear();
            m_Missions.AddFirst(mission);
            SubscribeEvent(mission);
            RefreshCount();
        }

        public bool AddMission(Mission mission)
        {
            if (mission.Type != mission.Type) return false;
            m_Missions.AddLast(mission);
            SubscribeEvent(mission);
            RefreshCount();
            return true;
        }

        private void SubscribeEvent(Mission mission)
        {
            mission.OnMissionComplete += RefreshCount;
        }

        private void UnsubscribeEvent(Mission mission)
        {
            mission.OnMissionComplete -= RefreshCount;
        }

        private void RefreshCount()
        {
            int completeCount = 0;
            foreach (Mission mission in m_Missions)
            {
                if (mission.IsFinished) completeCount++;
            }

            m_Count.text = string.Format("( {0} / {1} )", completeCount, m_Missions.Count); ;

            if (completeCount == m_Missions.Count)
            {
                m_CheckMark.gameObject.SetActive(true);
                m_Description.color = UIHelper.Player1_Color;
                m_Count.color = UIHelper.Player1_Color;
                if (OnFinished != null) OnFinished();
            }
        }

        private void OnDestroy()
        {
            foreach (Mission mission in m_Missions)
            {
                UnsubscribeEvent(mission);
            }
        }
    }
}