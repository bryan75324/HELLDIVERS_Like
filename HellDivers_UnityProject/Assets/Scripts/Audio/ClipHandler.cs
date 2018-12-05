using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ClipHandler : MonoBehaviour {

    [SerializeField] int m_ID;
    [SerializeField] bool b_PitchRandom = true;
    [SerializeField] bool b_PlayInWorld = true;
    [SerializeField] AudioSource m_Audio;
    public void SetID (int i) { m_ID = i; }

    private void UnLoad() { ObjectPool.m_Instance.UnLoadObjectToPool(m_ID, this.gameObject); }

    IEnumerator WaitToUnLoad()
    {
        yield return new WaitForSeconds(m_Audio.clip.length);
        UnLoad();
    }

    public void PlayInWorld(Vector3 pos, float volumeScale)
    {
        float height = Camera.main.transform.position.y - pos.y;
        pos.y += height;
        pos.z += -Mathf.Tan((90 - Camera.main.transform.eulerAngles.x) * Mathf.Deg2Rad) * (height - 1f);
        AudioSource.PlayClipAtPoint(m_Audio.clip, pos, volumeScale);
    }


    private void SetPitch()
    {
        if (!b_PitchRandom) return;
        m_Audio.pitch = Random.Range(0.95f, 1.4f);
    }
    private void PlaySound()
    {
        if (!b_PlayInWorld) { m_Audio.PlayOneShot(m_Audio.clip); }
        else PlayInWorld(transform.position, 1);
    }


    private void OnEnable()
    {
        SetPitch();
        PlaySound();
        StartCoroutine(WaitToUnLoad());
    }

}
