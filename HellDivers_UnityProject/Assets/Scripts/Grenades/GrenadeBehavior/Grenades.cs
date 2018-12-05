using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenades : MonoBehaviour, IDamager{

    [SerializeField] EGrenades m_Type;
    [SerializeField] int m_ID;
    [SerializeField] protected float m_fAngle = 30;
    #region Private Field
    protected GrenadeInfo grenadeInfo;
    protected Vector3 m_vForward;
    protected Player m_player;
    protected bool m_bGround = true;
    protected bool m_bCounting;
    protected float m_fDamageTime;

    public Player Damager { get { return m_player; } }

    public float Damage { get { return grenadeInfo.Damage; } }

    #endregion

    public void SetInfo(GrenadeInfo info, Player player)
    {
        grenadeInfo = info;
        m_bGround = false;
        this.transform.SetParent(GameObject.Find("Grenades").transform);
        m_vForward = transform.forward;
        m_player = player;
    }

    #region Private function
    protected virtual void Falling(float force)
    {
        var v0 = force * Mathf.Sin(m_fAngle * Mathf.Deg2Rad);
        var v = v0 + grenadeInfo.Gravity * grenadeInfo.FallingTime;
        transform.position += transform.up * v * Time.fixedDeltaTime;
    }

    protected virtual void Moving(float force)
    {
        var vF = force * Mathf.Cos(m_fAngle * Mathf.Deg2Rad);
        transform.position += m_vForward * vF * Time.fixedDeltaTime;
        transform.Rotate(0, 30, 0);
    }

    protected virtual void DoDamage(Vector3 pos)
    {
        var enemies = Physics.OverlapSphere(pos, grenadeInfo.Range, 1<<LayerMask.NameToLayer("Enemies") | 1<< LayerMask.NameToLayer("Player"));
        foreach (var enemy in enemies)
        {
            IDamageable target = enemy.gameObject.GetComponent<IDamageable>();
            target.TakeDamage(this, enemy.transform.position);
        }
    }

    protected virtual bool GroundCheck()
    {
        RaycastHit rh;
        if (Physics.Raycast(transform.position, -transform.up, out rh, .3f, 1 << LayerMask.NameToLayer("Terrain")))
        {
            grenadeInfo.FallingTime = grenadeInfo.Force = 0;

            transform.position = rh.point /*+ Vector3.down * .5f*/;

            m_bCounting = true;
            return true;
        }
        return false;
    }

    protected virtual void CountDown()
    {
        m_fDamageTime += Time.fixedDeltaTime;
        if (m_fDamageTime <= Time.fixedDeltaTime)
        {
            GameObject effect = ObjectPool.m_Instance.LoadGameObjectFromPool(grenadeInfo.ID * 10 + 1);
            GameObject sound = ObjectPool.m_Instance.LoadGameObjectFromPool(grenadeInfo.ID * 10 + 2);
            effect.transform.position = sound.transform.position = transform.position;
            effect.transform.forward = transform.forward;
            effect.SetActive(true);
            sound.SetActive(true);
        }
        if (m_fDamageTime > grenadeInfo.Timer)
        {
            if (grenadeInfo.ID == 4004)
            {
                GameObject soundExplosion = ObjectPool.m_Instance.LoadGameObjectFromPool(grenadeInfo.ID * 10 + 3);
                soundExplosion.SetActive(true);
            }
            grenadeInfo.Force = 1;
            DoDamage(this.transform.position);
            m_fDamageTime = 0;
            ObjectPool.m_Instance.UnLoadObjectToPool(grenadeInfo.ID, this.gameObject);
            m_bCounting = false;
        }
    }

    #endregion
    protected virtual void FixedUpdate()
    {
        if (!m_bGround)
        {
            Falling(grenadeInfo.Force);
            Moving(grenadeInfo.Force);
            grenadeInfo.FallingTime += Time.fixedDeltaTime;
            m_bGround = GroundCheck();
        }
        if (m_bCounting)
        {
            CountDown();
        }
    }

    #region MonoBehaviors
    #endregion

}
