using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HELLDIVERS.UI.InGame;

public class UIMissionBriefingIntroduction : MonoBehaviour {

    #region Event 
    public delegate void MissionIntroductionHolder();
    public event MissionIntroductionHolder OnSelect;
    public event MissionIntroductionHolder NotSelect;
    #endregion

    [SerializeField] private Transform m_UIRoot;
    [SerializeField] private UIMissionGroupInfo m_MissionGroupInfoPrefab;
    [SerializeField] private UIMissionCountInfo m_MissionCountInfoPrefab;
    [SerializeField] private Text m_Introduction;

    private UIMissionItem m_CurrentItem;

    private Dictionary<eMissionType, List<UIMissionInfo>> m_MissionElementMap = new Dictionary<eMissionType, List<UIMissionInfo>>();

    public void AddMissionInfo(Mission mission)
    {
        List<UIMissionInfo> pList;
        if (m_MissionElementMap.ContainsKey(mission.Type))
        {
            pList = m_MissionElementMap[mission.Type];
        }
        else
        {
            pList = new List<UIMissionInfo>();
            m_MissionElementMap.Add(mission.Type, pList);
        }

        switch (mission.Type)
        {
            case eMissionType.Tower:

                if (pList.Count > 0)
                {
                    UIMissionGroupInfo missionGroupUI = pList[0] as UIMissionGroupInfo;
                    missionGroupUI.AddMission(mission);
                }
                else
                {
                    UIMissionGroupInfo missionGroupUI = Instantiate(m_MissionGroupInfoPrefab, m_UIRoot);
                    missionGroupUI.GetComponent<UIMissionItem>().Init(mission.Type);
                    missionGroupUI.Initialize(mission);
                    missionGroupUI.gameObject.SetActive(true);
                    pList.Add(missionGroupUI);
                }

                break;

            case eMissionType.KillMob:
                UIMissionCountInfo missionCountUI = Instantiate(m_MissionCountInfoPrefab, m_UIRoot);
                missionCountUI.GetComponent<UIMissionItem>().Init(mission.Type);
                missionCountUI.Initialize(mission);
                missionCountUI.gameObject.SetActive(true);
                pList.Add(missionCountUI);
                break;
        }

        if (m_MissionElementMap.Count == 1) EventSystem.current.SetSelectedGameObject(m_MissionElementMap[mission.Type][0].gameObject);
    }

    private void Update()
    {
        if(m_CurrentItem.m_bCouldSelect == true)
        {
            if (OnSelect != null) OnSelect();
        }
    }
    [ContextMenu("Test")]
    public void Selected(UIMissionItem item)
    {
        if(item.m_bCouldSelect)
        {
            m_CurrentItem = item;
            if (OnSelect != null) OnSelect();
        }
        else
        {
            m_CurrentItem = item;
            if (NotSelect != null) NotSelect();
        }
        m_Introduction.text = item.m_Introduction;
    }
}
