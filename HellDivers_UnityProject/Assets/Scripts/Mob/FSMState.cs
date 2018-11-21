﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eFSMTransition
{
    NullTransition = 0,
    Go_Respawn,
    Go_Idle,
    Go_MoveTo,
    Go_Chase,
    Go_ChaseToRemoteAttack,
    Go_Attack,
    Go_PatrolAttack,
    Go_RemoteAttack,
    Go_FishGetHurt,
    Go_PatrolGetHurt,
    Go_TankGetHurt,
    Go_WanderIdle,
    Go_NoPlayerWanderIdle,
    Go_Wander,
    Go_NoPlayerWander,
    Go_CallArmy,
    Go_Flee,
    Go_Dodge,
    Go_Dead,
}
public enum eFSMStateID
{
    NullStateID = 0,
    RespawnStateID,
    IdleStateID,
    MoveToStateID,
    ChaseStateID,
    ChaseToRemoteAttackStateID,
    AttackStateID,
    PatrolAttackStateID,
    RemoteAttackStateID,
    FishGetHurtStateID,
    PatrolGetHurtStateID,
    TankGetHurtStateID,
    WanderIdleStateID,
    NoPlayerWanderIdleStateID,
    WanderStateID,
    NoPlayerWanderStateID,
    CallArmyStateID,
    FleeStateID,
    DodgeStateID,
    DeadStateID,
}

public class FSMState
{
    public eFSMStateID m_StateID;
    public Dictionary<eFSMTransition, FSMState> m_Map;
    public float m_fCurrentTime;
    public float m_AnimatorLeaveTime;

    public FSMState()
    {
        m_StateID = eFSMStateID.NullStateID;
        m_fCurrentTime = 0.0f;
        m_Map = new Dictionary<eFSMTransition, FSMState>();
    }

    public void AddTransition(eFSMTransition trans, FSMState toState)
    {
        if (m_Map.ContainsKey(trans))
        {
            return;
        }

        m_Map.Add(trans, toState);
    }
    public void DelTransition(eFSMTransition trans)
    {
        if (m_Map.ContainsKey(trans))
        {
            m_Map.Remove(trans);
        }

    }

    public FSMState TransitionTo(eFSMTransition trans)
    {
        if (m_Map.ContainsKey(trans) == false)
        {
            return null;
        }
        return m_Map[trans];
    }

    public virtual void DoBeforeEnter(MobInfo data)
    {

    }

    public virtual void DoBeforeLeave(MobInfo data)
    {

    }

    public virtual void Do(MobInfo data)
    {

    }

    public virtual void CheckCondition(MobInfo data)
    {

    }
}

public class FSMRespawnState : FSMState
{
    private float m_fIdleTime;
    GameObject GO;
    Animator m_EffectAnimator;
    public FSMRespawnState()
    {
        m_StateID = eFSMStateID.RespawnStateID;
    }
    public override void DoBeforeEnter(MobInfo data)
    {
        m_fCurrentTime = 0.0f;
        m_fIdleTime = Random.Range(1.0f, 1.5f);

        GO = ObjectPool.m_Instance.LoadGameObjectFromPool(3001);
        GO.transform.position = data.m_Go.transform.position;
        GO.SetActive(true);
        m_EffectAnimator = GO.GetComponent<Animator>();
        m_EffectAnimator.SetTrigger("startTrigger");
    }

    public override void DoBeforeLeave(MobInfo data)
    {
        ObjectPool.m_Instance.UnLoadObjectToPool(3001, GO);
    }

    public override void Do(MobInfo data)
    {
        m_fCurrentTime += Time.deltaTime;
    }

    public override void CheckCondition(MobInfo data)
    {
        if (m_fCurrentTime > m_fIdleTime)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_WanderIdle);
        }
    }
}

public class FSMIdleState : FSMState
{
    private float m_fIdleTim;

    public FSMIdleState()
    {
        m_StateID = eFSMStateID.IdleStateID;
    }
    public override void DoBeforeEnter(MobInfo data)
    {
        m_fCurrentTime = 0.0f;
        m_fIdleTim = Random.Range(1.0f, 2.0f);
        data.m_AnimationController.SetAnimator(m_StateID, true);
    }

    public override void DoBeforeLeave(MobInfo data)
    {
        data.m_AnimationController.SetAnimator(m_StateID, false);
    }

    public override void Do(MobInfo data)
    {
        m_fCurrentTime += Time.deltaTime;
    }

