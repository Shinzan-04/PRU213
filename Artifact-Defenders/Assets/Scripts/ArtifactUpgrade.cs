using UnityEngine;

/// <summary>
/// Quản lý việc nâng cấp Artifact (trụ) bao gồm level, máu tối đa và hiệu ứng animation.
/// </summary>
public class ArtifactUpgrade : MonoBehaviour
{
    [Header("Animator & Artifact")]
    public Animator animator; // giữ để hiển thị animation level 1,2,3
    private Artifact artifact;
    private ArtifactLaserAttack laserAttack;

    [Header("Player Inventory")]
    public PlayerUpgradeInventory playerInv;

    [Header("Upgrade Settings")]
    [SerializeField] private int[] upgradeCosts = { 0, 20, 30 };
    [SerializeField] private int[] maxHealthByLevel = { 0, 100, 200, 300 };

    [Header("Upgrade UI")]
    public UpgradeUI upgradeUI;

    [Header("Upgrade Effect")]
    public GameObject upgradeEffectPrefab; // kéo vào Inspector prefab particle system

    private int level = 1;

    void Start()
    {
        level = Mathf.Clamp(level, 1, 3);

        artifact = GetComponent<Artifact>();
        laserAttack = GetComponent<ArtifactLaserAttack>();

        // Cập nhật máu max và animation level ban đầu
        UpdateLevelVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryUpgrade();
        }
    }

    void TryUpgrade()
    {
        if (level >= 3)
        {
            upgradeUI?.ShowTowerMaxLevel();
            return;
        }

        int cost = upgradeCosts[Mathf.Min(level, upgradeCosts.Length - 1)];

        if (!playerInv.UseStones(cost))
        {
            upgradeUI?.ShowNotEnoughStones();
            return;
        }

        // Nâng cấp level
        level = Mathf.Min(level + 1, 3);

        // Cập nhật animation, máu, laser
        UpdateLevelVisual();

        if (upgradeEffectPrefab != null)
        {
            Vector3 effectPos = transform.position; 
            GameObject effect = Instantiate(upgradeEffectPrefab, effectPos, Quaternion.identity);

            Destroy(effect, 3f); // tự hủy sau 3 giây
        }


        upgradeUI?.ShowUpgradeSuccessful();
    }

    private void UpdateLevelVisual()
    {
        if (animator != null)
            animator.SetInteger("level", level); // chuyển trực tiếp sang animation level mới

        if (artifact != null)
            artifact.SetMaxHealth(maxHealthByLevel[level]);

        if (laserAttack != null)
            laserAttack.enabled = (level >= 3);
    }

    // Gọi từ nút UI
    public void OnUpgradeButtonPressed()
    {
        TryUpgrade();
    }

}
