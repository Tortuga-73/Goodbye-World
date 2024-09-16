using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private TextMeshProUGUI ammoText = default;
    private float maxAmmo;
    private float currentAmmo;

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
    }
    private void OnDisable()
    {
        PlayaStatusScript.OnDamage -= UpdateHealth;
        PlayaStatusScript.OnHeal -= UpdateHealth;
        Gun.OnShoot -= UpdateAmmo;
        Gun.OnReload -= UpdateAmmo;
        WeaponSwitch.OnWeaponSwap -= WeaponSwap;
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
}
