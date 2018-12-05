using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundManager))]
[RequireComponent(typeof(Rigidbody))]
public class Stratagem : MonoBehaviour
{
    #region Properties

    /// <summary>
    /// Set the transform root for launch postion.
    /// </summary>
    public Transform LaunchPos { get { return m_LaunchPos; } set { m_LaunchPos = value; } }

    /// <summary>
    /// The info is used to initilize the stratagem.
    /// </summary>
    public StratagemInfo Info { get { return m_Info; } }

    /// <summary>
    /// The current state of the stratagem.
    /// </summary>
    public eState State { get { return m_eState; } }

    /// <summary>
    /// Is the stratagem has any uses ?
    /// </summary>
    public bool IsOutOfUses
    {
        get
        {
            if (Info.Uses == -1) return false;
            else if (m_UsesCount >= Info.Uses) return true;
            else return false;
        }
    }

    /// <summary>
    /// Represention the number of how many times has been used.
    /// </summary>
    public int UsesCount { get { return m_UsesCount; } }

    /// <summary>
    /// Is the stratagem cooling down ?
    /// </summary>
    public bool IsCooling { get { return m_IsCooling; } }

    /// <summary>
    /// The timer of CoolDown time. It start when do Throw.
    /// </summary>
    public float CoolTimer { get { return m_CoolTimer; } }

    /// <summary>
    /// The timer of Activatoin. It start when do Land on "terrain".
    /// </summary>
    public float ActTimer { get { return m_ActivationTimer; } }

    /// <summary>
    /// The count down timer of Activatoin. It start when do Land on "terrain".
    /// </summary>
    public float ActTimeCountDown { get { return Info.Activation - m_ActivationTimer; } }

    #endregion Properties

    #region Events

    public delegate void StratagemEventHolder();

    public event StratagemEventHolder OnThrow;

    public event StratagemEventHolder OnGetReady;

    public event StratagemEventHolder OnStartCoolDown;

    public event StratagemEventHolder OnCoolDown;

    public event StratagemEventHolder OnEndCoolDown;

    public event StratagemEventHolder OnStartActivation;

    public event StratagemEventHolder OnActivation;

    public event StratagemEventHolder OnEndActivation;

    #endregion Events

    #region Private Variable

    [SerializeField] private StratagemInfo m_Info;
    private SoundManager m_SoundManager;
    private Transform m_ReadyPos;
    private Transform m_LaunchPos;
    private GameObject m_Display;
    private GameObject m_Result;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private float m_Radius = 0.25f;
    private eState m_eState = eState.Idle;
    private int m_UsesCount;
    private bool m_IsCooling;
    private float m_CoolTimer;
    private float m_ActivationTimer;

    #endregion Private Variable

    #region Initializer

    /// <summary>
    /// Setup stratagem by id key which is in the gamedata.stratagem table.
    /// </summary>
    /// <param name="id">The id key which is in the gamedata.stratagem table.</param>
    /// <param name="launchPos">The spawn transform root.</param>
    /// <returns></returns>
    public bool SetStratagemInfo(int id, Transform readyPos, Transform launchPos)
    {
        StratagemInfo newInfo;
        if (TryGetInfoFromGameData(id, out newInfo) == false) return false;

        this.gameObject.name = string.Format("Stratagem {0}", m_Info.Title);

        // Load display prefab.
        if (newInfo.DisplayID.Equals(m_Info.DisplayID) == false)
        {
            GameObject display;

            if (ResourceManager.m_Instance == null)
            {
                Debug.LogWarningFormat("Stratagem Warning ({0}) : ResourcesManager doesn't exist, Using Resource.Load()", this.gameObject.name);
                display = Resources.Load(string.Format("{0}/{1}", StratagemSystem.DisplayFolder, newInfo.DisplayID)) as GameObject;
            }
            else
            {
                display = ResourceManager.m_Instance.LoadData(typeof(GameObject), StratagemSystem.DisplayFolder, newInfo.DisplayID) as GameObject;
            }

            if (display == null) display = StratagemSystem.DefaultDisplay;

            DestroyImmediate(m_Display);
            m_Display = Instantiate(display, this.transform.position, Quaternion.identity, this.transform);
            m_Animator = m_Display.GetComponent<Animator>();
        }

        // Load result prefab.
        if (newInfo.ResultID.Equals(m_Info.ResultID) == false)
        {
            GameObject result;

            if (ResourceManager.m_Instance == null)
            {
                Debug.LogWarningFormat("Stratagem Warning ({0}) : ResourcesManager doesn't exist, Using Resource.Load()", this.gameObject.name);
                result = Resources.Load(string.Format("{0}/{1}", StratagemSystem.ResultFolder, newInfo.ResultID)) as GameObject;
            }
            else
            {
                result = ResourceManager.m_Instance.LoadData(typeof(GameObject), StratagemSystem.ResultFolder, newInfo.ResultID.ToString()) as GameObject;
            }

            if (result == null) result = StratagemSystem.DefaultResult;
            m_Result = result;

            int count = (newInfo.Uses == -1) ? 10 : newInfo.Uses;
            ObjectPool.m_Instance.InitGameObjects(result, count, newInfo.ResultID);
        }

        // Load Sound
        SoundDataSetting data = ResourceManager.m_Instance.LoadData(typeof(SoundDataSetting), "Sounds/Stratagems", "SoundDataSetting") as SoundDataSetting;
        m_SoundManager.SetAudioClips(data.SoundDatas);

        m_LaunchPos = launchPos;
        m_ReadyPos = readyPos;
        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = Vector3.zero;

        newInfo.CopyTo(m_Info);
        ResetState();
        return true;
    }

