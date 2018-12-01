using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIWeaponList : MonoBehaviour {

    #region SerializeField

    [Header("== Script Setting ==")]
    [SerializeField] UIWeaponDisplay weaponDisplay;
    [Header("== Prefabs Setting ==")]
    [SerializeField] GameObject m_WeaponUI;
    [SerializeField] GameObject m_WeaponUI_Lock;
    [SerializeField] GameObject m_Content;
    [Header("== Panel Setting ==")]
    [SerializeField] GameObject m_LockPanel;

    #endregion

    #region Private Field

    List<GameObject> m_weapons = new List<GameObject>();

    int posY
    {
        get { return _posY; }
        set
        {
            if (value < 0) _posY = 0;
            else if (value > 852) _posY = 852;
            else _posY = value;
        }
    }

    int _posY;

    int m_unlockWeaponCount;

    int m_currentCount;

    GameObject m_currentSelectObject;

    #endregion

    #region Monobehaviors

    private void Start()
    {
        int PlayerID = weaponDisplay.SetPlayer.PlayerID;

        CreateWeaponUI(PlayerID);
        m_unlockWeaponCount = m_weapons.Count;
        CreateWeaponUILock(PlayerID);

        UpdateMoney();
        SubscriptAxisEvent();
        ChangeWeapon();
    }

    private void OnEnable()
    {
        if (m_weapons.Count < 1) return;
        UpdateMoney();
        SubscriptAxisEvent();
        ChangeWeapon();
    }

    private void OnDisable()
    {
        UnsubscribeAxisEvent();
    }

    private void LateUpdate()
    {
        Vector3 pos = m_Content.transform.localPosition;
        pos.y = posY;
        m_Content.transform.localPosition = Vector3.Lerp(m_Content.transform.localPosition, pos, 0.05f);
    }

    #endregion

    #region Subscript Event

    private void SubscriptAxisEvent()
    {
        weaponDisplay.SetPlayer.Control.AxisUp += ButtonUp;
        weaponDisplay.SetPlayer.Control.AxisDown += ButtonDown;
        weaponDisplay.SetPlayer.Control.AxisSubmit += ButtonSubmit;
        weaponDisplay.SetPlayer.Control.AxisCancel += ButtonCancel;
    }

    private void UnsubscribeAxisEvent()
    {
        weaponDisplay.SetPlayer.Control.AxisUp -= ButtonUp;
        weaponDisplay.SetPlayer.Control.AxisDown -= ButtonDown;
        weaponDisplay.SetPlayer.Control.AxisSubmit -= ButtonSubmit;
        weaponDisplay.SetPlayer.Control.AxisCancel -= ButtonCancel;
    }

    #endregion

    #region Method

    private void ChangeWeapon()
    {
        for (int i = 0; i < m_weapons.Count; i++)
        {
            if ( GameData.Instance.WeaponInfoTable[Determine()].Type == GameData.Instance.WeaponInfoTable[int.Parse(m_weapons[i].name)].Type)
            {
                m_currentSelectObject = m_weapons[i];
                OnSelectEvent(m_currentSelectObject);
                return;
            }
        }
    }

    public void UpdateMoney()
    {
        weaponDisplay.SetCoin(PlayerManager.Instance.Players[weaponDisplay.SetPlayer.PlayerID].info.Money);
    }

    public void LevelUp(int id)
    {
        for (int i = 0; i < m_weapons.Count; i++)
        {
            if ((id-1).ToString() == m_weapons[i].gameObject.name)
            {
                m_weapons[i].gameObject.name = id.ToString();
                weaponDisplay.SetWeaponUI(m_weapons[i].gameObject, id);
                return;
            }
        }
    }

    private void CreateWeaponUI(int player)
    {
        GameObject go;
        List<int> unlockWeapons = PlayerManager.Instance.Players[player].info.UnlockedWeapons;
        for (int i = 0; i < unlockWeapons.Count; i++)
        {
            go = Instantiate(m_WeaponUI, m_Content.transform);
            int id = unlockWeapons[i];
            go.name = id.ToString();
            weaponDisplay.SetWeaponUI(go, id);
            m_weapons.Add(go);
        }
    }

    private void CreateWeaponUILock(int player)
    {
        GameObject go;
        int rank = PlayerManager.Instance.Players[player].info.Rank;
        foreach (var item in GameData.Instance.UnlockWeaponsTable)
        {
            if (item.Key < rank + 1) continue;
            for (int i = 0; i < item.Value.Count; i++)
            {
                go = Instantiate(m_WeaponUI_Lock, m_Content.transform);
                int id = item.Value[i];
                go.name = id.ToString();
                go.GetComponent<LobbyUI_Weapon>().IsLocok(true);
                go.GetComponent<LobbyUI_Weapon>().SetWeaponUI(id, item.Key);
                m_weapons.Add(go);
            }
        }

    }

    private int Determine()
    {
        if (weaponDisplay.SetPlayer.m_bPrimary) return weaponDisplay.SetPlayer.PriWeaponID;
        else return weaponDisplay.SetPlayer.SecWeaponID;
    }

    #endregion

    #region Button Behaviors

    private void ButtonUp()
    {
        if (m_currentSelectObject == m_weapons[0]) return;
        DisSelectEvent(m_currentSelectObject);
        ButtonNavUP();
        OnSelectEvent(m_currentSelectObject);
    }

    private void ButtonDown()
    {
        if (m_currentSelectObject == m_weapons[m_weapons.Count-1]) return;
        DisSelectEvent(m_currentSelectObject);
        ButtonNavDown();
        OnSelectEvent(m_currentSelectObject);
    }

    private void ButtonSubmit()
    {
        OnClickEvent();
    }

    private void ButtonCancel()
    {
        DisSelectEvent(m_currentSelectObject);
        weaponDisplay.SetPlayer.SelectWeaponUI(false);
    }

    #endregion
    
    #region Set Button

    private void SetInfo(GameObject go)
    {
        int id = int.Parse(go.name);
        int type = GameData.Instance.WeaponInfoTable[id].Type;
        weaponDisplay.Info.SetWeaponInfo(id, type);
    }

    private void OnValueChange(GameObject go)
    {
        for (int j = 0; j < m_weapons.Count; j++)
        {
            if (m_weapons[j].name == go.name)
            {
                posY = -150 + 150 * j;
            }
        }
    }

    private void ButtonNavDown()
    {
        GameObject go = m_currentSelectObject;
        if (go == m_weapons[m_weapons.Count - 1]) return;
        for (int i = 0; i < m_weapons.Count-1; i++)
        {
            if (go == m_weapons[i])
            {
                m_currentSelectObject = m_weapons[i + 1];
                m_currentCount = i + 1 ;
                return;
            }
        }
    }

    private void ButtonNavUP()
    {
        GameObject go = m_currentSelectObject;
        if (go == m_weapons[0]) return;
        for (int i = 1; i < m_weapons.Count; i++)
        {
            if (go == m_weapons[i])
            {
                m_currentSelectObject = m_weapons[i - 1];
                m_currentCount = i -1;
                return;
            }
        }
    }

    private void OnSelectEvent(GameObject go)
    {
        OnValueChange(go);
        go.GetComponent<LobbyUI_Weapon>().SetHighlightBG();
        if (m_currentCount > m_unlockWeaponCount - 1)
        {
            m_LockPanel.SetActive(true);
            return;
        }
        SetInfo(go);
        weaponDisplay.Button.CheckLevelUp();
        weaponDisplay.SetPlayer.PlayAudio();
    }

    private void DisSelectEvent(GameObject go)
    {
        go.GetComponent<LobbyUI_Weapon>().SetBG();
        m_LockPanel.SetActive(false);
    }
    
    private void OnClickEvent() 
    {
        if (m_currentCount > m_unlockWeaponCount - 1) return;
        weaponDisplay.SetCurID(weaponDisplay.Info.ID);
        DisSelectEvent(m_currentSelectObject);
        UnsubscribeAxisEvent();
        weaponDisplay.Button.SetCancel();
        weaponDisplay.Button.OnSelectButton();
    }

    public void Return()
    {
        weaponDisplay.Button.SetCancel(false);
        SubscriptAxisEvent();
        OnSelectEvent(m_currentSelectObject);
    }

    #endregion
}
