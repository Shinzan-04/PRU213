using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [Header("Tuning")]
    public int maxMana = 100;
    public float regenRate = 5f; // Mana/giây

    [Header("Current Stats")]
    [SerializeField] int currentMana;

    void Awake()
    {
        currentMana = maxMana;
    }

    void Update()
    {
        // Hồi Mana (Regen)
        if (currentMana < maxMana)
        {
            float newMana = currentMana + regenRate * Time.deltaTime;
            currentMana = Mathf.Min(maxMana, Mathf.RoundToInt(newMana));
        }
    }

    // Hàm public để tiêu tốn Mana
    public bool TryUseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            Debug.Log($"Mana used: {amount}. Remaining Mana: {currentMana}");
            return true;
        }

        Debug.Log("Not enough Mana!");
        return false;
    }

    // Hàm public để hồi Mana TỪ Đòn Đánh Thường
    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        Debug.Log($"Mana restored: {amount}. Current Mana: {currentMana}");
    }

    // GETTER QUAN TRỌNG CHO UI
    public int GetCurrentMana() => currentMana;
    public int GetMaxMana() => maxMana;
}