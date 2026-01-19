using UnityEngine;
using UnityEngine.UI;

public class PlayerAttackUI : MonoBehaviour
{
    [SerializeField] Text attackText; // Legacy Text
    private int currentAttack = -1;

    void Start()
    {
        var slash = FindObjectOfType<PlayerSlash>();
        if (slash != null)
        {
            Debug.Log("✅ Found PlayerSlash: " + slash.name);
            UpdateAttackText(slash.damage);
        }
        else
        {
            Debug.Log("❌ PlayerSlash not found!");
            attackText.text = "Attack: ???";
        }
    }

    public void UpdateAttackText(int attack)
    {
        // Cập nhật chỉ khi giá trị thay đổi
        if (attack != currentAttack)
        {
            currentAttack = attack;
            attackText.text = "Attack: " + currentAttack;
        }
    }
}
