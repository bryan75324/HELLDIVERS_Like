using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade_Mine : Grenades
{
    [SerializeField] Animator BombAnimator;
    bool bEnd = false;


    protected override void CountDown()
    {
        RaycastHit rh;
        m_fDamageTime += Time.fixedDeltaTime;
        if (!bEnd)
        {
            if (m_fDamageTime <= Time.fixedDeltaTime)
            {
                this.GetComponent<MeshRenderer>().enabled = false;
                BombAnimator.SetTrigger("startTrigger");
                GameObject sound = ObjectPool.m_Instance.LoadGameObjectFromPool(grenadeInfo.ID * 10 + 2);
                sound.SetActive(true);
            }
            if (Physics.SphereCast(transform.position - Vector3.up, 2, Vector3.up, out rh, 100, 1 << LayerMask.NameToLayer("Enemies") | 1 << LayerMask.NameToLayer("Player")) || m_fDamageTime > grenadeInfo.Timer)
            {
                Debug.Log(rh.point);
                BombAnimator.SetTrigger("endTrigger");
                GameObject effect = ObjectPool.m_Instance.LoadGameObjectFromPool(grenadeInfo.ID * 10 + 1);
                GameObject sound = ObjectPool.m_Instance.LoadGameObjectFromPool(grenadeInfo.ID * 10 + 3);
                effect.transform.position = sound.transform.position = this.transform.position;
                effect.transform.forward = this.transform.forward;
                effect.SetActive(true);
                sound.SetActive(true);
                Debug.Log("load");
                m_fDamageTime = 0;
                bEnd = true;
            }
        }
        if (bEnd && m_fDamageTime > 0.4f)
        {
            DoDamage(this.transform.position);
            m_fDamageTime = 0;
            grenadeInfo.Force = 1;
            ObjectPool.m_Instance.UnLoadObjectToPool(grenadeInfo.ID, this.gameObject);
            m_bCounting = false;
            bEnd = false;
        }
    }

}
