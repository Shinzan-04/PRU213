using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible of the player movement and managing the <see cref="SpriteRenderer.flipX"/> property
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public Joystick joystick;

    new Rigidbody2D rigidbody;
    Vector2 normVector;
    SpriteRenderer sprite;

    float timer;
    bool harvesting;

    bool attacking;
    float attackTimer;
    public Vector2 MoveDirection { get; private set; }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (harvesting && Time.time > timer)
            harvesting = false;

        if (attacking && Time.time > attackTimer)
            attacking = false;
        FlipSprite();
    }

    private void FlipSprite()
    {
        // Lấy hướng từ bàn phím trước
        float h = Input.GetAxisRaw("Horizontal");

        // Nếu joystick có input, dùng joystick thay
        if (joystick != null && Mathf.Abs(joystick.Horizontal) > 0.1f)
        {
            h = joystick.Horizontal;
        }

        if (h > 0.1f)
            sprite.flipX = false;
        else if (h < -0.1f)
            sprite.flipX = true;
    }

    void FixedUpdate()
    {
        if (harvesting || attacking)
        {
            rigidbody.linearVelocity = Vector2.zero;
        }
        else
        {
            // Lấy input từ bàn phím
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Nếu joystick có input, dùng joystick thay
            if (joystick != null)
            {
                if (Mathf.Abs(joystick.Horizontal) > 0.1f || Mathf.Abs(joystick.Vertical) > 0.1f)
                {
                    h = joystick.Horizontal;
                    v = joystick.Vertical;
                }
            }

            // Gán lại vector di chuyển (đã bao gồm joystick hoặc bàn phím)
            normVector = new Vector2(h, v);

            // Giữ hướng di chuyển mượt
            if (normVector.sqrMagnitude > 1)
                normVector = normVector.normalized;

            // Cập nhật vận tốc
            rigidbody.linearVelocity = normVector * movementSpeed;
        }
        MoveDirection = normVector;
    }


    public void HarvestStopMovement(float time)
    {
        harvesting = true;
        timer = Time.time + time;
    }

    public bool IsHarvesting()
    {
        return harvesting;
    }

    public bool IsAttacking()
    {
        return attacking;
    }

    public Vector2 GetVelocity()
    {
        return rigidbody.linearVelocity;
    }

    public void StopMovementForAttack(float time)
    {
        attacking = true;
        attackTimer = Time.time + time;
    }

}
