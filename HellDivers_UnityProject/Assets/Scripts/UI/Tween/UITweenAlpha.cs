using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HELLDIVERS.UI
{
    public abstract class UITweenAlpha : MonoBehaviour
    {
        #region Property

        /// <summary>
        /// Tween progress time lenght.
        /// </summary>
        public float TimeLenght
        {
            get
            { return m_time; }
            set
            {
                if (value <= 0) m_time = 0.01f;
                else m_time = value;
            }
        }

        public float CurrentTime { get { return currentTime; } }

        #endregion Property

        #region Serialize Field

        [SerializeField] [Range(0.01f, 100)] protected float m_time = 1;
        [SerializeField] protected AnimationCurve m_AnimaCurve;

        #endregion Serialize Field

        #region Events

        /// <summary>
        /// Event on tween animation finished;
        /// </summary>
        public event UIEventHolder OnTweenFinished;

        /// <summary>
        /// Event on tween animation start.
        /// </summary>
        public event UIEventHolder OnTweenStart;

        // Do tween progress
        private UIEventHolder OnTween;

        #endregion Events

        #region MonoBehaviour

        // Initialization
        private void Awake()
        {
            this.enabled = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (OnTween != null) OnTween();
        }

        #endregion MonoBehaviour

        #region Public Function

        /// <summary>
        /// Play forward, strat at 0 percent of animation curve.
        /// </summary>
        [ContextMenu("Play Forward")]
        public void PlayForward()
        {
            Play(0, 1);
        }

        /// <summary>
        /// Play backward, start at 100 percent of animation curve setting.
        /// </summary>
        [ContextMenu("Play Backward")]
        public void PlayBackward()
        {
            Play(1, 0);
        }

        /// <summary>
        /// Play tween animation by manual setting.
        /// </summary>
        /// <param name="startPercentage">Strat percentage of animation curve setting</param>
        /// <param name="endPercentage">End percentage of animation curve setting</param>
        public void Play(float startPercentage, float endPercentage)
        {
            this.enabled = true;

            currentTime = startPercentage;
            endTime = endPercentage;
            speed = 1 / m_time;

            if (startPercentage <= endPercentage)
            {
                OnTween = DoForwardProgress;
            }
            else
            {
                OnTween = DoBackwardProgress;
            }

            if (OnTweenStart != null) OnTweenStart();
        }

        #endregion Public Function

        #region Progress

        protected virtual void OnProgress()
        {
        }

        private void DoForwardProgress()
        {
            if (currentTime < endTime)
            {
                currentTime += Time.deltaTime * speed;
                if (currentTime > endTime) currentTime = endTime;
                OnProgress();
            }
            else
            {
                if (OnTweenFinished != null) OnTweenFinished();
                OnTween = null;
                this.enabled = false;
            }
        }

        private void DoBackwardProgress()
        {
            if (currentTime > endTime)
            {
                currentTime -= Time.deltaTime * speed;
                if (currentTime < endTime) currentTime = endTime;
                OnProgress();
            }
            else
            {
                if (OnTweenFinished != null) OnTweenFinished();
                OnTween = null;
                this.enabled = false;
            }
        }

        private float currentTime;
        private float endTime;
        private float speed;

        #endregion Progress
    }
}