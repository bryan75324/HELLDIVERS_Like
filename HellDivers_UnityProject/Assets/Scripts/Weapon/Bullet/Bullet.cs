///2018.09.10
///Ivan.CC
///
/// Bullet behaviour.
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IDamager
{
    #region SerializeField

    [Header("== Bullet Info ==")]
    [SerializeField] private eWeaponType m_Type;

    [SerializeField] private int m_ID;
    [SerializeField] private float m_fSpeed = 100;

    #endregion SerializeField

    #region Mnonbehaviors

    public void SetID(int i) { m_ID = i; }

    private void Start()
    {
        m_fRange = GameData.Instance.WeaponInfoTable[m_ID].Range;
        m_fDamage = GameData.Instance.WeaponInfoTable[m_ID].Damage;
        m_fNextPosDis = Time.fixedDeltaTime * m_fSpeed;
    }

    private void FixedUpdate()
    {
        m_Time += Time.fixedDeltaTime;
        if (m_Time <= m_fRange / m_fSpeed)
        {
            Detect();
            this.transform.position = this.transform.position + this.transform.forward * m_fNextPosDis;
        }
        else
        {
            BulletDeath();
        }
    }

    #endregion Mnonbehaviors

    #region Bullet Method

    //Detect if bullet hit mob's, mob's sheld, or obstacle
    private void Detect()
    {
        RaycastHit rh;
        GameObject go = null;
        IDamageable target = null;
        if (Physics.Raycast(transform.position, transform.forward, out rh, m_fNextPosDis, 1 << LayerMask.NameToLayer("Enemies")))
        {
            go = rh.collider.gameObject;
            target = go.GetComponent<IDamageable>();
            if (m_Target != go)
            {
                if (m_ID == 1901) target.TakeDamage(m_fDamage, rh.point);
                else target.TakeDamage(this, rh.point);
                m_Target = go;
            }
            if (GameData.Instance.WeaponInfoTable[m_ID].Type == 3 || GameData.Instance.WeaponInfoTable[m_ID].Type == 5)
            {
                PlayHitEffect(rh.normal, rh.point, 30);
            }
            else
            {
                PlayHitEffect(rh.normal, rh.point, 10);
                BulletDeath();
            }
        }
        else if (Physics.Raycast(transform.position, transform.forward, out rh, m_fNextPosDis, 1 << LayerMask.NameToLayer("Battle")))
        {
            RaycastHit rh2;
            if (Physics.Raycast(transform.position, transform.forward, out rh2, m_fNextPosDis * 2, 1 << LayerMask.NameToLayer("Enemies")))
            {
                go = rh2.collider.gameObject;
                target = go.GetComponent<IDamageable>();
                if (m_Target != go)
                {
                    if (m_ID == 1901) target.TakeDamage(m_fDamage, rh2.point);
                    else target.TakeDamage(this, rh2.point);                    
                    m_Target = go;
                }
                if (GameData.Instance.WeaponInfoTable[m_ID].Type == 3 || GameData.Instance.WeaponInfoTable[m_ID].Type == 5)
                {
                    PlayHitEffect(rh2.normal, rh2.point, 30);
                }
                else
                {
                    PlayHitEffect(rh2.normal, rh2.point, 10);
                    BulletDeath();
                }
            }
            else
            {
                go = rh.collider.gameObject.transform.parent.gameObject;

                target = go.GetComponent<IDamageable>();
                if (m_Target != go)
                {
                    if (m_ID == 1901) target.TakeDamage(m_fDamage, rh2.point);
                    else target.TakeDamage(this, rh.point);
                    m_Target = go;
                }
                if (GameData.Instance.WeaponInfoTable[m_ID].Type != 3 && GameData.Instance.WeaponInfoTable[m_ID].Type != 5)
                {
                    BulletDeath();
                }
            }
        }
        else if (Physics.Raycast(transform.position, transform.forward, out rh, m_fNextPosDis, 1 << LayerMask.NameToLayer("Player")))
        {
            go = rh.collider.gameObject;
            target = go.GetComponent<IDamageable>();
            if (m_Target != go)
            {
                target.TakeDamage(m_fDamage *.5f, rh.point);
                m_Target = go;
            }
            if (GameData.Instance.WeaponInfoTable[m_ID].Type != 3 && GameData.Instance.WeaponInfoTable[m_ID].Type != 5)
            {
                PlayHitEffect(rh.normal, rh.point, 30);
                BulletDeath();
            }
        }
        else if (Physics.Raycast(transform.position, transform.forward, out rh, m_fNextPosDis, 1 << LayerMask.NameToLayer("Obstcale")))
        {
            PlayHitEffect(rh.normal, rh.point, 20);
            PlayHitSound(997);
            BulletDeath();
        }
    }

    //Play hit effet
    private void PlayHitEffect(Vector3 face, Vector3 pos, int id)
    {
        GameObject go = ObjectPool.m_Instance.LoadGameObjectFromPool(id);
        go.transform.forward = face;
        go.transform.position = pos;
        go.SetActive(true);
        go.GetComponent<EffectController>().EffectStart();
    }

    private void PlayHitSound(int id)
    {
        GameObject go = ObjectPool.m_Instance.LoadGameObjectFromPool(id);
        if (go == null) return;
        go.SetActive(true);
    }

    //Bullet play death and reset private field
    private void BulletDeath()
    {
        m_Target = null;
        m_Time = 0;
        ObjectPool.m_Instance.UnLoadObjectToPool(m_ID, this.gameObject);
    }

    public void SetPlayer(Player player)
    {
        m_BulletPlayer = player;
    }

    #endregion Bullet Method

    

    #region Private Field

    private GameObject m_Target;
    private float m_fNextPosDis;
    private float m_fRange;
    private float m_Time;
    private float m_fDamage;
    private Player m_BulletPlayer;


    public Player Damager { get { return m_BulletPlayer; } }
    public float Damage { get { return m_fDamage; } }

    #endregion Private Field
}