    /*----------------------------------------------
     * Reset the (State = Standby) & (Timers = 0). *
    -----------------------------------------------*/

    private void ResetState()
    {
        m_UsesCount = 0;
        m_CoolTimer = 0.0f;
        m_ActivationTimer = 0.0f;
        m_eState = eState.Idle;
        StopAllCoroutines();
    }

    /*--------------------------------------------------------------------------
     * Get stratagem info by id key which is in the gamedata.stratagem tables. *
     * If it does not exist return null.                                       *
    ---------------------------------------------------------------------------*/

    private bool TryGetInfoFromGameData(int id, out StratagemInfo getInfo)
    {
        if (GameData.Instance.StratagemTable.ContainsKey(id) == false)
        {
            Debug.LogErrorFormat("Stratagem Error : Can't found ID : [{0}] from game data", id);
            getInfo = null;
            return false;
        }
        getInfo = GameData.Instance.StratagemTable[id];
        return true;
    }

    #endregion Initializer

    #region MonoBehaviour

    private void Awake()
    {
        m_Info = new StratagemInfo();
        m_SoundManager = this.GetComponent<SoundManager>();
    }

    // Use this for initialization
    private void Start()
    {
        m_Rigidbody = this.GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (m_eState == eState.ThrowOut) DoThrowOut();
    }

    #endregion MonoBehaviour

    #region Public Function

    /// <summary>
    /// Show the stratagem object & reset to the launch position.
    /// Translate to Ready state.
    /// </summary>
    public void GetReady()
    {
        if (IsOutOfUses) return;
        if (IsCooling || State != eState.Idle) return;

        this.transform.parent = m_ReadyPos;
        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = Vector3.zero;
        m_Animator.SetTrigger("Start");

        // Uses add count. ( Info.uses = -1 ) is meaning for unlimited.
        if (Info.Uses != -1) m_UsesCount++;

        m_eState = eState.Ready;
        if (OnGetReady != null) OnGetReady();
    }

    /// <summary>
    /// Add force to this gameobject for throw it out.
    /// Translate to Throw out state.
    /// </summary>
    /// <param name="force">Force for throw it out use relative force</param>
    public void Throw(Vector3 force)
    {
        if (IsCooling || State != eState.Ready) return;
        this.transform.position = m_LaunchPos.position;
        this.transform.rotation = m_LaunchPos.rotation;

        this.transform.parent = null;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.AddRelativeForce(force);
        m_IsCooling = true;
        m_Animator.SetTrigger("Throw");

        // Start the cooldown timer.
        if (Info.CoolDown > 0) StartCoroutine(DoCoolDown(Info.CoolDown));

        // Translate to ThrowOut state.
        m_eState = eState.ThrowOut;

        if (OnThrow != null) OnThrow();
    }

