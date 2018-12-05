using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayerFSMTrans
{
    NullTransition,
    Go_Start,
    Go_Gun,
    Go_MeleeAttack,
    Go_Reload,
    Go_Stratagem,
    Go_Throw,
    Go_ThrowBomb,
    Go_SwitchWeapon,
    Go_PickUp,
    Go_Victory,
    Go_Dead,
    Go_Relive,
    Go_Roll,
}

public enum ePlayerFSMStateID
{
    NullStateID,
    StartStateID,
    GunStateID,
    MeleeAttackID,
    ReloadStateID,
    StratagemStateID,
    ThrowStateID,
    ThrowBombStateID,
    SwitchWeaponID,
    PickUpID,
    VictoryID,
    DeadStateID,
    ReliveStateID,
    RollStateID,
}

public class PlayerFSMState
{
    public ePlayerFSMStateID m_StateID;
    public Dictionary<ePlayerFSMTrans, PlayerFSMState> m_Map;
    public float m_fCurrentTime;

    public PlayerFSMState()
    {
        m_StateID = ePlayerFSMStateID.NullStateID;
        m_fCurrentTime = 0.0f;
        m_Map = new Dictionary<ePlayerFSMTrans, PlayerFSMState>();
    }

    public void AddTransition(ePlayerFSMTrans trans, PlayerFSMState toState)
    {
        if (m_Map.ContainsKey(trans))
        {
            return;
        }

        m_Map.Add(trans, toState);
    }

    public void DelTransition(ePlayerFSMTrans trans)
    {
        if (m_Map.ContainsKey(trans))
        {
            m_Map.Remove(trans);
        }
    }

    public PlayerFSMState TransitionTo(ePlayerFSMTrans trans)
    {
        if (m_Map.ContainsKey(trans) == false)
        {
            return null;
        }
        return m_Map[trans];
    }

    public virtual void DoBeforeEnter(PlayerController data)
    {
    }

    public virtual void DoBeforeLeave(PlayerController data)
    {
    }

    public virtual void Do(PlayerController data)
    {
    }

    public virtual void CheckCondition(PlayerController data)
    {
    }
}

public class PlayerFSMStartState : PlayerFSMState
{
    float fTimer = 0.0f;
    public PlayerFSMStartState()
    {
        m_StateID = ePlayerFSMStateID.StartStateID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        fTimer = 0.0f;
    }

    public override void DoBeforeLeave(PlayerController data)
    {
    }

    public override void Do(PlayerController data)
    {
        data.m_MoveMode = "Stop";
        fTimer += Time.deltaTime;
    }

    public override void CheckCondition(PlayerController data)
    {
        if(fTimer > 3.0f) data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
        //AnimatorStateInfo info = data.m_Animator.GetCurrentAnimatorStateInfo(0);
        //if (info.IsName("Standing"))
        //{
        //    if (info.normalizedTime > 0.9f)
        //    {
        //        data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
        //    }
        //}
    }
}

public class PlayerFSMGunState : PlayerFSMState
{
    private bool shoot;
    private int count = 0;

    public PlayerFSMGunState()
    {
        m_StateID = ePlayerFSMStateID.GunStateID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        count = 0;
    }

    public override void DoBeforeLeave(PlayerController data)
    {
        data.m_PAC.SetAnimator(m_StateID, false);
    }

    public override void Do(PlayerController data)
    {
        if (GameData.Instance.WeaponInfoTable[data.m_WeaponController._CurrentWeapon].FireMode == 0)
        {
            if ((Input.GetButton("Fire1") || Input.GetAxis(data.InputSetting.Fire) < 0) && count < 1)
            {
                if (data.m_WeaponController.ShootState())
                {
                    data.m_PAC.SetAnimator(m_StateID);
                    shoot = true;
                    count++;
                }
                else shoot = false;
            }
            else
            {
                shoot = false;
                data.m_WeaponController.m_fSpreadIncrease = 0;
            }
        }
        else
        {
            if ((Input.GetButton("Fire1") || Input.GetAxis(data.InputSetting.Fire) < 0)/* && data.m_WeaponController.CurrentWeaponInfo.Ammo > 0*/)
            {
                if (data.m_WeaponController.ShootState())
                {
                    data.m_PAC.SetAnimator(m_StateID);
                    shoot = true;
                }
            }
            else
            {
                data.m_WeaponController.m_fSpreadIncrease = 0;
                shoot = false;
            }
        }
        if ((!Input.GetButton("Fire1")) && Input.GetAxis(data.InputSetting.Fire) == 0) count = 0;
    }

