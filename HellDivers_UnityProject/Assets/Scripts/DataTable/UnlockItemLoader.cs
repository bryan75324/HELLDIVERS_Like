using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockItemLoader {
    
    public static Dictionary<int, List<int>> LoadData (string filePath)
    {
        Dictionary<int, List<int>> unlockLevel = new Dictionary<int, List<int>>();
        if(_LoadDataBase(filePath, ref unlockLevel))
        {
            Debug.Log("UnlockInfo has been load.");
            return unlockLevel;
        }
        else
        {
            Debug.Log("UnlockInfo load failed.");
            return null;
        }
    }

    private static bool _LoadDataBase(string tablePath, ref Dictionary<int, List<int>> Info)
    {
        Info.Clear();
        TextAsset datas = Resources.Load<TextAsset>(tablePath);
        if (datas != null)
        {
            string[] lines = datas.text.Split('\n');
            for (int i = 1; i < lines.Length -1; i++)
            {
                string[] LevelInfo = lines[i].Split(',');
                int level = int.Parse(LevelInfo[1]);
                int itemID = int.Parse(LevelInfo[0]);
                if (!Info.ContainsKey(level))
                {
                    List<int> pList = new List<int>();
                    pList.Add(itemID);
                    Info.Add(level, pList);
                }
                else
                {
                    Info[level].Add(itemID);
                }
            }
            return true;
        }
        return false;
    }

}
