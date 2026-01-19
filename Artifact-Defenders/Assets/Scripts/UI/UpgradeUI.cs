using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hiển thị text khi nâng cấp (Upgrade Artifact)
/// Có hỗ trợ localization (đa ngôn ngữ)
/// </summary>
public class UpgradeUI : MonoBehaviour
{
    [Header("Localization Strings")]
    public LocalizableString notEnoughStones;
    public LocalizableString upgradeSuccessful;
    public LocalizableString towerMaxLevel;

    [Header("UI Settings")]
    public float messageDuration = 2f;

    private Text text;
    private Coroutine currentCoroutine;

    void Start()
    {
        text = GetComponent<Text>();
        text.enabled = false;
    }

    /// <summary>
    /// Hiển thị một thông báo cụ thể trong thời gian nhất định.
    /// </summary>
    public void ShowMessage(LocalizableString message)
    {
        if (message == null) return;

        // Hiển thị nội dung theo ngôn ngữ hiện tại
        text.text = message.GetString(Localization.currentLanguage);
        text.enabled = true;

        // Nếu đang hiển thị thông báo khác → dừng coroutine cũ
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        text.enabled = false;
    }

    // ==== Hàm tiện lợi để gọi trong code khác ====
    public void ShowNotEnoughStones() => ShowMessage(notEnoughStones);
    public void ShowUpgradeSuccessful() => ShowMessage(upgradeSuccessful);
    public void ShowTowerMaxLevel() => ShowMessage(towerMaxLevel);
}