    public override void CheckCondition(PlayerController data)
    {
        if (data.m_PlayerFSM.CurrentGlobalStateID == ePlayerFSMStateID.RollStateID) return;

        if (Input.GetButton("Reload") || Input.GetKey(data.InputSetting.Reload))
        {
            if (data.m_WeaponController.ReloadState())
            {
                data.m_PAC.SetAnimator(m_StateID, false);
                data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Reload);
            }
        }
        else if (Input.GetButton("Grenade") || Input.GetAxis(data.InputSetting.Grenade) > 0)
        {
            data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_ThrowBomb);
        }
        else if ((Input.GetButton("Stratagem") || Input.GetKeyDown(data.InputSetting.Stratagem)) && shoot == false)
        {
            if (data.m_StratagemController.Stratagems.Count > 0)
            {
                data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Stratagem);
            }
        }
        else if (Input.GetButtonDown("WeaponSwitch") || Input.GetKeyDown(data.InputSetting.WeaponSwitch))
        {
            if (data.m_WeaponController.SwitchWeaponState())
            {
                data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_SwitchWeapon);
            }
        }
        else if (Input.GetButtonDown("Interactive") || Input.GetKeyDown(data.InputSetting.Interactive))
        {
            data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_PickUp);
        }
        else if (Input.GetButtonDown("MeleeAttack") || Input.GetKeyDown(data.InputSetting.MeleeAttack))
        {
            data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_MeleeAttack);
        }
        else if(Input.GetAxis(data.InputSetting.StratagemVertical) != 0 || Input.GetAxis(data.InputSetting.StratagemHorizontal) != 0)
        {
            data.m_GrenadesController.SwitchGrenades();
        }
    }
}

public class PlayerFSMMeleeAttackState : PlayerFSMState
{
    public PlayerFSMMeleeAttackState()
    {
        m_StateID = ePlayerFSMStateID.MeleeAttackID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        data.m_PAC.SetAnimator(m_StateID);
        data.m_MoveMode = "Stop";
    }

    public override void DoBeforeLeave(PlayerController data)
    {
    }

    public override void Do(PlayerController data)
    {
    }

    public override void CheckCondition(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("MeleeAttack"))
        {
            if (info.normalizedTime > 0.8f)
            {
                data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
            }
        }
    }
}

public class PlayerFSMReloadState : PlayerFSMState
{
    //bool bCheck;
    public PlayerFSMReloadState()
    {
        m_StateID = ePlayerFSMStateID.ReloadStateID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(1);
        if (!info.IsName("Reload") || !data.m_PAC.Animator.IsInTransition(1))
        {
            float speed = data.m_WeaponController.CurrentWeaponInfo.ReloadSpeed;
            speed = (3.33f / speed);
            data.m_PAC.SetAnimator(m_StateID, speed);
        }
    }

    public override void DoBeforeLeave(PlayerController data)
    {
    }

    public override void Do(PlayerController data)
    {

    }

    public override void CheckCondition(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(1);
        if (info.IsName("Reload") || data.m_PAC.Animator.IsInTransition(1))
        {
            //bCheck = true;
            if (info.normalizedTime > 0.9f)
            data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
        }
    }
}

public class PlayerFSMStratagemState : PlayerFSMState
{
    public PlayerFSMStratagemState()
    {
        m_StateID = ePlayerFSMStateID.StratagemStateID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        data.m_PAC.SetAnimator(m_StateID, true);
        data.m_StratagemController.StartCheckCodes();
    }

