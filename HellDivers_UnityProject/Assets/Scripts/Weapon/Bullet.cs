﻿///2018.09.10
///Ivan.CC
///
/// Bullet behaviour.
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField] private eWeaponType m_Type;
    
    //Bullet's speed
    private float m_fSpeed = 100;
    private float m_fRange;

    //Renderer m_bullet;
    //========================================================================
    void Start () {
        //m_bullet = this.gameObject.GetComponent<Renderer>();
        m_fRange = GameData.Instance.WeaponInfoTable[(int)m_Type].Range;
    }

    // Update is called once per frame
    void Update () {
        StartCoroutine(BulletDeath());
    }

    private void OnTriggerEnter(Collider other)
    {
        //m_bullet.enabled = false;
    }

    IEnumerator BulletDeath()
    {
        this.transform.position = this.transform.position + this.transform.forward * Time.deltaTime * m_fSpeed;
        yield return new WaitForSeconds(m_fRange/m_fSpeed);
        ObjectPool.m_Instance.UnLoadObjectToPool((int)m_Type + 100, this.gameObject);
    }


}
