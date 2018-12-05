using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HELLDIVERS.UI
{
    [RequireComponent(typeof(UITweenCanvasAlpha))]
    public class UIMissionTitle : MonoBehaviour
    {
        public UITweenCanvasAlpha CanavasTween { get { return m_CanvasTween; } }

        [SerializeField] private Text m_Title;
        private UITweenCanvasAlpha m_CanvasTween;

        private void Awake()
        {
            m_CanvasTween = this.GetComponent<UITweenCanvasAlpha>();
        }

        public void Initialize(bool success)
        {
            if (success)
            {
                m_Title.text = "MISSION COMPLETE";
            }
            else
            {
                m_Title.text = "MISSION FAILED";
            }
        }
    }
}