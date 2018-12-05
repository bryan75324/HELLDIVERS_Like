using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HELLDIVERS.UI
{
    [RequireComponent(typeof(SoundManager))]
    [RequireComponent(typeof(UITweenCanvasAlpha))]
    public class UIPlayerRewardDetail : MonoBehaviour
    {
        public UITweenCanvasAlpha CanvasTween { get { return m_CanvasTween; } }

        [SerializeField] private Text m_textLabel;
        [SerializeField] private Text m_textNumber;
        private UITweenCanvasAlpha m_CanvasTween;
        private SoundManager m_SoundManager;
        private int m_iNumber;
        private int m_iCurrentNum;
        private int m_iEvulateRate;

        public event UIEventHolder OnCountNumStart;

        public event UIEventHolder OnCountNumStop;

        private void Awake()
        {
            m_CanvasTween = this.GetComponent<UITweenCanvasAlpha>();
            m_SoundManager = this.GetComponent<SoundManager>();

            SoundDataSetting soundData = Resources.Load("Sounds/Reward/Detial_SoundDataSetting") as SoundDataSetting;
            m_SoundManager.SetAudioClips(soundData.SoundDatas);
        }

        public void Initialize(string label, int number)
        {
            m_textLabel.text = label;
            m_iCurrentNum = 0;
            m_iNumber = number;
            m_iEvulateRate = Mathf.FloorToInt(number * 0.1f);
            if (m_iEvulateRate < 1) m_iEvulateRate = 1;
            m_textNumber.text = m_iCurrentNum.ToString();
        }

        public void StartCountNum()
        {
            if (OnCountNumStart != null) OnCountNumStart();
            StartCoroutine(EvulateNum());
        }

        private IEnumerator EvulateNum()
        {
            m_SoundManager.PlayOnce(0);
            m_textNumber.text = m_iCurrentNum.ToString();
            while (m_iCurrentNum < m_iNumber)
            {
                m_iCurrentNum += m_iEvulateRate;
                if (m_iCurrentNum > m_iNumber) m_iCurrentNum = m_iNumber;
                m_textNumber.text = m_iCurrentNum.ToString();
                yield return null;
            }
            m_SoundManager.Stop();
            if (OnCountNumStop != null) OnCountNumStop();
        }
    }
}