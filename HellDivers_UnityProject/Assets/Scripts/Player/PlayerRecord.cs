using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerRecord
{
    public int TimesOfDeath
    {
        get { return m_TimesOfDeath; }
        set { if (value > m_TimesOfDeath) m_TimesOfDeath = value; }
    }

    public int NumOfKills
    {
        get { return m_NumOfKills; }
        set { if (value > m_NumOfKills) m_NumOfKills = value; }
    }

    public int NumOfMission
    {
        get { return m_NumOfMission; }
        set { if (value > m_NumOfMission) m_NumOfMission = value; }
    }

    public int Shots
    {
        get { return m_Shots; }
        set { if (value > m_Shots) m_Shots = value; }
    }

    public int Money { get { return m_Money; } set { m_Money = value; } }
    public int Exp { get { return m_Exp; } set { m_Exp = value; } }

    private int m_TimesOfDeath;
    private int m_NumOfKills;
    private int m_NumOfMission;
    private int m_Shots;
    private int m_Money;
    private int m_Exp;

    public void Reset()
    {
        m_TimesOfDeath = 0;
        m_NumOfKills = 0;
        m_NumOfMission = 0;
    }
}