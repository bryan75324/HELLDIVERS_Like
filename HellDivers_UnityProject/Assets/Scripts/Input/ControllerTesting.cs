using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTesting : MonoBehaviour {

    [SerializeField] WelcomeButtonController m_ButtonController;
    [SerializeField] AxisEvent m_Axis;
    [SerializeField] AudioSource m_Audio;
    private ControllerSetting m_ControllerSetting1;
    private ControllerSetting m_ControllerSetting2;

    bool m_Interactive;

    // Use this for initialization
    void Start () {
        m_ControllerSetting1 = InputManager.Instance.InputSettingMap[1];
        m_ControllerSetting2 = InputManager.Instance.InputSettingMap[2];


    }

    // Update is called once per frame
    void Update () {
        if (m_Interactive)
        {
            this.gameObject.SetActive(false);
        }
		if (Input.GetKey(m_ControllerSetting1.Submit))
        {
            m_ButtonController.SetController(m_ControllerSetting1);
            m_Axis.SetController(m_ControllerSetting1);
            m_Audio.Play();
            m_Interactive = true;
        }
        else if(Input.GetKey(m_ControllerSetting2.Submit))
        {
            m_ButtonController.SetController(m_ControllerSetting2);
            m_Axis.SetController(m_ControllerSetting2);
            m_Audio.Play();
            m_Interactive = true;
        }
	}
}
