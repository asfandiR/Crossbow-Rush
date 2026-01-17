using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        GlobalEventManager.Instance.OnGoldChanged.AddListener(UpdateCoinDisplay);
    }

    private void OnDisable()
    {
        GlobalEventManager.Instance.OnGoldChanged.RemoveListener(UpdateCoinDisplay);
    }

    private void UpdateCoinDisplay(int newGoldAmount)
    {
        if (coinText != null)
        {
            coinText.text = newGoldAmount.ToString();
        }
    }
}