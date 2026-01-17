using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController:MonoBehaviour
{
    [Header("Visuals (Optional)")]
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI priceText;
   
    private void Awake()
    {
        if (progressBar == null)
            Debug.LogWarning("ProgressBarController: progressBar is not assigned.");
        if (priceText == null)
            Debug.LogWarning("ProgressBarController: priceText is not assigned.");
    }

    public void SetProgress(float fraction)
    {
        if (progressBar != null)
            progressBar.fillAmount = Mathf.Clamp01(fraction);
    }
    public void SetPriceText(int amount)
    {
        if (priceText != null)
            priceText.text = amount.ToString();
    }
}