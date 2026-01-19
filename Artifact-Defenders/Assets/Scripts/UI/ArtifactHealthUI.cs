using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple script that updates a Slider with the Artifact health and shows the health number
/// </summary>
public class ArtifactHealthUI : MonoBehaviour
{
    Slider slider;
    [SerializeField] Artifact artifact;
    [SerializeField] Text healthText; // Thêm Text để hiển thị số máu

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = artifact.maxHealth;
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        slider.maxValue = artifact.maxHealth;
        slider.value = artifact.health;

        if (healthText != null)
            healthText.text = $"{artifact.health} / {artifact.maxHealth}";
    }
}