    public override void DoBeforeLeave(PlayerController data)
    {
        data.m_PAC.SetAnimator(m_StateID, false);
    }

    public override void Do(PlayerController data)
    {
        data.m_MoveMode = "Stop";
    }

    public override void CheckCondition(PlayerController data)
    {
        if (data.m_StratagemController.IsReady)
        {
            data.m_PAC.SetAnimator(m_StateID);
            data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Throw);
        }
        else if (!Input.GetButton("Stratagem") && !Input.GetKey(data.InputSetting.Stratagem))
        {
            data.m_StratagemController.StopCheckCodes();
            data.m_PAC.SetAnimator(m_StateID, false);
            data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
        }
    }
}

public class PlayerFSMThrowState : PlayerFSMState
{
    bool bThrow;
    float count = 0;
    public PlayerFSMThrowState()
    {
        m_StateID = ePlayerFSMStateID.ThrowStateID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        count = 0;
        bThrow = false;
        data.m_MoveMode = "Throw";
    }

    public override void DoBeforeLeave(PlayerController data)
    {
        data.m_PAC.SetAnimator(m_StateID, false);
    }

    public override void Do(PlayerController data)
    {
        if ((Input.GetButtonDown("Fire1") || Input.GetAxis(data.InputSetting.Fire) < 0 )&& count < 1)
        {
            bThrow = true;
            //Start Timer...
            data.m_StratagemController.PrepareThrow();
            count++;
        }

        if ((!Input.GetButton("Fire1") && Input.GetAxis(data.InputSetting.Fire) == 0) && bThrow)
        {
            data.m_MoveMode = "Throwing";
            data.m_PAC.SetAnimator(m_StateID, true);
        }

        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(1);
        if (info.IsName("ThrowOut"))
        {
            if (info.normalizedTime > 0.5f)
            {
                data.m_StratagemController.Throw();
            }
        }
    }

    public override void CheckCondition(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(1);
        if (info.IsName("ThrowOut"))
        {
            if (info.normalizedTime > 0.9f)
            {
                data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
            }
        }
    }
}

public class PlayerFSMThrowBombState : PlayerFSMState
{
    int count = 0;
    int throwCount = 0;
    bool bHolding = false;
    bool bThrow = true;
    public PlayerFSMThrowBombState()
    {
        m_StateID = ePlayerFSMStateID.ThrowBombStateID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        count = 0;
        throwCount = 0;
        bHolding = false;
        bThrow = true;
        data.m_MoveMode = "Throw";
        data.m_PAC.SetAnimator(m_StateID, false);
    }

    public override void DoBeforeLeave(PlayerController data)
    {
        
    }

    public override void Do(PlayerController data)
    {
        if (bThrow)
        {
            bHolding = data.m_GrenadesController.Holding();
            if (bHolding == false)
            {
                data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
                return;
            }
            else if(count < 1)
            {
                data.m_PAC.SetAnimator(m_StateID);
                count++;
            }
        }
        if (!Input.GetButton("Grenade") && Input.GetAxis(data.InputSetting.Grenade) == 0)
        {
            data.m_MoveMode = "Throwing";
            data.m_PAC.SetAnimator(m_StateID, true);
            return;
        }
    }

    public override void CheckCondition(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(1);
        if (info.IsName("ThrowOut"))
        {
            bThrow = false;
            if (info.normalizedTime > 0.4f && throwCount < 1)
            {
                throwCount++;
                data.m_GrenadesController.Throw();
            }
        }
        if (!info.IsName("ThrowOut") && bThrow == false)
        {
            data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
        }
    }
}

public class PlayerFSMSwitchWeaponState : PlayerFSMState
{
    public PlayerFSMSwitchWeaponState()
    {
        m_StateID = ePlayerFSMStateID.SwitchWeaponID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        data.m_MoveMode = "MoveOnly";
        data.m_PAC.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(PlayerController data)
    {
    }

    public override void Do(PlayerController data)
    {
    }

    public override void CheckCondition(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(1);
        if (info.IsName("SwitchWeapon"))
        {
            if (info.normalizedTime > 0.9f)
            {
                data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
            }
        }
    }
}

public class PlayerFSMPickUpState : PlayerFSMState
{
    private float Count = 0;

