using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtifactDefense : MonoBehaviour
{
    // CÁC THÔNG SỐ CÓ THỂ NÂNG CẤP
    [Header("Laser Stats")]
    [SerializeField] private float attackRange = 5f; // Phạm vi kích hoạt
    [SerializeField] private float fireRate = 0.5f;   // Tần suất bắn (2 lần/giây)
    [SerializeField] private float damage = 2f;      // Sát thương cơ bản
    [SerializeField] private LayerMask targetMask;  // Layer của quái vật

    private float fireCountdown = 0f;
    private Transform target; // Mục tiêu hiện tại
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.enabled = false; // Ẩn Laser ban đầu
        }

        // Gọi hàm tìm kiếm mục tiêu lặp lại mỗi 0.25 giây
        InvokeRepeating("UpdateTarget", 0f, 0.25f);
    }

    void Update()
    {
        // 1. Kiểm tra thời gian bắn
        if (fireCountdown > 0f)
        {
            fireCountdown -= Time.deltaTime;
        }

        if (target == null)
        {
            if (lr != null) lr.enabled = false; // Tắt laser nếu không có mục tiêu
            return;
        }

        // 3. Bắn khi đủ thời gian
        if (fireCountdown <= 0f)
        {
            ShootLaser();
            fireCountdown = 1f / fireRate; // Reset thời gian chờ
        }
    }

    void UpdateTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, targetMask);

        if (colliders.Length == 0)
        {
            target = null;
            return;
        }

        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        Transform lowestHealthEnemy = null;
        float lowestHealth = Mathf.Infinity;

        foreach (Collider2D col in colliders)
        {
            // 🔥 KHẮC PHỤC: Sử dụng EnemyHealth và truy cập thuộc tính 'current'
            EnemyHealth enemyHealth = col.GetComponent<EnemyHealth>();

            // Dù 'current' là int, chúng ta dùng float để so sánh
            if (enemyHealth == null) continue;

            // Tìm Gần Nhất
            float distanceToEnemy = Vector2.Distance(transform.position, col.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = col.transform;
            }

            // Tìm Máu Thấp Nhất (Truy cập thuộc tính 'current')
            // Chuyển int sang float để so sánh
            float currentHealth = (float)enemyHealth.current;

            if (currentHealth < lowestHealth)
            {
                lowestHealth = currentHealth;
                lowestHealthEnemy = col.transform;
            }
        }

        // THỰC HIỆN LOGIC ƯU TIÊN: MÁU THẤP NHẤT HOẶC GẦN NHẤT
        // Ưu tiên dọn dẹp quái vật sắp chết (máu <= 2)
        if (lowestHealth <= 2 && lowestHealthEnemy != null)
        {
            target = lowestHealthEnemy;
        }
        else
        {
            target = nearestEnemy;
        }
    }

    void ShootLaser()
    {
        // 🔥 KHẮC PHỤC: Sử dụng EnemyHealth và gọi hàm DamageEnemy
        EnemyHealth healthScript = target.GetComponent<EnemyHealth>();

        if (healthScript != null)
        {
            // GỌI HÀM NHẬN SÁT THƯƠNG TRÊN MỤC TIÊU
            // Sát thương Laser (float damage) phải được chuyển về int
            healthScript.DamageEnemy(Mathf.CeilToInt(damage));
        }

        // HIỂN THỊ LASER
        if (lr != null)
        {
            lr.enabled = true;
            // 🔥 THÊM Z VÀO VỊ TRÍ GỐC LASER
            Vector3 startPos = transform.position + Vector3.up * 1.5f;
            startPos.z = -1f; // Đẩy Laser ra phía trước (Z=-1 hoặc Z=-2)

            Vector3 targetPos = target.position;
            targetPos.z = -1f; // Đẩy điểm cuối ra phía trước

            lr.SetPosition(0, startPos);
            lr.SetPosition(1, targetPos);
        }

        // Tắt Laser sau một khoảng thời gian ngắn để tạo hiệu ứng "bắn"
        StartCoroutine(DisableLaserAfterDelay());
    }

    // Tắt Laser ngay lập tức sau khi bắn
    IEnumerator DisableLaserAfterDelay()
    {
        // Chờ 0.05 giây để tia sáng xuất hiện
        yield return new WaitForSeconds(0.05f);
        if (lr != null)
        {
            lr.enabled = false;
        }
    }

    // Vẽ Phạm vi Tấn công trong Editor (Chỉ để debug)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}