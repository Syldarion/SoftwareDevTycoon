using UnityEngine;
using System.Collections;

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
}