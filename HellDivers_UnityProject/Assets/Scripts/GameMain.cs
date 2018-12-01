using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HELLDIVERS.UI.InGame;

public class GameMain : MonoBehaviour
{
    #region Properties

    public static GameMain Instance { get; private set; }
    public float GameTime { get { return Time.realtimeSinceStartup - m_GameStartTime; } }
    public CameraFollowing CameraFolloing { get { return m_CameraFollowing; } }

    #endregion Properties

    #region Private Variable

    private AssetManager m_AssetManager = new AssetManager();
    private ResourceManager m_ResourceManager = new ResourceManager();
    private ObjectPool m_ObjectPool = new ObjectPool();
    private InteractiveItemManager m_ItemManager = new InteractiveItemManager();
    private MissionManager m_MissionManager = new MissionManager();
    private InGamePlayerManager m_PlayerManager;
    private InGameRewardManager m_RewardManager;
    private MobManager m_MobSpawner = new MobManager();
    private CameraFollowing m_CameraFollowing;
    private float m_GameStartTime;
    private bool m_bMissionCompleted;

    [SerializeField] private MapInfo m_MapInfo;
    [SerializeField] private uint m_NumberOfTowers = 1;

    #endregion Private Variable

    #region Delegate Function

    private delegate void GameStateDelegateFunc();

    private GameStateDelegateFunc DoCheckCondition;

    #endregion Delegate Function

    #region MonoBehaviour

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        m_AssetManager.Init();
        m_ResourceManager.Init();
        m_ObjectPool.Init();
        m_MapInfo = Instantiate(m_MapInfo);
        m_MapInfo.Init();
        UIInGameMain.Instance.Init();
        m_ItemManager.Init();
        m_MissionManager.Init();
        m_MobSpawner.Init();
        m_CameraFollowing = Camera.main.GetComponent<CameraFollowing>();
        m_PlayerManager = this.gameObject.AddComponent<InGamePlayerManager>();
        m_RewardManager = new GameObject("RewardManager").AddComponent<InGameRewardManager>();
    }

    // Use this for initialization
    private void Start()
    {
        for (int i = 1; i < PlayerManager.Instance.Players.Count + 1; i++)
        {
            m_PlayerManager.CreatePlayer(PlayerManager.Instance.Players[i].info, i);
        }

        m_MissionManager.CreateTowerMissions(m_NumberOfTowers);
        m_MissionManager.CreateMission(eMissionType.KillMob);
    }

    // Update is called once per frame
    private void Update()
    {
        if (DoCheckCondition != null) DoCheckCondition();
    }

    #endregion MonoBehaviour

    #region Game Control

    [ContextMenu("Mission Start")]
    public void GameStart(Transform spawnPos)
    {
        m_GameStartTime = Time.realtimeSinceStartup;

        m_PlayerManager.SpawnPlayers(spawnPos);
        UIInGameMain.Instance.DrawGameUI();

        m_MobSpawner.SpawnPatrol(20);
        InvokeRepeating("SpawnMobs", 1.0f, 30.0f);

        m_MissionManager.StartAllMission();

        DoCheckCondition = CheckGameCondition;
    }

    private void CheckGameCondition()
    {
        if (InGamePlayerManager.Instance.Players.Count > 1 && m_PlayerManager.IsAllPlayerDead())
        {
            MissionFailed();
            DoCheckCondition = null;
        }

        if (m_MissionManager.MainMissionCount <= 0)
        {
            MissionComplete();
            DoCheckCondition = null;
        }
    }

    [ContextMenu("Mission Complete")]
    public void MissionComplete()
    {
        m_bMissionCompleted = true;
        m_RewardManager.SetGameDurationTime(GameTime);

        for (int i = 0; i < m_PlayerManager.Players.Count; i++)
        {
            Player player = m_PlayerManager.Players[i];
            m_RewardManager.SetReward(player.SerialNumber, player.Record);
        }

        foreach (var missionList in m_MissionManager.Missions)
        {
            foreach (var mission in missionList.Value)
            {
                if (mission.IsFinished) m_RewardManager.SetMissionReward(mission.Type, mission.Reward);
            }
        }

        StartCoroutine(MissionCompleteProgress());
    }

    private IEnumerator MissionCompleteProgress()
    {
        MusicManager.Instance.PlayMusic(eMusicSelection.MissionSuccess, 3);
        UIInGameMain.Instance.DrawMissionCompletedUI();

        yield return new WaitForSeconds(UIInGameMain.Instance.UIMissionCompleteTimeLenght);

        SceneController.Instance.ToReward();
    }

    [ContextMenu("Mission Failed")]
    public void MissionFailed()
    {
        if (m_bMissionCompleted) return;
        StartCoroutine(MissionFailedProgress());
    }

    private IEnumerator MissionFailedProgress()
    {
        MusicManager.Instance.PlayMusic(eMusicSelection.MissionFailed, 3);

        yield return new WaitForSeconds(3);

        UIInGameMain.Instance.DrawMissionFailedUI();

        yield break;
    }

    [ContextMenu("Mission Abandon")]
    public void MissionAbandon()
    {
        SceneController.Instance.ToLobby();
    }

    public void SpawnMobs()
    {
        int fish = Random.Range(2, 4);
        int fishVariant = Random.Range(-1, 2);
        int patrol = Random.Range(0, 2);
        int tank = Random.Range(-1, 2);
        //m_MobSpawner.SpawnMobs(fish, fishVariant, patrol, tank);
        m_MobSpawner.SpawnMobs(0, 0, 0, 0);
    }

    #endregion Game Control
}