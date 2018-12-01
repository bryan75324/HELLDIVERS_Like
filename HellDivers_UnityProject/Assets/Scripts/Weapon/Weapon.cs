///2018.09.10
///Ivan.CC
///
/// Weapon's data.
///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo
{
    #region Properties
    public int ID { private set; get; }
    public int Type { private set; get; }
    public string Title { private set; get; }
    public float Damage { private set; get; }
    public float Explosion_Damage { private set; get; }
    public float FireRate { get { return 1 / (_FireRate * 0.017f); } }
    public float FirePerMinute { get { return _FireRate; } }
    public int Capacity { private set; get; }
    public int Start_Mags { private set; get; }
    public int Max_Mags { private set; get; }
    public float Empty_Reload_Speed { private set; get; }
    public float Tactical_Reload_Speed { private set; get; }
    public float Min_Spread { private set; get; }
    public float Max_Spread { private set; get; }
    public float Spread_Increase_per_shot { private set; get; }
    public float Range { private set; get; }
    public float FireMode { private set; get; }
    public string Name { private set; get; }
    public string Image { private set; get; }
    public int Cost { private set; get; }
    #endregion Properties

    #region Set Properties
    public void SetID(int id) { ID = id; }
    public void SetType(int type) { Type = type; }
    public void SetTitle(string title) { Title = title; }
    public void SetDamage(float damage) { Damage = damage; }
    public void SetExplosion(float explosion) { Explosion_Damage = explosion; }
    public void SetFireRate(float fireRate) { _FireRate = fireRate; }
    public void SetCapacity(int capacity) { Capacity = capacity; }
    public void SetStart_Mags(int sMags) { Start_Mags = sMags; }
    public void SetMax_Mags(int mMags) { Max_Mags = mMags; }
    public void SetEmpty_Reload_Speed(float erSpeed) { Empty_Reload_Speed = erSpeed; }
    public void SetTactical_Reload_Speed(float trSpeed) { Tactical_Reload_Speed = trSpeed; }
    public void SetMin_Spread(float minSpread) { Min_Spread = minSpread; }
    public void SetMax_Spread(float maxSpread) { Max_Spread = maxSpread; }
    public void SetSpread_Increase_per_shot(float SpreadIPS) { Spread_Increase_per_shot = SpreadIPS; }
    public void SetRange(float range) { Range = range; }
    public void SetFireMode(float mode) { FireMode = mode; }
    public void SetName(string name) { Name = name; }
    public void SetImage (string image) { Image = image; }
    public void SetCost (int cost) { Cost = cost; }
    #endregion Set Properties

    #region Properties

    public int Ammo
    {
        get { return m_Ammo; }
        set
        {
            if (value > Capacity) m_Ammo = Capacity;
            else if (value < 0) m_Ammo = 0;
            else m_Ammo = value;
        }
    }

    public int Mags
    {
        get { return m_Mags; }
        set
        {
            if (value > Max_Mags) m_Mags = Max_Mags;
            else if (value < 0) m_Mags = 0;
            else m_Mags = value;
        }
    }

    public float ReloadSpeed
    {
        get
        {
            float reloadTime = (m_Ammo <= 0) ? Empty_Reload_Speed : Tactical_Reload_Speed;
            return reloadTime;
        }
    }

    #endregion Properties

    #region Private field

    private float _FireRate;
    private int m_Ammo;
    private int m_Mags;

    #endregion Private field

    public void CopyTo(WeaponInfo other)
    {
        other.SetID(this.ID);
        other.SetType(this.Type);
        other.SetTitle(this.Title);
        other.SetDamage(this.Damage);
        other.SetExplosion(this.Explosion_Damage);
        other.SetFireRate(this.FirePerMinute);
        other.SetCapacity(this.Capacity);
        other.SetStart_Mags(this.Start_Mags);
        other.SetMax_Mags(this.Max_Mags);
        other.SetEmpty_Reload_Speed(this.ReloadSpeed);
        other.SetTactical_Reload_Speed(this.Tactical_Reload_Speed);
        other.SetMin_Spread(this.Min_Spread);
        other.SetMax_Spread(this.Max_Spread);
        other.SetSpread_Increase_per_shot(this.Spread_Increase_per_shot);
        other.SetRange(this.Range);
        other.SetFireMode(this.FireMode);
        other.SetName(this.Name);
        other.SetImage(this.Image);
        other.SetCost(this.Cost);
        other.Ammo = this.Ammo;
        other.Mags = this.Mags;
        
    }
}

