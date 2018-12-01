using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HELLDIVERS.UI
{
    [RequireComponent(typeof(UITweenCanvasAlpha))]
    public class UIPlayerRewardExpBar : MonoBehaviour
    {
        public UITweenCanvasAlpha CanvasTween { get { return m_CanvasTween; } }
        public int CurrentRank { get { return m_CurrentRank.Rank; } }

        public delegate void ExpBarEventHolder();

        public event ExpBarEventHolder OnRankUpdate;

        [SerializeField] private Text m_textExp;
        [SerializeField] private Image m_imgFill;
        private UITweenCanvasAlpha m_CanvasTween;
        private int m_CurrentExp;
        private int m_TargetExp;
        private RankData m_CurrentRank;
        private RankData m_NextRank;
        private float m_EvaluateAmount;

        public void Initialize(int startAmount, int targetAmount, int startrank = 1)
        {
            m_CurrentExp = startAmount;
            m_TargetExp = targetAmount;

            m_CurrentRank = GameData.Instance.RankTable[startrank];
            m_NextRank = GameData.Instance.RankTable[m_CurrentRank.Rank + 1];

            RefreshRankData();
            RefreshBar();
        }

        public void DoEvaluate()
        {
            StartCoroutine(EvaluateExpToTarget());
        }

        private void Awake()
        {
            m_CanvasTween = this.GetComponent<UITweenCanvasAlpha>();
        }

        private IEnumerator EvaluateExpToTarget()
        {
            while (m_CurrentExp < m_TargetExp)
            {
                RefreshRankData();
                RefreshBar();
                m_CurrentExp += Mathf.FloorToInt(m_EvaluateAmount);
                if (m_CurrentExp > m_TargetExp) m_CurrentExp = m_TargetExp;
                yield return null;
            }
        }

        private void RefreshRankData()
        {
            while (m_CurrentExp >= m_NextRank.Exp)
            {
                m_CurrentRank = m_NextRank;
                m_NextRank = GameData.Instance.RankTable[m_NextRank.Rank + 1];
                if (OnRankUpdate != null) OnRankUpdate();
            }
        }

        private void RefreshBar()
        {
            if (m_CurrentExp >= m_NextRank.Exp) RefreshRankData();
            float maxAmount = m_NextRank.Exp - m_CurrentRank.Exp;
            m_EvaluateAmount = maxAmount * 0.01f;
            float fillAmount = m_CurrentExp - m_CurrentRank.Exp;
            m_textExp.text = string.Format("{0}/{1}", fillAmount, maxAmount);
            m_imgFill.fillAmount = Mathf.Clamp01(fillAmount / maxAmount);
        }
    }
}