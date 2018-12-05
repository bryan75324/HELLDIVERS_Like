using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HELLDIVERS.UI.InGame
{
    public class UIMissionCountInfo : UIMissionInfo
    {
        public event UIEventHolder OnFinished;

        public MissionKillMob CurrentMission { get { return m_currentMission; } }

        private MissionKillMob m_currentMission;

        protected override void OnInitialized(Mission mission)
        {
            m_currentMission = mission as MissionKillMob;
            SubscribeEvent(m_currentMission);
            RefreshCount();
        }

        private void SubscribeEvent(MissionKillMob mission)
        {
            mission.OnCount += RefreshCount;
        }

        private void UnsubscribeEvent(MissionKillMob mission)
        {
            mission.OnCount -= RefreshCount;
        }

        private void RefreshCount()
        {
            if (m_currentMission.CurrentAmount >= m_currentMission.Data.KillAmount)
            {
                m_CheckMark.gameObject.SetActive(true);
                m_Description.color = UIHelper.Player1_Color;
                m_Count.color = UIHelper.Player1_Color;
                if (OnFinished != null) OnFinished();
            }

            m_Count.text = string.Format("( {0} / {1} )", m_currentMission.CurrentAmount, m_currentMission.Data.KillAmount);
        }

        private void OnDestroy()
        {
            UnsubscribeEvent(m_currentMission);
        }
    }
}