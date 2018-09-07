using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//射擊模式，Load weapon，
public class Weapon_Battle : MonoBehaviour
{
    private int m_WeaponNum = 1;

    //private float m_fCooltime = 1.0f;
    //private float m_fRefillTime = 2.0f;

    WeaponStore m_weaponStore;
    CMain main;
    WeaponActive weaponActive1;
    WeaponActive weaponActive2;
    //==================================================================================================
    // Use this for initialization
    private void Start()
    {
        main = new CMain();
        m_weaponStore = new WeaponStore(new WeaponFactory());        
        weaponActive1 = m_weaponStore.WeaponProduce(main.SecondWeapon, 1);
        weaponActive2 = m_weaponStore.WeaponProduce(eWeaponType.ShotGun, 2);

    }

    private void Update()
    {
        Shoot(m_WeaponNum);
        Refilled(m_WeaponNum);
        SwitchWeapon();
    }

    private void Shoot(int type)
    {
        if (type == 1) { weaponActive1.Shot(1); }
        if (type == 2) { weaponActive2.Shot(2); }
    }
    private void Refilled(int type)
    {
        if (type == 1) { weaponActive1.Refill(1); }
        if (type == 2) { weaponActive2.Refill(2); }
    }

    private void SwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Z)) {
            m_WeaponNum = (m_WeaponNum == 1) ? 2 : 1;
            Debug.Log(m_WeaponNum);
        }
    }







}

    


