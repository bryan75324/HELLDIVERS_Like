using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionBriefingMapPoint : MonoBehaviour {

    #region Variable
    private Vector3 m_Pos = new Vector3();
    private Vector3 m_Center = new Vector3();
    private Vector3 m_Dir = new Vector3();
    private Image m_Image;
    [SerializeField] private Color m_NormalColor;
    [SerializeField] private Color m_HighLightColor;
    #endregion

    public GameObject CurrentTarget { get { return m_CurrentTarget; } }
    private GameObject m_CurrentTarget;
    private eMapPointType m_CurrentType;

    public void Init(GameObject target, eMapPointType type)
    {
        m_Image = this.GetComponent<Image>();
        m_Image.color = m_NormalColor;
        m_CurrentTarget = target;
        m_CurrentType = type;
        FindRadarCenter();
        //CalculatePosition();
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //if (UIMissionBriefing.Instance.Target == this.gameObject)
        //{
        //    m_Image.color = m_HighLightColor;
        //    return;
        //}
        //if (Vector3.Distance(this.transform.position, UIMissionBriefing.Instance.Concentric.transform.position) < 20f)
        //{
        //    Selected();
        //}
        //else
        //{
        //    UnSelected();
        //}
	}

    //private void Selected()
    //{
    //    m_Image.color = m_HighLightColor;

    //    UIMissionBriefing.Instance.Map.SetHightLight(this);
    //}

    //private void UnSelected()
    //{
    //    m_Image.color = m_NormalColor;

    //    UIMissionBriefing.Instance.Map.DeleteHighLight(this);
    //}

    private void FindRadarCenter()
    {
        m_Center = MapInfo.Instance.MapOrigin.position;
    }

    //private void CalculatePosition()
    //{
    //    FindRadarCenter();
    //    Vector3 targetPos = m_CurrentTarget.transform.position;
    //    m_Dir = targetPos - m_Center;

    //    float mapWidth = UIMissionBriefing.Instance.Map.MapRoot.GetComponent<RectTransform>().sizeDelta.x;
    //    float mapHeight = UIMissionBriefing.Instance.Map.MapRoot.GetComponent<RectTransform>().sizeDelta.y;

    //    m_Pos = this.transform.localPosition;
    //    m_Pos.x = m_Dir.x * mapWidth / UIMissionBriefing.Instance.Map.MapWidth - mapWidth * 0.5f;
    //    m_Pos.y = m_Dir.z * mapHeight / UIMissionBriefing.Instance.Map.MapHeight - mapHeight * 0.5f;
    //    this.transform.localPosition = m_Pos;
    //}
}