    public override void CheckCondition(MobInfo data)
    {
        if (data.m_Player == null || data.m_Player.IsDead)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_WanderIdle);
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_NoPlayerWanderIdle);
            return;
        }
        if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude <= data.m_fAttackRange)
        {
            if (m_fCurrentTime > m_fIdleTim)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
            }
        }
        if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude > data.m_fAttackRange)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
        }
        if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude < data.m_fPatrolVisionLength)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Dodge);
        }
    }
}

public class FSMMoveToState : FSMState
{
    public FSMMoveToState()
    {
        m_StateID = eFSMStateID.MoveToStateID;
    }


    public override void DoBeforeEnter(MobInfo data)
    {

    }

    public override void DoBeforeLeave(MobInfo data)
    {

    }

    public override void Do(MobInfo data)
    {
        data.navMeshAgent.enabled = true;
        data.m_vTarget = data.m_Player.transform.position;
        SteeringBehaviours.NavMove(data);
        Vector3 v = (SteeringBehaviours.GroupBehavior(data, 20, true) + SteeringBehaviours.GroupBehavior(data, 20, false)) * 2f * Time.deltaTime;
        data.m_Go.transform.position += v;
    }

    public override void CheckCondition(MobInfo data)
    {
        bool bAttack = false;
        GameObject go = MobInfo.AIFunction.CheckEnemyInSight(data, ref bAttack);
        if (go != null)
        {
            if (bAttack)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
            }
            else
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
            }
            return;
        }
    }
}

public class FSMChaseState : FSMState
{
    public FSMChaseState()
    {
        m_StateID = eFSMStateID.ChaseStateID;
    }

    public override void DoBeforeEnter(MobInfo data)
    {
        data.m_AnimationController.SetAnimator(m_StateID, true);
    }

    public override void DoBeforeLeave(MobInfo data)
    {
        data.m_AnimationController.SetAnimator(m_StateID, false);
    }

    public override void Do(MobInfo data)
    {
        data.navMeshAgent.enabled = true;
        data.m_vTarget = data.m_Player.transform.position;
        SteeringBehaviours.NavMove(data);
        Vector3 v = (SteeringBehaviours.GroupBehavior(data, 20, true) + SteeringBehaviours.GroupBehavior(data, 20, false)) * 2f * Time.deltaTime;
        data.m_Go.transform.position += v;
    }

    public override void CheckCondition(MobInfo data)
    {
        if (data.m_Player == null || data.m_Player.IsDead)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_WanderIdle);
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_NoPlayerWanderIdle);
            return;
        }

        bool bAttack = false;
        MobInfo.AIFunction.CheckTargetEnemyInSight(data, data.m_Player.gameObject, ref bAttack);

        if (bAttack)
        {
            AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Move"))
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
            }
        }
    }
}

public class FSMChaseToRemoteAttackState : FSMState
{
    public FSMChaseToRemoteAttackState()
    {
        m_StateID = eFSMStateID.ChaseToRemoteAttackStateID;
    }

    public override void DoBeforeEnter(MobInfo data)
    {
        data.m_AnimationController.SetAnimator(m_StateID, true);
    }

    public override void DoBeforeLeave(MobInfo data)
    {
        data.m_AnimationController.SetAnimator(m_StateID, false);
    }

    public override void Do(MobInfo data)
    {
        data.navMeshAgent.enabled = true;
        data.m_vTarget = data.m_Player.transform.position;
        SteeringBehaviours.NavMove(data);
        Vector3 v = (SteeringBehaviours.GroupBehavior(data, 20, true) + SteeringBehaviours.GroupBehavior(data, 20, false)) * 2f * Time.deltaTime;
        data.m_Go.transform.position += v;
    }

    public override void CheckCondition(MobInfo data)
    {
        if (data.m_Player == null || data.m_Player.IsDead)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_WanderIdle);
            return;
        }
        if((data.m_Player.transform.position - data.m_Go.transform.position).magnitude < data.m_fPatrolVisionLength)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_RemoteAttack);
        }
        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Move"))
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
        }
    }
}

public class FSMAttackState : FSMState
{
    private int AttackCount = 0;
    private int Count = 0;
    Vector3 vDir;
    public FSMAttackState()
    {
        m_StateID = eFSMStateID.AttackStateID;
    }

