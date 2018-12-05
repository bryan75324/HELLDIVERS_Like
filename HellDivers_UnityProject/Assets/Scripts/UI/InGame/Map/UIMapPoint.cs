using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HELLDIVERS.UI;

public class UIMapPoint : MonoBehaviour {

    private Vector3 m_Pos = new Vector3();
    private Vector3 m_Center = new Vector3();
    private Vector3 m_Dir = new Vector3();
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
        FindRadarCenter();
        CalculatePosition();
        switch (type)
        {
            case eMapPointType.FISH:
                FishAI fish = target.GetComponent<FishAI>();
                fish.OnSpawn += ShowPoint;
                fish.OnDeath += HidePoint;
                break;
            case eMapPointType.MISSIONTOWER:
                m_Image = this.GetComponent<Image>();
                //m_Color.r = 0.2666f;
                //m_Color.g = 0.843f;
                //m_Color.b = 1;
                m_Color = m_Image.color;
                MissionTower missionTower = target.GetComponent<MissionTower>();
                missionTower.OnActive += CompletePoint;
                missionTower.OnActivating += MobManager.m_Instance.StopAutoSpawn;
                missionTower.OnActive += MobManager.m_Instance.StartAutoSpawn;
                UIPanelMap.Instance.DisplayPoint += ShowPoint;
                UIPanelMap.Instance.HidePoint += ClosePoint;
                break;
        }
    }
    public void Init(Player player, eMapPointType type)
    {
        m_CurrentPlayer = player;
        m_CurrentTarget = player.gameObject;
        m_CurrentType = type;
        m_Image = this.GetComponent<Image>();
        m_Color = UIHelper.GetPlayerColor(player.SerialNumber);
        m_Image.color = m_Color;
        FindRadarCenter();
        CalculatePosition();
        switch (m_CurrentType)
        {
            case eMapPointType.PLAYER:
                UIPanelMap.Instance.DisplayPoint += ShowPoint;
                UIPanelMap.Instance.HidePoint += ClosePoint;
                m_CurrentPlayer.OnSpawnBegin += ShowPoint;
                m_CurrentPlayer.OnDeathBegin += HidePoint;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        FindRadarCenter();
        CalculatePosition();
    }

    private void OnDestroy()
    {
        if (m_CurrentTarget == null) return;
        switch (m_CurrentType)
        {
            case eMapPointType.FISH:
                FishAI fish = m_CurrentTarget.GetComponent<FishAI>();
                fish.OnSpawn -= ShowPoint;
                fish.OnDeath -= HidePoint;
                break;
            case eMapPointType.MISSIONTOWER:
                MissionTower missionTower = m_CurrentTarget.GetComponent<MissionTower>();
                missionTower.OnActive -= CompletePoint;
                missionTower.OnActivating -= MobManager.m_Instance.StopAutoSpawn;
                missionTower.OnActive -= MobManager.m_Instance.StartAutoSpawn;
                UIPanelMap.Instance.DisplayPoint -= ShowPoint;
                UIPanelMap.Instance.HidePoint -= ClosePoint;
                break;
            case eMapPointType.PLAYER:
                UIPanelMap.Instance.DisplayPoint -= ShowPoint;
                UIPanelMap.Instance.HidePoint -= ClosePoint;
                m_CurrentPlayer.OnSpawnBegin -= ShowPoint;
                m_CurrentPlayer.OnDeathBegin -= HidePoint;
                break;
        }
    }

    private void ShowPoint()
    {
        this.gameObject.SetActive(true);
        m_Color.a = 0.8f;
        m_Image.color = m_Color;
        CalculatePosition();
        if(m_CurrentType == eMapPointType.PLAYER)
        {
            this.transform.SetAsLastSibling();
        }
    }
    private void ClosePoint()
    {
        this.gameObject.SetActive(true);
        m_Color.a = 0.0f;
        m_Image.color = m_Color;
    }

    private void HidePoint()
    {
        switch (m_CurrentType)
        {
            case eMapPointType.FISH:
                ObjectPool.m_Instance.UnLoadObjectToPool(9202, this.gameObject);
                break;
            case eMapPointType.PLAYER:
                ObjectPool.m_Instance.UnLoadObjectToPool(9201, this.gameObject);
                break;
        }
    }

    private void CompletePoint()
    {
        m_Image = this.GetComponent<Image>();
        m_Color = m_Image.color;
        m_Color.r = 0.5f;
        m_Color.g = 0.5f;
        m_Color.b = 0.5f;
        m_Image.color = m_Color;
    }

    private void FindRadarCenter()
    {
        m_Center = MapInfo.Instance.MapOrigin.transform.position;
    }

    private void CalculatePosition()
    {
        if(m_CurrentType == eMapPointType.PLAYER)
        {
            int m_PlayersCount = 0;
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
            Vector3 forward = m_CurrentTarget.transform.forward;
            Vector3 m_2DForward = new Vector3();
            m_2DForward.x = forward.x;
            m_2DForward.y = forward.z;
            this.transform.up = m_2DForward;
        }
        FindRadarCenter();
        Vector3 targetPos = m_CurrentTarget.transform.position;
        m_Dir = targetPos - m_Center;
        m_Pos = this.transform.localPosition;
        m_Pos.x = m_Dir.x * (UIPanelMap.Instance.RectWidth) / UIPanelMap.Instance.MapWidth;
        m_Pos.y = m_Dir.z * (UIPanelMap.Instance.RectHeight) / UIPanelMap.Instance.MapHeight;
        this.transform.localPosition = m_Pos;
    }
}
