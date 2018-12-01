using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionBriefingConcentric : MonoBehaviour {

    #region Event
    public delegate void ConcentricHolder();
    public event ConcentricHolder OnClick;
    #endregion

    [SerializeField] private UIMissionBriefingMap m_Map;

    private string m_OriIntroduction;
    private RectTransform m_MapRect;
    private Vector3 m_Pos;
    private float m_RectWidth;
    private float m_RectHeight;
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        MovePoint(); 

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("Submit"))
        {
            if (OnClick != null) OnClick();
            //UIMissionBriefing.Instance.ComfirmSpawnPosition();
        }
    }

    private void MovePoint()
    {
        float h = Input.GetAxis("DirectionHorizontal");
        float v = Input.GetAxis("DirectionVertical");
        this.transform.localPosition += new Vector3(h, v, 0) * 3f;

        if (this.transform.localPosition.x > 350)
        {
            m_Pos = this.transform.localPosition;
            m_Pos.x = 350;
            this.transform.localPosition = m_Pos;
        }
        else if (this.transform.localPosition.x < -350)
        {
            m_Pos = this.transform.localPosition;
            m_Pos.x = -350;
            this.transform.localPosition = m_Pos;
        }
        if (this.transform.localPosition.y > 250)
        {
            m_Pos = this.transform.localPosition;
            m_Pos.y = 250;
            this.transform.localPosition = m_Pos;
        }
        else if (this.transform.localPosition.y < -250)
        {
            m_Pos = this.transform.localPosition;
            m_Pos.y = -250;
            this.transform.localPosition = m_Pos;
        }
    }
}
