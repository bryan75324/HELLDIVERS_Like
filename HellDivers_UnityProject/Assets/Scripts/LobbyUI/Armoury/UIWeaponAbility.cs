using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponAbility : MonoBehaviour {

    [Header("== Set Current Ability ==")]
    [SerializeField] Text m_WeaponName;
    [Header("== Set Current Value ==")]
    [SerializeField] RectTransform m_Current;
    [Header("== Set LevelUP Value ==")]
    [SerializeField] RectTransform m_LevelUP;
    [Header("== Set Value ==")]
    [SerializeField] Text m_CurValue;
    [SerializeField] Text m_CurUnit;
    [Header("== Set GameObject ==")]
    [SerializeField] GameObject m_Ability;
    [SerializeField] Text m_FireMode;
    Vector2 vec = new Vector2();

    public void SetAbility(string name, float currentWidth, float nextWidth, string value, string unit)
    {
        m_Ability.SetActive(true);
        m_WeaponName.text = name;
        vec.x = currentWidth;
        m_Current.sizeDelta = vec;
        vec.x = nextWidth;
        m_LevelUP.sizeDelta = vec;
        m_CurValue.text = value;
        m_CurValue.gameObject.SetActive(true);
        m_CurUnit.text = unit;
        m_CurUnit.transform.parent.gameObject.SetActive(true);
    }

    public void SetWord(string name, string mode)
    {
        m_FireMode.gameObject.SetActive(true);
        m_WeaponName.text = name;
        m_FireMode.text = mode;
    }

}
