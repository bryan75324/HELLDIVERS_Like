using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

[RequireComponent(typeof(SoundManager))]
[RequireComponent(typeof(CheckCodesMechine))]
public class MissionTower : Mission, IInteractable
{
    #region Properties

    public Player CurrentPlayer { get; private set; }
    public string ID { get { return m_Data.ID; } }
    public float Radius { get { return m_Data.InteractRadius; } }
    public float ActiveTimer { get { return m_ActTimer; } }

    public float ActTimeCountDown
    {
        get
        {
            float result = (m_Data.ActivatingTime < m_ActTimer) ? 0 : m_Data.ActivatingTime - m_ActTimer;
            return result;
        }
    }

    public Vector3 Position { get { return this.transform.position; } }
    public CheckCodesMechine CodeMechine { get { return m_CodeMechine; } }
    public eCode[] Codes { get { return m_Codes; } }

    #endregion Properties

    #region Private Variable

    private MissionTowerData m_Data;
    private float m_ActTimer;
    private float m_MobTimer;
    private int m_CurrentPhase;
    private int m_CodeLenght;
    private GameObject m_TowerGo;
    private Animator m_Animator;
    private eCode[] m_Codes;
    private CheckCodesMechine m_CodeMechine;
    private SoundManager m_SoundManager;

    private delegate void TowerDoState();

    private TowerDoState DoState;

    #endregion Private Variable

    #region Events

    public delegate void TowerEventHolder();

    public event TowerEventHolder OnActivating;

    public event TowerEventHolder OnStop;

    public event TowerEventHolder OnActive;

    #endregion Events

    #region Initializer

    public void Initialize(MissionTowerData data)
    {
        m_Priority = data.Priority;
        m_Type = data.Type;
        m_Reward = data.Reward;
        m_Data = ScriptableObject.CreateInstance<MissionTowerData>();
        data.CopyTo(m_Data);
        m_Data.PhaseDatas.Sort();
        m_Codes = GenerateCode();
    }

    #endregion Initializer

    #region MonoBehaviour

    private void Awake()
    {
        m_SoundManager = this.GetComponent<SoundManager>();
        SoundDataSetting soundData = Resources.Load("Sounds/Mission/MissionTower_SoundDataSetting") as SoundDataSetting;
        m_SoundManager.SetAudioClips(soundData.SoundDatas);

        m_CodeMechine = this.GetComponent<CheckCodesMechine>();
        m_CodeMechine.OnGetResult += SuccessOnCheckCode;
        m_CodeMechine.OnFaild += StartCheckCodes;
        m_CodeMechine.OnGetResult += () => m_SoundManager.PlayInWorld(0, this.transform.position);
        m_CodeMechine.OnChecking += () => m_SoundManager.PlayInWorld(3, this.transform.position);
        m_CodeMechine.OnFaild += () => m_SoundManager.PlayInWorld(2, this.transform.position);
    }

    // Use this for initialization
    private void Start()
    {
        m_Type = eMissionType.Tower;
        m_TowerGo = ResourceManager.m_Instance.LoadData(typeof(GameObject), "Mission/Towers", "Tower01") as GameObject;
        m_TowerGo = Instantiate(m_TowerGo, this.transform);
        m_Animator = m_TowerGo.GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (DoState != null) DoState();
    }

    private void OnDisable()
    {
        InteractiveItemManager.Instance.RemoveItem(this);
    }

    private void OnDestroy()
    {
        m_CodeMechine.OnGetResult -= SuccessOnCheckCode;
    }

    #endregion MonoBehaviour

    #region Public Function

    public void OnInteract(Player player)
    {
        if (CurrentPlayer != null) return;
        CurrentPlayer = player;
        m_CodeMechine.InputHorizontal = player.InputSettting.StratagemHorizontal;
        m_CodeMechine.InputVertical = player.InputSettting.StratagemVertical;
        m_SoundManager.PlayInWorld(4, this.transform.position);
        StartCheckCodes();
    }

    #endregion Public Function

    #region Private Function

    protected override void OnStart()
    {
        InteractiveItemManager.Instance.AddItem(this);
    }

