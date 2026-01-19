using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script based animation for the Player
/// </summary>
public class PlayerSpriteAnim : MonoBehaviour
{
    public Sprite[] idleSprites;
    public Sprite[] runSprites;
    public Sprite[] attackSprites;
    public float frameTime = 0.1f;
    private float timer = 0;
    private int frame;
    private SpriteRenderer spriteRenderer;
    private Sprite[] currentAnim;
    private PlayerMovement playerMovement;
    private bool attacking = false;
    private float attackEndTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        currentAnim = idleSprites;
    }

    void Update()
    {
        if (attacking && Time.time > attackEndTime)
        {
            attacking = false;
            frame = 0; // reset frame
        }
        UpdateCurrentAnim();
        if (Time.time > timer)
        {
            if (currentAnim == null || currentAnim.Length == 0) return;

            // Nếu đang attack và đã hết frame cuối cùng → giữ nguyên frame cuối
            if (attacking && frame >= currentAnim.Length)
            {
                spriteRenderer.sprite = currentAnim[currentAnim.Length - 1];
                return;
            }
            // đổi sprite frame tiếp theo
            spriteRenderer.sprite = currentAnim[frame % currentAnim.Length];
            frame++;
            timer = Time.time + frameTime;
        }
    }

    void UpdateCurrentAnim()
    {
        if (attacking) // từ PlayAttack()
        {
            currentAnim = attackSprites;
        }
        else if (playerMovement.IsHarvesting())
        {
            currentAnim = idleSprites;
        }
        else if (playerMovement.IsAttacking())
        {
            currentAnim = attackSprites;
        }
        else if (playerMovement.GetVelocity().sqrMagnitude < 0.01f)
        {
            currentAnim = idleSprites;
        }
        else
        {
            currentAnim = runSprites;
        }
    }
    /// <summary>
    /// Gọi từ PlayerSlash để chơi animation Attack trong khoảng duration (s)
    /// </summary>
    public void PlayAttack(float duration)
    {
        attacking = true;
        attackEndTime = Time.time + duration;
        frame = 0;
    }

    public bool IsAttacking()
    {
        return attacking;
    }
}