    /// <summary>
    /// Reset used count = 0.
    /// </summary>
    public void Reset()
    {
        StopAllCoroutines();
        m_UsesCount = 0;
        m_CoolTimer = 0;
        m_IsCooling = false;
        m_ActivationTimer = 0;
        m_eState = eState.Idle;
        m_Animator.SetTrigger("Idle");
    }

    #endregion Public Function

    #region Finite State Machine

    /// <summary>
    /// Representation of the stratagem object current statement.
    /// </summary>
    public enum eState
    {
        /// <summary>
        /// It's the start state.
        /// Hide display object, Do nothing.
        /// Next state is Ready state.
        /// </summary>
        Idle,

        /// <summary>
        /// Display on the launch position.
        /// Waiting for throw it out.
        /// </summary>
        Ready,

        /// <summary>
        /// The stratagem is out of launch position root.
        /// Checking the terrain for landing.
        /// </summary>
        ThrowOut,

        /// <summary>
        /// After land on terrain, start counting timer.
        /// When time's up, spawn the traget item, and back to Idle state.
        /// </summary>
        Activating
    }

    /*--------------------------------------------------------------------------------
     * In ThrowOut state, checking the stratagem object is land on "Terrain" or not. *
     * When it landed successfully, then translate to Activating state.              *
     --------------------------------------------------------------------------------*/

    private void DoThrowOut()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, m_Radius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                m_Rigidbody.isKinematic = true;
                this.transform.rotation = Quaternion.Euler(Vector3.zero);
                m_Animator.SetTrigger("Land");

                // Start the activation timer.
                StartCoroutine(DoActivating(Info.Activation));
            }
        }
    }

    /*----------------------------------------------------------------------
     * It's a timer for activaing process.                                 *
     * When the "End" animation was finished, than translate to Idle state *
     ----------------------------------------------------------------------*/

    private IEnumerator DoActivating(float targetTime)
    {
        m_eState = eState.Activating;

        m_SoundManager.PlayInWorld(0, this.transform.position);

        m_ActivationTimer = 0.0f;
        if (OnStartActivation != null) OnStartActivation();

        while (m_ActivationTimer < targetTime)
        {
            if (OnActivation != null) OnActivation();
            yield return new WaitForSeconds(Time.deltaTime);
            m_ActivationTimer += Time.deltaTime;
        }

        if (OnEndActivation != null) OnEndActivation();
        m_Animator.SetTrigger("End");

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo currentAnima = m_Animator.GetCurrentAnimatorStateInfo(0);
            return (currentAnima.IsName("End") && currentAnima.normalizedTime >= 1);
        });

        m_SoundManager.PlayInWorld(1, this.transform.position);
        LoadResult();  // Load stratagem result from object pool.

        m_eState = eState.Idle;
        yield break;
    }

    /*-----------------------------------------
     * It's a timer for cooling down process. *
     -----------------------------------------*/

    private IEnumerator DoCoolDown(float targetTime)
    {
        m_IsCooling = true;
        if (OnStartCoolDown != null) OnStartCoolDown();

        m_CoolTimer = 0.0f;
        while (m_CoolTimer < targetTime)
        {
            if (OnCoolDown != null) OnCoolDown();
            yield return new WaitForSeconds(Time.deltaTime);
            m_CoolTimer += Time.deltaTime;
        }
        m_IsCooling = false;
        if (OnEndCoolDown != null) OnEndCoolDown();
        yield break;
    }

    /*--------------------------------------------------------
     * If pool is exist. Load result object form Object pool *
     * Or create by self onject resource m_Result            *
     --------------------------------------------------------*/

    private void LoadResult()
    {
        GameObject result = null;
        if (ObjectPool.m_Instance == null)
        {
            result = Instantiate(m_Result);
        }
        else
        {
            result = ObjectPool.m_Instance.LoadGameObjectFromPool(m_Info.ResultID);
        }

        if (result != null)
        {
            Quaternion randRotation = Quaternion.Euler(0.0f, Random.Range(0, 360), 0.0f);
            result.transform.SetPositionAndRotation(this.transform.position, randRotation);
            result.SetActive(true);
        }
    }

    #endregion Finite State Machine
}