    public override void DoBeforeEnter(MobInfo data)
    {
        AttackCount = 0;
        Count = 0;
        m_AnimatorLeaveTime = Random.Range(0.7f, 1.0f);
        data.navMeshAgent.enabled = false;
    }

    public override void DoBeforeLeave(MobInfo data)
    {

    }


    public override void Do(MobInfo data)
    {
        data.navMeshAgent.enabled = true;

        data.m_vTarget = data.m_Player.transform.position;
        vDir = data.m_Player.transform.position - data.m_Go.transform.position;

        if (Vector3.Angle(data.m_Go.transform.forward, vDir) >= 5.0f && Count < 1)
        {
            float fRight = Vector3.Dot(vDir.normalized, data.m_Go.transform.right);
            if (fRight >= 0)
            {
                data.m_Go.transform.Rotate(new Vector3(0, 10, 0), Space.Self);
            }
            else if (fRight < 0)
            {
                data.m_Go.transform.Rotate(new Vector3(0, -10, 0), Space.Self);
            }
        }
        if (Vector3.Angle(vDir, data.m_Go.transform.forward) <= 10.0f && Count < 1)
        {
            data.m_AnimationController.SetAnimator(m_StateID);
            Count++;
        }

        Vector3 v = (SteeringBehaviours.GroupBehavior(data, 10, true) + SteeringBehaviours.GroupBehavior(data, 10, false)) * 2f * Time.deltaTime;
        data.m_Go.transform.position += v;

        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Attack"))
        {
            if (info.normalizedTime > 0.27f && AttackCount < 1)
            {
                if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude <= data.m_fAttackRange + 0.5f)
                    DoDamage(data);
                AttackCount++;
            }
        }
    }

    public override void CheckCondition(MobInfo data)
    {
        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (data.m_Player == null || data.m_Player.IsDead)
        {
            if (info.normalizedTime > m_AnimatorLeaveTime)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_WanderIdle);
            }
            return;
        }
        if (info.IsName("Attack"))
        {
            if (info.normalizedTime > m_AnimatorLeaveTime)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
            }
        }

    }

    private void DoDamage(MobInfo data)
    {
        data.m_Player.TakeDamage(10.0f, data.m_Go.transform.position);
    }
}

public class FSMPatrolAttackState : FSMState
{
    int Count = 0;
    int AttackCount = 0;
    bool bIsReady = false;
    bool bFire = false;
    bool bCompleteFire = false;
    float FireCoolDownTime = 0.0f;
    int bulletIndex = 0;
    List<float> Degree = new List<float>();
    GameObject[] GO = new GameObject[3];
    float[] fireDegree = new float[3];
    Vector3 vDir;
    public FSMPatrolAttackState()
    {
        m_StateID = eFSMStateID.PatrolAttackStateID;
    }

    public override void DoBeforeEnter(MobInfo data)
    {
        Count = 0;
        AttackCount = 0;
        m_fCurrentTime = 0;
        bIsReady = false;
        bFire = false;
        m_AnimatorLeaveTime = Random.Range(0.7f, 1.0f);
        data.navMeshAgent.enabled = false;
    }

    public override void DoBeforeLeave(MobInfo data)
    {
        m_fCurrentTime = 0;
        bulletIndex = 0;
        bIsReady = false;
        bCompleteFire = false;
        data.m_MobAimLine.CloseAimLine();
    }
    public override void Do(MobInfo data)
    {
        if (bIsReady)
        {
            m_fCurrentTime += Time.deltaTime;
            if (m_fCurrentTime >= 1.0f)
            {
                data.m_AnimationController.SetAnimator(m_StateID);
                Count++;
                m_fCurrentTime = 0;
            }
            AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Attack"))
            {
                if (info.normalizedTime > 0.27f && AttackCount < 1)
                {
                    Fire(data);
                    data.m_MobAimLine.CloseAimLine();
                    AttackCount++;
                    bIsReady = false;
                }
            }
            return;
        }
        if (bFire)
        {
            if (bulletIndex == 3)
            {
                bFire = false;
                bCompleteFire = true;
            }
            FireCoolDownTime += Time.deltaTime;
            if (FireCoolDownTime > 0.2f)
            {
                GO[bulletIndex].SetActive(true);
                bulletIndex++;
                FireCoolDownTime = 0.0f;
            }
        }
        data.m_vTarget = data.m_Player.transform.position;
        vDir = data.m_Player.transform.position - data.m_Go.transform.position;

        if (Vector3.Angle(data.m_Go.transform.forward, vDir) >= 5.0f && Count < 1)
        {
            float fRight = Vector3.Dot(vDir.normalized, data.m_Go.transform.right);
            if (fRight >= 0)
            {
                data.m_Go.transform.Rotate(new Vector3(0, 5, 0), Space.Self);
            }
            else if (fRight < 0)
            {
                data.m_Go.transform.Rotate(new Vector3(0, -5, 0), Space.Self);
            }
        }
        if (Vector3.Angle(vDir, data.m_Go.transform.forward) <= 5f && Count < 1)
        {
            data.m_MobAimLine.OpenAimLine();
            bIsReady = true;
        }
    }

