using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
    public Image Foreground;
    public Gradient BarGradient;
    public Text BarText;

    private Vector2 maxSize;
    private float currentProgress;

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

    public void SetBarText(string text)
    {
        BarText.text = text ?? "";
        BarText.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }
}