[System.Serializable]
public class Weapon : IWeaponBehaviour
{
    protected WeaponInfo _weaponInfo;
    protected virtual int activeAmmo { get { return (int)((weaponInfo.Range * 0.01f) / weaponInfo.FireRate) + 1; } }

    #region Behaviours

    public virtual WeaponInfo weaponInfo { get { return _weaponInfo; } }

    public virtual void Init(int weaponID)
    {
        _weaponInfo = new WeaponInfo();
        GameData.Instance.WeaponInfoTable[(int)weaponID].CopyTo(_weaponInfo);
    }

    public virtual void WeaponLoader()
    {
        string m_sWeapon = "Bullet_" + weaponInfo.Title;
        string m_sEffect = "Effect_" + weaponInfo.Title;
        string m_sSound = "Sound_" + weaponInfo.Title;
        string m_sForderEffect = "WeaponStorage/Effect";
        string m_sForderBullet = "WeaponStorage/Bullet";
        string m_sForderSound = "WeaponStorage/Sound";

        Object effect_HitMob;
        Object effect_HitObs;
        Object effect_HitThrough;
        Object sound_HitObs;
        Object sound_Empty;
        Object sound_Reload;
        Object sound;
        Object weapon;
        Object effect;


        if (ResourceManager.m_Instance != null)
        {
            effect_HitMob = ResourceManager.m_Instance.LoadData(typeof(GameObject), m_sForderEffect, "HitMob", false);
            effect_HitObs = ResourceManager.m_Instance.LoadData(typeof(GameObject), m_sForderEffect, "HitObs", false);
            effect_HitThrough = ResourceManager.m_Instance.LoadData(typeof(GameObject), m_sForderEffect, "HitThrough", false);
            sound_HitObs = ResourceManager.m_Instance.LoadData(typeof(GameObject), m_sForderSound, "Sound_HitObs", false);
            sound_Empty = ResourceManager.m_Instance.LoadData(typeof(GameObject), m_sForderSound, "Sound_Empty", false);
            sound_Reload = ResourceManager.m_Instance.LoadData(typeof(GameObject), m_sForderSound, "Sound_Reload", false);
            weapon = ResourceManager.m_Instance.LoadData(typeof(GameObject), m_sForderBullet, m_sWeapon, false);
            effect = ResourceManager.m_Instance.LoadData(typeof(GameObject), m_sForderEffect, m_sEffect, false);
            sound = ResourceManager.m_Instance.LoadData(typeof(GameObject), m_sForderSound, m_sSound, false);
        }
        else
        {
            Debug.LogWarning("No ResourceManager.");
            effect_HitMob = Resources.Load(m_sForderEffect + "/HitMob");
            effect_HitObs = Resources.Load(m_sForderEffect + "/HitObs");
            effect_HitThrough = Resources.Load(m_sForderEffect + "/HitThrough");
            sound_HitObs = Resources.Load(m_sForderSound + "/" + "Sound_HitObs");
            sound_Empty = Resources.Load(m_sForderSound + "/" + "Sound_Empty");
            sound_Reload = Resources.Load(m_sForderSound + "/" + "Sound_Reload");
            weapon = Resources.Load(m_sForderBullet + "/" + m_sWeapon);
            effect = Resources.Load(m_sForderEffect + "/" + m_sEffect);
            sound = Resources.Load(m_sForderSound + "/" + m_sSound);
        }

        if (ObjectPool.m_Instance == null) ObjectPool.m_Instance.Init();
        ObjectPool.m_Instance.InitGameObjects(effect_HitMob, activeAmmo * 2, 10);
        ObjectPool.m_Instance.InitGameObjects(effect_HitObs, activeAmmo * 2, 20);
        ObjectPool.m_Instance.InitGameObjects(effect_HitThrough, activeAmmo * 2, 30);
        ObjectPool.m_Instance.InitGameObjects(weapon, activeAmmo, _weaponInfo.ID);
        ObjectPool.m_Instance.InitGameObjects(effect, 3, _weaponInfo.ID * 10 + 1);
        ObjectPool.m_Instance.InitGameObjects(sound, 3, _weaponInfo.ID * 10 + 2);
        ObjectPool.m_Instance.InitGameObjects(sound_HitObs, 1, 997);
        ObjectPool.m_Instance.InitGameObjects(sound_Reload, 1, 998);
        ObjectPool.m_Instance.InitGameObjects(sound_Empty, 1, 999);
    }

    public virtual void Shot(Transform t, float spread, Player player)
    {
    }

    public virtual void Reload()
    {
    }

    #endregion Behaviours
}