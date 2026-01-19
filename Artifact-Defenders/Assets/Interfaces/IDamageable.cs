using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount, Vector2 hitFromWorld, float knockbackForce);
}