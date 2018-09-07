using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : WeaponSystem {

    public static WeaponController m_Instance;

    //Decide the type of Weapon only from this script
    public int m_iType { private set; get;}

    //Weapons' Level   
    internal int m_iLevel { private set; get; }
    
    //========================================================================================================

    public void Init()
    {
        m_Instance = this;
        m_iLevel = 1;
        m_iType = 0;
        Initalized();
    }

    /// <summary>
    /// Choice weapon
    /// </summary>
    /// <param name="bRight">choice direction</param>
    /// <returns></returns>
    public int ChoiceWeapon(bool bRight)
    {
        m_iType += (bRight) ? 1 : -1;
        if (m_iType >= (int)eWeaponType.LastOne) { m_iType = (int)eWeaponType.FirstOne + 1; }
        if (m_iType <= (int)eWeaponType.FirstOne) { m_iType = (int)eWeaponType.LastOne - 1; }
        Debug.Log("E: iType = " + m_iType);
        return m_iType;
    }
    

    /// <summary>
    /// Weapon's attribute follow by its level
    /// </summary>
    /// <returns></returns>
    public int WeaponLevel()
    {
        m_iLevel++;        
        return m_iLevel;
    }

    /// <summary>
    /// Get weapon's Info in float list
    /// </summary>
    /// <param name="etype">Weapon type</param>
    /// <param name="level">Weapon level</param>
    /// <returns></returns>
    public List<float> WeaponInfo(eWeaponType etype, int level)
    {
        m_iLevel = level;
        List<float> pList = m_dWeaponInfo[etype];
        WeaponAttribute(etype, out m_iAmmo, out m_fDamage, out m_fRefillTime, out m_fCoolTime);        
        pList[0] = m_iAmmo;
        pList[1] = m_fDamage;
        pList[2] = m_fRefillTime;
        pList[3] = m_fCoolTime;        
        return pList;
    }

    //Calculate the weapons' attribute with Level
    private void WeaponAttribute(eWeaponType type, out float ammo, out float damage, out float refilltime, out float cooltime)
    {
        List<float> pList = m_dWeaponInfo[type];
        ammo = pList[0] + pList[0] * m_iAmmoWt * m_fLevelWt * (m_iLevel - 1);
        damage = pList[1] + pList[1] * m_fDamageWt * m_fLevelWt * (m_iLevel - 1);
        refilltime = pList[2] + pList[2] * m_fRefillTimeWt * m_fLevelWt * (m_iLevel - 1);
        cooltime = pList[3] + pList[3] * m_fCoolTimeWt * m_fLevelWt * (m_iLevel - 1);
    }
       
}
