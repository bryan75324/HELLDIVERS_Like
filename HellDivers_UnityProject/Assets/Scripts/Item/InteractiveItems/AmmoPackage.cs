using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundManager))]
public class AmmoPackage : InteractiveItem
{
    private SoundManager m_SoundManager;

    private void Awake()
    {
        m_SoundManager = this.GetComponent<SoundManager>();
        SoundDataSetting soundData = ResourceManager.m_Instance.LoadData(typeof(SoundDataSetting), "Sounds/Item", "Item_SoundDataSetting") as SoundDataSetting;
        m_SoundManager.SetAudioClips(soundData.SoundDatas);
    }

    public override void OnInteract(Player player)
    {
        int[] ids = player.WeaponController.ActivedWeaponID;
        bool bFillUp = false;
        for (int i = 0; i < ids.Length; i++)
        {
            int currentWeaponId = player.WeaponController.ActivedWeaponID[i];
            int num = player.WeaponController.ActiveWeapon[currentWeaponId].weaponInfo.Start_Mags;
            bool bCurrentFillUp = player.WeaponController.AddMags(currentWeaponId, num);
            if (bCurrentFillUp) bFillUp = true;
        }
        if (bFillUp == false) return;

        m_SoundManager.PlayInWorld(0, this.transform.position);

        if (ObjectPool.m_Instance != null)
        {
            int type = int.Parse(ID);
            ObjectPool.m_Instance.UnLoadObjectToPool(type, this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}