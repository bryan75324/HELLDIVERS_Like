using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeInfo {

    public int ID { private set ; get ; }
    public int Type { private set; get; }
    public string Title { private set; get; }
    public float Damage { private set; get; }
    public float Timer { private set; get; }
    public float Range { private set; get; }
    public int MaxCount { private set; get; }

    public void SetID(int id) { ID = id; }
    public void SetType(int type) { Type = type; }
    public void SetTitle(string title) { Title = title; }
    public void SetDamage(float damage) { Damage = damage; }
    public void SetTimer(float timer) { Timer = timer; }
    public void SetRange(float range) { Range = range; }
    public void SetMaxCount(int count) { MaxCount = count; }

    public float Force
    {
        get { return m_fForce; }
        set
        {
            if (m_fForce > 12) m_fForce = 12;
            else m_fForce = value;
        }
    }
    public float Gravity { get { return -9.8f; } }
    public float FallingTime;
    protected float m_fForce = 1;

    public void CopyTo(GrenadeInfo other)
    {
        other.SetID(this.ID);
        other.SetType(this.Type);
        other.SetTitle(this.Title);
        other.SetDamage(this.Damage);
        other.SetTimer(this.Timer);
        other.SetRange(this.Range);
        other.SetMaxCount(this.MaxCount);
        other.Force = this.Force;
    }

}

