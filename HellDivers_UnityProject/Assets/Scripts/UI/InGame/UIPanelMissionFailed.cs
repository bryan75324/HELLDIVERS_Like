using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HELLDIVERS.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanelMissionFailed : MonoBehaviour
{
    [SerializeField] private Image m_Indicator;
    [SerializeField] private Button m_BtnRestart;
    [SerializeField] private Button m_BtnAbandon;
    [SerializeField] private float m_StartAlpha;
    [SerializeField] private float m_TargetAlpha = 0.5f;
    private CanvasGroup m_CanvasGroup;

    private UIEventHolder DoState;

    private void Awake()
    {
        this.gameObject.SetActive(false);
        m_CanvasGroup = this.GetComponent<CanvasGroup>();
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (DoState != null) DoState();
    }

    public void StartUI()
    {
        this.gameObject.SetActive(true);
        m_CanvasGroup.alpha = m_StartAlpha;
        EventSystem.current.SetSelectedGameObject(m_BtnRestart.gameObject);
        DoState = DoFadeIn;
    }

    private void DoFadeIn()
    {
        m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, m_TargetAlpha, Time.deltaTime);
        if (m_CanvasGroup.alpha >= m_TargetAlpha - 0.01f)
        {
            m_CanvasGroup.alpha = m_TargetAlpha;
            DoState = null;
        }
    }

    public void ClickRestart()
    {
        if (SceneController.Instance != null) SceneController.Instance.ToGameScene();
    }

    public void ClickAbandon()
    {
        GameMain.Instance.MissionAbandon();
    }

    public void UpdateIndicator()
    {
        m_Indicator.transform.SetParent(EventSystem.current.currentSelectedGameObject.transform);
    }
}