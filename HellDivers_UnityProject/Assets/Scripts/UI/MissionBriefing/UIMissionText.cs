using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionText: MonoBehaviour {

    public Text m_Text;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
    }
    public void SetText(string s)
    {
        Debug.Log("SetText");
        m_Text.text = s;
    }
}
