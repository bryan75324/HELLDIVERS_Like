using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HELLDIVERS.UI
{
    public class UITweenImageAlpha : UITweenAlpha
    {
        [SerializeField] private Image m_Image;
        private Color m_color;

        // Use this for initialization
        private void Start()
        {
            m_color = m_Image.color;
        }

        protected override void OnProgress()
        {
            m_color.a = m_AnimaCurve.Evaluate(CurrentTime);
            m_Image.color = m_color;
        }
    }
}