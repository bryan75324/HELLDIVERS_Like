using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HELLDIVERS.UI;
using HELLDIVERS.UI.InGame;

public class UIMissionBriefing : MonoBehaviour
{
    [SerializeField] private UITweenCanvasAlpha m_UITweenCanvasAlpha;
    [SerializeField] private UIMissionBriefingMap m_Map;
    [SerializeField] private UIMissionBriefingIntroduction m_MissionIntroduction;
    [SerializeField] private Transform m_PanelMap;
    [SerializeField] private Image m_Backround;
    [SerializeField] private Color m_BackroundColor;

    public static UIMissionBriefing Instance { get; private set; }

    public UIMissionBriefingMap Map { get { return m_Map; } }
    public UIMissionBriefingIntroduction MissionIntroduction { get { return m_MissionIntroduction; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
        
        m_Map = Instantiate(m_Map, m_PanelMap);
    }

    public void Init(MapInfo mapInfo)
    {
        for (int i = 0; i < mapInfo.SpawnPos.Count; i++)
        {
            AddPoint(mapInfo.SpawnPos[i].gameObject, eMapPointType.SPAWNPOINT);
        }
        m_Map.Concentric.OnClick += ComfirmSpawnPosition;
    }

    private void Update()
    {
        
    }

    public void DrawUI()
    {
        this.transform.SetAsLastSibling();
    }

    public void AddMission(Mission mission)
    {
        m_MissionIntroduction.AddMissionInfo(mission);
    }

    public void AddPoint(GameObject target, eMapPointType type)
    {
        m_Map.AddPointPrefab(target, type);
    }

    public void MissionSelected()
    {
        //m_MissionIntroduction.Selected();
    }

    public void ComfirmSpawnPosition()
    {
        if (m_Map.ComfirmSpawnPosition())
        {
            this.transform.SetAsLastSibling();
            m_UITweenCanvasAlpha.PlayBackward();
            m_UITweenCanvasAlpha.OnTweenFinished += () => { Destroy(this.gameObject); };
            m_Map.Concentric.OnClick -= ComfirmSpawnPosition;
        }
        return;
    }
}
