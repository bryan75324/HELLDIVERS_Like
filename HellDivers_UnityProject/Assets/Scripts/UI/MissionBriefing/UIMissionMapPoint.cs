using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionMapPoint : MonoBehaviour {

    [SerializeField] private Image m_Icon;
    [SerializeField] private Color m_NormalColor;
    [SerializeField] private Color m_HighlightColor;

    public GameObject Tower { get { return m_Tower; } set { m_Tower = value; } }

    private GameObject m_Tower;

    public void Init(GameObject target)
    {
        m_Tower = target;
    }

    private void Awake()
    {
        m_Icon.color = m_NormalColor;
    }

    public void Highlight()
    {
        m_Icon.color = m_HighlightColor;
    }

    public void Normal()
    {
        m_Icon.color = m_NormalColor;
    }
}
