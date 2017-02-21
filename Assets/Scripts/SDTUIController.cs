using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SDTUIController : Singleton<SDTUIController>
{
    public List<CanvasGroup> ActiveCanvases;

    private bool locked;

    void Awake()
    {
        Instance = this;
        ActiveCanvases = new List<CanvasGroup>();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void OpenCanvas(CanvasGroup canvas, bool pause = true, bool allowOthers = false, float revealTime = 0.0f)
    {
        if (locked) return;

        if (IsCanvasOpen(canvas))
        {
            CloseCanvas(canvas, pause, revealTime);
            return;
        }

        if (pause)
            TimeManager.Pause();

        if (!allowOthers)
        {
            Debug.Log("Don't allow others block");
            if(ActiveCanvases.Count > 0)
                foreach (CanvasGroup cg in ActiveCanvases)
                    CloseCanvas(cg);
            locked = true;
        }

        ActiveCanvases.Add(canvas);

        StartCoroutine(OpenCanvasOverTime(canvas, revealTime));
    }

    private IEnumerator OpenCanvasOverTime(CanvasGroup canvas, float time)
    {
        float alpha_change_per_frame = (1.0f / time) * Time.deltaTime;

        while (canvas.alpha < 1.0f)
        {
            canvas.alpha += alpha_change_per_frame;
            yield return null;
        }
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    public void CloseCanvas(CanvasGroup canvas, bool unpause = true, float hideTime = 0.0f)
    {
        if (!ActiveCanvases.Contains(canvas)) return;

        if (unpause)
            TimeManager.Unpause();

        ActiveCanvases.Remove(canvas);
        if (ActiveCanvases.Count <= 0)
            locked = false;

        StartCoroutine(CloseCanvasOverTime(canvas, hideTime));
    }

    private IEnumerator CloseCanvasOverTime(CanvasGroup canvas, float time)
    {
        float alpha_change_per_frame = (1.0f / time) * Time.deltaTime;

        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        while (canvas.alpha > 0.0f)
        {
            canvas.alpha -= alpha_change_per_frame;
            yield return null;
        }
    }

    public bool IsCanvasOpen(CanvasGroup canvas)
    {
        return canvas.alpha > 0 && canvas.interactable && canvas.blocksRaycasts;
    }
}