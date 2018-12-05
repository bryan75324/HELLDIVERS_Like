using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public class GameObjectData
    {
        public GameObject m_go;
        public bool m_bUse;
    }

    static public ObjectPool m_Instance;

    private Dictionary<int, List<GameObjectData>> m_dObjectDitc;

    public void Init()
    {
        m_Instance = this;
        m_dObjectDitc = new Dictionary<int, List<GameObjectData>>();
    }

    public void InitGameObjects(Object o, int iCount, int iType)
    {
        List<GameObjectData> pList;

        if (m_dObjectDitc.ContainsKey(iType) == false)
        {
            pList = new List<GameObjectData>();
            m_dObjectDitc.Add(iType, pList);
        }
        else
        {
            pList = m_dObjectDitc[iType];
            //need safer
        }
        for (int i = 0; i < iCount; i++)
        {
            GameObjectData gData = new GameObjectData();
            gData.m_go = GameObject.Instantiate(o) as GameObject;
            SetParent(iType, gData.m_go);
            gData.m_bUse = false;
            gData.m_go.SetActive(false);
            pList.Add(gData);
        }
    }

    public void RemoveObjectFromPool(int iType)
    {
        if (m_dObjectDitc.ContainsKey(iType) == false) { return; }

        m_dObjectDitc[iType].Clear();
        m_dObjectDitc.Remove(iType);
    }

    public GameObject LoadGameObjectFromPool(int iType)
    {
        if (m_dObjectDitc.ContainsKey(iType) == false) { return null; }
        List<GameObjectData> pList;
        pList = m_dObjectDitc[iType];
        GameObject rgo = null;
        for (int i = 0; i < pList.Count; i++)
        {
            if (pList[i].m_bUse == false)
            {
                pList[i].m_bUse = true;
                rgo = pList[i].m_go;
                break;
            }
        }
        return rgo;
    }

    public void UnLoadObjectToPool(int iType, GameObject go)
    {
        if (m_dObjectDitc.ContainsKey(iType) == false) { return; }
        List<GameObjectData> pList;
        pList = m_dObjectDitc[iType];
        for (int i = 0; i < pList.Count; i++)
        {
            if (pList[i].m_go == go)
            {
                go.SetActive(false);
                pList[i].m_bUse = false;
                break;
            }
        }
    }

    private void SetParent(int type, GameObject go)
    {
        GameObject parent;
        char c = type.ToString()[0];
        int i = int.Parse(c.ToString());
        switch (i)
        {
            case 1:
                parent = GameObject.Find("Bullet");
                if (parent == null) parent = new GameObject("Bullet");
                go.transform.parent = parent.transform;
                break;

            case 2:
                parent = GameObject.Find("Stratagem");
                if (parent == null) parent = new GameObject("Stratagem");
                go.transform.parent = parent.transform;
                break;

            case 3:
                parent = GameObject.Find("Enemies");
                if (parent == null) parent = new GameObject("Enemies");
                go.transform.parent = parent.transform;
                break;

            case 4:
                parent = GameObject.Find("Grenades");
                if (parent == null) parent = new GameObject("Grenades");
                go.transform.parent = parent.transform;
                break;

            case 9:
                parent = GameObject.Find("Point");
                if (parent == null) parent = new GameObject("Point");
                go.transform.SetParent(parent.transform);
                break;

            default:
                parent = GameObject.Find("Others");
                if (parent == null) parent = new GameObject("Others");
                go.transform.parent = parent.transform;
                break;
        }
    }
}