    public override void CheckCondition(MobInfo data)
    {
        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Attack"))
        {
            if (info.normalizedTime > m_AnimatorLeaveTime && bCompleteFire)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
            }
            return;
        }
        if (data.m_bIsPlayerDead)
        {
            if (info.normalizedTime > m_AnimatorLeaveTime)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_NoPlayerWanderIdle);
            }
        }
    }

    private void Fire(MobInfo data)
    {
        Degree.Add(-20);
        Degree.Add(0);
        Degree.Add(20);
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                int random = Random.Range(0, 3);
                fireDegree[i] = Degree[random];
                Degree.RemoveAt(random);
            }
            if (i == 1)
            {
                int random = Random.Range(0, 2);
                fireDegree[i] = Degree[random];
                Degree.RemoveAt(random);
            }
            if (i == 2)
            {
                int random = Random.Range(0, 1);
                fireDegree[i] = Degree[random];
                Degree.RemoveAt(random);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            GO[i] = ObjectPool.m_Instance.LoadGameObjectFromPool(3201);
            if (GO[i] == null) continue;
            GO[i].transform.position = data.m_Go.transform.position + Vector3.up;
            GO[i].transform.forward = data.m_Go.transform.forward;
            GO[i].transform.Rotate(new Vector3(0, fireDegree[i], 0));
        }
        bFire = true;
    }
}

public class FSMRemoteAttackState : FSMState
{
    int Count = 0;
    GameObject GOBullet;
    Vector3 vDir;
    public FSMRemoteAttackState()
    {
        m_StateID = eFSMStateID.RemoteAttackStateID;
    }

    public override void DoBeforeEnter(MobInfo data)
    {
        Count = 0;
        m_AnimatorLeaveTime = Random.Range(0.7f, 1.0f);
        data.navMeshAgent.enabled = false;
    }

    public override void DoBeforeLeave(MobInfo data)
    {

    }
    public override void Do(MobInfo data)
    {
        data.m_vTarget = data.m_Player.transform.position;
        vDir = data.m_Player.transform.position - data.m_Go.transform.position;

        if (Vector3.Angle(data.m_Go.transform.forward, vDir) >= 5.0f && Count < 1)
        {
            float fRight = Vector3.Dot(vDir.normalized, data.m_Go.transform.right);
            if (fRight >= 0)
            {
                data.m_Go.transform.Rotate(new Vector3(0, 5, 0), Space.Self);
            }
            else if (fRight < 0)
            {
                data.m_Go.transform.Rotate(new Vector3(0, -5, 0), Space.Self);
            }
        }
        if (Vector3.Angle(vDir, data.m_Go.transform.forward) <= 5f && Count < 1)
        {
            Count++;
            Fire(data);
        }
    }

    public override void CheckCondition(MobInfo data)
    {
        //temp...
        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);

        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Attack"))
        {
            if (info.normalizedTime > m_AnimatorLeaveTime)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
            }
            return;
        }
        if (data.m_Player == null || data.m_Player.IsDead)
        {
            if (info.normalizedTime > m_AnimatorLeaveTime)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_NoPlayerWanderIdle);
            }
        }
    }

    private void Fire(MobInfo data)
    {
        GOBullet = ObjectPool.m_Instance.LoadGameObjectFromPool(3201);
        if (GOBullet == null) return;
        GOBullet.transform.position = data.m_Go.transform.position + Vector3.up;
        GOBullet.transform.forward = data.m_Go.transform.forward;
        GOBullet.transform.Rotate(new Vector3(0, 0, 0));
        //GOBullet.SetActive(true);
    }
}

public class FSMFishGetHurtState : FSMState
{
    public FSMFishGetHurtState()
    {
        m_StateID = eFSMStateID.FishGetHurtStateID;
    }


