using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour {

    [SerializeField] private Text m_Introduction;
    Button m_Button;
    // Use this for initialization
    void Start () {
		
	}
    public void Selected()
    {
        m_Button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        m_Introduction.text = m_Button.name;
    }
}
