using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using HELLDIVERS.UI.InGame;

public class MobManager
{
    #region Events

    public delegate void MobEventKill();

    public event MobEventKill OnKill;

    public event MobEventKill OnDestroyAll;

    #endregion Events

    static public MobManager m_Instance;
    [SerializeField] private MissionTower m_MissionTower;

    #region Kill Count Variable

    private int m_TotalKill;
    private int m_TotalFishKill;
    private int m_TotalFishVariantKill;
    private int m_TotalPatrolKill;
    private int m_TotalTankKill;

    public int TotalKill { get { return m_TotalKill; } }
    public int TotalFishKill { get { return m_TotalFishKill; } }
    public int TotalFishVariantKill { get { return m_TotalFishVariantKill; } }
    public int TotalPatrolKill { get { return m_TotalPatrolKill; } }
    public int TotalTankKill { get { return m_TotalTankKill; } }

    #endregion Kill Count Variable

    private int m_FishCount;
    private int m_FishVariantCount;
    private int m_PatrolCount;
    private int m_TankCount;

    private GameObject m_GOFish;
    private GameObject m_GOFishVariant;
    private GameObject m_GOPatrol;
    private GameObject m_GOTank;

    private GameObject m_GOWarning;
    private GameObject m_GOSpwanEffect;
    private GameObject m_GOBullet;
    private GameObject m_GOGroundFissure;
    private GameObject m_GOFireBall;
    private GameObject m_GOFireBomb;

    private GameObject m_GOBloodSpurtBig;
    private GameObject m_GOBloodSpurtSmall;
    private GameObject m_GOBloodSpurtDead;

    private bool m_bAutoSpawn = true;

    public void Init()
    {
        m_Instance = this;

        #region Mob Prefab

        m_GOFish = Resources.Load("Mobs/Fish/Fish") as GameObject;
        m_GOFishVariant = Resources.Load("Mobs/Fish2/Fish2") as GameObject;
        m_GOPatrol = Resources.Load("Mobs/Patrol/Patrol") as GameObject;
        m_GOTank = Resources.Load("Mobs/Tank/Tank") as GameObject;
        ObjectPool.m_Instance.InitGameObjects(m_GOFish, 40, 3100);
        ObjectPool.m_Instance.InitGameObjects(m_GOFishVariant, 10, 3300);
        ObjectPool.m_Instance.InitGameObjects(m_GOPatrol, 50, 3200);
        ObjectPool.m_Instance.InitGameObjects(m_GOTank, 10, 3400);

        #endregion Mob Prefab

        #region Mob Effect

        m_GOSpwanEffect = Resources.Load("Mobs/Effect/SpawnEffect") as GameObject;
        m_GOWarning = Resources.Load("Mobs/Effect/EnemyAlert") as GameObject;
        m_GOBullet = Resources.Load("Mobs/Patrol/PatrolBullet") as GameObject;
        m_GOGroundFissure = Resources.Load("Mobs/Tank/GroundFissure") as GameObject;
        m_GOFireBall = Resources.Load("Mobs/Tank/FireBall") as GameObject;
        m_GOFireBomb = Resources.Load("Mobs/Tank/FireBomb") as GameObject;
        ObjectPool.m_Instance.InitGameObjects(m_GOSpwanEffect, 50, 3001);
        ObjectPool.m_Instance.InitGameObjects(m_GOWarning, 5, 3210);
        ObjectPool.m_Instance.InitGameObjects(m_GOBullet, 80, 3201);
        ObjectPool.m_Instance.InitGameObjects(m_GOGroundFissure, 10, 3401);
        ObjectPool.m_Instance.InitGameObjects(m_GOFireBall, 10, 3402);
        ObjectPool.m_Instance.InitGameObjects(m_GOFireBomb, 10, 3403);

        #endregion Mob Effect

        #region Mob Blood

        m_GOBloodSpurtBig = Resources.Load("Mobs/Effect/BloodGushFX/BloodSpurtBig") as GameObject;
        m_GOBloodSpurtSmall = Resources.Load("Mobs/Effect/BloodGushFX/BloodSpurtSmall") as GameObject;
        m_GOBloodSpurtDead = Resources.Load("Mobs/Effect/BloodGushFX/BloodSpurtDead") as GameObject;
        ObjectPool.m_Instance.InitGameObjects(m_GOBloodSpurtBig, 200, 3002);
        ObjectPool.m_Instance.InitGameObjects(m_GOBloodSpurtSmall, 200, 3003);
        ObjectPool.m_Instance.InitGameObjects(m_GOBloodSpurtDead, 200, 3004);

        #endregion Mob Blood

        m_TotalKill = 0;
        m_TotalFishKill = 0;
        m_TotalFishVariantKill = 0;
        m_TotalPatrolKill = 0;
        m_TotalTankKill = 0;
    }

