using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HELLDIVERS.UI.InGame
{
    public class UIInGameMain : MonoBehaviour
    {
        public static UIInGameMain Instance { get; private set; }
        public float UIMissionCompleteTimeLenght { get { return m_PanelMissionCompleted.TimeLenght; } }

        [SerializeField] private UIPanelPlayerInfo m_PanelPlayerInfo;
        [SerializeField] private UIPanelStratagemAct m_PanelStratagemAct;
        [SerializeField] private UIDynamicHpBar m_DynamicHpBar;
        [SerializeField] private UIMissionBriefing m_PanelMissionBriefing;
        [SerializeField] private UIPanelMission m_PanelMissionInfo;
        [SerializeField] private UIDynamicMissionMsg m_DynamicMissionMsg;
        [SerializeField] private UIPanelMissionComplete m_PanelMissionCompleted;
        [SerializeField] private UIPanelMissionFailed m_PanelMissionFaild;
        [SerializeField] private UIPanelRadar m_PanelRadar;
        [SerializeField] private UIPanelMap m_PanelMap;
        private Dictionary<Player, UIDynamicHpBar> m_DynamicHpBarMap;

        public void Init()
        {
            m_PanelPlayerInfo = Instantiate(m_PanelPlayerInfo, this.transform);
            m_PanelStratagemAct = Instantiate(m_PanelStratagemAct, this.transform);
            m_PanelRadar = Instantiate(m_PanelRadar, this.transform);
            m_PanelMap = Instantiate(m_PanelMap, this.transform);
            m_PanelRadar.Init();
            m_PanelMap.Init();
            m_PanelMissionInfo = Instantiate(m_PanelMissionInfo, this.transform);
            m_PanelMissionBriefing = Instantiate(m_PanelMissionBriefing, this.transform);
            m_PanelMissionBriefing.Init(MapInfo.Instance);
            m_PanelMissionCompleted = Instantiate(m_PanelMissionCompleted, this.transform);
            m_PanelMissionFaild = Instantiate(m_PanelMissionFaild, this.transform);
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(this.gameObject);
            }
            m_DynamicHpBarMap = new Dictionary<Player, UIDynamicHpBar>();
        }

        public void DrawGameUI()
        {
            m_PanelPlayerInfo.gameObject.SetActive(true);
            m_PanelStratagemAct.gameObject.SetActive(true);
            foreach (KeyValuePair<Player, UIDynamicHpBar> playerHpBar in m_DynamicHpBarMap)
            {
                playerHpBar.Value.gameObject.SetActive(true);
            }
        }

        public void AddPlayer(Player player)
        {
            m_PanelPlayerInfo.AddPlayer(player);
            m_PanelStratagemAct.AddPlayer(player);
            m_PanelRadar.AddPointPrefab(player, eMapPointType.PLAYER);
            m_PanelMap.AddPointPrefab(player, eMapPointType.PLAYER);
            AddDynamicHpBar(player);
        }

        public void AddRadarPoint(GameObject target, eMapPointType type)
        {
            m_PanelRadar.AddPointPrefab(target, type);
        }

        public void AddMapPoint(GameObject target, eMapPointType type)
        {
            m_PanelMissionBriefing.AddPoint(target, type);
            m_PanelMap.AddPointPrefab(target, type);
        }

        public void AddDynamicHpBar(Player player)
        {
            if (m_DynamicHpBarMap.ContainsKey(player)) return;

            UIDynamicHpBar hpBar = Instantiate(m_DynamicHpBar, this.transform).GetComponent<UIDynamicHpBar>();
            hpBar.Init(player);
            m_DynamicHpBarMap.Add(player, hpBar);
            hpBar.gameObject.SetActive(false);
        }

        public void AddDynamicMissionMsg(MissionTower mission)
        {
            UIDynamicMissionMsg missionMsg = Instantiate(m_DynamicMissionMsg, this.transform);
            missionMsg.Init(mission);
        }

        public void AddMissionInfo(Mission mission)
        {
            m_PanelMissionInfo.AddMissionInfo(mission);
            m_PanelMissionBriefing.AddMission(mission);
        }

        public void DrawMissionBrefingUI()
        {
            m_PanelMissionBriefing.DrawUI();
        }

        public void DrawMissionCompletedUI()
        {
            m_PanelMissionCompleted.transform.SetAsLastSibling();
            m_PanelMissionCompleted.gameObject.SetActive(true);
            m_PanelMissionCompleted.StartUI();
        }

        public void DrawMissionFailedUI()
        {
            m_PanelMissionFaild.gameObject.SetActive(true);
            m_PanelMissionFaild.transform.SetAsLastSibling();
            m_PanelMissionFaild.StartUI();
        }
    }
}