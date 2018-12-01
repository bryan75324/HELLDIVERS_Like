using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionBriefing : MonoBehaviour
{
    [SerializeField] private Transform m_PanelMap;
    [SerializeField] private UIMissionBriefingMap m_Map;
    [SerializeField] private GameObject m_GOConcentric;
    [SerializeField] private Image m_Backround;
    [SerializeField] private Color m_BackroundColor;

    public static UIMissionBriefing Instance { get; private set; }

    public UIMissionBriefingMap Map { get { return m_Map; } }
    public GameObject Concentric { get { return m_GOConcentric; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
        
        m_Map = Instantiate(m_Map, m_PanelMap);
    }

    public void Init(MapInfo mapInfo)
    {
        for (int i = 0; i < mapInfo.SpawnPos.Count; i++)
        {
            AddPoint(mapInfo.SpawnPos[i].gameObject, eMapPointType.SPAWNPOINT);
            Debug.Log("Creata SpawnPoint");
        }
        m_Map.Concentric.OnClick += ComfirmSpawnPosition;
    }

    public void DrawUI()
    {
        this.transform.SetAsLastSibling();
    }

    public void AddPoint(GameObject target, eMapPointType type)
    {
        m_Map.AddPointPrefab(target, type);
    }

    public void ComfirmSpawnPosition()
    {
        //Debug.Log(m_BackroundColor.a);
        if (m_Map.ComfirmSpawnPosition())
        {
            StartCoroutine(FadeOut());
            m_Map.Concentric.OnClick -= ComfirmSpawnPosition;
        }
        return;
    }

    IEnumerator FadeOut()
    {
        for (m_BackroundColor.a = 0.0f ; m_BackroundColor.a < 1; m_BackroundColor.a += Time.deltaTime * 0.5f)
        {
            m_Backround.color = m_BackroundColor;

            if (m_BackroundColor.a > 0.9f)
            {
                Destroy(this.gameObject);
                yield break;
            }
            yield return 0;
        }
        yield return 0;
    }
}
