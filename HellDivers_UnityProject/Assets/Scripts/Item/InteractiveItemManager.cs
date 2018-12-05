using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveItemManager
{
    public static InteractiveItemManager Instance { get; private set; }
    private static Dictionary<string, List<IInteractable>> m_ItemMap;

    public void Init()
    {
        Instance = this;
        m_ItemMap = new Dictionary<string, List<IInteractable>>();
    }

    public void AddItem(IInteractable item)
    {
        if (m_ItemMap.ContainsKey(item.ID))
        {
            m_ItemMap[item.ID].Add(item);
        }
        else
        {
            List<IInteractable> itemList = new List<IInteractable>();
            itemList.Add(item);
            m_ItemMap.Add(item.ID, itemList);
        }
    }

    public void RemoveItem(IInteractable item)
    {
        if (m_ItemMap.ContainsKey(item.ID))
        {
            m_ItemMap[item.ID].Remove(item);
        }
    }

    public bool OnInteractive(Player player)
    {
        IInteractable nearestItem = GetNearestItem(player.transform.position);
        if (nearestItem != null)
        {
            nearestItem.OnInteract(player);
            return true;
        }
        return false;
    }

    private IInteractable GetNearestItem(Vector3 position)
    {
        if (m_ItemMap.Count <= 0) return null;

        IInteractable nearestItem = null;
        float nearestDist = float.MaxValue;
        foreach (KeyValuePair<string, List<IInteractable>> itemList in m_ItemMap)
        {
            List<IInteractable> currentList = itemList.Value;
            if (currentList.Count <= 0) continue;
            for (int i = 0; i < currentList.Count; i++)
            {
                IInteractable currentItem = currentList[i];

                float dist = Vector3.Distance(position, currentItem.Position); // Check item interactable radius.
                if (dist > currentItem.Radius) continue;

                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestItem = currentItem;
                }
            }
        }
        return nearestItem;
    }
}