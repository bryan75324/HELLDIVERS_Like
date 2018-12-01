using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

    [SerializeField] private float m_Speed = 30.0f;
    [SerializeField] private float m_LifeTime = 3.0f;
    private float m_Timer = 0.0f;
    private float m_NextPosDis;

    public GameObject m_Effect;
    private Animator m_Animator;
    // Use this for initialization
    private void OnEnable()
    {
        if (m_Animator == null) return;
        m_Timer = 0.0f;
        m_Animator.SetTrigger("startTrigger");
    }
    void Start()
    {
        m_NextPosDis = Time.fixedDeltaTime * m_Speed;
        m_Animator = m_Effect.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= m_LifeTime)
        {
            ObjectPool.m_Instance.UnLoadObjectToPool(3402, this.gameObject);
        }
        Detect();
        this.transform.position = this.transform.position + this.transform.forward * m_Speed * Time.deltaTime;
    }

    private void Detect()
    {
        RaycastHit rh;
        List<Player> pList = InGamePlayerManager.Instance.Players;
        if (pList != null && pList.Count > 0)
        {
            for (int i = 0; i < pList.Count; i++)
            {
                if (pList[i].IsDead) continue;
                float Dist = (pList[i].transform.position - this.transform.position).magnitude;
                if (Dist <= 2.0f)
                {
                    BulletDeath();
                    break;
                }
            }
        }
        if (Physics.Raycast(transform.position, transform.forward, out rh, m_NextPosDis, 1 << LayerMask.NameToLayer("Obstcale")))
        {
            BulletDeath();
        }
    }

    private void BulletDeath()
    {
        GameObject go = ObjectPool.m_Instance.LoadGameObjectFromPool(3403);
        go.transform.position = this.transform.position;
        go.SetActive(true);
        m_Animator.SetTrigger("endTrigger");
        ObjectPool.m_Instance.UnLoadObjectToPool(3402, this.gameObject);
    }
}
