using UnityEngine;
using UnityEngine.UI;
//using TMPro; // Nếu bạn dùng TextMeshPro, nếu không thì dùng UnityEngine.UI.Text

public class ManaSliderUI : MonoBehaviour
{
    [Header("Data Source")]
    // Kéo thả component PlayerMana từ Player GameObject vào đây
    public PlayerMana playerMana;

    [Header("UI Elements")]
    // Kéo thả component Slider (của thanh Mana) vào đây
    public Slider manaSlider;

    // Kéo thả component Text/TextMeshProUGUI vào đây
    //public TextMeshProUGUI manaText; // Hoặc 'public Text manaText;' nếu không dùng TMPro
    public Text manaText;

    void Start()
    {
        if (playerMana == null || manaSlider == null)
        {
            Debug.LogError("ManaSliderUI: Missing PlayerMana or Slider component!");
            enabled = false;
            return;
        }

        // 1. Khởi tạo Max Value cho Slider
        manaSlider.maxValue = playerMana.GetMaxMana();

        // 2. Cập nhật giá trị ban đầu
        UpdateManaUI();
    }

    void Update()
    {
        // 3. Cập nhật UI mỗi frame
        UpdateManaUI();
    }

    private void UpdateManaUI()
    {
        int current = playerMana.GetCurrentMana();
        int max = playerMana.GetMaxMana();

        // Cập nhật Slider Value (Value tự động tính Fill Amount)
        manaSlider.value = current;

        // Cập nhật Text (ví dụ: "85/100")
        if (manaText != null)
        {
            manaText.text = $"{current}/{max}";
        }
    }
}