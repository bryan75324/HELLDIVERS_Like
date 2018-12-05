using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundManager))]
[RequireComponent(typeof(GrenadesController))]
[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(StratagemController))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(CharacterController))]
public class Player : Character
{
    #region Properties

    /// <summary>
    /// Repersent of player serial number.
    /// </summary>
    public int SerialNumber { get { return m_SerialNumber; } }

    /// <summary>
    /// Current player infos.
    /// </summary>
    public PlayerInfo Info { get { return m_Data; } }

    /// <summary>
    /// Repersent of player records in game.
    /// </summary>
    public PlayerRecord Record { get { return m_Record; } }

    /// <summary>
    /// Repersent of player input settings.
    /// </summary>
    public ControllerSetting InputSettting { get { return m_ControllerSetting; } }

    /// <summary>
    /// Player weapon behavior controller.
    /// </summary>
    public WeaponController WeaponController { get { return m_WeapoonController; } }

    /// <summary>
    /// Player stratagem behavior controller.
    /// </summary>
    public StratagemController StratagemController { get { return m_StratagemController; } }

    /// <summary>
    /// Player grenade behavior controller.
    /// </summary>
    public GrenadesController GrenadesController { get { return m_GrenadesController; } }

    /// <summary>
    /// Player sound behavior controller.
    /// </summary>
    public SoundManager SoundManager { get { return m_SoundManager; } }

    /// <summary>
    /// Repersent of player body's transform.
    /// </summary>
    public PlayerParts Parts { get { return m_Parts; } }

    #endregion Properties

    #region Private Variable

    private int m_SerialNumber;
    private PlayerInfo m_Data;
    private PlayerRecord m_Record;
    private PlayerParts m_Parts;
    private ControllerSetting m_ControllerSetting;
    private PlayerController m_Controller;
    private StratagemController m_StratagemController;
    private WeaponController m_WeapoonController;
    private GrenadesController m_GrenadesController;
    private SoundManager m_SoundManager;

    #endregion Private Variable

    #region Event

    public delegate void PlayerEventHolder();

    public delegate void PlayerStatusCollect(Player player);

    public event PlayerEventHolder OnSpawnBegin;

    public event PlayerEventHolder OnSpawnFinish;

    public event PlayerEventHolder OnDeathBegin;

    public event PlayerEventHolder OnDamaged;

    public event PlayerEventHolder OnTakeHealth;

    #endregion Event

    #region Initializer

    /// <summary>
    /// Initialize by player info to setup player's weapons and stratagems.
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(PlayerInfo data, int serialNum = 1)
    {
        data.CopyTo(m_Data);
        m_SerialNumber = serialNum;
        m_Record = new PlayerRecord();

        this.OnDeathBegin += () => { m_Record.TimesOfDeath++; };
        m_ControllerSetting = PlayerManager.Instance.Players[serialNum].controllerSetting;
        m_Controller.SetJoyNumber(m_ControllerSetting);

        // Setup stratagems
        m_StratagemController.CheckCodesMechine.InputVertical = m_ControllerSetting.StratagemVertical;
        m_StratagemController.CheckCodesMechine.InputHorizontal = m_ControllerSetting.StratagemHorizontal;
        if (m_StratagemController.Stratagems.Count > 0) m_StratagemController.Clear();
        if (m_Data.Stratagems.Count > 0) m_StratagemController.AddStratagems(m_Data.Stratagems, m_Parts.RightHand, m_Parts.LaunchPoint);

        // Setup weapons
        m_WeapoonController.ClearWeapon();
        m_WeapoonController.OnFire += () => { m_Record.Shots++; };
        if (m_Data.Weapons.Count > 0) m_WeapoonController.AddMultiWeapons(m_Data.Weapons, m_Parts.LaunchPoint, this);

        // Setup grenades
        if (m_Data.Grenades.Count > 0) m_GrenadesController.AddGrenades(data.Grenades, m_Parts.RightHand, m_Parts.LaunchPoint, this);

        // Setup sounds
        SoundDataSetting soundData = ResourceManager.m_Instance.LoadData(typeof(SoundDataSetting), "Sounds/Player", "SoundDataSetting") as SoundDataSetting;
        m_SoundManager.SetAudioClips(soundData.SoundDatas);
        OnSpawnFinish += () => m_SoundManager.PlayInWorld(UnityEngine.Random.Range(1010, 1013), this.transform.position);
        OnDeathBegin += () => m_SoundManager.PlayInWorld(UnityEngine.Random.Range(1020, 1023), this.transform.position);
        OnDamaged += () => m_SoundManager.PlayInWorld(UnityEngine.Random.Range(1030, 1033), this.transform.position);
    }

