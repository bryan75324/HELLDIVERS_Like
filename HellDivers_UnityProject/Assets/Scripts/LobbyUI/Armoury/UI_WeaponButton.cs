using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponButton : MonoBehaviour {
    
    [SerializeField] UIWeaponDisplay weaponDisplay;
    [SerializeField] Image m_LevelUp;
    [SerializeField] Image m_Select;
    [SerializeField] Text m_Cost;

    public GameObject LevelUp { get { return m_LevelUp.gameObject; } }
    public GameObject Select  { get { return m_Select.gameObject; } }
    //public GameObject 

    private GameObject m_CurrentObject;
    private AudioSource m_Audio;
    private Color m_HighLight = new Color(0.788f, 0.635f, 0.133f, 1.0f);
    private Color m_BGColor = new Color(1, 1, 1, 1);
    private Color m_NonSelectableColor = new Color(1, 1, 1, 0.286f);
    private Color m_tCostColor = new Color(1, 0.81f, 0.18f, 1);
    private Color m_tNotEnough = new Color(1, 0.18f, 0.18f, 1);
    private int mi_money;
    private bool mb_costing;

    private void Start()
    {
        m_Audio = m_LevelUp.gameObject.GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
        if (!mb_costing) return;
        mi_money =(int)Mathf.Lerp(mi_money, PlayerManager.Instance.Players[weaponDisplay.SetPlayer.PlayerID].info.Money, 0.1f);
        weaponDisplay.SetCoin(mi_money);
        if (mi_money == PlayerManager.Instance.Players[weaponDisplay.SetPlayer.PlayerID].info.Money) mb_costing = false;
    }

    #region Button Behaviors

    public void SetCancel(bool plus = true)
    {
        if (plus) weaponDisplay.SetPlayer.Control.AxisCancel += weaponDisplay.WeaponList.Return;
        else
        {
            DisSelectButton();
            DisLevelUpButton();
            weaponDisplay.SetPlayer.Control.AxisCancel -= weaponDisplay.WeaponList.Return;
            weaponDisplay.SetPlayer.Control.AxisLeft -= SetLeftNav;
        }
    }

    private void SetLeftNav()
    {
        if (m_CurrentObject == LevelUp || !CheckLevelUp()) return;
        DisSelectButton();
        OnLevelUpButton();
        m_CurrentObject = LevelUp;
    }

    private void SetRightNav()
    {
        if (m_CurrentObject == Select) return;
        DisLevelUpButton();
        OnSelectButton();
        m_CurrentObject = Select;
    }

    public void OnSelectButton()
    {
        SetHighlightBG(m_Select);
        weaponDisplay.SetPlayer.Control.AxisSubmit += ClickSelectButton;
        weaponDisplay.SetPlayer.Control.AxisLeft += SetLeftNav;
        weaponDisplay.SetPlayer.Control.AxisRight -= SetRightNav;
    }

    private void OnLevelUpButton()
    {
        SetHighlightBG(m_LevelUp);
        weaponDisplay.SetPlayer.Control.AxisSubmit += ClickLevelUpButton;
        weaponDisplay.SetPlayer.Control.AxisLeft -= SetLeftNav;
        weaponDisplay.SetPlayer.Control.AxisRight += SetRightNav;
    }

    private void DisSelectButton()
    {
        SetBG(m_Select);
        weaponDisplay.SetPlayer.Control.AxisSubmit -= ClickSelectButton;
    }

    private void DisLevelUpButton()
    {
        SetBG(m_LevelUp);
        weaponDisplay.SetPlayer.Control.AxisSubmit -= ClickLevelUpButton;
    }

    private void ClickSelectButton()
    {
        SetCancel(false);
        ClickSelect(weaponDisplay.SetPlayer.PlayerID);
    }

    private void ClickLevelUpButton()
    {
        ClickLevelUp(weaponDisplay.SetPlayer.PlayerID);
    }

    #endregion

    #region Button Method
    public bool CheckLevelUp()
    {
        int id = weaponDisplay.Info.ID;
        int cost = GameData.Instance.WeaponInfoTable[id].Cost;
        m_Cost.color = m_tCostColor;
        int money = PlayerManager.Instance.Players[weaponDisplay.SetPlayer.PlayerID].info.Money;
        if (money < cost)
        {
            m_Cost.color = m_tNotEnough;
            SetNonSelectableBG(m_LevelUp);
            return false;
        }
        return true;
    }

    private void CostMoney()
    {
        int cost = GameData.Instance.WeaponInfoTable[weaponDisplay.Info.ID].Cost;
        mi_money = PlayerManager.Instance.Players[weaponDisplay.SetPlayer.PlayerID].info.Money;
        PlayerManager.Instance.Players[weaponDisplay.SetPlayer.PlayerID].info.CostMoney(cost);
        mb_costing = true;
    }

    private void SetBG(Image BG)
    {
        BG.color = m_BGColor;
    }

    private void SetHighlightBG(Image BG)
    {
        BG.color = m_HighLight;
    }

    private void SetNonSelectableBG(Image BG)
    {
        BG.color = m_NonSelectableColor;
    }

    private void ClickLevelUp(int player)
    {
        int type = GameData.Instance.WeaponInfoTable[weaponDisplay.CurWeaponID].Type;
        List<int> pList = weaponDisplay.Info.weapons[type];
        if (pList[pList.Count - 1] != weaponDisplay.CurWeaponID)
        {
            int id = weaponDisplay.CurWeaponID;
            PlayerManager.Instance.Players[player].info.LevelUpWeapon(ref id);
            weaponDisplay.SetCurID(id);
            CostMoney();
            m_Audio.Play();
            weaponDisplay.WeaponList.LevelUp(id);
            weaponDisplay.Info.SetWeaponInfo(id, type);
            if (weaponDisplay.CurWeaponID == pList[pList.Count - 1] || !CheckLevelUp())
            {
                SetRightNav();
                SetNonSelectableBG(m_LevelUp);
            }   
        }
    }

    private void ClickSelect(int player)
    {
        GameObject go = null;
        SetPlayerWeapon setPlayer = weaponDisplay.SetPlayer;
        if (setPlayer.m_bPrimary)
        {
            if (GameData.Instance.WeaponInfoTable[weaponDisplay.CurWeaponID].Type == GameData.Instance.WeaponInfoTable[setPlayer.SecWeaponID].Type)
            {
                go = setPlayer.m_secondary;
                weaponDisplay.SetWeaponUI(go, setPlayer.PriWeaponID, false);
                setPlayer.SetSecWeaponID(setPlayer.PriWeaponID);
            }
            go = setPlayer.m_primary;
            weaponDisplay.SetWeaponUI(go, weaponDisplay.CurWeaponID);
            setPlayer.SetPriWeaponID(weaponDisplay.CurWeaponID);
        }
        else
        {
            if (GameData.Instance.WeaponInfoTable[weaponDisplay.CurWeaponID].Type == GameData.Instance.WeaponInfoTable[setPlayer.PriWeaponID].Type)
            {
                go = setPlayer.m_primary;
                weaponDisplay.SetWeaponUI(go, setPlayer.SecWeaponID, false);
                setPlayer.SetPriWeaponID(setPlayer.SecWeaponID);
            }
            go = setPlayer.m_secondary;
            weaponDisplay.SetWeaponUI(go, weaponDisplay.CurWeaponID);
            setPlayer.SetSecWeaponID(weaponDisplay.CurWeaponID);
        }
        setPlayer.SelectWeaponUI(false);
    }

    #endregion

}
