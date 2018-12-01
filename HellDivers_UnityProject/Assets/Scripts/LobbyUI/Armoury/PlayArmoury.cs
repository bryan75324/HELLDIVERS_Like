using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArmoury : MonoBehaviour {

    [SerializeField] GameObject m_PlayerInGame;

    #region Private Field
    Dictionary<int, bool> m_dPlayerReady = new Dictionary<int, bool>();
    float f_Timer = 1;
    bool b_Loding;
    int[] Members
    {
        get
        {
            var key = new int[PlayerManager.Instance.Players.Count];
            PlayerManager.Instance.Players.Keys.CopyTo(key, 0);
            return key;
        }
    }
    #endregion

    #region MonoBehaviors

    void Start () { foreach (var item in Members) { CreatPlayerMenu(item); } }

    void LateUpdate () {
        if (b_Loding) return;
        else if (!CheckState()) { return; }
        else
        {
            if (f_Timer < 0)
            {
                SceneController.Instance.ToGameScene();
                foreach (int player in Members)
                {
                    PlayerManager.Instance.SavePlayerInfo(player);
                }
                b_Loding = true;
            }
            f_Timer -= Time.fixedDeltaTime;
        }
	}

    #endregion

    #region Private Method


    public void CreatPlayerMenu(int i)
    {
        Vector3 pos = new Vector3(150 + 810 * (i - 1), 780, 0);
        Quaternion rotate = new Quaternion(0, 0, 0, 1);
        GameObject go = Instantiate(m_PlayerInGame, pos, rotate, this.transform);
        go.name = "Player" + i;
        go.GetComponent<ControlEvent>().SetID(i);
        m_dPlayerReady.Add(i, false);
    }

    private bool CheckState()
    {
        for (int i = 1; i < m_dPlayerReady.Count + 1 ; i++)
        {
            if (m_dPlayerReady[i] == false) return false;
        }
        return true;
    }

    #endregion

    public void SetPlayerState(int i, bool ready = true)
    {
        m_dPlayerReady[i] = ready;
    }
}
