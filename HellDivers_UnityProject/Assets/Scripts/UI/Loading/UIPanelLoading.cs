using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HELLDIVERS.UI;

[RequireComponent(typeof(UITweenCanvasAlpha))]
public class UIPanelLoading : MonoBehaviour
{
    [SerializeField] private UITweenImageAlpha m_BlackCardTween;
    [SerializeField] private Image m_Background;
    [SerializeField] private Image m_LoadingBar;
    [SerializeField] private Image m_LoadingFill;
    [SerializeField] private Text m_TipText;
    private UITweenCanvasAlpha m_Tweener;

    private CanvasGroup m_CanvasGroup;
    private float m_targetAlpha;
    private float m_alphaLerp = 0.05f;
    private float m_targetFillAmount;

    private void Awake()
    {
        m_Tweener = this.GetComponent<UITweenCanvasAlpha>();
    }

    [ContextMenu("Fade In")]
    public void FadeIn()
    {
        this.gameObject.SetActive(true);
        m_Tweener.CanvasGroup.alpha = 1;
        m_BlackCardTween.gameObject.SetActive(true);
        m_BlackCardTween.PlayBackward();
        m_BlackCardTween.OnTweenFinished += HideBlackCard;
    }

    private void HideBlackCard()
    {
        m_BlackCardTween.OnTweenFinished -= HideBlackCard;
        m_BlackCardTween.gameObject.SetActive(false);
    }

    [ContextMenu("Fade Out")]
    public void FadeOut()
    {
        this.gameObject.SetActive(true);
        m_Tweener.PlayBackward();
    }

    public void SetLoadingBarProcess(float amount)
    {
        m_LoadingFill.fillAmount = Mathf.Clamp01(amount);
    }
}