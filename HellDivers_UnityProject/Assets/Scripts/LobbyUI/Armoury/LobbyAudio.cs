using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LobbyAudio : MonoBehaviour {

    [SerializeField] AudioSource m_Audio_Select;
    [SerializeField] AudioSource m_Audio_Click;
    
    // Use this for initialization
    void Awake () {
	}
	
    public void PlaySelectSound(float volume)
    {
        if (m_Audio_Select == null) return;
        SetRadomPitch(m_Audio_Select);
        m_Audio_Select.PlayOneShot(m_Audio_Select.clip, volume);
    }

    public void PlayClickSound(float volume)
    {
        if (m_Audio_Click == null) return;
        SetRadomPitch(m_Audio_Click);
        m_Audio_Click.PlayOneShot(m_Audio_Click.clip, volume);
    }
    public void StopSound()
    {
        m_Audio_Click.Stop();
        m_Audio_Select.Stop();
    }
    private void SetRadomPitch(AudioSource audio)
    {
        audio.pitch = Random.Range(0.95f, 1.05f);
    }

}
