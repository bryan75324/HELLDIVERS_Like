using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundManager))]
public class HealPackage : InteractiveItem
{
    public float Heal { get { return m_Heal; } }
    [SerializeField] private float m_Heal;
    private SoundManager m_SoundManager;

    private void Awake()
    {
        m_SoundManager = this.GetComponent<SoundManager>();
        SoundDataSetting soundData = ResourceManager.m_Instance.LoadData(typeof(SoundDataSetting), "Sounds/Item", "Item_SoundDataSetting") as SoundDataSetting;
        m_SoundManager.SetAudioClips(soundData.SoundDatas);
    }

    public override void OnInteract(Player player)
    {
        bool bHeal = player.TakeHealth(Heal);
        if (bHeal == false) return;

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