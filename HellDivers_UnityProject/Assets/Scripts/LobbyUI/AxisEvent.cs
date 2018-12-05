using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AxisEvent : MonoBehaviour {

    [SerializeField] AudioSource m_Audio;

    bool b_Setting1;
    bool b_Setting2;
	// Update is called once per frame
	void FixedUpdate () {
        if (!PlayerManager.Instance.Players.ContainsKey(1) && m_controller == null) { return; }
        if (!b_Setting1) b_Setting1 = SetPlayer1();
        if (!b_Setting2) b_Setting2 = SetPlayer2();
        
        if (timer < 0)
        {
            if (InputSetting(m_controller)) return;
            else if (m_controller2 != null && InputSetting(m_controller2)) return; 
        }
        timer -= Time.fixedDeltaTime;
    }
    private bool SetPlayer1()
    {
        if (PlayerManager.Instance.Players.ContainsKey(1))
        {
            m_controller = PlayerManager.Instance.Players[1].controllerSetting;
            return true;
        }
        return false;
    }

    private bool SetPlayer2()
    {
        if (PlayerManager.Instance.Players.ContainsKey(1) && PlayerManager.Instance.Players.ContainsKey(2))
        {
            m_controller2 = (m_controller == PlayerManager.Instance.Players[1].controllerSetting) ?
                PlayerManager.Instance.Players[2].controllerSetting : PlayerManager.Instance.Players[1].controllerSetting;
            return true;
        }
        return false;
    }

    private bool InputSetting(ControllerSetting controller)
    {
        currentAxis = new AxisEventData(EventSystem.current);
        currentButton = EventSystem.current.currentSelectedGameObject;

        if (Input.GetAxis(controller.Vertical) > 0)
        {
            currentAxis.moveDir = MoveDirection.Up;
            ExecuteEvents.Execute(currentButton, currentAxis, ExecuteEvents.moveHandler);
            timer = timeBetweenInputs;
            return true;

        }
        else if (Input.GetAxis(controller.Vertical) < 0)
        {
            currentAxis.moveDir = MoveDirection.Down;
            ExecuteEvents.Execute(currentButton, currentAxis, ExecuteEvents.moveHandler);
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetAxis(controller.Horizontal) > 0)
        {
            currentAxis.moveDir = MoveDirection.Right;
            ExecuteEvents.Execute(currentButton, currentAxis, ExecuteEvents.moveHandler);
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetAxis(controller.Horizontal) < 0)
        {
            currentAxis.moveDir = MoveDirection.Left;
            ExecuteEvents.Execute(currentButton, currentAxis, ExecuteEvents.moveHandler);
            timer = timeBetweenInputs;
            return true;
        }
        else if (Input.GetKeyDown(controller.Submit))
        {
            if (m_Audio == null) return true;
            if (m_Audio.isPlaying) { m_Audio.Stop(); }
            m_Audio.Play();
            return true;
        }
        return false;
    }

    public void SetController(ControllerSetting controller) { m_controller = controller; }

    private ControllerSetting m_controller;
    private ControllerSetting m_controller2;
    private AxisEventData currentAxis;
    private GameObject currentButton;
    private float timer = 0.5f;
    private float timeBetweenInputs = 0.15f;
}
