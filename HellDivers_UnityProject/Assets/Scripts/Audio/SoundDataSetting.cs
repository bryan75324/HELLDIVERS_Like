using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDataSetting", menuName = "Sound Data Setting", order = 1000)]
public class SoundDataSetting : ScriptableObject
{
    public List<SoundData> SoundDatas { get { return m_SoundDatas; } }

    [SerializeField] private List<SoundData> m_SoundDatas;
}