///2018.09.10
///Ivan.CC
///
/// Weapon's behaviour for player.
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region SetWeapon
    
    /// <summary>
    /// Add multi weapons into player
    /// </summary>
    /// <param name="WeaponsID">weapon's ID</param>
    /// <param name="pos">weapon's position</param>
    public void AddMultiWeapons(List<int> WeaponsID, Transform pos, Player player)
    {
        for (int i = 0; i < WeaponsID.Count; i++)
        {
            AddWeapon(WeaponsID[i], pos, player);
        }
    }
   
    /// <summary>
    /// Add single weapon into player
    /// </summary>
    /// <param name="weaponID">weapon's ID</param>
    /// <param name="pos">weapon's position</param>
    public void AddWeapon(int weaponID, Transform pos, Player player)
    {
        if (GameData.Instance.WeaponInfoTable.ContainsKey(weaponID) == false) { return; }
        if (!m_dWeapon.ContainsKey(weaponID))
        {
            WeaponComponent component = new WeaponComponent();
            component.Behaviour = m_weaponFactory.CreateWeapon(weaponID);
            component.Behaviour.WeaponLoader();
            component.Effect = SetFireEffect(weaponID, pos);
            m_dWeapon.Add(weaponID, component);
            SetSoundID(weaponID);
            _CurrentWeapon = weaponID;
            m_player = player;
            m_tGunPos = pos;
        }
        SetWeaponInfo(weaponID);
    }
    
    /// <summary>
    /// Reset active weapons' ammo and mags to origine
    /// </summary>
    public void ResetWeaponInfo()
    {
        for (int i = 0; i < ActivedWeaponID.Length; i++)
        {
            SetWeaponInfo(ActivedWeaponID[i]);
        }
    }
    
    /// <summary>
    /// Reset the designate weapon to origine
    /// </summary>
    /// <param name="type"></param>
    public void SetWeaponInfo(int type)
    {
        m_dWeapon[type].Behaviour.weaponInfo.Mags = GameData.Instance.WeaponInfoTable[type].Start_Mags;
        m_dWeapon[type].Behaviour.weaponInfo.Ammo = GameData.Instance.WeaponInfoTable[type].Capacity;
    }
    
    /// <summary>
    /// Remove the designate weapon's info from dictionary and objectpool's dictionary
    /// </summary>
    /// <param name="weaponID"></param>
    public void RemoveWeapon(int weaponID)
    {
        if (GameData.Instance.WeaponInfoTable.ContainsKey(weaponID) == false) { return; }
        if (m_dWeapon.ContainsKey(weaponID) == false) { return; }
        m_dWeapon.Remove(weaponID);
        ObjectPool.m_Instance.RemoveObjectFromPool(weaponID);
    }
    
    /// <summary>
    /// Remove all the weapon from dictionary and objectpool
    /// </summary>
    public void ClearWeapon()
    {
        if (ActivedWeaponID.Length == 0) { return; }
        for (int i = 0; i < ActivedWeaponID.Length; i++) { ObjectPool.m_Instance.RemoveObjectFromPool(ActivedWeaponID[i]); }
        Transform t = GameObject.Find("Bullet").transform;
        Debug.Log(t.childCount);
        for (int i = 0; i < t.childCount; i++) { Destroy(t.GetChild(i).gameObject); }
        for (int i = 0; i < ActivedWeaponID.Length; i++)
        {
            Destroy(m_dWeapon[ActivedWeaponID[i]].Effect);
        }
        m_dWeapon.Clear();
    }
    
    /// <summary>
    /// Add the designate weapon's mags
    /// </summary>
    /// <param name="weaponID">weapon's ID</param>
    /// <param name="count">mags count</param>
    /// <returns></returns>
    public bool AddMags(int weaponID, int count)
    {
        if (m_dWeapon.ContainsKey(weaponID) == false) { return false; }
        if (m_dWeapon[weaponID].Behaviour.weaponInfo.Mags >= m_dWeapon[weaponID].Behaviour.weaponInfo.Max_Mags) { return false; }
        m_dWeapon[weaponID].Behaviour.weaponInfo.Mags += count;
        if (OnPickMags != null) OnPickMags();
        return true;
    }
    
    #endregion SetWeapon

    private GameObject SetFireEffect(int i, Transform t)
    {
        GameObject go = ObjectPool.m_Instance.LoadGameObjectFromPool(i * 10 + 1);
        go.GetComponent<EffectController>().SetID(i * 10 + 1);
        go.transform.parent = t;
        go.transform.forward = t.forward;
        go.transform.localPosition = Vector3.zero;
        return go;
    }

    private void SetSoundID(int i)
    {
        List<GameObject> gos = new List<GameObject>();
        for (int j = 0; j < 5; j++)
        {
            GameObject go = ObjectPool.m_Instance.LoadGameObjectFromPool(i * 10 + 2);
            if (go == null) break;
            go.GetComponent<ClipHandler>().SetID(i * 10 + 2);
            gos.Add(go);
        }
        for (int j = 0; j < gos.Count; j++)
        {
            ObjectPool.m_Instance.UnLoadObjectToPool(i * 10 + 2, gos[j]);
        }
    }

    public void LoadSound(int i, Transform t)
    {
        GameObject go = ObjectPool.m_Instance.LoadGameObjectFromPool(i);
        if (go == null) return;
        go.transform.position = t.position;
        go.SetActive(true);
    }


    #region WeaponBehaviours

    /// <summary>
    /// Define if weapon is shooting, and the ammo's left; 
    /// </summary>
    /// <returns></returns>
    public bool ShootState()
    {
        if (CurrentWeaponInfo.Ammo <= 0 )
        {
            LoadSound(999, m_tGunPos);
            return false;
        }
        if (!m_bShooting) { return false; }
        m_bShooting = false;
        StartCoroutine(WaitCooling());
        return true;
    }
    
    //Weapon's cooling after shooting, it's time decide by shooting rate
    private IEnumerator WaitCooling()
    {
        CurEffect.SetActive(true);
        LoadSound(_CurrentWeapon * 10 + 2, m_tGunPos);
        yield return new WaitForSeconds(0.2f);
        CurBehaviors.Shot(m_tGunPos, m_fSpreadIncrease, m_player);
        if (OnFire != null) OnFire();
        yield return new WaitForSeconds(CurrentWeaponInfo.FireRate);
        m_bShooting = true;
        m_fSpreadIncrease += CurrentWeaponInfo.Spread_Increase_per_shot;
        if (_CurrentWeapon == 1601|| _CurrentWeapon == 1701)
            CurEffect.GetComponent<Animator>().SetTrigger("endTrigger");
        yield break;
    }

    /// <summary>
    /// Define weapon is reloading, and the ammo is full or not.
    /// </summary>
    /// <returns></returns>
    public bool ReloadState()
    {
        if (CurrentWeaponInfo.Ammo >= CurrentWeaponInfo.Capacity || CurrentWeaponInfo.Mags <= 0) { return false; }
        else if (m_bReloading) { return false; }
        m_bReloading = true;
        StartCoroutine(WaitReloading());
        return true;
    }
    
    //Weapon reloading speed decided by reloading speed.
    private IEnumerator WaitReloading()
    {
        if (OnReload != null) OnReload();
        LoadSound(998, m_tGunPos);
        yield return new WaitForSeconds(CurrentWeaponInfo.ReloadSpeed);
        CurBehaviors.Reload();
        if (OnReloadEnd != null) OnReloadEnd();
        m_bReloading = false;
        yield break;
    }
    
    /// <summary>
    /// Switch weapon into next.
    /// </summary>
    /// <returns></returns>
    public bool SwitchWeaponState()
    {
        for (int i = 0; i < ActivedWeaponID.Length; i++)
        {
            if (i == ActivedWeaponID.Length - 1)
            {
                _CurrentWeapon = ActivedWeaponID[0];
                if (OnSwitch != null) OnSwitch();
                return true; 
            }
            else if (ActivedWeaponID[i] == _CurrentWeapon)
            {
                _CurrentWeapon = ActivedWeaponID[i + 1];
                if (OnSwitch != null) OnSwitch();
                return true;
            }
        }
        return false;
    }
    
    #endregion WeaponBehaviours

    #region Delegate
    public delegate void EventHolder();
    public event EventHolder OnFire;
    public event EventHolder OnReload;
    public event EventHolder OnReloadEnd;
    public event EventHolder OnSwitch;
    public event EventHolder OnPickMags;
    #endregion

    #region Public Field

    public Dictionary<int, IWeaponBehaviour> ActiveWeapon
    {
        get
        {
            Dictionary<int, IWeaponBehaviour> weapons = new Dictionary<int, IWeaponBehaviour>();
            foreach (var item in m_dWeapon) { weapons.Add(item.Key, item.Value.Behaviour); }
            return weapons;
        }
    }

    public WeaponInfo CurrentWeaponInfo { get { return CurBehaviors.weaponInfo; } }

    public IWeaponBehaviour CurBehaviors { get { return m_dWeapon[_CurrentWeapon].Behaviour; } }

    public GameObject CurEffect { get { return m_dWeapon[_CurrentWeapon].Effect; } }

    public int _CurrentWeapon { get; private set; }

    public int[] ActivedWeaponID
    {
        get
        {
            int[] keys = new int[m_dWeapon.Count];
            m_dWeapon.Keys.CopyTo(keys, 0);
            return keys;
        }
    }

    public float m_fSpreadIncrease;

    public bool m_bAutoFire = true;

    public bool m_bShooting = true;
    
    #endregion

    #region Private member
    private WeaponFactory m_weaponFactory = new WeaponFactory();
    private Player m_player;
    private Transform m_tGunPos;
    private Dictionary<int, WeaponComponent> m_dWeapon = new Dictionary<int, WeaponComponent>();
    private bool m_bReloading;
    #endregion Private member

    class WeaponComponent
    {
        public IWeaponBehaviour Behaviour;
        public GameObject Effect;
    }
}