    #endregion Initializer

    #region MonoBehaviour

    private void Awake()
    {
        this.tag = "Player";
        m_Data = new PlayerInfo();
        m_Parts = GetComponent<PlayerParts>();
        m_Controller = GetComponent<PlayerController>();
        m_WeapoonController = GetComponent<WeaponController>();
        m_StratagemController = GetComponent<StratagemController>();
        m_GrenadesController = GetComponent<GrenadesController>();
        m_SoundManager = GetComponent<SoundManager>();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion MonoBehaviour

    #region Public Function

    /// <summary>
    /// Set player spawn on the position.
    /// </summary>
    /// <param name="spawnPos">Spawn position</param>
    public void Spawn(Vector3 spawnPos)
    {
        m_CurrentHp = m_MaxHp;

        this.transform.position = spawnPos;

        // Reset stratagem
        m_StratagemController.Reset();

        // Reset weapons
        m_WeapoonController.ResetWeaponInfo();

        // Reset grenades
        m_GrenadesController.ResetGrenades();

        this.gameObject.SetActive(true);
        m_bDead = false;
        StartCoroutine(OnSpawn());

        if (OnSpawnBegin != null) OnSpawnBegin();
    }

    /// <summary>
    /// Increase current health point HP.
    /// </summary>
    /// <param name="heal">Heal point</param>
    /// <returns>Was the current health point increased or not ?</returns>
    public bool TakeHealth(float heal)
    {
        if (IsDead || m_CurrentHp >= m_MaxHp) return false;

        CurrentHp += heal;

        if (OnTakeHealth != null) OnTakeHealth();
        return true;
    }

    /// <summary>
    /// Decrease current health point by damage point.
    /// </summary>
    /// <param name="dmg">Damage point</param>
    /// <param name="hitPoint">Hit point position</param>
    /// <returns>Was the current health point decreased or not ?</returns>
    public override bool TakeDamage(float dmg, Vector3 hitPoint)
    {
        if (m_Controller.m_PlayerFSM.CurrentGlobalStateID == ePlayerFSMStateID.RollStateID) return false;
        bool bHurt = m_Controller.PerformPlayerHurt();
        if (bHurt == false) return false;

        if (base.TakeDamage(dmg, hitPoint))
        {
            if (OnDamaged != null) OnDamaged();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Do dead. perform daead animation then disable this player.
    /// </summary>
    [ContextMenu("DoDeath")]
    public override void Death()
    {
        if (IsDead) return;
        m_bDead = true;
        StartCoroutine(DoDeath());

        // Reset stragem
        m_StratagemController.Reset();

        if (OnDeathBegin != null) OnDeathBegin();
    }

    /// <summary>
    /// Interact with items which are exist in interactive item manager.
    /// </summary>
    public void InteractWithItem()
    {
        if (InteractiveItemManager.Instance == null) return;
        InteractiveItemManager.Instance.OnInteractive(this);
    }

    public void Victory()
    {
        if (IsDead) return;
        m_Controller.PerformPlayerVictory();
    }

    #endregion Public Function

    #region Private Function

    /*--------------------------------------------------------
     * Check animation time was finished and set this alive. *
     --------------------------------------------------------*/

    private IEnumerator OnSpawn()
    {
        if (m_Controller != null)
        {
            m_Controller.PerformPlayerRelive();
            yield return new WaitUntil(() => m_Controller.bIsAlive);
        }

        GameMain.Instance.CameraFolloing.AddTarget(this.transform);
        m_bDead = false;

        if (OnSpawnFinish != null) OnSpawnFinish();
    }

    /*------------------------------------------------------
     * Check animation time was finished and disable this. *
     ------------------------------------------------------*/

    private IEnumerator DoDeath()
    {
        if (m_Controller != null)
        {
            m_Controller.PerformPlayerDead();
            yield return new WaitUntil(() => m_Controller.bIsDead);
        }

        GameMain.Instance.CameraFolloing.RemoveTarget(this.transform);

        InGamePlayerManager.Instance.RespawnPlayer(this);
        this.gameObject.SetActive(false);
    }

    #endregion Private Function
}