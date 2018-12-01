using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HELLDIVERS.UI
{
    public class UITweenImageAlpha : MonoBehaviour
    {
        public float TimeLenght
        {
            get
            { return m_Time; }
            set
            {
                if (value < 0) m_Time = 0;
                else m_Time = value;
            }
        }

        [SerializeField] private Image m_Image;
        [SerializeField] private float m_Time;
        [SerializeField] private float m_StartAlpha;
        [SerializeField] private float m_EndAlpha;

        #region Events

        /// <summary>
        /// Event on tween animation finished;
        /// </summary>
        public event UIEventHolder OnTweenFinished;

        /// <summary>
        /// Event on tween animation start.
        /// </summary>
        public event UIEventHolder OnTweenStart;

        #endregion Events
    }
}