    private void StartCheckCodes()
    {
        m_CodeMechine.Clear();
        m_CodeMechine.AddCodes(this, m_Codes);
        m_CodeMechine.StartCheckCodes();
        DoState = CheckCodeState;
    }

    private void StopCheckCodes()
    {
        m_SoundManager.Stop();
        m_CodeMechine.StopCheckCodes();
        CurrentPlayer = null;
        m_Animator.SetTrigger("Stop");
        m_CurrentPhase = 0;
        m_MobTimer = 0;
        m_ActTimer = 0;
        DoState = null;
        if (OnStop != null) OnStop();
    }

    private void SuccessOnCheckCode()
    {
        m_Animator.SetTrigger("Start");
        m_ActTimer = 0;
        DoState = ActivationState;
    }

    /*-----------------------------
     * It's "Checking Code State" *
     -----------------------------*/

    private void CheckCodeState()
    {
        if (Vector3.Distance(this.transform.position, CurrentPlayer.transform.position) > m_Data.ActivateRadius
            || Input.GetKeyDown(CurrentPlayer.InputSettting.Cancel)
            || CurrentPlayer.IsDead)
        {
            StopCheckCodes();
        }
    }

    /*-----------------------------------------------------
     * It's "Activation State" timer, count on m_ActTimer *
     -----------------------------------------------------*/

    private void ActivationState()
    {
        if (Vector3.Distance(this.transform.position, CurrentPlayer.transform.position) > m_Data.ActivateRadius || CurrentPlayer.IsDead)
        {
            CurrentPlayer.SoundManager.PlayOnce(1040);
            m_SoundManager.Stop();
            StopCheckCodes();
            return;
        }

        if (m_ActTimer < m_Data.ActivatingTime)
        {
            m_ActTimer += Time.fixedDeltaTime;

            if (m_CurrentPhase < m_Data.PhaseDatas.Count)
            {
                m_MobTimer += Time.fixedDeltaTime;

                if (m_MobTimer >= m_Data.PhaseDatas[m_CurrentPhase].Time)
                {
                    PhaseData phase = m_Data.PhaseDatas[m_CurrentPhase];
                    MobManager.m_Instance.SpawnMobs(phase.FishNum, phase.FishVariantNum, phase.PatrolNum, phase.TankNum, this.transform, m_Data.MinRadius, m_Data.MaxRadius);
                    m_CurrentPhase++;
                }
            }

            if (OnActivating != null) OnActivating();
        }
        else
        {
            m_Animator.SetTrigger("Finish");
            m_SoundManager.PlayInWorld(1, this.transform.position);
            m_bFinished = true;
            CompleteMission();
            InteractiveItemManager.Instance.RemoveItem(this);

            List<Player> pList = InGamePlayerManager.Instance.Players;
            for (int i = 0; i < pList.Count; i++)
            {
                pList[i].Record.NumOfMission++;
            }

            if (OnActive != null) OnActive();
            StartCoroutine(MissionFinish());
            DoState = null;
        }
    }

    private IEnumerator MissionFinish()
    {
        yield return new WaitUntil(() =>
            {
                if (m_Animator.IsInTransition(0)) return false;
                AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Finish") && stateInfo.normalizedTime >= 1) return true;
                else return false;
            });

        yield break;
    }

    /*------------------------------------------------------------------------
     * Create random codes lenth between m_CodeLenghtMin amd m_CodeLenghtMax *
     ------------------------------------------------------------------------*/

    private eCode[] GenerateCode()
    {
        m_CodeLenght = UnityEngine.Random.Range(m_Data.CodeLenghtMin, m_Data.CodeLenghtMax);
        eCode[] codes = new eCode[m_CodeLenght];

        for (int i = 0; i < m_CodeLenght; i++)
        {
            int codeChar = UnityEngine.Random.Range(1, 4);
            eCode ecode = (eCode)Enum.Parse(typeof(eCode), codeChar.ToString());
            codes[i] = ecode;
        }

        return codes;
    }

    #endregion Private Function

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireArc(this.transform.position, Vector3.up, this.transform.forward, 360f, Radius);
    }

#endif
}