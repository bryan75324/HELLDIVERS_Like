using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HELLDIVERS.UI.InGame
{
    public class UIDynamicMissionMsg : MonoBehaviour
    {
        public MissionTower CurrentTower { get; private set; }
        public string Message { get { return m_textMessage.text; } set { m_textMessage.text = value; } }

        [SerializeField] private Text m_textMessage;
        [SerializeField] private Text m_textTime;
        [SerializeField] private Transform m_arrowRoot;
        [SerializeField] private Image m_imgArrow;
        [SerializeField] private Color m_BrightColor;
        [SerializeField] private Color m_DarkColor;
        [SerializeField] private Vector3 m_PositionOffset;
        private List<Image> m_Arrows;

        public void Init(MissionTower tower)
        {
            CurrentTower = tower;

            m_Arrows = new List<Image>();
            for (int i = 0; i < CurrentTower.Codes.Length; i++)
            {
                Image arrow = CreateArrow(CurrentTower.Codes[i]);
                arrow.gameObject.SetActive(true);
                m_Arrows.Add(arrow);
            }
            m_arrowRoot.gameObject.SetActive(false);

            SubscribeEvent();
            ToIdle();
        }

        private Image CreateArrow(eCode code)
        {
            Image codeImg = Instantiate(m_imgArrow, m_arrowRoot);
            switch (code)
            {
                case eCode.Up:
                    codeImg.transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;

                case eCode.Down:
                    codeImg.transform.rotation = Quaternion.Euler(0, 0, -90);
                    break;

                case eCode.Left:
                    codeImg.transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;

                case eCode.Right:
                    codeImg.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
            return codeImg;
        }

        private void Update()
        {
            if (CurrentTower != null)
            {
                Vector3 pos = CurrentTower.transform.position + m_PositionOffset;
                this.transform.position = Camera.main.WorldToScreenPoint(pos);
            }
        }

        private void OnDestroy()
        {
            UnsubscribeEvent();
        }

        private void SubscribeEvent()
        {
            CurrentTower.CodeMechine.OnStart += RefreshCodeImg;
            CurrentTower.CodeMechine.OnChecking += RefreshCodeImg;
            CurrentTower.CodeMechine.OnGetResult += ToActivatingState;
            CurrentTower.OnActivating += RefreshTimer;
            CurrentTower.OnStop += ToIdle;
            CurrentTower.OnActive += DoDestory;
        }

        private void UnsubscribeEvent()
        {
            CurrentTower.CodeMechine.OnStart -= RefreshCodeImg;
            CurrentTower.CodeMechine.OnChecking -= RefreshCodeImg;
            CurrentTower.CodeMechine.OnGetResult -= ToActivatingState;
            CurrentTower.OnActivating -= RefreshTimer;
            CurrentTower.OnStop -= ToIdle;
            CurrentTower.OnActive -= DoDestory;
        }

        private void ToIdle()
        {
            m_textMessage.text = "Mission Tower";
            m_textTime.gameObject.SetActive(false);
            m_arrowRoot.gameObject.SetActive(false);
        }

        private void RefreshCodeImg()
        {
            Color lightColor = (CurrentTower.CurrentPlayer != null) ? UIHelper.GetPlayerColor(CurrentTower.CurrentPlayer.SerialNumber) : m_BrightColor;
            m_textMessage.text = "Input password";
            m_arrowRoot.gameObject.SetActive(CurrentTower.CodeMechine.IsChecking);
            if (CurrentTower.CodeMechine.IsChecking)
            {
                foreach (Image img in m_Arrows)
                {
                    img.color = m_DarkColor;
                }

                for (int i = 0; i < CurrentTower.CodeMechine.Step; i++)
                {
                    m_Arrows[i].color = lightColor;
                }
            }
        }

        private void ToActivatingState()
        {
            m_textMessage.text = "Activating";
            m_textTime.gameObject.SetActive(true);
            m_arrowRoot.gameObject.SetActive(false);
        }

        private void RefreshTimer()
        {
            string minutes = Mathf.Floor(CurrentTower.ActTimeCountDown / 60).ToString("00");
            string seconds = (CurrentTower.ActTimeCountDown % 60).ToString("00");
            m_textTime.text = string.Format("{0}:{1}", minutes, seconds);
        }

        private void DoDestory()
        {
            Destroy(this.gameObject);
        }
    }
}