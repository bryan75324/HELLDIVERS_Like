using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelRadar : MonoBehaviour {

    public delegate void UIRadarEventHolder();
    public event UIRadarEventHolder UpdatePoint;

    public static UIPanelRadar Instance { get; private set; }

    #region Variable
    private GameObject m_GORadarPoint;
    public float RectWidth { get { return m_RectWidth; } }
    public float RectHeight { get { return m_RectHeight; } }
    public float RadarRadius { get { return m_RadarRadius; } }
    public Color Color { get { return m_Color; } }
    public float Timer { get { return m_Timer; } }
    public List<GameObject> PointList { get { return m_PointList; } }
    [SerializeField] private UIRadarPoint m_PointPrefab;
    private RectTransform m_RectTransform;
    private float m_RectWidth;
    private float m_RectHeight;
    private Image m_Image;
    private Color m_Color;
    private float m_Timer;
    private bool m_bAdd = true;
    public bool m_bDisPlay = true;
    private int bPlyaersCount;
    private List<GameObject> m_PointList = new List<GameObject>();

    [SerializeField] private float m_RadarRadius = 50.0f;
    #endregion

    public void Init()
    {
        if (Instance == null) Instance = this;
        m_GORadarPoint = Resources.Load("UI/InGame/Radar/PlayerPoint") as GameObject;
        ObjectPool.m_Instance.InitGameObjects(m_GORadarPoint, 3, 9101);
        m_GORadarPoint = Resources.Load("UI/InGame/Radar/MobPoint") as GameObject;
        ObjectPool.m_Instance.InitGameObjects(m_GORadarPoint, 40, 9102);
        m_RectTransform = this.GetComponent<RectTransform>();
        m_RectWidth = m_RectTransform.sizeDelta.x;
        m_RectHeight = m_RectTransform.sizeDelta.y;
        m_Image = this.GetComponent<Image>();
        m_Color = m_Image.color;
    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        if (m_bDisPlay == false)
        {
            m_Color.a = 0.0f;
            m_Image.color = m_Color;
            return;
        }
        else
        {
            m_Color.a = 100/255f;
            m_Image.color = m_Color;
        }
        

        Blink();
        CountTimer();


        List<Player> pList = InGamePlayerManager.Instance.Players;
        if (pList != null && pList.Count > 0)
        {
            for (int i = 0; i < pList.Count; i++)
            {
                if (pList[i].IsDead)
                {
                    bPlyaersCount++;
                }
            }
            if(bPlyaersCount >= pList.Count)
            {
                if (m_Color.a == 0) return;
                m_Color.a -= Time.deltaTime *0.3f;
                m_Image.color = m_Color;
            }
            else{
                m_Color.a = 0.4f;
                m_Image.color = m_Color;
            }
            bPlyaersCount = 0;
        }
        #region Input
        

        if (Input.GetKeyDown(KeyCode.G))
        {
            MobManager.m_Instance.SpawnMobs(1, 0, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            MobManager.m_Instance.SpawnMobs(0, 1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            MobManager.m_Instance.SpawnMobs(0, 0, 1, 0);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            MobManager.m_Instance.SpawnMobs(0, 0, 0, 1);
        }
        #endregion
    }

    public void AddPointPrefab(GameObject target, eMapPointType type)
    {
        GameObject go;
        UIRadarPoint p;
        switch (type)
        {
            case eMapPointType.FISH:
                go = ObjectPool.m_Instance.LoadGameObjectFromPool(9102);
                go.SetActive(true);
                p = go.GetComponent<UIRadarPoint>();
                p.Init(target, type);
                p.transform.SetParent(this.transform);
                m_PointList.Add(go);
                break;
            case eMapPointType.PATROL:
                go = ObjectPool.m_Instance.LoadGameObjectFromPool(9102);
                go.SetActive(true);
                p = go.GetComponent<UIRadarPoint>();
                p.Init(target, type);
                p.transform.SetParent(this.transform);
                m_PointList.Add(go);
                break;
            case eMapPointType.FISHVARIANT:
                go = ObjectPool.m_Instance.LoadGameObjectFromPool(9102);
                go.SetActive(true);
                p = go.GetComponent<UIRadarPoint>();
                p.Init(target, type);
                p.transform.SetParent(this.transform);
                m_PointList.Add(go);
                break;
            case eMapPointType.TANK:
                go = ObjectPool.m_Instance.LoadGameObjectFromPool(9102);
                go.SetActive(true);
                p = go.GetComponent<UIRadarPoint>();
                p.Init(target, type);
                p.transform.SetParent(this.transform);
                m_PointList.Add(go);
                break;
        }
    }
    public void AddPointPrefab(Player player, eMapPointType type)
    {
        GameObject go;
        UIRadarPoint p;
        switch (type)
        {
            case eMapPointType.PLAYER:
                go = ObjectPool.m_Instance.LoadGameObjectFromPool(9101);
                go.SetActive(true);
                p = go.GetComponent<UIRadarPoint>();
                p.Init(player, type);
                p.transform.SetParent(this.transform);
                //m_PointList.Add(go);
                break;
        }
    }
    private void Blink()
    {
        if (m_bAdd)
        {
            m_Color.b += Time.deltaTime * 0.1f;
            m_Color.g += Time.deltaTime * 0.1f;
            m_Image.color = m_Color;
            if (m_Color.b > 0.35) m_bAdd = false;
        }
        else if (m_bAdd == false)
        {
            m_Color.b -= Time.deltaTime * 0.1f;
            m_Color.g -= Time.deltaTime * 0.1f;
            m_Image.color = m_Color;
            if (m_Color.b <= 0.3) m_bAdd = true;
        }
    }

    private void CountTimer()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= 1.0f && m_Color.b > 0.35f)
        {
            if(m_PointList.Count > 0)
            {
                if(UpdatePoint != null) UpdatePoint();
            }
            m_Timer = 0.0f;
        }
    }

    public void SetDisplay(bool bPlay)
    {
        m_bDisPlay = bPlay;
    }
}