    public override void DoBeforeEnter(MobInfo data)
    {
        data.navMeshAgent.enabled = false;
        data.m_AnimationController.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(MobInfo data)
    {

    }

    public override void Do(MobInfo data)
    {
        data.m_Go.transform.position += data.m_Go.transform.forward * -1 * Time.deltaTime;
    }

    public override void CheckCondition(MobInfo data)
    {

        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("GetHurt"))
        {
            if (info.normalizedTime > 0.95f)
            {
                Vector3 v = data.m_Player.transform.position - data.m_Go.transform.position;
                float fDist = v.magnitude;
                if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude < data.m_fAttackRange)
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
                }
                else
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
                }
            }
        }
    }
}

public class FSMPatrolGetHurtState : FSMState
{
    public FSMPatrolGetHurtState()
    {
        m_StateID = eFSMStateID.PatrolGetHurtStateID;
    }


    public override void DoBeforeEnter(MobInfo data)
    {
        data.navMeshAgent.enabled = false;
        data.m_AnimationController.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(MobInfo data)
    {

    }

    public override void Do(MobInfo data)
    {
        data.m_Go.transform.position += data.m_Go.transform.forward * -1 * Time.deltaTime;
    }

    public override void CheckCondition(MobInfo data)
    {
        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("GetHurt"))
        {
            if (info.normalizedTime > 0.9f)
            {
                if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude >= data.m_fPatrolVisionLength)
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
                    return;
                }
                if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude < data.m_fPatrolVisionLength)
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Dodge);
                    return;
                }
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Flee);
            }
        }
    }
}

public class FSMTankGetHurtState : FSMState
{
    public FSMTankGetHurtState()
    {
        m_StateID = eFSMStateID.TankGetHurtStateID;
    }


    public override void DoBeforeEnter(MobInfo data)
    {
        data.navMeshAgent.enabled = false;
        data.m_AnimationController.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(MobInfo data)
    {

    }

    public override void Do(MobInfo data)
    {
        data.m_Go.transform.position += data.m_Go.transform.forward * -1 * Time.deltaTime;
    }

    public override void CheckCondition(MobInfo data)
    {
        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("GetHurt"))
        {
            if (info.normalizedTime > 0.9f)
            {
                if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude >= data.m_fPatrolVisionLength)
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
                    return;
                }
                if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude < data.m_fPatrolVisionLength)
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Dodge);
                    return;
                }
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Flee);
            }
        }
    }
}

public class FSMWanderIdleState : FSMState
{
    private float m_fIdleTime;
    public FSMWanderIdleState()
    {
        m_StateID = eFSMStateID.WanderIdleStateID;
    }

    public override void DoBeforeEnter(MobInfo data)
    {
        m_AnimatorLeaveTime = Random.Range(0.7f, 1.0f);
        data.navMeshAgent.enabled = false;
        data.m_vTarget = new Vector3(0, 0, 0);
        m_fCurrentTime = 0.0f;
        m_fIdleTime = Random.Range(1.0f, 3.0f);
        data.m_AnimationController.SetAnimator(m_StateID, true);
    }

    public override void DoBeforeLeave(MobInfo data)
    {
        data.m_AnimationController.SetAnimator(m_StateID, false);
    }

    public override void Do(MobInfo data)
    {
        m_fCurrentTime += Time.deltaTime;
    }

    public override void CheckCondition(MobInfo data)
    {
        if (m_fCurrentTime > m_fIdleTime && SteeringBehaviours.CreatRandomTarget(data) == true)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Wander);
        }
        if (data.m_Player != null && data.m_Player.IsDead == false)
        {
            float Dist = (data.m_Player.transform.position - data.m_Go.transform.position).magnitude;

            if (Dist < data.m_fPatrolVisionLength)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Flee);
            }
            AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("WanderIdle"))
            {
                if (info.normalizedTime > m_AnimatorLeaveTime)
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_ChaseToRemoteAttack);
                }
            }
        }
    }
    public void ToIdle(MobInfo data)
    {
        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Idle);
    }
}

