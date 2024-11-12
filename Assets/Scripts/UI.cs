using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private TextMeshProUGUI ammoText = default;
    [SerializeField] private TextMeshProUGUI scoreText = default;
    private float maxAmmo;
    private float currentAmmo;
    private float score = 0f;

    private void Start()
    {
        UpdateHealth(100f);
    }
    private void OnEnable()
    {
        PlayaStatusScript.OnDamage += UpdateHealth;
        PlayaStatusScript.OnHeal += UpdateHealth;
        Gun.OnShoot += UpdateAmmo;
        Gun.OnReload += UpdateAmmo;
        WeaponSwitch.OnWeaponSwap += WeaponSwap;
        Enemy.OnKilledEnemy += UpdateScore;
    }
    private void OnDisable()
    {
        PlayaStatusScript.OnDamage -= UpdateHealth;
        PlayaStatusScript.OnHeal -= UpdateHealth;
        Gun.OnShoot -= UpdateAmmo;
        Gun.OnReload -= UpdateAmmo;
        WeaponSwitch.OnWeaponSwap -= WeaponSwap;
        Enemy.OnKilledEnemy -= UpdateScore;
    }
    private void UpdateHealth(float currentHealth)
    {
        healthText.text = currentHealth.ToString("00");
    }
    private void UpdateAmmo(float cAmm)
    {
        currentAmmo = cAmm;
        AmmoText();
    }
    private void WeaponSwap(float cAmm, float mAmm)
    {
        currentAmmo = cAmm;
        maxAmmo = mAmm;
        AmmoText();
    }
    private void AmmoText()
    {
        ammoText.text = currentAmmo.ToString("00") + " / " + maxAmmo.ToString("00");
    }
    private void UpdateScore(float scoreChange)
    {
        score += scoreChange;
        scoreText.text = score.ToString("00");
    }
}
