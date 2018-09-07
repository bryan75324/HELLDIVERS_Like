using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStore
{
    WeaponFactory _weaponFactory;

    public WeaponStore(WeaponFactory factory) { _weaponFactory = factory; }

    public WeaponActive WeaponProduce(eWeaponType eType, int OPtype)
    {
        WeaponActive weaponActive;
        weaponActive = _weaponFactory.CreateWeapon(eType);
        weaponActive.Shot(OPtype);
        weaponActive.Refill(OPtype);

        return weaponActive;
    }
}

public class WeaponFactory {
    public WeaponActive CreateWeapon(eWeaponType eType)
    {        
        WeaponActive weaponActive;
        if (eWeaponType.ShotGun == eType) { return weaponActive = new Weapon_ShotGun(); }
        else if (eWeaponType.Rifle == eType) { return weaponActive = new Weapon_Rifle(); }
        else { return null; }
                
        
    }
}



