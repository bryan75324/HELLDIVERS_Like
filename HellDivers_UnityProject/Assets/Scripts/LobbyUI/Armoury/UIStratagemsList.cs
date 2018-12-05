using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStratagemsList : MonoBehaviour {

    [SerializeField] UIStratagemsDisplay stratagemsDisplay;
    [SerializeField] GameObject m_Content;
    [SerializeField] GameObject m_StratagemUI;

    List<GameObject> m_Stratagems = new List<GameObject>();

    GameObject m_currentSelectObject;
	// Use this for initialization
	void Start ()
    {
        int playerID = stratagemsDisplay.SetPlayer.PlayerID;
        CreateStratagemUI(playerID);
        SubscriptAxisEvent();
        ChangeStratagem();
    }
    private void OnEnable()
    {
        if (m_Stratagems.Count < 1) return;
        SubscriptAxisEvent();
        ChangeStratagem();
    }

    private void OnDisable()
    {
        UnsubscribeAxisEvent();
    }
    // Update is called once per frame
    void Update () {
		
	}


    private void CreateStratagemUI(int player)
    {
        GameObject go;
        List<int> unlockStratagems = PlayerManager.Instance.Players[player].info.UnlockedStratagems;
        for (int i = 0; i < unlockStratagems.Count; i++)
        {
            go = Instantiate(m_StratagemUI, m_Content.transform);
            int id = unlockStratagems[i];
            go.name = id.ToString();
            stratagemsDisplay.SetStratagemUI(go, id);
            m_Stratagems.Add(go);
        }
    }

    private void ChangeStratagem()
    {
        int serial = stratagemsDisplay.SetPlayer.CurStratagemPos;
        for (int i = 0; i < m_Stratagems.Count; i++)
        {
            if (stratagemsDisplay.SetPlayer.Stratagems[serial].name == m_Stratagems[i].name)
            {
                m_currentSelectObject = m_Stratagems[i];
                OnSelectEvent(m_currentSelectObject);
                return;
            }
        }
        m_currentSelectObject = m_Stratagems[0];
        OnSelectEvent(m_currentSelectObject);
    }

    private void OnClickCheck()
    {
        for (int i = 0; i < stratagemsDisplay.SetPlayer.Stratagems.Length; i++)
        {
            if (stratagemsDisplay.SetPlayer.Stratagems[i].name == stratagemsDisplay.Info.ID.ToString())
            {
                int id = int.Parse(stratagemsDisplay.SetPlayer.Stratagems[stratagemsDisplay.SetPlayer.CurStratagemPos].name);
                stratagemsDisplay.SetPlayer.SetStratagems(i, id);
                return;
            }
        }
    }

    #region SubscriptAxis Method

    private void SubscriptAxisEvent()
    {
        stratagemsDisplay.SetPlayer.Control.AxisUp += ButtonUp;
        stratagemsDisplay.SetPlayer.Control.AxisDown += ButtonDown;
        stratagemsDisplay.SetPlayer.Control.AxisLeft += ButtonLeft;
        stratagemsDisplay.SetPlayer.Control.AxisRight += ButtonRight;
        stratagemsDisplay.SetPlayer.Control.AxisSubmit += ButtonSubmit;
        stratagemsDisplay.SetPlayer.Control.AxisCancel += ButtonCancel;
    }

    private void UnsubscribeAxisEvent()
    {
        stratagemsDisplay.SetPlayer.Control.AxisUp -= ButtonUp;
        stratagemsDisplay.SetPlayer.Control.AxisDown -= ButtonDown;
        stratagemsDisplay.SetPlayer.Control.AxisLeft -= ButtonLeft;
        stratagemsDisplay.SetPlayer.Control.AxisRight -= ButtonRight;
        stratagemsDisplay.SetPlayer.Control.AxisSubmit -= ButtonSubmit;
        stratagemsDisplay.SetPlayer.Control.AxisCancel -= ButtonCancel;
    }

    #endregion

    #region Button Behaviors

    private void ButtonUp()
    {
        DisSelectEvent(m_currentSelectObject);
        ButtonNavUP();
        OnSelectEvent(m_currentSelectObject);
    }

    private void ButtonDown()
    {
        DisSelectEvent(m_currentSelectObject);
        ButtonNavDown();
        OnSelectEvent(m_currentSelectObject);
    }

    private void ButtonLeft()
    {
        DisSelectEvent(m_currentSelectObject);
        ButtonNavLeft();
        OnSelectEvent(m_currentSelectObject);
    }

    private void ButtonRight()
    
{
        DisSelectEvent(m_currentSelectObject);
        ButtonNavRight();
        OnSelectEvent(m_currentSelectObject);
    }

    private void ButtonSubmit()
    {
        OnClickEvent();
    }

    private void ButtonCancel()
    {
        DisSelectEvent(m_currentSelectObject);
        UnsubscribeAxisEvent();
        stratagemsDisplay.SetPlayer.SelectStratagemUI(false);
    }

    #endregion

    #region Button Navigation

    private void ButtonNavUP()
    {
        GameObject go = m_currentSelectObject;
        if (go == m_Stratagems[0] || go == m_Stratagems[1] || go == m_Stratagems[2]) return;
        for (int i = 3; i < m_Stratagems.Count; i++)
        {
            if (go == m_Stratagems[i])
            {
                m_currentSelectObject = m_Stratagems[i - 3];
                return;
            }
        }
    }

    private void ButtonNavDown()
    {
        GameObject go = m_currentSelectObject;
        int left = m_Stratagems.Count % 3;
        switch (left)
        {
            case 0:
                for (int i = 0; i < m_Stratagems.Count-3; i++)
                {
                    if (go == m_Stratagems[i])
                    {
                            if(i == m_Stratagems.Count - 1 || i == m_Stratagems.Count - 2 || i == m_Stratagems.Count - 3) return;
                            m_currentSelectObject = m_Stratagems[i + 3];
                        return;
                    }
                }
                break;
            case 1:
                for (int i = 0; i < m_Stratagems.Count; i++)
                {
                    if (go == m_Stratagems[i])
                    {
                        if (i + 3 > m_Stratagems.Count - 1) m_currentSelectObject = m_Stratagems[m_Stratagems.Count - 1];
                        else { m_currentSelectObject = m_Stratagems[i + 3]; }
                        return;
                    }
                }
                break;
            case 2:
                for (int i = 0; i < m_Stratagems.Count-2; i++)
                {
                    if (go == m_Stratagems[i])
                    {
                        if (i == m_Stratagems.Count - 1 || i == m_Stratagems.Count - 2) return;
                        if (i + 3 > m_Stratagems.Count - 1) m_currentSelectObject = m_Stratagems[m_Stratagems.Count - 1];
                        else { m_currentSelectObject = m_Stratagems[i + 3]; }
                        return;
                    }
                }
                break;
        }

    }

    private void ButtonNavLeft()
    {
        GameObject go = m_currentSelectObject;
        if (go == m_Stratagems[0]) return;
        for (int i = 1; i < m_Stratagems.Count; i++)
        {
            if (go == m_Stratagems[i])
            {
                m_currentSelectObject = m_Stratagems[i - 1];
                return;
            }
        }
    } 

    private void ButtonNavRight()
    {
        GameObject go = m_currentSelectObject;
        for (int i = 0; i < m_Stratagems.Count-1; i++)
        {
            if (go == m_Stratagems[i])
            {
                m_currentSelectObject = m_Stratagems[i + 1];
                return;
            }
        }
    }
        
    #endregion

    private void SetInfo(GameObject go)
    {
        int id = int.Parse(go.name);
        stratagemsDisplay.Info.SetStratagemInfo(id);
    }

    private void OnValueChange(GameObject go) { }

    private void OnSelectEvent(GameObject go)
    {
        SetInfo(go);
        OnValueChange(go);
        go.GetComponent<LobbyUI_Stratagems>().SetHighlightBG();
        stratagemsDisplay.SetPlayer.Audio.PlaySelectSound(1);
    }

    private void DisSelectEvent(GameObject go)
    {
        go.GetComponent<LobbyUI_Stratagems>().SetBG();
    }

    private void OnClickEvent()
    {
        DisSelectEvent(m_currentSelectObject);
        UnsubscribeAxisEvent();
        OnClickCheck();
        stratagemsDisplay.SetPlayer.SetStratagems(stratagemsDisplay.SetPlayer.CurStratagemPos, stratagemsDisplay.Info.ID);
        stratagemsDisplay.SetPlayer.Audio.PlayClickSound(1);
        stratagemsDisplay.SetPlayer.SelectStratagemUI(false);
    }

}
