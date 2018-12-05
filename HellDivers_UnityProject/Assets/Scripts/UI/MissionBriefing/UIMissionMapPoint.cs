using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionMapPoint : MonoBehaviour {

    [SerializeField] private Image m_Icon;
    [SerializeField] private Color m_NormalColor;
    [SerializeField] private Color m_HighlightColor;

    private RectTransform m_RectTransform;
    private float m_OriWidth;
    private float m_OriHeight;
    private float m_Width = 60f;
    private float m_Height = 60f;

    public GameObject SpawnPoint { get { return m_SpawnPoint; } set { m_SpawnPoint = value; } }

    private GameObject m_SpawnPoint;

    public void Init(GameObject target, eMapPointType type)
    {
        switch (type)
        {
            case eMapPointType.SPAWNPOINT:
                m_RectTransform = this.GetComponent<RectTransform>();
                m_OriWidth = m_RectTransform.sizeDelta.x;
                m_OriHeight = m_RectTransform.sizeDelta.y;
                m_SpawnPoint = target;
                break;
            case eMapPointType.MISSIONTOWER:
                UIMissionBriefing.Instance.MissionIntroduction.OnSelect += Selected;
                UIMissionBriefing.Instance.MissionIntroduction.NotSelect += NotSelected;
                break;
        }
    }

    private void Awake()
    {
        m_Icon.color = m_NormalColor;
    }

    public void Highlight()
    {
        m_RectTransform.sizeDelta = new Vector2(m_Width,m_Height);
    }

    public void Normal()
    {
        m_RectTransform.sizeDelta = new Vector2(m_OriWidth, m_OriHeight);
    }

    public void Selected()
    {
        m_Icon.color = m_HighlightColor;
    }
    public void NotSelected()
    {
        m_Icon.color = m_NormalColor;
    }
}
