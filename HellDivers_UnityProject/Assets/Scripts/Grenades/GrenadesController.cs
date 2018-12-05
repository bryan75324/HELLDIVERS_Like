using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadesController : MonoBehaviour
{
    public enum eSwitch { UP, LEFT, DOWN, RIGHT }

    #region Public Method

    #region Set Greades
    /// <summary>
    /// Equip grenades and set grenade to current active grenade, refresh grenade counter
    /// </summary>
    /// <param name="ids">Grenades' ID</param>
    /// <param name="t">Strat Position</param>
    public void AddGrenades(List<int> ids, Transform startPos, Transform throwPos, Player player)
    {
        foreach (int id in ids)
        {
            GrenadeComponent component = new GrenadeComponent();
            component.m_Behavior = m_GrenadeFactory.CreateGrenade(id);
            component.Count = component.m_Behavior.grenadeInfo.MaxCount;
            m_dGrenades.Add(id, component);
        }
        CurrentID = GrenadeKeys[0];
        m_StarPos = startPos;
        m_ThrowPos = throwPos;
        m_Player = player;
        }

    /// <summary>
    /// Add the count of grenades with id
    /// </summary>
    /// <param name="id">Grenades' ID</param>
    /// <param name="count">Add count</param>
    public void AddGrenadesCount(int id, int count)
    {
        m_dGrenades[id].AddCount(id);
        if (id == CurrentID)
        {
            if (OnCount != null) OnCount();
        }
    }

    /// <summary>
    ///  Input StratagemVertical or StratagemHorizontal to switch type of grenades
    /// </summary>
    public void SwitchGrenades()
    {
        eSwitch? input = GetESwitch();
        if (m_dGrenades.Count <= 1) { return; }
        switch (input)
        {
            case eSwitch.UP:
                Equipment(GrenadeKeys[0]);
                break;

            case eSwitch.LEFT:
                Equipment(GrenadeKeys[1]);
                break;

            case eSwitch.RIGHT:
                if (m_dGrenades.Count < 3) { return; }
                Equipment(GrenadeKeys[2]);
                break;

            case eSwitch.DOWN:
                if (m_dGrenades.Count< 4) { return; }
                Equipment(GrenadeKeys[3]);
                break;

            default:
                break;
        }
    }

    public void ResetGrenades()
    {
        foreach (var item in m_dGrenades)
        {
            m_dGrenades[item.Key].Count = 2;
        }
        if (OnCount != null) OnCount();

    }
    #endregion

    #region Grenade Method
    /// <summary>
    /// Hold grenade for enhance throwing force
    /// </summary>
    /// <returns></returns>
    public bool Holding()
    {
        if (GrenadeCounter <= 0) { return false; }
        if (!m_bHolding)
        {
            m_bHolding = LoadGrenade();
            return m_bHolding;
        }
        else mCurBehaviors.grenadeInfo.Force += 5 * Time.fixedDeltaTime;
        return true;
    }

    /// <summary>
    /// Release the button to throw the grenade
    /// </summary>
    public void Throw()
    {
        if (!m_bHolding) return;
        GrenadeCounter--;
        mCurBehaviors.Throw(ref m_Grenade, m_ThrowPos);
        m_Grenade.GetComponent<Grenades>().SetInfo(mCurBehaviors.grenadeInfo, m_Player);
        m_bHolding = false;
        
        if (OnCount != null) OnCount();
    }
    #endregion

    #endregion Public Method

    #region Private method

    //Get switch code
    private eSwitch? GetESwitch()
    {
        if (Input.GetAxisRaw(m_InputVertical) == 1) { return eSwitch.UP; }
        else if (Input.GetAxisRaw(m_InputVertical) == -1) { return eSwitch.DOWN; }
        else if (Input.GetAxisRaw(m_InputHorizontal) == -1) { return eSwitch.LEFT; }
        else if (Input.GetAxisRaw(m_InputHorizontal) == 1) { return eSwitch.RIGHT; }
        else { return null; }
    }

    //Create or equip grenades
    private void Equipment(int id)
    {
        CurrentID = id;
        if (OnCount != null) OnCount();
        if (OnChangeID != null) OnChangeID();
    }

    //Set grenades active
    private bool LoadGrenade()
    {
        m_Grenade = m_dGrenades[CurrentID].LoadGrenade();
        if (m_Grenade == null) return false;
        m_bHolding = true;
        m_Grenade.transform.position = m_StarPos.position;
        m_Grenade.transform.forward = m_StarPos.forward;
        m_Grenade.transform.localScale *= 0.2f;
        m_Grenade.transform.SetParent(m_StarPos);
        m_Grenade.SetActive(true);
        return true;
    }

    //Create new grenade whitch doesn't exist in dictionary

    #endregion Private method

    #region Delegate of EventHolder

    public delegate void EventHolder();

    public EventHolder OnCount;
    public EventHolder OnChangeID;

    #endregion Delegate of EventHolder

    #region Public properties

    public Dictionary<int, int> ActiveGrenades
    {
        get
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            foreach (var item in m_dGrenades)
            {
                dic.Add(item.Key, item.Value.Count);
            }
            return dic;
        }
    }

    public int CurrentID { private set; get; }

    public int GrenadeCounter
    {
        get { return m_dGrenades[CurrentID].Count; }
        private set
        {
            m_dGrenades[CurrentID].Count = value;
        }
    }

    public int[] GrenadeKeys
    {
        get
        {
            int[] keys = new int[m_dGrenades.Count];
            m_dGrenades.Keys.CopyTo(keys, 0);
            return keys;
        }
    }

    #endregion Public properties

    #region Private field

    private Dictionary<int, GrenadeComponent> m_dGrenades = new Dictionary<int, GrenadeComponent>();
    private IGrenadesBehaviors mCurBehaviors { get { return m_dGrenades[CurrentID].m_Behavior; } }
    private GrenadeFactory m_GrenadeFactory = new GrenadeFactory();
    private GameObject m_Grenade;
    private Transform m_StarPos;
    private Transform m_ThrowPos;
    private Player m_Player;
    private bool m_bHolding;

    private string m_InputVertical = "StratagemVertical";
    private string m_InputHorizontal = "StratagemHorizontal";

    #endregion Private field

    protected void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ResetGrenades();
        }

    }

    class GrenadeComponent
    {
        public IGrenadesBehaviors m_Behavior;
        public GameObject LoadGrenade()
        {
            if (Count <= 0) return null;
            GameObject grenade = ObjectPool.m_Instance.LoadGameObjectFromPool(m_Behavior.grenadeInfo.ID) ?? null;
            return grenade;
        }
        public int Count
        {
            get { return _count;  }
            set
            {
                if (value > m_Behavior.grenadeInfo.MaxCount) _count = m_Behavior.grenadeInfo.MaxCount;
                else if (value < 0) _count = 0;
                else _count = value;
            }
        }
        public void AddCount(int i) { _count += i; }
        private int _count;
    }

}