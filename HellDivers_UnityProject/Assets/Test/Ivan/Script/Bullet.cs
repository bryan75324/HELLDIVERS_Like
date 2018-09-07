    ///2018.09.02
///Ivan.CC
///
/// Bullet behaviour.
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField] private eWeaponType m_Type;
         
    private WeaponController m_WC;

    private List<float> m_WeaponData = new List<float>();

    private Vector3 m_vRegion;
    private Vector3 m_vOrigine;
    private BoxCollider m_Region;

    //Bullet's speed
    private float m_fSpeed;
    

 //========================================================================
    void Start () {
        m_WC = new WeaponController();
        m_WC.Init();
        m_WeaponData = m_WC.WeaponInfo(m_Type, m_WC.m_iLevel);
        m_vRegion = new Vector3(m_WeaponData[4], m_WeaponData[5], 0);
        m_vOrigine = new Vector3(1, 1, 2);
        m_Region = GetComponent<BoxCollider>();
        m_fSpeed = 100;
        
    }

    // Update is called once per frame
    void Update () {
        Translate();
        StartCoroutine(BulletDeath());
    }

    //Bullet translate 
    private void Translate()
    {
        this.transform.position = this.transform.position + this.transform.forward * Time.deltaTime * m_fSpeed;
        m_Region.size = m_Region.size + m_vRegion * Time.deltaTime;                 
    }

    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.SetActive(false);
        other.gameObject.SetActive(false);
    }

    IEnumerator BulletDeath()
    {
        yield return new WaitForSeconds(1);
        m_vRegion = m_vOrigine;
        this.gameObject.SetActive(false);
    }

}
