using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeInventory : MonoBehaviour
{
    [Header("Stone Data")]
    public int upgradeStones = 0;
    public int maxStones = 10;

    [Header("UI References")]
    public Text stoneText;
    public GameObject floatingTextPrefab;
    public Transform floatingTextParent; // Canvas hoặc vị trí spawn

    void Start()
    {
        UpdateUI();
    }

    public void AddStone(int amount)
    {
        upgradeStones += amount;
        if (upgradeStones > maxStones)
            upgradeStones = maxStones;

        Debug.Log($"Nhặt được {amount} đá! Tổng: {upgradeStones}");
        UpdateUI();

        // Tạo hiệu ứng bay "+1"
        ShowFloatingText("+" + amount);
    }

    public bool UseStones(int amount)
    {
        if (upgradeStones >= amount)
        {
            upgradeStones -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (stoneText != null)
            stoneText.text = "x " + upgradeStones + " / " + maxStones;
    }

    void ShowFloatingText(string text)
    {
        if (floatingTextPrefab != null && floatingTextParent != null)
        {
            GameObject obj = Instantiate(floatingTextPrefab, floatingTextParent);
            obj.GetComponent<Text>().text = text;

            // Vị trí ngẫu nhiên nhẹ quanh icon
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(Random.Range(-20, 20), 0);

            Destroy(obj, 1f); // tự hủy sau 1s
        }
    }
}
