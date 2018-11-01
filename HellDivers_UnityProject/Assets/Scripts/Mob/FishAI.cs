﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MobAnimationsController))]
public class FishAI : Character {
    
    FSMSystem m_FSM;
    public AIData m_AIData;
    private MobAnimationsController m_MobAnimator;
    // Use this for initialization
    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        m_bDead = false;
        m_CurrentHp = m_MaxHp;
    }
    protected override void Start () {
        m_MaxHp = 1000;
        base.Start();
        
        m_MobAnimator = this.GetComponent<MobAnimationsController>();
        m_AIData = new AIData();
        m_FSM = new FSMSystem(m_AIData);
        m_AIData.m_Go = this.gameObject;
        m_AIData.m_FSMSystem = m_FSM;
        m_AIData.m_AnimationController = this.GetComponent<MobAnimationsController>();
        m_AIData.navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_AIData.navMeshAgent.enabled = false;
        
        FSMChaseState m_Chasestate = new FSMChaseState();
        FSMAttackState m_Attackstate = new FSMAttackState();
        FSMIdleState m_IdleState = new FSMIdleState();
        
        m_Chasestate.AddTransition(eFSMTransition.Go_Attack, m_Attackstate);
        
        m_Attackstate.AddTransition(eFSMTransition.Go_Idle, m_IdleState);
        m_Attackstate.AddTransition(eFSMTransition.Go_Chase, m_Chasestate);

        m_IdleState.AddTransition(eFSMTransition.Go_Chase, m_Chasestate);
        m_IdleState.AddTransition(eFSMTransition.Go_Attack, m_Attackstate);

        FSMGetHurtState m_GetHurtState = new FSMGetHurtState();
        FSMDeadState m_DeadState = new FSMDeadState();

        m_GetHurtState.AddTransition(eFSMTransition.Go_Chase, m_Chasestate);
        m_GetHurtState.AddTransition(eFSMTransition.Go_Attack, m_Attackstate);

        m_DeadState.AddTransition(eFSMTransition.Go_Chase, m_Chasestate);

        m_FSM.AddGlobalTransition(eFSMTransition.Go_Dead, m_DeadState);
        m_FSM.AddGlobalTransition(eFSMTransition.Go_GetHurt, m_GetHurtState);
        
        m_FSM.AddState(m_Chasestate);
        m_FSM.AddState(m_Attackstate);
        m_FSM.AddState(m_DeadState);
    }
	
	// Update is called once per frame
	void Update () {
        if (m_AIData.m_PlayerGO == null)
        {
            m_AIData.m_PlayerGO = GameObject.FindGameObjectWithTag("Player");
        }

        m_FSM.DoState();

        if (m_bDead)
        {
            AnimatorStateInfo info = m_MobAnimator.Animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Dead"))
            {
                if (info.normalizedTime > 0.9f)
                {
                    m_FSM.PerformTransition(eFSMTransition.Go_MoveTo);
                    ObjectPool.m_Instance.UnLoadObjectToPool(3001, this.gameObject);
                    MobManager.m_FishCount--;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.U)) Death();
    }
    public override void Death()
    {
        m_bDead = true;
        m_FSM.PerformGlobalTransition(eFSMTransition.Go_Dead);
    }
    public void PerformGetHurt()
    {
        AnimatorStateInfo info = m_MobAnimator.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("GetHurt"))
        {
            return;
        }
        m_FSM.PerformGlobalTransition(eFSMTransition.Go_GetHurt);
        return;
    }
    public override bool TakeDamage(float dmg, Vector3 hitPoint)
    {
        if (IsDead) return false;

        CurrentHp -= dmg;
        if (m_CurrentHp <= 0)
        {
            Death();
            return true;
        }
        PerformGetHurt();
        return true;
    }
    public override bool TakeDamage(IDamager damager, Vector3 hitPoint)
    {
        return TakeDamage(damager.Damage, hitPoint);
    }

    private void OnDrawGizmos()
    {
        if (m_AIData == null || m_FSM == null)
        {
            return;
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 2.0f);
        if (m_FSM.CurrentStateID == eFSMStateID.MoveToStateID)
        {
            Gizmos.color = Color.green;
        }
        else if (m_FSM.CurrentStateID == eFSMStateID.ChaseStateID)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(this.transform.position, m_AIData.m_vTarget);
        }
        else if (m_FSM.CurrentStateID == eFSMStateID.AttackStateID)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, m_AIData.m_vTarget);
        }
        Gizmos.DrawWireSphere(m_AIData.m_vTarget, 0.5f);

        Gizmos.DrawWireSphere(this.transform.position, m_AIData.m_fAttackRange);
    }
}
