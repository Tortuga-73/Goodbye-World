using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private TextMeshProUGUI ammoText = default;
    [SerializeField] private TextMeshProUGUI scoreText = default;
    [SerializeField] private TextMeshProUGUI waveText = default;
    [SerializeField] private TextMeshProUGUI killText = default;
    private float maxAmmo;
    private float currentAmmo;
    private int killCount;

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
        PlayaStatusScript.OnUpdateScore += UpdateScoreUI;
        WaveSpawner.OnWaveComplete += WaveText;
        Enemy.OnAddKill += KillText;
    }
    private void OnDisable()
    {
        PlayaStatusScript.OnDamage -= UpdateHealth;
        PlayaStatusScript.OnHeal -= UpdateHealth;
        Gun.OnShoot -= UpdateAmmo;
        Gun.OnReload -= UpdateAmmo;
        WeaponSwitch.OnWeaponSwap -= WeaponSwap;
        PlayaStatusScript.OnUpdateScore -= UpdateScoreUI;
        WaveSpawner.OnWaveComplete -= WaveText;
        Enemy.OnAddKill -= KillText;
    }
    private void UpdateHealth(float currentHealth)
    {
        healthText.text = "Health: " + currentHealth.ToString("00");
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
        ammoText.text = "Ammo: " + currentAmmo.ToString("00") + " / " + maxAmmo.ToString("00");
    }
    private void UpdateScoreUI(float newScore)
    {
        scoreText.text = "Score: " + newScore.ToString();
    }
    private void WaveText(int waveNum)
    {
        waveText.text = "Wave: " + waveNum.ToString();
    }
    private void KillText(bool asdf)
    {
        killCount++;
        killText.text = "Kills: " + killCount.ToString();
    }
}