public class FSMWanderState : FSMState
{
    public FSMWanderState()
    {
        m_StateID = eFSMStateID.WanderStateID;
    }
    public override void DoBeforeEnter(MobInfo data)
    {
        data.navMeshAgent.enabled = true;
        data.m_AnimationController.SetAnimator(m_StateID, true);
    }
    public override void DoBeforeLeave(MobInfo data)
    {
        data.m_AnimationController.SetAnimator(m_StateID, false);
    }
    public override void Do(MobInfo data)
    {
        data.m_vTarget.y = data.m_Go.transform.position.y;
        SteeringBehaviours.NavMove(data);
    }
    public override void CheckCondition(MobInfo data)
    {
        if (data.m_ID == 3100)
        {
            if (data.m_Player == null || data.m_Player.IsDead)
            {
                if (Vector3.Distance(data.m_vTarget, data.m_Go.transform.position) < 0.1f)
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_WanderIdle);
                }
                return;
            }
            if (data.m_Player.IsDead == false)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
                return;
            }
            return;
        }
        if (data.m_ID == 3200)
        {
            if (Vector3.Distance(data.m_vTarget, data.m_Go.transform.position) < 0.1f)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_WanderIdle);
            }
            if (data.m_Player == null || data.m_Player.IsDead) return;
            float Dist = (data.m_Player.transform.position - data.m_Go.transform.position).magnitude;
            if (Dist < data.m_fPatrolVisionLength)
            {
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Flee);
            }
        }
    }
}

public class FSMNoPlayerWanderIdleState : FSMState
{
    private float m_fIdleTime;
    public FSMNoPlayerWanderIdleState()
    {
        m_StateID = eFSMStateID.NoPlayerWanderIdleStateID;
    }

    public override void DoBeforeEnter(MobInfo data)
    {
        m_AnimatorLeaveTime = Random.Range(0.7f, 1.0f);
        data.navMeshAgent.enabled = false;
        data.m_vTarget = new Vector3(0, 0, 0);
        m_fCurrentTime = 0.0f;
        m_fIdleTime = Random.Range(1.0f, 3.0f);
        data.m_AnimationController.SetAnimator(m_StateID, true);
    }

    public override void DoBeforeLeave(MobInfo data)
    {
        data.m_AnimationController.SetAnimator(m_StateID, false);
    }

    public override void Do(MobInfo data)
    {
        m_fCurrentTime += Time.deltaTime;
    }

    public override void CheckCondition(MobInfo data)
    {
        if (m_fCurrentTime > m_fIdleTime && SteeringBehaviours.CreatRandomTarget(data) == true)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_NoPlayerWander);
        }

        if (data.m_Player != null && data.m_Player.IsDead == false)
        {
            AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("WanderIdle2"))
            {
                if (info.normalizedTime > m_AnimatorLeaveTime)
                {
                    if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude > data.m_fAttackRange)
                    {
                        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
                    }
                    else
                    {
                        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Dodge);
                    }

                }
            }
        }
    }
}

public class FSMNoPlayerWanderState : FSMState
{
    public FSMNoPlayerWanderState()
    {
        m_StateID = eFSMStateID.NoPlayerWanderStateID;
    }
    public override void DoBeforeEnter(MobInfo data)
    {
        data.navMeshAgent.enabled = true;
        data.m_AnimationController.SetAnimator(m_StateID, true);
    }
    public override void DoBeforeLeave(MobInfo data)
    {
        data.m_AnimationController.SetAnimator(m_StateID, false);
    }
    public override void Do(MobInfo data)
    {
        data.m_vTarget.y = data.m_Go.transform.position.y;
        SteeringBehaviours.NavMove(data);
    }
    public override void CheckCondition(MobInfo data)
    {
        if (Vector3.Distance(data.m_vTarget, data.m_Go.transform.position) < 0.1f)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_NoPlayerWanderIdle);
        }

        if (data.m_Player != null && data.m_Player.IsDead == false)
        {
            AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("WanderIdle2"))
            {
                if (info.normalizedTime > m_AnimatorLeaveTime)
                {
                    if ((data.m_Player.transform.position - data.m_Go.transform.position).magnitude > data.m_fAttackRange)
                    {
                        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
                    }
                    else
                    {
                        data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Dodge);
                    }
                }
            }
        }
    }
}

public class FSMCallArmyState : FSMState
{
    int count = 0;
    public FSMCallArmyState()
    {
        m_StateID = eFSMStateID.CallArmyStateID;
    }

    public override void DoBeforeEnter(MobInfo data)
    {
        count = 0;
        data.m_AnimationController.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(MobInfo data)
    {

    }

    public override void Do(MobInfo data)
    {
        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("CallArmy") && count < 1)
        {
            if (info.normalizedTime > 0.5f)
            {
                MobManager.m_Instance.SpawnFish(10, data.m_Go.transform, 2, 5);
                count++;
            }
        }
    }

