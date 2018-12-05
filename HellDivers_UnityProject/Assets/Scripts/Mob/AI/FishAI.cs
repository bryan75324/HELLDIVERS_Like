using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MobAnimationsController))]
public class FishAI : Character
{
    FSMSystem m_FSM;
    public MobInfo m_AIData;
    public eFSMStateID m_CurrentState;
    public GameObject m_GODeadBlood;
    public GameObject m_GOHurtBlood;
    public BloodSpurt m_DeadBloodSpurt;
    public BloodSpurt m_HurtBloodSpurt;

    private MobAnimationsController m_MobAnimator;
    private BoxCollider m_BodyCollider;
    private CapsuleCollider m_DamageCollider;
    private float Timer = 2.0f;

    private SoundManager m_SoundManager;

    #region Events

    public delegate void MobEventHolder();
    public event MobEventHolder OnSpawn;
    public event MobEventHolder OnDeath;

    #endregion

    // Use this for initialization
    private void Awake()
    {
    }
    private void OnEnable()
    {
        m_SoundManager = this.GetComponent<SoundManager>();
        SoundDataSetting Soundsetting = ResourceManager.m_Instance.LoadData(typeof(SoundDataSetting), "Sounds/Mobs/Fish", "SoundDataSetting") as SoundDataSetting;
        m_SoundManager.SetAudioClips(Soundsetting.SoundDatas);
        MobManager.m_Instance.OnDestroyAll += Death;
        if (m_FSM == null) return;
        m_AIData.m_Go = this.gameObject;
        m_bDead = false;
        m_CurrentHp = m_MaxHp;
        m_BodyCollider.enabled = true;
        m_DamageCollider.enabled = true;
        m_GODeadBlood = null;
        m_GOHurtBlood = null;
        m_DeadBloodSpurt = null;
        m_HurtBloodSpurt = null;
        m_FSM.PerformTransition(eFSMTransition.Go_Respawn);
        if (OnSpawn != null) OnSpawn();
    }
    public void OnDisable()
    {
        MobManager.m_Instance.OnDestroyAll -= Death;
    }
    protected override void Start()
    {
        m_AIData = new MobInfo();
        GameData.Instance.MobInfoTable[3100].CopyTo(m_AIData);
        
        m_MaxHp = m_AIData.m_fHp;
        base.Start();

        m_MobAnimator = this.GetComponent<MobAnimationsController>();
        m_BodyCollider = this.GetComponent<BoxCollider>();
        m_DamageCollider = this.GetComponentInChildren<CapsuleCollider>();
        m_FSM = new FSMSystem(m_AIData);
        m_AIData.m_Go = this.gameObject;
        m_AIData.m_FSMSystem = m_FSM;
        m_AIData.m_AnimationController = this.GetComponent<MobAnimationsController>();
        m_AIData.navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_AIData.navMeshAgent.speed = Random.Range(6.5f, 7.0f);
        m_AIData.navMeshAgent.enabled = false;
        m_AIData.m_SoundManager = m_SoundManager;

        #region FSMMap
        FSMRespawnState m_RespawnState = new FSMRespawnState();
        FSMChaseState m_Chasestate = new FSMChaseState();
        FSMAttackState m_Attackstate = new FSMAttackState();
        FSMIdleState m_IdleState = new FSMIdleState();
        FSMWanderIdleState m_WanderIdleState = new FSMWanderIdleState();
        FSMWanderState m_WanderState = new FSMWanderState();

        m_RespawnState.AddTransition(eFSMTransition.Go_WanderIdle, m_WanderIdleState);

        m_Chasestate.AddTransition(eFSMTransition.Go_Attack, m_Attackstate);

        m_Attackstate.AddTransition(eFSMTransition.Go_Idle, m_IdleState);
        m_Attackstate.AddTransition(eFSMTransition.Go_Chase, m_Chasestate);

        m_IdleState.AddTransition(eFSMTransition.Go_Chase, m_Chasestate);
        m_IdleState.AddTransition(eFSMTransition.Go_Attack, m_Attackstate);

        m_WanderIdleState.AddTransition(eFSMTransition.Go_Wander, m_WanderState);
        m_WanderIdleState.AddTransition(eFSMTransition.Go_Chase, m_Chasestate);

        m_WanderState.AddTransition(eFSMTransition.Go_WanderIdle, m_WanderIdleState);
        m_WanderState.AddTransition(eFSMTransition.Go_Chase, m_Chasestate);

        FSMFishGetHurtState m_GetHurtState = new FSMFishGetHurtState();
        FSMDeadState m_DeadState = new FSMDeadState();

        m_GetHurtState.AddTransition(eFSMTransition.Go_Chase, m_Chasestate);
        m_GetHurtState.AddTransition(eFSMTransition.Go_Attack, m_Attackstate);

        m_DeadState.AddTransition(eFSMTransition.Go_Respawn, m_RespawnState);

        m_FSM.AddGlobalTransition(eFSMTransition.Go_WanderIdle, m_WanderIdleState);
        m_FSM.AddGlobalTransition(eFSMTransition.Go_FishGetHurt, m_GetHurtState);
        m_FSM.AddGlobalTransition(eFSMTransition.Go_Dead, m_DeadState);

        m_FSM.AddState(m_RespawnState);
        m_FSM.AddState(m_WanderIdleState);
        m_FSM.AddState(m_IdleState);
        m_FSM.AddState(m_Chasestate);
        m_FSM.AddState(m_Attackstate);
        m_FSM.AddState(m_GetHurtState);
        m_FSM.AddState(m_DeadState);
        m_FSM.AddState(m_WanderState);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bDead == false)
        {
            Timer += Time.deltaTime;

            if (Timer > 2.0f)
            {
                MobInfo.AIFunction.SearchPlayer(m_AIData);
                Timer = 0.0f;
                return;
            }
            if (m_AIData.m_Player == null || m_AIData.m_Player.IsDead)
            {
                MobInfo.AIFunction.SearchPlayer(m_AIData);
            }
            if (MobInfo.AIFunction.CheckAllPlayersLife() == false)
            {
                if (m_CurrentState != eFSMStateID.WanderIdleStateID && m_CurrentState != eFSMStateID.WanderStateID)
                {
                    m_FSM.PerformGlobalTransition(eFSMTransition.Go_WanderIdle);
                }
            }
        }
        
