using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance { get; private set; }
    private GameData m_GameData = new GameData();
    private InputManager m_InputManager = new InputManager();
    private PlayerManager m_PlayerManager = new PlayerManager();
    private DataSaverManager m_DataManager = new DataSaverManager();
    private MusicManager m_MusicManager;
    private SceneController m_SceneController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);

        m_MusicManager = this.gameObject.AddComponent<MusicManager>();
        m_SceneController = this.gameObject.AddComponent<SceneController>();
        DontDestroyOnLoad(this.gameObject);

        m_GameData.Init();
        m_InputManager.Init();
        m_PlayerManager.Init();
        m_DataManager.Init();
        m_MusicManager.Init();
        m_SceneController.Init();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F10))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
#endif
    }

    [ContextMenu("Print Player Info")]
    public void PrintPlayerInfo()
    {
        foreach (var player in PlayerManager.Instance.Players)
        {
            PlayerInfo info = player.Value.info;
            Debug.LogFormat("Player {0} : Rank {1}", player.Key, info.Rank);
            Debug.LogFormat("Player {0} : Exp {1}", player.Key, info.Exp);
        }
    }
}