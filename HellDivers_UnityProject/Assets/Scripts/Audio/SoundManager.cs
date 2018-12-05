using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public int Id;
    public AudioClip audioClip;
    public float volumeScale = 1;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public AudioSource Audio { get { return m_Audio; } }
    public Dictionary<int, SoundData> AudioClips { get { return m_ClipsMap; } }

    private AudioSource m_Audio;
    private Dictionary<int, SoundData> m_ClipsMap = new Dictionary<int, SoundData>();

    private void Awake()
    {
        m_Audio = this.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        m_Audio.Stop();
    }

    public void SetAudioClips(List<SoundData> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            SetAudioClip(datas[i]);
        }
    }

    public void SetAudioClip(SoundData data)
    {
        if (m_ClipsMap.ContainsKey(data.Id))
        {
            m_ClipsMap[data.Id] = data;
        }
        else
        {
            m_ClipsMap.Add(data.Id, data);
        }
    }

    public void SetAudioClips(Dictionary<int, SoundData> datas)
    {
        m_ClipsMap = datas;
    }

    public void PlayLoop(int Id, float volumeScale)
    {
        if (m_ClipsMap.ContainsKey(Id) == false) return;
        m_Audio.clip = m_ClipsMap[Id].audioClip;
        m_Audio.volume = volumeScale;
        m_Audio.loop = true;
        m_Audio.Play();
    }

    public void PlayLoop(int Id)
    {
        if (m_ClipsMap.ContainsKey(Id) == false) return;
        PlayLoop(Id, m_ClipsMap[Id].volumeScale);
    }

    public SoundManager PlayLoopInWorld(int Id, Vector3 pos, float volumeScale)
    {
        if (m_ClipsMap.ContainsKey(Id) == false) return null;
        GameObject go = new GameObject();
        go.transform.position = pos;
        SoundManager soundManager = go.AddComponent<SoundManager>();
        soundManager.SetAudioClips(m_ClipsMap);
        soundManager.Audio.volume = volumeScale;
        soundManager.Audio.spatialBlend = 1;
        soundManager.Audio.dopplerLevel = 0;
        soundManager.PlayLoop(Id);
        return soundManager;
    }

    public SoundManager PlayLoopInWorld(int Id, Vector3 pos)
    {
        if (m_ClipsMap.ContainsKey(Id) == false) return null;
        return PlayLoopInWorld(Id, pos, m_ClipsMap[Id].volumeScale);
    }

    public void PlayOnce(int Id, float volumeScale)
    {
        if (m_ClipsMap.ContainsKey(Id) == false) return;
        m_Audio.PlayOneShot(m_ClipsMap[Id].audioClip, volumeScale);
    }

    public void PlayOnce(int Id)
    {
        if (m_ClipsMap.ContainsKey(Id) == false) return;
        PlayOnce(Id, m_ClipsMap[Id].volumeScale);
    }

    public void PlayInWorld(int Id, Vector3 pos, float volumeScale)
    {
        if (m_ClipsMap.ContainsKey(Id) == false) return;
        float height = Camera.main.transform.position.y - pos.y;
        pos.y += height;
        pos.z += -Mathf.Tan((90 - Camera.main.transform.eulerAngles.x) * Mathf.Deg2Rad) * (height - 1f);
        AudioSource.PlayClipAtPoint(m_ClipsMap[Id].audioClip, pos, volumeScale);
    }

    public void PlayInWorld(int Id, Vector3 pos)
    {
        if (m_ClipsMap.ContainsKey(Id) == false) return;
        PlayInWorld(Id, pos, m_ClipsMap[Id].volumeScale);
    }

    public void Stop()
    {
        m_Audio.Stop();
        m_Audio.loop = false;
        m_Audio.volume = 1;
    }
}