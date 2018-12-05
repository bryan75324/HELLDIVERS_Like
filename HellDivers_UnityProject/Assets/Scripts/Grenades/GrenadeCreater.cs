using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCreater : IGrenadesBehaviors {

    protected GrenadeInfo _grenadeInfo;

    public virtual GrenadeInfo grenadeInfo { get { return _grenadeInfo; } }

    public virtual void Init(int grenadeID)
    {
        _grenadeInfo = new GrenadeInfo();
        GameData.Instance.GrenadeInfoTable[grenadeID].CopyTo(_grenadeInfo);
        string m_sGrenade = "Grenade_" + _grenadeInfo.Title;
        string m_sEffect = "Effect_" + _grenadeInfo.Title;
        string m_sSound = "Sound_" + _grenadeInfo.Title;
        string m_sSound_Explosion = "Sound_" + _grenadeInfo.Title + "_Explosion";
        Object grenade;
        Object effect;
        Object sound;
        Object soundExplosion = null;

        if (ResourceManager.m_Instance != null)
        {
            grenade = ResourceManager.m_Instance.LoadData(typeof(GameObject), "GrenadesStorage/Grenades", m_sGrenade, false);
            effect = ResourceManager.m_Instance.LoadData(typeof(GameObject), "GrenadesStorage/Effects", m_sEffect, false);
            sound = ResourceManager.m_Instance.LoadData(typeof(GameObject), "GrenadesStorage/Sounds", m_sSound, false);
            if (grenadeID == 4003 || grenadeID == 4004)
            {
                soundExplosion = ResourceManager.m_Instance.LoadData(typeof(GameObject), "GrenadesStorage/Sounds", m_sSound_Explosion, false);
            }
        }
        else
        {
            grenade = Resources.Load("GrenadesStorage/Grenades/" + m_sGrenade);
            effect = Resources.Load("GrenadesStorage/Effects/" + m_sEffect);
            sound = Resources.Load("GrenadesStorage/Sounds/" + m_sSound);
            if (grenadeID == 4004 || grenadeID == 4005)
            {
                soundExplosion = Resources.Load("GrenadesStorage/Sounds/" + m_sSound_Explosion);
            }

        }
        if (ObjectPool.m_Instance == null) ObjectPool.m_Instance.Init();
        ObjectPool.m_Instance.InitGameObjects(grenade, _grenadeInfo.MaxCount, grenadeID);
        ObjectPool.m_Instance.InitGameObjects(effect, _grenadeInfo.MaxCount, grenadeID * 10 + 1);
        ObjectPool.m_Instance.InitGameObjects(sound, _grenadeInfo.MaxCount, grenadeID * 10 + 2);
        if (soundExplosion != null) ObjectPool.m_Instance.InitGameObjects(soundExplosion, _grenadeInfo.MaxCount, grenadeID * 10 + 3);
    }

    public virtual void Throw(ref GameObject go, Transform t)
    {
        go.transform.position = t.position;
        go.transform.forward = t.forward;
        go.transform.localScale *= 5f;
    }


}
