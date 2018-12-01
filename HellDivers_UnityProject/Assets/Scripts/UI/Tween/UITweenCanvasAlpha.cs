using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HELLDIVERS.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UITweenCanvasAlpha : UITweenAlpha
    {
        public CanvasGroup CanvasGroup { get { return m_Canvas; } }

        private CanvasGroup m_Canvas;

        private void Awake()
        {
            this.enabled = false;
            m_Canvas = this.GetComponent<CanvasGroup>();
        }

        protected override void OnProgress()
        {
            m_Canvas.alpha = m_AnimaCurve.Evaluate(CurrentTime);
        }
    }
}