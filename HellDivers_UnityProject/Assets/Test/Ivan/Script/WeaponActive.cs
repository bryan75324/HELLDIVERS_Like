using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponActive
{
    void Shot(int OPtype);
    void Refill(int OPtype);
}

public class Weapon_Rifle : WeaponActive
{
    public bool m_bCooling = false;
    public bool m_bRefilling = false;

    private Transform m_tGunPos = GameObject.FindGameObjectWithTag("Gun").GetComponent<Transform>();
    private List<GameObject> m_Weapon_CurrentActives = new List<GameObject>();


    public void Shot(int OPtype)
    {
        if (Input.GetKeyDown(KeyCode.Space) && !m_bCooling && !m_bRefilling)
        {
            Debug.Log("Rifle");
            GameObject go = ObjectPool.m_Instance.LoadGameObjectFromPool(OPtype);
            if (go != null)
            {
                go.transform.position = m_tGunPos.position;
                go.transform.forward = m_tGunPos.forward;
                m_Weapon_CurrentActives.Add(go);
                go.SetActive(true);
                //m_bCooling = true;
            }
            else { Debug.Log("There is no ammo."); }
        }
        
    }
    public void Refill(int OPtype)
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            int iCount = m_Weapon_CurrentActives.Count;
            for (int i = 0; i < iCount; i++)
            {
                ObjectPool.m_Instance.UnLoadObjectToPool(OPtype, m_Weapon_CurrentActives[0]);
                m_Weapon_CurrentActives.RemoveAt(0);
            }
        }
    }
}

public class Weapon_ShotGun : WeaponActive
{
    public bool m_bCooling = false;
    public bool m_bRefilling = false;

    private Transform m_tGunPos = GameObject.FindGameObjectWithTag("Gun").GetComponent<Transform>();
    private List<GameObject> m_Weapon_CurrentActives = new List<GameObject>();

    public void Shot(int OPtype)
    {
        if (Input.GetKeyDown(KeyCode.Space) && !m_bCooling && !m_bRefilling)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject go = ObjectPool.m_Instance.LoadGameObjectFromPool(OPtype);
                if (go != null)
                {
                    go.transform.position = m_tGunPos.position;
                    go.transform.forward = m_tGunPos.forward * 0.3f;
                    go.transform.Rotate(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                    m_Weapon_CurrentActives.Add(go);
                    go.SetActive(true);
                    //m_bCooling = true;
                }
                else
                {
                    Debug.Log("There is no ammo.");
                    break;
                }
            }
        }
    }

    public void Refill(int OPtype)
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            int iCount = m_Weapon_CurrentActives.Count;
            for (int i = 0; i < iCount; i++)
            {
                ObjectPool.m_Instance.UnLoadObjectToPool(OPtype, m_Weapon_CurrentActives[0]);
                m_Weapon_CurrentActives.RemoveAt(0);
            }
        }
    }

}