    public void SpawnPatrol(int num)
    {
        if (MapInfo.Instance == null) return;
        if (MapInfo.Instance.MobPos.Count <= 0) return;

        int spawnIndex;
        for (int i = 0; i < num; i++)
        {
            spawnIndex = Random.Range(0, MapInfo.Instance.MobPos.Count);
            m_GOPatrol = ObjectPool.m_Instance.LoadGameObjectFromPool(3200);
            if (m_GOPatrol == null) return;
            m_GOPatrol.SetActive(true);
            m_GOPatrol.transform.position = MapInfo.Instance.MobPos[spawnIndex].position;
            m_PatrolCount++;
        }
    }

    public void SpawnMobs(int fishCount, int fishVariantCount, int patrolCount, int tankCount)
    {
        if (m_bAutoSpawn == false) return;

        List<Player> pList = InGamePlayerManager.Instance.Players;
        Vector3 Center = new Vector3();
        Center.Set(0, 0, 0);
        for (int i = 0; i < pList.Count; i++)
        {
            Center += pList[i].transform.position;
        }
        Center /= pList.Count;

        Vector3 spawnTarget = Vector3.forward;

        for (int i = 0; i < 30; i++)
        {
            spawnTarget = Vector3.forward;
            spawnTarget = Quaternion.AngleAxis(Random.Range(1f, 360f), Vector3.up) * spawnTarget;
            spawnTarget *= Random.Range(70f, 75f);
            spawnTarget += Center;
            if (Physics.Linecast(Center, spawnTarget, 1 << LayerMask.NameToLayer("Obstcale")))
            {
                continue;
            }
            else
            {
                for (int j = 0; j < fishCount; j++)
                {
                    if (m_FishCount > 20) break;
                    m_GOFish = ObjectPool.m_Instance.LoadGameObjectFromPool(3100);
                    if (m_GOFish == null) return;
                    m_GOFish.transform.position = spawnTarget;
                    m_GOFish.SetActive(true);
                    m_FishCount++;
                    if (UIInGameMain.Instance != null)
                        UIInGameMain.Instance.AddRadarPoint(m_GOFish, eMapPointType.FISH);
                }
                for (int j = 0; j < fishVariantCount; j++)
                {
                    if (m_FishVariantCount > 3) break;
                    m_GOFishVariant = ObjectPool.m_Instance.LoadGameObjectFromPool(3300);
                    if (m_GOFishVariant == null) return;
                    m_GOFishVariant.transform.position = spawnTarget;
                    m_GOFishVariant.SetActive(true);
                    m_FishVariantCount++;
                    if (UIInGameMain.Instance != null)
                        UIInGameMain.Instance.AddRadarPoint(m_GOFishVariant, eMapPointType.FISHVARIANT);
                }
                for (int j = 0; j < patrolCount; j++)
                {
                    if (m_PatrolCount > 30) break;
                    m_GOPatrol = ObjectPool.m_Instance.LoadGameObjectFromPool(3200);
                    if (m_GOPatrol == null) return;
                    m_GOPatrol.transform.position = spawnTarget;
                    m_GOPatrol.SetActive(true);
                    PatrolAI patrolAI = m_GOPatrol.GetComponent<PatrolAI>();
                    patrolAI.m_bGoIdle = true;
                    m_PatrolCount++;
                    if (UIInGameMain.Instance != null)
                        UIInGameMain.Instance.AddRadarPoint(m_GOPatrol, eMapPointType.PATROL);
                }
                for (int j = 0; j < tankCount; j++)
                {
                    if (m_TankCount > 5) break;
                    m_GOTank = ObjectPool.m_Instance.LoadGameObjectFromPool(3400);
                    if (m_GOTank == null) return;
                    m_GOTank.transform.position = spawnTarget;
                    m_GOTank.SetActive(true);
                    m_TankCount++;
                    if (UIInGameMain.Instance != null)
                        UIInGameMain.Instance.AddRadarPoint(m_GOTank, eMapPointType.TANK);
                }
                break;
            }
        }
        Debug.Log("Fish:" + m_FishCount + " Fish2:" + m_FishVariantCount + " Patrol:" + m_PatrolCount + " Tank:" + m_TankCount);
    }

