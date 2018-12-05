using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PressToContinue : MonoBehaviour {

    [SerializeField] Button m_Press;

    public void OnEnable()
    {
        m_Press.onClick.AddListener(() => ToLauncher());
        EventSystem.current.SetSelectedGameObject(m_Press.gameObject);
    }

    //private void FixedUpdate()
    //{
    //    if (Input.GetButton("Submit"))
    //    {
    //        ToLauncher();
    //    }
    //}

    public void ToLauncher()
    {
        SceneController.Instance.ToLauncher();
    }
}
