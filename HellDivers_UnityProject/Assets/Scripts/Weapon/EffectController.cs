using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour {

    [SerializeField] int m_ID;

    public void SetID(int i) { m_ID = i; }
    public void EffectStart()
    {
        m_Animator.SetTrigger("startTrigger");
    }
    #region Private Method
    private bool CheckState()
    {
        m_StateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        if (m_StateInfo.IsName("end") && m_StateInfo.normalizedTime > 0.8f) { return true; }
        else return false;
    }
    private void UnLoadObject()
    {
        ObjectPool.m_Instance.UnLoadObjectToPool(m_ID, this.gameObject);    
    }
    #endregion

    #region MonoBehaviors
    void Awake ()
    {
        m_Animator = GetComponent<Animator>();
        if (m_Animator == null) GetComponentInChildren<Animator>();
    }

    private void OnEnable() { EffectStart(); }

    private void FixedUpdate()
    {
        if (CheckState())
        {
            m_fTime += Time.fixedDeltaTime;
        }
        if (m_fTime > 0.1f)
        {
            m_fTime = 0;
            UnLoadObject();
        }
    }
    #endregion

    #region Private Field
    AnimatorStateInfo m_StateInfo;
    Animator m_Animator;
    float m_fTime;
    #endregion
}
