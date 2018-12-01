using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HELLDIVERS.UI.InGame;

public class InGamePlayerManager : MonoBehaviour
{
    public static InGamePlayerManager Instance { get; private set; }
    public List<Player> Players { get { return m_Players; } }
    public float SpawnRadius { get { return m_SpawnRadius; } set { m_SpawnRadius = (value < 0) ? 0 : value; } }
    public float RespawnTime { get { return m_RespawnTime; } set { m_RespawnTime = (value < 0) ? 0 : value; } }
    [SerializeField] private float m_SpawnRadius = 10.0f;
    [SerializeField] private float m_RespawnTime = 5.0f;
    private List<Player> m_Players;
    private Dictionary<Player, PlayerInfo> m_PlayerMap;

    public class PlayerStates
    {
        public PlayerStates(PlayerInfo info, Player player)
        {
            this.playerInfo = info;
            this.player = player;
        }

        public PlayerInfo playerInfo;
        public Player player;
        public int timesOfDeath;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        m_Players = new List<Player>();
        m_PlayerMap = new Dictionary<Player, PlayerInfo>();
    }

    public void CreatePlayer(PlayerInfo playerInfo, int num = 1)
    {
        GameObject playerGo = ResourceManager.m_Instance.LoadData(typeof(GameObject), "Characters/Ch00", "ch00") as GameObject;
        playerGo = GameObject.Instantiate(playerGo);
        Player player = playerGo.AddComponent<Player>();
        player.Initialize(playerInfo, num);
        m_PlayerMap.Add(player, playerInfo);
        m_Players.Add(player);
        playerGo.SetActive(false);

        // Create player info UI
        UIInGameMain.Instance.AddPlayer(player);
    }

    public void SpawnPlayers(Transform pos)
    {
        if (m_PlayerMap.Count <= 0) return;

        Transform spawnPos = pos;

        foreach (KeyValuePair<Player, PlayerInfo> player in m_PlayerMap)
        {
            Vector3 position = spawnPos.position;
            if (Physics.CheckSphere(spawnPos.position, 2.0f, 1 << LayerMask.NameToLayer("Player")))
            {
                position = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f) * spawnPos.forward * 2.0f + position;
            }

            player.Key.transform.position = position;
            player.Key.gameObject.SetActive(true);

            if (GameMain.Instance.CameraFolloing.Targets.Count <= 0)
            {
                GameMain.Instance.CameraFolloing.FocusOnTarget(player.Key.transform);
            }
            else
            {
                GameMain.Instance.CameraFolloing.AddTarget(player.Key.transform);
            }
        }
    }

    public bool IsAllPlayerDead()
    {
        for (int i = 0; i < m_Players.Count; i++)
        {
            if (m_Players[i].IsDead == false) return false;
        }
        return true;
    }

    public void RespawnPlayer(Player player)
    {
        if (m_Players.Count >= 2) return;
        StartCoroutine(RespawnProcess(player));
    }

    private IEnumerator RespawnProcess(Player player)
    {
        yield return new WaitForSeconds(RespawnTime);
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Enemies") | 1 << LayerMask.NameToLayer("Obstacle");
        Vector3 spawnPos = player.transform.position;
        bool bBlock;
        do
        {
            spawnPos = player.transform.position;
            float radius = Random.Range(0, SpawnRadius);
            spawnPos += Quaternion.Euler(0.0f, Random.Range(0, 360), 0.0f) * Vector3.forward * radius;
            bBlock = Physics.CheckSphere(spawnPos, 1.5f, layerMask);
        } while (bBlock);

        player.Spawn(spawnPos);
    }
}