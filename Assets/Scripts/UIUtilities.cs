using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public static class UIUtilities
{
    public static void ActivateCanvasGroup(CanvasGroup group)
    {
        if (group == null) return;

        group.alpha = 1;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public static void DeactivateCanvasGroup(CanvasGroup group)
    {
        if (group == null) return;

        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    public static T[] GetSiblingsOfType<T>(GameObject obj) where T : UIBehaviour
    {
        Transform parent = obj.transform.parent;
        Assert.IsNotNull(parent);

        GameObject[] siblings = new GameObject[parent.childCount - 1];

        int i1 = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).gameObject == obj)
                continue;
            siblings[i1] = parent.GetChild(i).gameObject;
            i1++;
        }
        
        return siblings.Where(x => x.GetComponent<T>() != null).Select(x => x.GetComponent<T>()).ToArray();
    }
}