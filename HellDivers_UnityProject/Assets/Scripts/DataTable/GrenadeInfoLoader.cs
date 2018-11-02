﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeInfoLoader {

    public static Dictionary<int, GrenaderInfo> LoadData(string filePath)
    {
        Dictionary<int, GrenaderInfo> grenadeInfo = new Dictionary<int, GrenaderInfo>();
        if (_LoadDataBase(filePath, ref grenadeInfo) == true)
        {
            Debug.Log("GrenadeInfo has been load.");
            return grenadeInfo;
        }
        else
        {
            Debug.Log("GrenadeInfo load failed.");
            return null;
        }
    }

    private static bool _LoadDataBase(string tablePath, ref Dictionary<int, GrenaderInfo> Info)
    {
        Info.Clear();
        TextAsset datas = Resources.Load<TextAsset>(tablePath);
        if (datas != null)
        {
            string[] lines = datas.text.Split('\n');
            for (int i = 1; i < lines.Length - 1; i++)
            {
                string[] grenadeInfo = lines[i].Split(',');
                GrenaderInfo data = new GrenaderInfo();
                data.SetID(int.Parse(grenadeInfo[0]));
                data.SetType(int.Parse(grenadeInfo[1]));
                data.SetTitle(grenadeInfo[2]);
                data.SetDamage(float.Parse(grenadeInfo[3]));
                data.SetTimer(float.Parse(grenadeInfo[4]));
                data.SetRange(float.Parse(grenadeInfo[5]));
                Info.Add(data.ID, data);
            }
            return true;
        }
        return false;
    }


}