    public override void CheckCondition(MobInfo data)
    {
        float Dist = (data.m_Player.transform.position - data.m_Go.transform.position).magnitude;

        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("CallArmy"))
        {
            if (info.normalizedTime > 0.9f)
            {
                if (Dist < data.m_fPatrolVisionLength)
                {
                    data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Dodge);
                    return;
                }
                data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Chase);
            }
        }
    }
}

public class FSMFleeState : FSMState
{
    Animator m_Animator;
    Vector3 vec;
    GameObject GO;
    public FSMFleeState()
    {
        m_StateID = eFSMStateID.FleeStateID;
    }


    public override void DoBeforeEnter(MobInfo data)
    {
        data.navMeshAgent.enabled = true;
        data.navMeshAgent.speed *= 2f;
        data.m_AnimationController.SetAnimator(m_StateID, true);

        GO = ObjectPool.m_Instance.LoadGameObjectFromPool(3210);
        GO.SetActive(true);
        data.m_GOEffectWarning = GO;
        m_Animator = GO.GetComponent<Animator>();
        m_Animator.SetTrigger("startTrigger");
    }

    public override void DoBeforeLeave(MobInfo data)
    {
        data.navMeshAgent.speed *= 0.5f;
        data.m_AnimationController.SetAnimator(m_StateID, false);
        m_Animator.SetTrigger("endTrigger");
        ObjectPool.m_Instance.UnLoadObjectToPool(3210, GO);

    }

    public override void Do(MobInfo data)
    {
        vec = data.m_Go.transform.forward;
        vec += data.m_Go.transform.position;
        vec.y += 0.5f;
        GO.transform.position = vec;

        data.m_vTarget = data.m_Go.transform.position + (data.m_Go.transform.position - data.m_Player.transform.position).normalized;
        data.m_vTarget.y = data.m_Go.transform.position.y;
        SteeringBehaviours.NavMove(data);
    }

    public override void CheckCondition(MobInfo data)
    {
        float Dist = (data.m_Player.transform.position - data.m_Go.transform.position).magnitude;

        if (Dist > data.m_fPatrolVisionLength * 1.5f)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_CallArmy);
        }
    }
}

public class FSMDodgeState : FSMState
{
    Animator m_Animator;
    Vector3 vec;
    GameObject GO;
    public FSMDodgeState()
    {
        m_StateID = eFSMStateID.DodgeStateID;
    }

    public override void DoBeforeEnter(MobInfo data)
    {
        data.navMeshAgent.enabled = true;
        data.navMeshAgent.speed *= 2f;
        data.m_AnimationController.SetAnimator(m_StateID, true);
    }

    public override void DoBeforeLeave(MobInfo data)
    {
        data.navMeshAgent.speed *= 0.5f;
        data.m_AnimationController.SetAnimator(m_StateID, false);

    }

    public override void Do(MobInfo data)
    {
        data.m_vTarget = data.m_Go.transform.position + (data.m_Go.transform.position - data.m_Player.transform.position).normalized;
        data.m_vTarget.y = data.m_Go.transform.position.y;
        SteeringBehaviours.NavMove(data);
    }

    public override void CheckCondition(MobInfo data)
    {
        float Dist = (data.m_Player.transform.position - data.m_Go.transform.position).magnitude;

        if (Dist > data.m_fPatrolVisionLength * 1.5f)
        {
            data.m_FSMSystem.PerformTransition(eFSMTransition.Go_Attack);
        }
    }
}

public class FSMDeadState : FSMState
{
    public FSMDeadState()
    {
        m_StateID = eFSMStateID.DeadStateID;
    }


    public override void DoBeforeEnter(MobInfo data)
    {
        data.navMeshAgent.enabled = false;
        data.m_AnimationController.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(MobInfo data)
    {

    }

    public override void Do(MobInfo data)
    {
        data.m_Go.transform.position += data.m_Go.transform.forward * -1 * Time.deltaTime;
    }

    public override void CheckCondition(MobInfo data)
    {
        AnimatorStateInfo info = data.m_AnimationController.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Dead"))
        {
            if (info.normalizedTime > 0.9f)
            {
                MobManager.m_Instance.UnloadMob(data.m_ID, data);
                MobManager.m_Instance.DecreaseMobCount(data.m_ID);
            }
        }
    }
}
