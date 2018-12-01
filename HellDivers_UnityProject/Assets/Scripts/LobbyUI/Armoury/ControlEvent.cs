using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ControlEvent : MonoBehaviour {

    public int PlayerID { get; private set; }
    public void SetID(int i) { PlayerID = i; }
    public ControllerSetting Controller { get; private set; }

    public delegate void AxisEvent();
    public event AxisEvent AxisDown;
    public event AxisEvent AxisUp;
    public event AxisEvent AxisLeft;
    public event AxisEvent AxisRight;
    public event AxisEvent AxisSubmit;
    public event AxisEvent AxisCancel;
    public event AxisEvent AxisSecret;

    private float timer;
    private float timeBetweenInputs = 0.25f;

    // Use this for initialization
    void Start()
    {
        Controller = PlayerManager.Instance.Players[PlayerID].controllerSetting;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!PlayerManager.Instance.Players.ContainsKey(PlayerID) || !PlayerManager.Instance.Players[PlayerID].controllerSetting) { return; }
        if (timer < 0)
        {
            InputSetting(Controller);
        }
        timer -= Time.fixedDeltaTime;
    }

    private bool InputSetting(ControllerSetting m_controller)
    {
        if (Input.GetAxis(m_controller.Vertical) > 0 || Input.GetAxis("Vertical") > 0)
        {
            if (AxisUp != null) AxisUp();
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetAxis(m_controller.Vertical) < 0 || Input.GetAxis("Vertical") < 0)
        {
            if (AxisDown != null) AxisDown();
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetAxis(m_controller.Horizontal) > 0 || Input.GetAxis("Horizontal") > 0)
        {
            if (AxisRight != null) AxisRight();
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetAxis(m_controller.Horizontal) < 0 || Input.GetAxis("Horizontal") < 0)
        {
            if (AxisLeft != null) AxisLeft();
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetKeyDown(m_controller.Submit) || Input.GetKeyDown(KeyCode.Space))
        {
            if (AxisSubmit != null) AxisSubmit();
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetKeyDown(m_controller.Cancel)|| Input.GetKeyDown(KeyCode.Escape))
        {
            if (AxisCancel != null) AxisCancel();
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetButtonDown("Return") || Input.GetKeyDown(KeyCode.Backspace))
        {
            PlayerManager.Instance.UnloadPlayerMap();
            SceneController.Instance.ToLauncher();
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            if (AxisSecret != null) AxisSecret();
            timer = timeBetweenInputs;
            return true;
        }
        return false;
    }
    
}
