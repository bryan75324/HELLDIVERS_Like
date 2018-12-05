using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpurt : MonoBehaviour {

    //ParticleSystem m_ParticleSystem;
    private float m_Timer = 0.0f;
    private MobInfo m_MobInfo;
    private void OnEnable()
    {
        m_Timer = 0.0f;
    }
    public void Init(MobInfo data, Vector3 hitPoint)
    {
        Vector3 pos = this.transform.position;
        pos = data.m_Go.transform.position;

        switch(data.m_ID){
            case 3100:
                pos.y += 2.0f;
                m_MobInfo = data;
                data.m_Go.GetComponent<FishAI>().OnDeath += Release;
                break;
            case 3200:
                pos.y += 1.0f;
                m_MobInfo = data;
                data.m_Go.GetComponent<PatrolAI>().OnDeath += Release;
                break;
            case 3300:
                pos.y += 2.0f;
                m_MobInfo = data;
                data.m_Go.GetComponent<FishVariantAI>().OnDeath += Release;
                break;
            case 3400:
                pos.y += 3.0f;
                m_MobInfo = data;
                data.m_Go.GetComponent<TankAI>().OnDeath += Release;
                break;
            default:
                pos.y += 1.0f;
                break;
        }

        this.transform.position = pos;
        Vector3 dir = hitPoint - data.m_Go.transform.position;
        dir.y = 0.0f;
        this.transform.forward = dir;
        this.gameObject.SetActive(true);
        this.transform.SetParent(data.m_Go.transform);
    }

    public void Release()
    {
        switch (m_MobInfo.m_ID)
        {
            case 3100:
                m_MobInfo.m_Go.GetComponent<FishAI>().OnDeath -= Release;
                break;
            case 3200:
                m_MobInfo.m_Go.GetComponent<PatrolAI>().OnDeath -= Release;
                break;
            case 3300:
                m_MobInfo.m_Go.GetComponent<FishVariantAI>().OnDeath -= Release;
                break;
            case 3400:
                m_MobInfo.m_Go.GetComponent<TankAI>().OnDeath -= Release;
                break;
            default:
                break;
        }
        this.transform.parent = null;
    }
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        m_Timer += Time.deltaTime;
        if (m_Timer > 2.0f)
        {
            this.transform.parent = null;
            switch (m_MobInfo.m_ID)
            {
                case 3100:
                    m_MobInfo.m_Go.GetComponent<FishAI>().OnDeath -= Release;
                    ObjectPool.m_Instance.UnLoadObjectToPool(3003, this.gameObject);
                    ObjectPool.m_Instance.UnLoadObjectToPool(3004, this.gameObject);
                    break;
                case 3200:
                    m_MobInfo.m_Go.GetComponent<PatrolAI>().OnDeath -= Release;
                    ObjectPool.m_Instance.UnLoadObjectToPool(3003, this.gameObject);
                    ObjectPool.m_Instance.UnLoadObjectToPool(3004, this.gameObject);
                    break;
                case 3300:
                    m_MobInfo.m_Go.GetComponent<FishVariantAI>().OnDeath -= Release;
                    ObjectPool.m_Instance.UnLoadObjectToPool(3003, this.gameObject);
                    ObjectPool.m_Instance.UnLoadObjectToPool(3004, this.gameObject);
                    break;
                case 3400:
                    m_MobInfo.m_Go.GetComponent<TankAI>().OnDeath -= Release;
                    ObjectPool.m_Instance.UnLoadObjectToPool(3002, this.gameObject);
                    ObjectPool.m_Instance.UnLoadObjectToPool(3004, this.gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}
