using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WelcomeButtonController : MonoBehaviour {

    #region SerializeField
    [Header("== Button ==")]
    [SerializeField] Button m_Start;
    [SerializeField] Button m_Continue;
    [SerializeField] Button m_Producer;
    [SerializeField] Button m_Exit;
    [SerializeField] Button m_Press;
    [Header("== Circle ==")]
    [SerializeField] GameObject m_Circle;
    [Header("== GameObject ==")]
    [SerializeField] GameObject m_Menu;
    [SerializeField] GameObject m_FadePanel;
    [SerializeField] GameObject m_Animantion;
    #endregion

    public ControllerSetting m_Controller { private set; get; }

    private DataSaver m_data;
    private float m_fTimer;
    private bool m_bSetting;

    #region Mono Behaviors
    private void Start()
    {
        SetPress();
        SetStart();
        SetContinue();
        SetCredits();
        SetExit();
        m_fTimer = 0.2f;
    }

    private void LateUpdate()
    {
        if (!m_bSetting)
        {
            if (m_fTimer < 0)
            {
                EventSystem.current.SetSelectedGameObject(m_Press.gameObject);
                m_fTimer = 0.5f;
            }
            m_fTimer -= Time.fixedDeltaTime;
        }
    }
    #endregion

    #region Set Button event

    private void SetPress()
    {
        m_Press.onClick.AddListener(() => m_bSetting = true);
        m_Press.onClick.AddListener(() => Change(m_Press.gameObject, m_Menu));
        m_Press.onClick.AddListener(() => SetSelectGO(m_Continue));
    }

    private void SetStart()
    {
        AddEvent(m_Start);
        m_Start.onClick.AddListener(() => DataLoader(1, false));
        m_Start.onClick.AddListener(() => StartCoroutine(ChangePanel()));
    }

    private void SetContinue()
    {
        AddEvent(m_Continue);
        m_Continue.onClick.AddListener(() => DataLoader(1));
        m_Continue.onClick.AddListener(() => StartCoroutine(ChangeScene()));
    }

    private void SetCredits()
    {
        AddEvent(m_Producer);
        m_Producer.onClick.AddListener(() => StartCoroutine(ChangeScene(false)));
    }

    private void SetExit()
    {
        AddEvent(m_Exit);
        m_Exit.onClick.AddListener(() => Application.Quit());
    }

    //For Start
    IEnumerator ChangePanel()
    {
        CloseButton();
        yield return new WaitForSeconds(.8f);
        Change(m_Menu, m_Animantion);
        this.gameObject.SetActive(false);
        
    }

    //For Continue
    IEnumerator ChangeScene(bool toLobby = true)
    {
        CloseButton();
        yield return new WaitForSeconds(1.5f);
        if (toLobby) SceneController.Instance.ToLobby();
        else SceneController.Instance.ToCredits();
    }

    #endregion

    #region Method

    //Disable button control 
    private void CloseButton()
    {
        m_FadePanel.SetActive(true);
        m_Start.interactable = false;
        m_Continue.interactable = false;
        m_Producer.interactable = false;
        m_Exit.interactable = false;
    }

    //Add event into button
    private void AddEvent(Button btn)
    {
        EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener((eventdata) => SwitchCircle(btn.gameObject));
        trigger.triggers.Add(entry);
    }

    //Move the circle to the front of the chosen button
    private void SwitchCircle(GameObject go)
    {
        Vector3 pos = m_Circle.transform.position;
        pos.y = go.transform.position.y;
        pos.x = go.transform.position.x - 10;
        m_Circle.transform.position = pos;
    }

    //Change active panel
    private void Change(GameObject pre, GameObject next)
    {
        pre.SetActive(false);
        next.SetActive(true);
    }

    //Set next highlight button
    private void SetSelectGO(Button next)
    {
        EventSystem.current.SetSelectedGameObject(next.gameObject, null);
    }

    //Set controller
    public void SetController(ControllerSetting controller)
    {
        m_Controller = controller;
    } 

    #endregion

    #region Start 

    private void CreateData()
    {
        if (m_Controller == null) { m_Controller = InputManager.Instance.InputSettingMap[1]; }
        PlayerManager.Instance.CreatePlayer(1, m_Controller, m_data);
    }

    private void LoadData()
    {
        if (m_Controller == null) { m_Controller = InputManager.Instance.InputSettingMap[1]; }
        PlayerManager.Instance.LoadPlayer(1, m_Controller, m_data);
    }

    private void DataLoader(int i, bool load = true)
    {
        m_data = DataSaverManager.Instance.DataSaverMap[i];
        if (!load) CreateData();
        else LoadData();
    }

    #endregion

}
