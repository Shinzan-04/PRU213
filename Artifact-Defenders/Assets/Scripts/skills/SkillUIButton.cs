using UnityEngine;

public class SkillUIButton : MonoBehaviour
{
    public SkillManager skillManager;

    public void DashButton()
    {
        Vector2 dir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        skillManager.dash?.TryUse(dir);
    }

    public void SpinButton()
    {
        skillManager.spin?.TryUse();
    }

    public void BoomerangButton()
    {
        skillManager.boomerang?.TryUse();
    }
}
