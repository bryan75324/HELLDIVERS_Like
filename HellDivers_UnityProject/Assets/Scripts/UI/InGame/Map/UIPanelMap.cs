using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelMap : MonoBehaviour {

    [SerializeField] private UIPanelRadar m_PanelRadar;
    [SerializeField] private UIMapPoint m_PointPrefab;

    public static UIPanelMap Instance { get; private set; }

    #region Variable
    [SerializeField] private float m_MapRadius = 40.0f;
    [SerializeField] private float m_MapWidth = 544.0f;
    [SerializeField] private float m_MapHeight = 720.0f;

    public float RectWidth { get { return m_RectWidth; } }
    public float RectHeight { get { return m_RectHeight; } }
    public float MapRadius { get { return m_MapRadius; } }
    public float MapWidth{ get { return m_MapWidth; } }
    public float MapHeight { get { return m_MapHeight; } }
    public Color Color { get { return m_Color; } }
    public List<GameObject> PointList { get { return m_PointList; } }

    private RectTransform m_RectTransform;
    private float m_RectWidth;
    private float m_RectHeight;
    private Image m_Image;
    private Color m_Color;
    private List<GameObject> m_PointList = new List<GameObject>();
    #endregion 

    private GameObject m_GORMapPoint;

    private bool m_bDisplay = false;
    private float m_Timer = 0.0f;

    private Image m_RadarImage;
    private Color m_RadarColor;

    // Use this for initialization

    public void Init()
    {
        if (Instance == null) Instance = this;

        m_GORMapPoint = Resources.Load("UI/InGame/Map/PlayerPoint") as GameObject;
        ObjectPool.m_Instance.InitGameObjects(m_GORMapPoint, 3, 9201);
        m_GORMapPoint = Resources.Load("UI/InGame/Map/MobPoint") as GameObject;
        ObjectPool.m_Instance.InitGameObjects(m_GORMapPoint, 40, 9202);
        m_GORMapPoint = Resources.Load("UI/InGame/Map/MissionPoint") as GameObject;
        ObjectPool.m_Instance.InitGameObjects(m_GORMapPoint, 10, 9203);
        m_RectTransform = this.GetComponent<RectTransform>();
        m_RectWidth = m_RectTransform.sizeDelta.x;
        m_RectHeight = m_RectTransform.sizeDelta.y;
        m_Image = this.GetComponent<Image>();
        m_Color = m_Image.color;
        m_Color.a = 0;
        m_Image.color = m_Color;
    }
    void Start()
    {
        //m_RadarImage = m_PanelRadar.GetComponent<Image>();
        //m_RadarColor = m_RadarImage.GetComponent<Color>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bDisplay == false && (Input.GetKeyDown(KeyCode.M) || Input.GetButtonDown("Map")))
        {
            m_bDisplay = true;
        }
        else if (m_bDisplay && (Input.GetKeyDown(KeyCode.M) || Input.GetButtonDown("Map")))
        {
            m_Timer = 0.0f;
            m_bDisplay = false;
        }
        if (m_bDisplay)
        {
            if (m_Color.a <= 1)
            {
                m_Color.a += Time.deltaTime * 5.0f;
                m_Image.color = m_Color;
            }

            m_Timer += Time.deltaTime;
            if(m_Timer >= 5.0f)
            {
                m_Timer = 0.0f;
                m_bDisplay = false;
            }
        }
        else
        {
            if (m_Color.a <= 0) return;
            m_Color.a -= Time.deltaTime * 5.0f;
            m_Image.color = m_Color;
        }
    }

    public void AddPointPrefab(GameObject target, eMapPointType type)
    {
        GameObject go;
        UIMapPoint p;
        switch (type)
        {
            case eMapPointType.FISH:
                go = ObjectPool.m_Instance.LoadGameObjectFromPool(9202);
                go.SetActive(true);
                p = go.GetComponent<UIMapPoint>();
                p.Init(target, type);
                p.transform.SetParent(this.transform);
                m_PointList.Add(go);
                break;
            case eMapPointType.MISSIONTOWER:
                go = ObjectPool.m_Instance.LoadGameObjectFromPool(9203);
                go.SetActive(true);
                p = go.GetComponent<UIMapPoint>();
                p.Init(target, type);
                p.transform.SetParent(this.transform);
                m_PointList.Add(go);
                break;
        }
    }
    public void AddPointPrefab(Player player, eMapPointType type)
    {
        GameObject go;
        UIMapPoint p;
        switch (type)
        {
            case eMapPointType.PLAYER:
                go = ObjectPool.m_Instance.LoadGameObjectFromPool(9201);
                go.SetActive(true);
                p = go.GetComponent<UIMapPoint>();
                p.Init(player, type);
                p.transform.SetParent(this.transform);
                m_PointList.Add(go);
                break;
        }
    }
    public void DeletePointPrefab(GameObject target)
    {
    }
}
