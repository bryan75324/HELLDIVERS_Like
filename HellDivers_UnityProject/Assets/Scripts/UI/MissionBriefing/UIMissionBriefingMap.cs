﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionBriefingMap : MonoBehaviour
{
    public Transform MapPointRoot { get { return m_MapPointRoot; } }

    [SerializeField] private Transform m_MapPointRoot;
    [SerializeField] private UIMissionBriefingConcentric m_Concentric;
    [SerializeField] private UIMissionMapPoint m_SpawnPointPrefab;
    [SerializeField] private UIMissionMapPoint m_TowerPointPrefab;

    public UIMissionBriefingConcentric Concentric { get { return m_Concentric; } }

    #region Variable
    private Vector3 m_WorldOrigin;
    private UIMissionMapPoint m_Target;
    private float m_MapWidth = 544.0f;
    private float m_MapHeight = 720.0f;

    private List<UIMissionMapPoint> m_PointList = new List<UIMissionMapPoint>();
    #endregion
    
    // Use this for initialization
    void Awake()
    {
        //m_WorldOrigin = MapInfo.Instance.MapOrigin.position;
        m_WorldOrigin = new Vector3(49.9f, 54.6f, 255.4f);
    }

    private void Update()
    {
        foreach (UIMissionMapPoint mapPoint in m_PointList)
        {
            if(Vector3.Distance(mapPoint.transform.position, m_Concentric.transform.position) < 20f)
            {
                m_Target = mapPoint;
                mapPoint.Highlight();
            }
            else
            {
                if (mapPoint == m_Target) m_Target = null;
                mapPoint.Normal();
            }
        }
    }

    public void AddPointPrefab(GameObject target, eMapPointType type)
    {
        UIMissionMapPoint mapPoint = null;
        switch (type)
        {
            case eMapPointType.MISSIONTOWER:
                mapPoint = Instantiate(m_TowerPointPrefab, m_MapPointRoot);
                mapPoint.Init(target, type);
                break;
            case eMapPointType.SPAWNPOINT:
                mapPoint = Instantiate(m_SpawnPointPrefab, m_MapPointRoot);
                mapPoint.Init(target, type);
                m_PointList.Add(mapPoint);
                break;
        }

        if (mapPoint != null) CalculatePosition(target.transform.position, ref mapPoint);
    }

    private void CalculatePosition(Vector3 target,ref UIMissionMapPoint mapPoint)
    {
        Vector3 m_Dir = target - m_WorldOrigin;
        float mapWidth = m_MapPointRoot.GetComponent<RectTransform>().sizeDelta.x;
        float mapHeight = m_MapPointRoot.GetComponent<RectTransform>().sizeDelta.y;

        Vector3 m_Pos = mapPoint.transform.localPosition;
        m_Pos.x = m_Dir.x * mapWidth / m_MapWidth - mapWidth * 0.5f;
        m_Pos.y = m_Dir.z * mapHeight / m_MapHeight - mapHeight * 0.5f;
        mapPoint.transform.localPosition = m_Pos;
    }

    public bool ComfirmSpawnPosition()
    {
        if (m_Target == null) return false;
        if (m_Target.SpawnPoint == null) return false;
        GameMain.Instance.GameStart(m_Target.SpawnPoint.transform);
        return true;
    }
}