        m_FSM.DoState();

        m_CurrentState = m_AIData.m_FSMSystem.CurrentStateID;
        if (Input.GetKeyDown(KeyCode.U)) Death();
    }

    public void PerformGetHurt(Vector3 point)
    {
        if (IsDead) return;
        AnimatorStateInfo info = m_MobAnimator.Animator.GetCurrentAnimatorStateInfo(0);
        if (m_MobAnimator.Animator.IsInTransition(0) || info.IsName("GetHurt"))
        {
            return;
        }
        StartCoroutine(Displacement(point, 1.0f));
        m_FSM.PerformGlobalTransition(eFSMTransition.Go_FishGetHurt);
        return;
    }

    IEnumerator Displacement(Vector3 point, float time)
    {
        Vector3 dir = this.transform.position - point;
        dir.y = 0;
        dir.Normalize();
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            this.transform.position += dir * Time.deltaTime;
            yield return 0;
        }
    }

    public void PerformDead()
    {
        m_MobAnimator.Animator.ResetTrigger("GetHurt");
        m_FSM.PerformGlobalTransition(eFSMTransition.Go_Dead);
    }

    public override bool TakeDamage(float damage, Vector3 hitPoint)
    {
        if (IsDead) return false;
        CurrentHp -= damage;
        
        if (m_CurrentHp <= 0)
        {
            m_BodyCollider.enabled = false;
            m_DamageCollider.enabled = false;
            StartCoroutine(Displacement(hitPoint, 0.2f));

            m_GODeadBlood = ObjectPool.m_Instance.LoadGameObjectFromPool(3004);
            m_DeadBloodSpurt = m_GODeadBlood.GetComponent<BloodSpurt>();
            m_DeadBloodSpurt.Init(m_AIData, this.transform.position + Vector3.up);
            m_SoundManager.PlayInWorld(3904, this.transform.position, 0.5f);
            Death();
        }
        else
        {
            m_GOHurtBlood = ObjectPool.m_Instance.LoadGameObjectFromPool(3003);
            m_HurtBloodSpurt = m_GOHurtBlood.GetComponent<BloodSpurt>();
            m_HurtBloodSpurt.Init(m_AIData, hitPoint);
            PerformGetHurt(hitPoint);
        }
        return true;
    }
    
    public override bool TakeDamage(IDamager damager, Vector3 hitPoint)
    {
        if(TakeDamage(damager.Damage, hitPoint))
        {
            if (damager.Damager != null && IsDead)
            {
                damager.Damager.Record.NumOfKills++;
                damager.Damager.Record.Exp += (int)m_AIData.m_Exp;
                damager.Damager.Record.Money += (int)m_AIData.m_Money;
            }
            return true;
        }
        return false;
    }

    public override void Death()
    {
        m_bDead = true;
        PerformDead();
        if (OnDeath != null) OnDeath();
    }

    private void OnDrawGizmos()
    {
    //    if (m_AIData == null || m_FSM == null)
    //    {
    //        return;
    //    }
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 2.0f);

    //    if (m_FSM.CurrentStateID == eFSMStateID.ChaseStateID)
    //    {
    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawLine(this.transform.position, m_AIData.m_vTarget);
    //    }
    //    else if (m_FSM.CurrentStateID == eFSMStateID.AttackStateID)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawLine(this.transform.position, m_AIData.m_vTarget);
    //    }
    //    //Gizmos.DrawWireSphere(m_AIData.m_vTarget, 0.5f);

    //    Gizmos.DrawWireSphere(this.transform.position, m_AIData.m_fAttackRange);
    }
}
