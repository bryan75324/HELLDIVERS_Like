using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGrenades
{
    NORMAL,
    LIGHTNING,
    MINE,
    TIMEBOMB,
}

public interface  IGrenadesBehaviors
{
    GrenadeInfo grenadeInfo { get; }
    void Init(int grenadeID);
    void Throw(ref GameObject go, Transform t);
}

public class GrenadeFactory
{
    public IGrenadesBehaviors CreateGrenade(int id)
    {
        int iType = GameData.Instance.GrenadeInfoTable[id].Type;
        IGrenadesBehaviors grenadesBehaviors;

        grenadesBehaviors = new GrenadeCreater();

        grenadesBehaviors.Init(id);

        return grenadesBehaviors;
    }
}