    public void SpawnMobs(int fishCount, int fishVariantCount, int patrolCount, int tankCount, Transform center, float minRadius, float maxRadius)
    {
        Vector3 spawnTarget = center.forward;

        for (int i = 0; i < fishCount; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                spawnTarget = Vector3.forward;
                spawnTarget = Quaternion.AngleAxis(Random.Range(1f, 360f), Vector3.up) * spawnTarget;
                spawnTarget *= Random.Range(minRadius, maxRadius);
                spawnTarget += center.position;
                if (Physics.Linecast(center.position, spawnTarget, 1 << LayerMask.NameToLayer("Obstcale")))
                {
                    continue;
                }
                else
                {
                    m_GOFish = ObjectPool.m_Instance.LoadGameObjectFromPool(3100);
                    if (m_GOFish == null) return;
                    m_GOFish.transform.position = spawnTarget;
                    m_GOFish.SetActive(true);
                    m_FishCount++;
                    if (UIPanelRadar.Instance != null)
                        UIPanelRadar.Instance.AddPointPrefab(m_GOFish, eMapPointType.FISH);
                    break;
                }
            }
        }
        for (int i = 0; i < fishVariantCount; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                spawnTarget = Vector3.forward;
                spawnTarget = Quaternion.AngleAxis(Random.Range(1f, 360f), Vector3.up) * spawnTarget;
                spawnTarget *= Random.Range(minRadius, maxRadius);
                spawnTarget += center.position;
                if (Physics.Linecast(center.position, spawnTarget, 1 << LayerMask.NameToLayer("Obstcale")))
                {
                    continue;
                }
                else
                {
                    m_GOFishVariant = ObjectPool.m_Instance.LoadGameObjectFromPool(3300);
                    if (m_GOFishVariant == null) return;
                    m_GOFishVariant.transform.position = spawnTarget;
                    m_GOFishVariant.SetActive(true);
                    m_FishVariantCount++;
                    if (UIInGameMain.Instance != null)
                        UIInGameMain.Instance.AddRadarPoint(m_GOFishVariant, eMapPointType.FISHVARIANT);
                    break;
                }
            }
        }
        for (int i = 0; i < patrolCount; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                spawnTarget = Vector3.forward;
                spawnTarget = Quaternion.AngleAxis(Random.Range(1f, 360f), Vector3.up) * spawnTarget;
                spawnTarget *= Random.Range(minRadius, maxRadius);
                spawnTarget += center.position;
                if (Physics.Linecast(center.position, spawnTarget, 1 << LayerMask.NameToLayer("Obstcale")))
                {
                    continue;
                }
                else
                {
                    m_GOPatrol = ObjectPool.m_Instance.LoadGameObjectFromPool(3200);
                    if (m_GOPatrol == null) return;
                    m_GOPatrol.transform.position = spawnTarget;
                    m_GOPatrol.SetActive(true);
                    PatrolAI patrolAI = m_GOPatrol.GetComponent<PatrolAI>();
                    patrolAI.m_bGoIdle = true;
                    m_PatrolCount++;
                    if (UIInGameMain.Instance != null)
                        UIInGameMain.Instance.AddRadarPoint(m_GOPatrol, eMapPointType.PATROL);
                    break;
                }
            }
        }
        for (int i = 0; i < tankCount; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                spawnTarget = Vector3.forward;
                spawnTarget = Quaternion.AngleAxis(Random.Range(1f, 360f), Vector3.up) * spawnTarget;
                spawnTarget *= Random.Range(minRadius, maxRadius);
                spawnTarget += center.position;
                if (Physics.Linecast(center.position, spawnTarget, 1 << LayerMask.NameToLayer("Obstcale")))
                {
                    continue;
                }
                else
                {
                    m_GOTank = ObjectPool.m_Instance.LoadGameObjectFromPool(3400);
                    if (m_GOTank == null) return;
                    m_GOTank.transform.position = spawnTarget;
                    m_GOTank.SetActive(true);
                    m_PatrolCount++;
                    if (UIInGameMain.Instance != null)
                        UIInGameMain.Instance.AddRadarPoint(m_GOTank, eMapPointType.TANK);
                    break;
                }
            }
        }
    }

    public void UnloadMob(int ID, MobInfo data)
    {
        ObjectPool.m_Instance.UnLoadObjectToPool(ID, data.m_Go);
        switch (ID)
        {
            case 3100:
                AddTotalKill();
                m_FishCount--;
                break;

            case 3200:
                AddTotalKill();
                m_PatrolCount--;
                break;

            case 3300:
                AddTotalKill();
                m_FishVariantCount--;
                break;

            case 3400:
                AddTotalKill();
                m_TankCount--;
                break;
        }
    }

    public void Dead()
    {
        if (OnKill != null) OnKill();
    }

    public void AddTotalKill()
    {
        //if (OnKill != null) OnKill();
    }

    public void StopAutoSpawn()
    {
        m_bAutoSpawn = false;
    }

    public void StartAutoSpawn()
    {
        m_bAutoSpawn = true;
    }

    public void DestoryAllMobs()
    {
        if (OnDestroyAll != null) OnDestroyAll();
    }
}