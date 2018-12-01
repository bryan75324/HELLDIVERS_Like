using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HELLDIVERS.UI;

[RequireComponent(typeof(UITweenCanvasAlpha))]
public class UIMissionReward : MonoBehaviour
{
    public UITweenCanvasAlpha CanvasTween { get { return m_CanvasTweener; } }

    [SerializeField] private Image m_Icon;
    [SerializeField] private Image m_CkeckMark;
    [SerializeField] private Text m_Description;
    private UITweenCanvasAlpha m_CanvasTweener;
    private eMissionType m_Type;

    public delegate void MissionRewardEventHolder(eMissionType type);

    public MissionRewardEventHolder OnTweenFinished;

    public void Initialize(eMissionType type)
    {
        m_Type = type;
        SetupDisplay(type);
    }

    public void DrawUI()
    {
        this.gameObject.SetActive(true);
        m_CanvasTweener.PlayForward();
    }

    private void SetupDisplay(eMissionType type)
    {
        switch (type)
        {
            case eMissionType.Tower:
                m_Icon.sprite = ResourceManager.m_Instance.LoadSprite(typeof(Sprite), UIHelper.MissionIconFolder, "icon_Mission_01");
                m_Description.text = "Activate Truth Transmitter";
                break;

            case eMissionType.KillMob:
                m_Icon.sprite = ResourceManager.m_Instance.LoadSprite(typeof(Sprite), UIHelper.MissionIconFolder, "icon_Mission_02");
                m_Description.text = "Exterminate Enemys";
                break;
        }
    }

    private void OnCanvasTweenFinished()
    {
        if (OnTweenFinished != null) OnTweenFinished(m_Type);
    }

    private void Awake()
    {
        m_CanvasTweener = this.GetComponent<UITweenCanvasAlpha>();
        m_CanvasTweener.OnTweenFinished += OnCanvasTweenFinished;
    }
}