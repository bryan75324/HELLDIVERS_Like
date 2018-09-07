using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLoader {
        

    public Dictionary<int, List<float>> m_WeaponDatas = new Dictionary<int, List<float>>();

    // Use this for initialization
    public WeaponLoader (eWeaponType type1, int level1 , eWeaponType type2, int level2) {
        WeaponController WC = new WeaponController();
        WC.Init();
        AssetManager AssetManager = new AssetManager();
        AssetManager.Init();
        ResourceManager rm = new ResourceManager();
        rm.Init();
        Weapon_Battle weapon = new Weapon_Battle();

        m_WeaponDatas.Add(1, WC.WeaponInfo(type1, level1));
        m_WeaponDatas.Add(2, WC.WeaponInfo(type2, level2));        

        string m_sFirstWeapon = "Bullet_" + type1.ToString();
        string m_sSecondWeapon = "Bullet_" + type2.ToString();

        Object m_FirstWeapon = rm.LoadData(typeof(GameObject), "Prefabs", m_sFirstWeapon, false);
        Object m_SecondWeapon = rm.LoadData(typeof(GameObject), "Prefabs", m_sSecondWeapon, false);

        if (ObjectPool.m_Instance != null)
        {
            ObjectPool.m_Instance.InitGameObjects(m_FirstWeapon, (int)m_WeaponDatas[1][0], 1);
            ObjectPool.m_Instance.InitGameObjects(m_SecondWeapon, (int)m_WeaponDatas[2][0], 2);
        }
        else
        {
            ObjectPool OP = new ObjectPool();
            OP.InitGameObjects(m_FirstWeapon, (int)m_WeaponDatas[1][0], 1);
            OP.InitGameObjects(m_SecondWeapon, (int)m_WeaponDatas[2][0], 2);
        }
    }



}
