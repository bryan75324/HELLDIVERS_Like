using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerWeapon : MonoBehaviour {

    #region SerializeField
    [Header("== UI Setting ==")]
        [SerializeField] ControlEvent m_Control;
        [SerializeField] Image m_Top;
        [SerializeField] Text m_tPlayerName;
        [SerializeField] Text m_tRank;
        [SerializeField] Image m_iRank;
        [SerializeField] Text m_tConfirm;
        [SerializeField] Image m_Confirm;
    [Header("== Prefabs Setting ==")]
        [SerializeField] GameObject m_WeaponUI;
        [SerializeField] GameObject m_StratagemeUI;
        [SerializeField] GameObject m_SelectWeapon;
        [SerializeField] GameObject m_SelectStratagem;
    [Header("== Set Panel ==")]
        [SerializeField] GameObject m_PrimaryWeapon;
        [SerializeField] GameObject m_SecondaryWeapon;
        [SerializeField] GameObject m_Stratagem;
    [Header("== Set Audio ==")]
        [SerializeField] LobbyAudio m_Audio;
    [Header("== Private Field  ==")]
    [SerializeField] int _CurStratagemPos;
    #endregion

    #region Public Field
    public int PlayerID { get { return m_Control.PlayerID; } }
    public int PriWeaponID { private set; get; }
    public int SecWeaponID { private set; get; }
    public int CurStratagemPos { get { return _CurStratagemPos; } }
    public LobbyAudio Audio { get { return m_Audio; } }
    public ControlEvent Control { get { return m_Control; } }
    public bool m_bPrimary;
    public GameObject m_primary;
    public GameObject m_secondary;
    public GameObject[] Stratagems { get { return m_Stratagems; } }

    #endregion

    #region Private Field
    private Color m_Ready = new Color(1, 0.765f, 0, 1);
    private Color m_HighLight = new Color(0.788f, 0.635f, 0.133f, 1.0f);
    private Color m_BGColor = new Color(1, 1, 1, 0.286f);
    private GameObject m_CurrentObject;
    private GameObject m_UpObject;
    private GameObject m_DownObject;
    private GameObject[] m_Stratagems = new GameObject[4];
    private string m_path = "Canvas/ARMOURY/Player";
    #endregion

    #region MonoBehaviors

    void Start(){
        m_path += PlayerID +"/";
        SetPlayer(PlayerID);
        m_CurrentObject = m_Confirm.gameObject;
        OnSelect();
        Audio.StopSound();
        SubscriptAxisEvent();
    }

    private void OnEnable()
    {
        if (m_CurrentObject == null) return;
        SubscriptAxisEvent();
        OnSelect();
    }

    private void OnDisable()
    {
        Control.AxisCancel -= ButtonCanael;
        UnsubscribeAxisEvent();
    }

    #endregion

    #region Init Function

    private void SetPlayer(int player)
    {
        m_Top.color = (player == 1) ? HELLDIVERS.UI.UIHelper.Player1_Color : HELLDIVERS.UI.UIHelper.Player2_Color;
        m_tRank.color = (player == 1) ? HELLDIVERS.UI.UIHelper.Player1_Color : HELLDIVERS.UI.UIHelper.Player2_Color;
        PlayerInfo info = PlayerManager.Instance.Players[player].info;
        m_tRank.text = info.Rank.ToString();
        string rankIcon = string.Format("icon_rank_{0}", info.Rank.ToString("00"));
        m_iRank.sprite = ResourceManager.m_Instance.LoadSprite(typeof(Sprite), HELLDIVERS.UI.UIHelper.RankIconFolder, rankIcon);
        m_tPlayerName.text = info.Username;

        InitialWeapon(ref m_primary, player, 1, true);
        InitialWeapon(ref m_secondary, player, 0, false);
        
        InitialStratagems(player, 0);
        InitialStratagems(player, 1);
        InitialStratagems(player, 2);
        InitialStratagems(player, 3);
    }

    private void InitialWeapon(ref GameObject go, int player, int serial, bool b)
    {
        Transform t = (b) ? m_PrimaryWeapon.transform : m_SecondaryWeapon.transform;
        go = Instantiate(m_WeaponUI, t) as GameObject;
        LobbyUI_Weapon uI = go.GetComponent<LobbyUI_Weapon>();
        int id = PlayerManager.Instance.Players[player].info.Weapons[serial];
        uI.SetWeaponUI(id);
        if (b)  PriWeaponID = uI.ID;
        else SecWeaponID = uI.ID;
    }

    private void InitialStratagems(int player, int serial, bool nothing = false , bool b = false)
    {
        GameObject go = Instantiate(m_StratagemeUI, m_Stratagem.transform) as GameObject;
        LobbyUI_Stratagems uI = go.GetComponent<LobbyUI_Stratagems>();
        int id =(!nothing)? PlayerManager.Instance.Players[player].info.Stratagems[serial] : 0 ;
        go.name = id.ToString();
        uI.SetStratagemUI(id, b);
        m_Stratagems[serial] = go;
    }

    #endregion

    #region Public Method

    public void SelectWeaponUI(bool b)
    {
        string s = "Select_Weapon_SinglePlayer";
        GameObject go = GameObject.Find(m_path + s)?? GameObject.Find(m_path + s + "(Clone)");
        if (!go) { go = Instantiate(m_SelectWeapon, this.transform.parent) as GameObject; }
        go.SetActive(b);
        this.gameObject.SetActive(!b);
        if (b == false) { SetWeapon(); }
    }

    public void SelectStratagemUI(bool b)
    {
        string s = "Select_Stratagems_SinglePlayer";
        GameObject go = GameObject.Find(m_path + s) ?? GameObject.Find(m_path + s + "(Clone)");
        if (!go) { go = Instantiate(m_SelectStratagem, this.transform.parent) as GameObject; }
        go.SetActive(b);
        this.gameObject.SetActive(!b);
    }

    public void SetPriWeaponID(int i) { PriWeaponID = i; }

    public void SetSecWeaponID(int i) { SecWeaponID = i; }

    public void PlayAudio(AudioSource source)
    {
        source.pitch = Random.Range(0.95f, 1.5f);
        source.Play();
    }

    #endregion

    #region Panel Click Event

    private void SetWeapon()
    {
        m_primary.GetComponent<LobbyUI_Weapon>().SetWeaponUI(PriWeaponID); 
        m_secondary.GetComponent<LobbyUI_Weapon>().SetWeaponUI(SecWeaponID);
    }

    public void SetStratagems(int serial, int id)
    {
        Stratagems[serial].GetComponent<LobbyUI_Stratagems>().SetStratagemUI(id);

    }

    #endregion

    #region Confirm Event

    private void OnConfirm()
    {
        SetColor(ref m_Confirm, m_HighLight);
    }

    private void DisConfirm()
    {
        SetColor(ref m_Confirm, m_BGColor);
    }

    private void CancelConfirm()
    {
        m_tConfirm.text = "CORFIRM";
        SetColor(ref m_Confirm, m_HighLight);
        this.GetComponentInParent<PlayArmoury>().SetPlayerState(PlayerID, false);
        SubscriptAxisEvent();
    }

    private void SubmitConfirm()
    {
        PlayerManager.Instance.RefreshEquipment(PlayerID, WeaponsList(), StratagemsList());
        m_tConfirm.text = "READY";
        SetColor(ref m_Confirm, m_Ready);
        this.GetComponentInParent<PlayArmoury>().SetPlayerState(PlayerID);
        UnsubscribeAxisEvent();
    }

    #endregion

    #region Stratagems Method

    private void OnSelectStratagems()
    {
        m_Stratagems[CurStratagemPos].GetComponent<LobbyUI_Stratagems>().SetHighlightBG();
        Control.AxisLeft += NavLeft;
        Control.AxisRight += NavRight;
    }

    private void DisSelectStratagems()
    {
        m_Stratagems[CurStratagemPos].GetComponent<LobbyUI_Stratagems>().SetBG();
        Control.AxisLeft -= NavLeft;
        Control.AxisRight -= NavRight;
    }

    private void OnStratagems(bool right)
    {
        m_Stratagems[CurStratagemPos].GetComponent<LobbyUI_Stratagems>().SetBG();
        if (right) _CurStratagemPos++;
        else _CurStratagemPos--;
        m_Stratagems[CurStratagemPos].GetComponent<LobbyUI_Stratagems>().SetHighlightBG();
        Audio.PlaySelectSound(1);
    }

    private void NavRight()
    {
        if (CurStratagemPos == 3) return;
        OnStratagems(true);
    }

    private void NavLeft()
    {
        if (CurStratagemPos == 0) return ;
        OnStratagems(false);
    }

    #endregion

    #region New Equiptment List

    private List<int> WeaponsList()
    {
        List<int> pList = new List<int>();
        pList.Add(SecWeaponID);
        pList.Add(PriWeaponID);
        return pList;
    }

    private List<int> StratagemsList()
    {
        List<int> pList = new List<int>();
        for (int i = 0; i < Stratagems.Length; i++)
        {
            int id = int.Parse(Stratagems[i].name);
            if (id == 0) continue;
            pList.Add(id);
        }
        return pList;
    }

    #endregion

    #region Subscript Axis

    private void SubscriptAxisEvent()
    {
        Control.AxisUp += ButtonUp;
        Control.AxisDown += ButtonDown;
        Control.AxisSubmit += ButtonSumbit;
        Control.AxisCancel += ButtonCanael;
        Control.AxisSecret += GetRich;
    }

    private void UnsubscribeAxisEvent()
    {
        Control.AxisUp -= ButtonUp;
        Control.AxisDown -= ButtonDown;
        Control.AxisSubmit -= ButtonSumbit;
        Control.AxisSecret -= GetRich;
    }

    #endregion

    #region Button Support Function

    private void OnSelect()
    {
        if (m_CurrentObject == m_PrimaryWeapon)
        {
            SetObject(m_PrimaryWeapon, m_SecondaryWeapon);
            m_primary.GetComponent<LobbyUI_Weapon>().SetHighlightBG();
        }
        else if (m_CurrentObject == m_SecondaryWeapon)
        {
            SetObject(m_PrimaryWeapon, m_Stratagem);
            m_secondary.GetComponent<LobbyUI_Weapon>().SetHighlightBG();
        }
        else if(m_CurrentObject == m_Stratagem)
        {
            SetObject(m_SecondaryWeapon, m_Confirm.gameObject);
            OnSelectStratagems();
        }
        else if (m_CurrentObject == m_Confirm.gameObject)
        {
            SetObject(m_Stratagem, m_Confirm.gameObject);
            OnConfirm();
        }
        Audio.PlaySelectSound(1);
    }

    private void DisSelect()
    {
        if (m_CurrentObject == m_PrimaryWeapon)
        {
            m_primary.GetComponent<LobbyUI_Weapon>().SetBG();
        }
        else if (m_CurrentObject == m_SecondaryWeapon)
        {
            m_secondary.GetComponent<LobbyUI_Weapon>().SetBG();
        }
        else if (m_CurrentObject == m_Stratagem)
        {
            DisSelectStratagems();
        }
        else if (m_CurrentObject == m_Confirm.gameObject)
        {
            DisConfirm();
        }

    }

    private void OnClick()
    {
        if (m_CurrentObject == m_PrimaryWeapon)
        {
            m_bPrimary = true;
            SelectWeaponUI(true);
        }
        else if (m_CurrentObject == m_SecondaryWeapon)
        {
            m_bPrimary = false;
            SelectWeaponUI(true);
        }
        else if(m_CurrentObject == m_Stratagem)
        {
            SelectStratagemUI(true);
        }
        else if (m_CurrentObject == m_Confirm.gameObject)
        {
            SubmitConfirm();
        }
        Audio.PlayClickSound(1);
    }

    private void SetColor(ref Image target, Color color) { target.color = color; }

    private void SetObject(GameObject up, GameObject down)
    {
        m_UpObject = up;
        m_DownObject = down;
    }

    private void GetRich()
    {
        PlayerManager.Instance.Players[PlayerID].info.AddMoney(5000);
        Debug.LogWarning("Money: " + PlayerManager.Instance.Players[PlayerID].info.Money);
    }

    #endregion

    #region Button Behaviors

    private void ButtonUp()
    {
        if(m_CurrentObject == m_PrimaryWeapon) return; 
        DisSelect();
        m_CurrentObject = m_UpObject;
        OnSelect();
    }

    private void ButtonDown()
    {
        if (m_CurrentObject == m_Confirm.gameObject) return;
        DisSelect();
        m_CurrentObject = m_DownObject;
        OnSelect();
    }

    private void ButtonSumbit()
    {
        DisSelect();
        OnClick();
    } 

    private void ButtonCanael()
    {
        DisSelect();
        CancelConfirm();
        m_CurrentObject = m_Confirm.gameObject;
        OnSelect();
    }
         
    #endregion
}
