using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class ProgressBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Foreground;
    public Gradient BarGradient;
    public Text BarText;

    private Vector2 maxSize;
    private float currentProgress;
    private bool revealOnMouseOver;

    void Awake()
    {
        maxSize = GetComponent<RectTransform>().sizeDelta;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void SetProgress(float progress)
    {
        currentProgress = Mathf.Clamp(progress, 0.0f, 1.0f);

        Foreground.rectTransform.sizeDelta = new Vector2(currentProgress * maxSize.x, 0.0f);
        Foreground.color = BarGradient.Evaluate(currentProgress);
    }

    public void SetBarText(string text, bool reveal)
    {
        BarText.text = text ?? "";
        revealOnMouseOver = reveal;
        if (!reveal)
            BarText.gameObject.SetActive(!string.IsNullOrEmpty(text));
        else
            BarText.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (revealOnMouseOver)
            BarText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (revealOnMouseOver)
            BarText.gameObject.SetActive(false);
    }
}