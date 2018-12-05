using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFSMSystem
{
    private List<PlayerFSMState> m_states;
    private Dictionary<ePlayerFSMTrans, PlayerFSMState> m_GlobalMap;
    private ePlayerFSMStateID m_currentStateID;
    public ePlayerFSMStateID CurrentStateID { get { return m_currentStateID; } }
    private ePlayerFSMStateID m_CurrentGlobalStateID;
    public ePlayerFSMStateID CurrentGlobalStateID { get { return m_CurrentGlobalStateID; } }

    private PlayerFSMState m_currentState;
    private PlayerFSMState m_CurrentGlobalState = null;
    private PlayerFSMState m_PreviousState;
    public PlayerFSMState CurrentState { get { return m_currentState; } }
    private PlayerController m_Data;
    // Use this for initialization
    public PlayerFSMSystem(PlayerController data)
    {
        m_Data = data;
        m_states = new List<PlayerFSMState>();
        m_GlobalMap = new Dictionary<ePlayerFSMTrans, PlayerFSMState>();
    }

    public void AddGlobalTransition(ePlayerFSMTrans t, PlayerFSMState s)
    {
        m_GlobalMap.Add(t, s);
    }

    public void PerformGlobalTransition(ePlayerFSMTrans t)
    {
        if (m_GlobalMap.ContainsKey(t))
        {
            m_CurrentGlobalState = m_GlobalMap[t];
            m_CurrentGlobalState.DoBeforeEnter(m_Data);
            m_CurrentGlobalStateID = m_CurrentGlobalState.m_StateID;
        }
    }

    public void AddState(PlayerFSMState s)
    {
        if (s == null)
        {
            return;
        }

        if (m_states.Count == 0)
        {
            m_states.Add(s);
            m_currentState = s;
            m_currentStateID = s.m_StateID;
            return;
        }

        foreach (PlayerFSMState state in m_states)
        {
            if (state.m_StateID == s.m_StateID)
            {
                return;
            }
        }
        m_states.Add(s);

    }

    public void DeleteState(ePlayerFSMStateID id)
    {
        if (id == ePlayerFSMStateID.NullStateID)
        {
            return;
        }

        foreach (PlayerFSMState state in m_states)
        {
            if (state.m_StateID == id)
            {
                m_states.Remove(state);
                return;
            }
        }
    }

    public void PerformTransition(ePlayerFSMTrans trans)
    {
        if (trans == ePlayerFSMTrans.NullTransition)
        {
            return;
        }

        PlayerFSMState state = m_currentState.TransitionTo(trans);
        if (state == null)
        {
            return;
        }

        // Update the currentStateID and currentState
        
        m_currentState.DoBeforeLeave(m_Data);
        m_Data.m_MoveMode = "Origin";

        m_currentState = state;
        m_currentStateID = state.m_StateID;
        m_currentState.DoBeforeEnter(m_Data);
    }

    public void PerformPreviousTransition()
    {
        m_CurrentGlobalState.DoBeforeLeave(m_Data);
        m_CurrentGlobalState = null;
        m_CurrentGlobalStateID = ePlayerFSMStateID.NullStateID;
    }

    public void DoState()
    {
        if (m_Data.m_MoveMode.Equals("Dead") == false)
        {
            if (m_CurrentGlobalStateID == ePlayerFSMStateID.RollStateID)
            {
                m_currentState.CheckCondition(m_Data);
            }
            else
            {
                if (m_CurrentGlobalStateID != ePlayerFSMStateID.ReliveStateID)
                {
                    m_currentState.CheckCondition(m_Data);
                    m_currentState.Do(m_Data);
                }
            }
        }

        if (m_CurrentGlobalStateID == ePlayerFSMStateID.NullStateID) return;
        m_CurrentGlobalState.CheckCondition(m_Data);
        if (m_CurrentGlobalStateID == ePlayerFSMStateID.NullStateID) return;
        m_CurrentGlobalState.Do(m_Data);
    }
}