    public PlayerFSMPickUpState()
    {
        m_StateID = ePlayerFSMStateID.PickUpID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        Count = 0;
        data.m_PAC.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(PlayerController data)
    {
    }

    public override void Do(PlayerController data)
    {
        data.m_MoveMode = "Stop";
    }

    public override void CheckCondition(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("PickUp"))
        {
            if (info.normalizedTime > 0.6f && Count < 1)
            {
                data.m_Player.InteractWithItem();
                Count++;
            }
            if (info.normalizedTime > 0.9f)
            {
                data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
            }
        }
    }
}

public class PlayerFSMVictoryState : PlayerFSMState
{
    public PlayerFSMVictoryState()
    {
        m_StateID = ePlayerFSMStateID.VictoryID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        data.m_PAC.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(PlayerController data)
    {
    }

    public override void Do(PlayerController data)
    {
    }

    public override void CheckCondition(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(1);
        if (info.IsName("Victory"))
        {
            if (info.normalizedTime > 0.8f)
            {
                data.m_PlayerFSM.PerformPreviousTransition();
            }
        }
    }
}

public class PlayerFSMDeadState : PlayerFSMState
{
    public PlayerFSMDeadState()
    {
        m_StateID = ePlayerFSMStateID.DeadStateID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        data.m_MoveMode = "Dead";
        data.m_PAC.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(PlayerController data)
    {
    }

    public override void Do(PlayerController data)
    {
        AnimatorStateInfo info = data.m_Animator.GetCurrentAnimatorStateInfo(3);
        if (info.IsName("Death"))
        {
            if (info.normalizedTime < 0.95f) data.bIsDead = false;
            else
            {
                data.bIsDead = true;
                data.bIsAlive = false;
            }
            return;
        }
    }

    public override void CheckCondition(PlayerController data)
    {
    }
}

public class PlayerFSMReliveState : PlayerFSMState
{
    public PlayerFSMReliveState()
    {
        m_StateID = ePlayerFSMStateID.ReliveStateID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        data.m_MoveMode = "Stop";
        data.m_PAC.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(PlayerController data)
    {
        data.m_MoveMode = "Origin";
        data.m_PAC.ResetAnimator(data);
    }

    public override void Do(PlayerController data)
    {
        AnimatorStateInfo info = data.m_Animator.GetCurrentAnimatorStateInfo(3);
        if (info.IsName("Relive"))
        {
            if (info.normalizedTime < 0.9f) data.bIsAlive = false;
            else
            {
                data.bIsAlive = true;
                data.bIsDead = false;
            }
            return;
        }
    }

    public override void CheckCondition(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(3);
        if (info.IsName("Relive"))
        {
            if (info.normalizedTime > 0.95f)
            {
                data.m_PlayerFSM.PerformTransition(ePlayerFSMTrans.Go_Gun);
                data.m_PlayerFSM.PerformPreviousTransition();
            }
        }
    }
}

public class PlayerFSMRollState : PlayerFSMState
{
    public PlayerFSMRollState()
    {
        m_StateID = ePlayerFSMStateID.RollStateID;
    }

    public override void DoBeforeEnter(PlayerController data)
    {
        data.m_MoveMode = "Stop";
        data.m_PAC.SetAnimator(m_StateID);
    }

    public override void DoBeforeLeave(PlayerController data)
    {
        data.m_MoveMode = "Origin";
    }

    public override void Do(PlayerController data)
    {
    }

    public override void CheckCondition(PlayerController data)
    {
        AnimatorStateInfo info = data.m_PAC.Animator.GetCurrentAnimatorStateInfo(3);
        if (info.IsName("Roll"))
        {
            if (info.normalizedTime >= 1.0f)
            {
                data.m_Collider.enabled = true;
                data.m_PlayerFSM.PerformPreviousTransition();
            }
        }
    }
}