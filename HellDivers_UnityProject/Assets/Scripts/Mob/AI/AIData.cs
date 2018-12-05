using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MobInfo
{
    public int m_ID;
    public float m_fHp;
    public float m_fProbeLength;
    public float m_fPatrolVisionLength;
    public float m_fAttackRange;
    public float m_fAttackDamage;
    public float m_fRadius;
    public float m_Speed;
    public float m_fMaxSpeed;
    public float m_fMaxRot;
    public float m_fAttackTime;
    public float m_fSight;
    public float m_Money;
    public float m_Exp;
    public bool m_bIsPlayerDead;

    public float m_fRot;
    public float m_fTempTurnForce;
    public float m_fMoveForce;
    public bool m_bCol;
    public bool m_bMove;
    public NavMeshAgent navMeshAgent;
    public GameObject m_Go;
    public GameObject m_PlayerGO;
    public GameObject m_TargetObject;
    public GameObject m_RadarPoint;
    public GameObject m_GOEffectWarning;
    public Vector3 m_vTarget;
    public FSMSystem m_FSMSystem;
    public MobAnimationsController m_AnimationController;
    public MobAimLine m_MobAimLine;
    public SoundManager m_SoundManager;
    public Player m_Player;


    public void SetID(int id) { m_ID = id; }
    public void SetHP(float hp) { m_fHp = hp; }
    public void SetProbeLength(float probeLength) { m_fProbeLength = probeLength; }
    public void SetSight(float sight) { m_fSight = sight; }
    public void SetRadius(float radius) { m_fRadius = radius; }
    public void SetVisionLength(float visionLength) { m_fPatrolVisionLength = visionLength; }
    public void SetAttackRange(float attakRange) { m_fAttackRange = attakRange; }
    public void SetAttackDamage(float attackDamage) { m_fAttackDamage= attackDamage; }
    public void SetMoney(float money) { m_Money = money; }
    public void SetExp(float exp) { m_Exp = exp; }
    
    public void CopyTo(MobInfo other)
    {
        other.m_ID = this.m_ID;
        other.m_fHp = this.m_fHp;
        other.m_fProbeLength = this.m_fProbeLength;
        other.m_fSight = this.m_fSight;
        other.m_fRadius = this.m_fRadius;
        other.m_fPatrolVisionLength = this.m_fPatrolVisionLength;
        other.m_fAttackRange = this.m_fAttackRange;
        other.m_fAttackDamage = this.m_fAttackDamage;
        other.m_Money = this.m_Money;
        other.m_Exp = this.m_Exp;
    }

    public class AIFunction
    {
        public static GameObject CheckEnemyInSight(MobInfo data, ref bool bAttack)
        {
            GameObject go = data.m_Player.gameObject;
            Vector3 v = go.transform.position - data.m_Go.transform.position;
            float fDist = v.magnitude;
            if (fDist < data.m_fAttackRange)
            {
                bAttack = true;
                return go;
            }
            else if (fDist < data.m_fSight)
            {
                bAttack = false;
                return go;
            }
            return null;
        }

        public static bool CheckTargetEnemyInSight(MobInfo data, GameObject target, ref bool bAttack)
        {
            GameObject go = target;
            Vector3 v = go.transform.position - data.m_Go.transform.position;
            float fDist = v.magnitude;
            if (fDist < data.m_fAttackRange)
            {
                bAttack = true;
                return true;
            }
            else if (fDist < data.m_fSight)
            {
                bAttack = false;
                return true;
            }
            return false;
        }

        public static void SearchPlayer(MobInfo data)
        {
            data.m_Player = null;
            List<Player> pList = InGamePlayerManager.Instance.Players;
            if (pList != null && pList.Count > 0)
            {
                float minDist = float.MaxValue;
                for (int i = 0; i < pList.Count; i++)
                {
                    float sqrDist = (pList[i].transform.position - data.m_Go.transform.position).sqrMagnitude;
                    if (pList[i].IsDead) continue;

                    if (sqrDist < minDist)
                    {
                        minDist = sqrDist;
                        data.m_Player = pList[i];
                    }
                }
            }
            return;
        }
        public static bool CheckAllPlayersLife()
        {
            int count = 0;
            List<Player> pList = InGamePlayerManager.Instance.Players;
            if (pList != null && pList.Count > 0)
            {
                for (int i = 0; i < pList.Count; i++)
                {
                    if (pList[i].IsDead) continue;

                    count++;
                }
            }
            if (count == 0) return false;
            else return true;
        }
    }
}
