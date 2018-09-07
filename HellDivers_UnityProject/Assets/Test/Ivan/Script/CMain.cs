using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMain : MonoBehaviour {

    public eWeaponType FirstWeapon;
    public eWeaponType SecondWeapon;
    [SerializeField] int FirstWeaponLevel;
    [SerializeField] int SecondWeaponLevel;

    WeaponLoader m_WeaponLoader;

    private void Start()
    {
        m_WeaponLoader = new WeaponLoader(FirstWeapon, FirstWeaponLevel, SecondWeapon, SecondWeaponLevel);
    }
    
}
