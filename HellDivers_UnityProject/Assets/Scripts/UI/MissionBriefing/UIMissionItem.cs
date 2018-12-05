using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMissionItem : MonoBehaviour {

    public string m_Introduction;
    public bool m_bCouldSelect;

    public void Init(eMissionType type)
    {
        switch (type)
        {
            case eMissionType.Tower:
                m_Introduction = "Guard the Tower missions require Helldivers to drop into a hazardous, " +
                "arena-like area to exterminate as many of the enemies of democracy as possible before being allowed to extract.\n" +
                "During a Guard the Tower, a time counter will appear at the middle of the tower.";
                m_bCouldSelect = true;
                break;
            case eMissionType.KillMob:
                m_Introduction = "Destroy the enemy as much as possible";
                m_bCouldSelect = false;
                break;
        }
    }
}
