using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HELLDIVERS.UI
{
    public class UIPlayerReward : MonoBehaviour
    {
        public PlayerInfo CurrentPlayerInfo { get { return currentPlayer; } }
        public PlayerRecord CurrentPlayerRecord { get { return currentRecord; } }

        public event UIEventHolder OnDrawUIFinished;

        private PlayerInfo currentPlayer;
        private PlayerRecord currentRecord;
        private Animator m_Animator;
        [SerializeField] private Image m_Header;
        [SerializeField] private Image m_RankIcon;
        [SerializeField] private Text m_RankText;
        [SerializeField] private Text m_PlayerName;
        [SerializeField] private UIPlayerRewardExpBar m_ExpBar;
        [SerializeField] private UIPlayerRewardDetail m_RewardDetailPrefab;
        private List<UIPlayerRewardDetail> m_Details = new List<UIPlayerRewardDetail>();

        public void Initialize(PlayerInfo player, PlayerRecord record, MissionReward missionReward, int serialNumber = 1)
        {
            currentPlayer = player;
            currentRecord = record;

            Color playerColor = UIHelper.GetPlayerColor(serialNumber);
            m_Header.color = playerColor;
            m_RankText.color = playerColor;

            m_PlayerName.text = currentPlayer.Username;
            m_RankText.text = currentPlayer.Rank.ToString();
            RefreshRankIcon(currentPlayer.Rank);

            m_ExpBar = Instantiate(m_ExpBar, this.transform);
            m_ExpBar.OnRankUpdate += RefreshRankInfo;
            m_ExpBar.Initialize(currentPlayer.Exp, currentPlayer.Exp + record.Exp + missionReward.EXP, currentPlayer.Rank);

            CreateDetail("DEATH", currentRecord.TimesOfDeath);
            CreateDetail("KILLS", currentRecord.NumOfKills);
            CreateDetail("SHOTS", currentRecord.Shots);
            CreateDetail("MONEY", currentRecord.Money + missionReward.Money);
        }

        private void CreateDetail(string label, int number)
        {
            UIPlayerRewardDetail detail = Instantiate(m_RewardDetailPrefab, this.transform);
            detail.Initialize(label, number);
            m_Details.Add(detail);
        }

        public void DrawUI()
        {
            StartCoroutine(DoDraw());
        }

        private IEnumerator DoDraw()
        {
            yield return new WaitUntil
                (() =>
                    {
                        if (m_Animator.IsInTransition(0)) return false;

                        var state = m_Animator.GetCurrentAnimatorStateInfo(0);
                        if (state.normalizedTime >= 0.98f) return true;
                        return false;
                    }
                );

            m_ExpBar.CanvasTween.PlayForward();
            yield return new WaitForSeconds(m_ExpBar.CanvasTween.TimeLenght);

            m_ExpBar.DoEvaluate();

            for (int i = 0; i < m_Details.Count; i++)
            {
                var tween = m_Details[i].CanvasTween;
                tween.PlayForward();
                yield return new WaitForSeconds(tween.TimeLenght);
                m_Details[i].StartCountNum();
            }

            if (OnDrawUIFinished != null) OnDrawUIFinished();
        }

        private void RefreshRankInfo()
        {
            RefreshRankIcon(m_ExpBar.CurrentRank);
            m_RankText.text = m_ExpBar.CurrentRank.ToString();
        }

        private void RefreshRankIcon(int rank)
        {
            string fileName = string.Format("icon_rank_{0}", rank.ToString("00"));
            m_RankIcon.sprite = ResourceManager.m_Instance.LoadSprite(typeof(Sprite), UIHelper.RankIconFolder, fileName);
        }

        private void Awake()
        {
            m_Animator = this.GetComponent<Animator>();
        }

        private void OnDestroy()
        {
            m_ExpBar.OnRankUpdate -= RefreshRankInfo;
        }
    }
}