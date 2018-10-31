﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SteeringBehavior
{
    /*
    static public void Move(AIData data)
    {
        if (data.m_bMove == false)
        {
            return;
        }
        Transform t = data.m_Go.transform;
        Vector3 cPos = data.m_Go.transform.position;
        Vector3 vR = t.right;
        Vector3 vF = t.forward;

        if (data.m_fTempTurnForce > data.m_fMaxRot)
        {
            data.m_fTempTurnForce = data.m_fMaxRot;
        }
        else if (data.m_fTempTurnForce < -data.m_fMaxRot)
        {
            data.m_fTempTurnForce = -data.m_fMaxRot;
        }
        Vector3 vOriF = vF;
        vF = vF + vR * data.m_fTempTurnForce;
        vF.Normalize();
        t.forward = vF;
        if (data.m_bCol == false)
        {
            //Debug.Log("CheckCollision");
            if (SteeringBehavior.CheckCollision(data))
            {
                //Debug.Log("CheckCollision true");
                t.forward = vOriF;
            }
        }
        //if (t.forward == (data.m_vTarget - cPos).normalized)
        //{
        data.m_Speed = data.m_Speed + data.m_fMoveForce * Time.deltaTime;
        if (data.m_Speed < 0.001f)
        {
            data.m_Speed = 0.001f;
        }
        else if (data.m_Speed > data.m_fMaxSpeed * data.m_fMoveForce * 0.5f)
        {
            data.m_Speed = data.m_fMaxSpeed;
        }
        cPos = cPos + t.forward * data.m_Speed;
        t.position = cPos;
        //}

    }
    static public bool CheckCollision(AIData data)
    {
        List<Obstacle> m_AvoidTargets = Main.m_Instance.GetObstacles();
        if (m_AvoidTargets == null)
        {
            return false;
        }
        Transform ct = data.m_Go.transform;
        Vector3 cPos = ct.position;
        Vector3 cForward = ct.forward;
        Vector3 vec;

        float fDist = 0.0f;
        float fDot = 0.0f;
        int iCount = m_AvoidTargets.Count;
        for (int i = 0; i < iCount; i++)
        {
            vec = m_AvoidTargets[i].transform.position - cPos;
            vec.y = 0.0f;
            fDist = vec.magnitude;
            if (fDist > data.m_fProbeLength + m_AvoidTargets[i].m_fRadius)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }

            vec.Normalize();
            fDot = Vector3.Dot(vec, cForward);
            if (fDot < 0)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }
            m_AvoidTargets[i].m_eState = Obstacle.eState.INSIDE_TEST;
            float fProjDist = fDist * fDot;
            float fDotDist = Mathf.Sqrt(fDist * fDist - fProjDist * fProjDist);
            if (fDotDist > m_AvoidTargets[i].m_fRadius + data.m_fRadius)
            {
                continue;
            }

            return true;


        }
        return false;
    }
    static public bool CollisionAvoid(AIData data)
    {
        List<Obstacle> m_AvoidTargets = Main.m_Instance.GetObstacles();
        Transform ct = data.m_Go.transform;
        Vector3 cPos = ct.position;
        Vector3 cForward = ct.forward;
        Vector3 vec;
        float fFinalDotDist;
        float fFinalProjDist;
        Vector3 vFinalVec = Vector3.forward;
        Obstacle oFinal = null;
        float fDist = 0.0f;
        float fDot = 0.0f;
        float fFinalDot = 0.0f;
        int iCount = m_AvoidTargets.Count;

        float fMinDist = 10000.0f;
        for (int i = 0; i < iCount; i++)
        {
            vec = m_AvoidTargets[i].transform.position - cPos;
            vec.y = 0.0f;
            fDist = vec.magnitude;
            if (fDist > data.m_fProbeLength + m_AvoidTargets[i].m_fRadius)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }

            vec.Normalize();
            fDot = Vector3.Dot(vec, cForward);
            if (fDot < 0)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }
            m_AvoidTargets[i].m_eState = Obstacle.eState.INSIDE_TEST;
            float fProjDist = fDist * fDot;
            float fDotDist = Mathf.Sqrt(fDist * fDist - fProjDist * fProjDist);
            if (fDotDist > m_AvoidTargets[i].m_fRadius + data.m_fRadius)
            {
                continue;
            }

            if (fDist < fMinDist)
            {
                fMinDist = fDist;
                fFinalDotDist = fDotDist;
                fFinalProjDist = fProjDist;
                vFinalVec = vec;
                oFinal = m_AvoidTargets[i];
                fFinalDot = fDot;
            }

        }

        if (oFinal != null)
        {
            Vector3 vCross = Vector3.Cross(cForward, vFinalVec);
            float fTurnMag = fFinalDot;
            if (vCross.y > 0.0f)
            {
                fTurnMag = -fTurnMag;
            }
            data.m_fTempTurnForce = fTurnMag;

            float fTotalLen = data.m_fProbeLength + oFinal.m_fRadius;
            float fRatio = fMinDist / fTotalLen;
            if (fRatio > 1.0f)
            {
                fRatio = 1.0f;
            }
            fRatio = 1.0f - fRatio;
            data.m_fMoveForce = -fRatio;
            oFinal.m_eState = Obstacle.eState.COL_TEST;
            data.m_bCol = true;
            data.m_bMove = true;
            return true;
        }
        data.m_bCol = false;
        return false;
    }
    static public bool Seek(AIData data)
    {
        Vector3 cPos = data.m_Go.transform.position;
        Vector3 vec = data.m_vTarget - cPos;
        vec.y = 0.0f;
        float fDist = vec.magnitude;
        if (fDist < data.m_Speed + 0.001f)
        {
            Vector3 vFinal = data.m_vTarget;
            vFinal.y = cPos.y;
            data.m_Go.transform.position = vFinal;
            data.m_fMoveForce = 0.0f;
            data.m_fTempTurnForce = 0.0f;
            data.m_Speed = 0.0f;
            data.m_bMove = false;
            return false;
        }
        Vector3 vf = data.m_Go.transform.forward;
        Vector3 vr = data.m_Go.transform.right;
        vec.Normalize();
        float fDotF = Vector3.Dot(vf, vec);
        if (fDotF > 0.96f)
        {
            fDotF = 1.0f;
            data.m_Go.transform.forward = vec;
            data.m_fTempTurnForce = 0.0f;
            data.m_fRot = 0.0f;
        }
        else
        {
            if (fDotF < -1.0f)
            {
                fDotF = -1.0f;
            }
            float fDotR = Vector3.Dot(vr, vec);

            if (fDotF < 0.0f)
            {

                if (fDotR > 0.0f)
                {
                    fDotR = 1.0f;
                }
                else
                {
                    fDotR = -1.0f;
                }

            }
            if (fDist < 1.0f)
            {
                fDotR = fDist * fDotR;
            }
            data.m_fTempTurnForce = fDotR;

            // data.m_fTempTurnForce *= 0.1f;


        }

        data.m_fMoveForce = fDotF;
        data.m_bMove = true;
        return true;
    }
    */


    static public bool Shake(AIData data)
    {
        Vector3 cPos = data.m_Go.transform.position;
        Vector3 vec = data.m_vTarget - cPos;
        vec.y = 0.0f;
        float fDist = vec.magnitude;
        Vector3 vf = data.m_Go.transform.forward;
        Vector3 vr = data.m_Go.transform.right;
        float fDotF = Vector3.Dot(vf, vec.normalized);
        float fDotR = Vector3.Dot(vr, vec.normalized);

        if (fDotF < -0.96f)
        {
            fDotF = -1.0f;
            data.m_Go.transform.forward = -vec;
            data.m_fTempTurnForce = 0.0f;
            data.m_fRot = 0.0f;
        }
        else
        {
            data.m_fTempTurnForce = -fDotR;
        }
        data.m_fMoveForce = -fDotF;
        data.m_bMove = true;
        return true;
    }

    static public bool Flee(AIData data)
    {
        data.m_vTarget = data.m_Go.transform.position;
        Vector3 cPos = data.m_Go.transform.position;
        Vector3 vec = data.m_PlayerGO.transform.position - cPos;
        vec.y = 0.0f;
        float fDist = vec.magnitude;
        Vector3 vf = data.m_Go.transform.forward;
        Vector3 vr = data.m_Go.transform.right;
        float fDotF = Vector3.Dot(vf, vec.normalized);
        float fDotR = Vector3.Dot(vr, vec.normalized);


        if (fDotF < -0.96f)
        {
            fDotF = -1.0f;
            data.m_Go.transform.forward = -vec;
            data.m_fTempTurnForce = 0.0f;
            data.m_fRot = 0.0f;
        }
        else
        {
            data.m_fTempTurnForce = -fDotR;
        }
        data.m_fMoveForce = -fDotF;
        data.m_bMove = true;
        return true;
    }

    static public bool CreatRandomTarget(AIData data)
    {
        Vector3 nextTarget = data.m_Go.transform.forward;
        nextTarget = Quaternion.AngleAxis(Random.Range(1f, 360f), Vector3.up) * nextTarget;
        nextTarget *= Random.Range(5f, 20f);
        nextTarget += data.m_Go.transform.position;
        if (Vector3.Distance(data.m_vTarget, data.m_Go.transform.position) < 1f || data.m_vTarget == Vector3.zero)
        {
            NavMeshHit nHit;
            if (NavMesh.Raycast(data.m_Go.transform.position, nextTarget, out nHit, NavMesh.AllAreas))
            {
                return false;
            }
            data.m_vTarget = nextTarget;
        }

        return true;
    }

}
