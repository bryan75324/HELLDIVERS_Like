using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HELLDIVERS.UI;

public enum eMapPointType
{
    PLAYER,
    FISH,
    FISHVARIANT,
    PATROL,
    TANK,
    MISSIONTOWER,
    SPAWNPOINT,
}

public class UIRadarPoint : MonoBehaviour
{
    private Vector3 m_Pos = new Vector3();
    private Vector3 m_Center = new Vector3();
    private Vector3 m_Dir = new Vector3();
    private int m_PlayersCount = 0;
    [SerializeField] private Image m_Image;
    private Color m_Color;
    public GameObject CurrentTarget { get { return m_CurrentTarget; } }
    private GameObject m_CurrentTarget;
    private Player m_CurrentPlayer;
    private eMapPointType m_CurrentType;

    public void Init(GameObject target, eMapPointType type)
    {
        m_CurrentTarget = target;
        m_CurrentType = type;
        m_Image = this.GetComponent<Image>();
        m_Color = m_Image.color;
        FindRadarCenter();
        CalculatePosition();
        switch (m_CurrentType)
        {
            case eMapPointType.FISH:
                FishAI fish = target.GetComponent<FishAI>();
                fish.OnSpawn += ShowPoint;
                fish.OnDeath += HidePoint;
                break;

            case eMapPointType.PATROL:
                PatrolAI patrol = target.GetComponent<PatrolAI>();
                patrol.OnSpawn += ShowPoint;
                patrol.OnDeath += HidePoint;
                break;

            case eMapPointType.FISHVARIANT:
                FishVariantAI fishVariant = target.GetComponent<FishVariantAI>();
                fishVariant.OnSpawn += ShowPoint;
                fishVariant.OnDeath += HidePoint;
                break;

            case eMapPointType.TANK:
                TankAI tank = target.GetComponent<TankAI>();
                tank.OnSpawn += ShowPoint;
                tank.OnDeath += HidePoint;
                break;
        }
        UIPanelRadar.Instance.UpdatePoint += UpdatePosition;
    }

    public void Init(Player player, eMapPointType type)
    {
        m_CurrentPlayer = player;
        m_CurrentTarget = player.gameObject;
        m_CurrentType = type;
        m_Image = this.GetComponent<Image>();
        m_Color = UIHelper.GetPlayerColor(player.SerialNumber);
        FindRadarCenter();
        CalculatePosition();
        switch (m_CurrentType)
        {
            case eMapPointType.PLAYER:
                m_CurrentPlayer.OnSpawnBegin += ShowPoint;
                m_CurrentPlayer.OnDeathBegin += HidePoint;
                break;
        }
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_CurrentType == eMapPointType.PLAYER)
        {
            UpdatePosition();
            return;
        }

        Blink();
    }

    private void OnDisable()
    {
        if (m_CurrentTarget == null) return;
        switch (m_CurrentType)
        {
            case eMapPointType.FISH:
                FishAI fish = m_CurrentTarget.GetComponent<FishAI>();
                fish.OnSpawn -= ShowPoint;
                fish.OnDeath -= HidePoint;
                break;

            case eMapPointType.PATROL:
                PatrolAI patrol = m_CurrentTarget.GetComponent<PatrolAI>();
                patrol.OnSpawn -= ShowPoint;
                patrol.OnDeath -= HidePoint;
                break;

            case eMapPointType.FISHVARIANT:
                FishVariantAI fishVariant = m_CurrentTarget.GetComponent<FishVariantAI>();
                fishVariant.OnSpawn -= ShowPoint;
                fishVariant.OnDeath -= HidePoint;
                break;

            case eMapPointType.TANK:
                TankAI tank = m_CurrentTarget.GetComponent<TankAI>();
                tank.OnSpawn -= ShowPoint;
                tank.OnDeath -= HidePoint;
                break;
                //case eMapPointType.PLAYER:
                //    m_CurrentPlayer.OnSpawnBegin -= ShowPoint;
                //    m_CurrentPlayer.OnDeathBegin -= HidePoint;
                //    break;
        }
        UIPanelRadar.Instance.UpdatePoint -= UpdatePosition;
    }

    private void ShowPoint()
    {
        this.gameObject.SetActive(true);
        CalculatePosition();
    }

    private void HidePoint()
    {
        switch (m_CurrentType)
        {
            case eMapPointType.FISH:
                ObjectPool.m_Instance.UnLoadObjectToPool(9102, this.gameObject);
                UIPanelRadar.Instance.PointList.Remove(this.gameObject);
                break;

            case eMapPointType.PATROL:
                ObjectPool.m_Instance.UnLoadObjectToPool(9102, this.gameObject);
                UIPanelRadar.Instance.PointList.Remove(this.gameObject);
                break;

            case eMapPointType.FISHVARIANT:
                ObjectPool.m_Instance.UnLoadObjectToPool(9102, this.gameObject);
                UIPanelRadar.Instance.PointList.Remove(this.gameObject);
                break;

            case eMapPointType.TANK:
                ObjectPool.m_Instance.UnLoadObjectToPool(9102, this.gameObject);
                UIPanelRadar.Instance.PointList.Remove(this.gameObject);
                break;

            case eMapPointType.PLAYER:
                this.gameObject.SetActive(false);
                break;
        }
    }

    private void FindRadarCenter()
    {
        List<Player> pList = InGamePlayerManager.Instance.Players;
        if (pList == null) return;
        m_Center.Set(0, 0, 0);
        for (int i = 0; i < pList.Count; i++)
        {
            if (pList[i].IsDead) continue;
            m_PlayersCount++;
            m_Center += pList[i].transform.position;
        }
        if (m_PlayersCount > 0)
        {
            m_Center /= m_PlayersCount;
        }
        m_PlayersCount = 0;
    }

    private void CalculatePosition()
    {
        m_Dir = m_CurrentTarget.transform.position - m_Center;
        m_Pos = this.transform.localPosition;
        m_Pos.x = m_Dir.x * (UIPanelRadar.Instance.RectWidth * 0.5f) / UIPanelRadar.Instance.RadarRadius;
        m_Pos.y = m_Dir.z * (UIPanelRadar.Instance.RectHeight * 0.5f) / UIPanelRadar.Instance.RadarRadius;
        this.transform.localPosition = m_Pos;
    }

    private void UpdatePosition()
    {
        FindRadarCenter();
        Vector3 forward = m_CurrentTarget.transform.forward;
        Vector3 m_2DForward = new Vector3();
        m_2DForward.x = forward.x;
        m_2DForward.y = forward.z;
        this.transform.up = m_2DForward;

        m_Dir = m_CurrentTarget.transform.position - m_Center;
        m_Pos = this.transform.localPosition;
        m_Pos.x = m_Dir.x * (UIPanelRadar.Instance.RectWidth * 0.5f) / UIPanelRadar.Instance.RadarRadius;
        m_Pos.y = m_Dir.z * (UIPanelRadar.Instance.RectHeight * 0.5f) / UIPanelRadar.Instance.RadarRadius;
        this.transform.localPosition = m_Pos;
        m_Color.a = 1f;
        m_Image.color = m_Color;
    }

    private void Blink()
    {
        m_Color.a -= Time.deltaTime;
        m_Image.color = m_Color;
    }
}