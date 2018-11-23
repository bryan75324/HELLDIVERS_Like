﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LobbyUI_Weapon : MonoBehaviour{

    [Header("== Set UI ==")]
    [SerializeField] Image m_BG;
    [SerializeField] Image m_WeaponTexture;
    [SerializeField] Text m_WeaponName;
    [Header("== Private Field ==")]
    [SerializeField] int m_ID;

    #region Getter
    public int ID { get { return m_ID; } }
    public int Type { get { return GameData.Instance.WeaponInfoTable[m_ID].Type; } }
    #endregion
    private Color m_HighLight = new Color(0.788f, 0.635f, 0.133f, 1.0f);
    private Color m_BGColor = new Color(1, 1, 1, 0.286f);
    
    #region Method
    public void SetWeaponUI (int i) {
        m_ID = i;
        WeaponInfo info = GameData.Instance.WeaponInfoTable[m_ID];
        Sprite sprite = ResourceManager.m_Instance.LoadSprite(typeof(Sprite),HELLDIVERS.UI.UIHelper.WeaponIconFolder, "icon_" + info.Image, false);
        m_WeaponName.text = info.Name;
        m_WeaponTexture.sprite = sprite;
	}

    public void SetBG() { m_BG.color = m_BGColor; }

    public void SetHighlightBG() { m_BG.color = m_HighLight; }


    #endregion
}
