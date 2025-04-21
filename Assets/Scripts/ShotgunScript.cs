using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotgunScript : MonoBehaviour
{
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 25f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private bool isFullAuto = false;
    [SerializeField] private int pelletCount = 10;

    public int maxAmmo = 5;
    public int currentAmmo;
    public float reloadTime = 3f;
    private bool isReloading = false;

    public Camera fpsCam;

    private float nextTimeToFire = 0f;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.2f;
    [SerializeField] private float zoomFOV = 60f;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float zoomSensModifier = 0.7f;
    KeyCode zoomKey;
    KeyCode reloadKey = KeyCode.R;
    [SerializeField] private bool canZoom = true;
    private Coroutine zoomRoutine;

    private float defaultSensX;
    private float defaultSensY;
    private float zoomSensX;
    private float zoomSensY;

    public static Action<float> OnShoot;
    public static Action<float> OnReload;

    private void OnEnable()
    {
        WeaponSwitch.stopReload += StopReload;
    }

    private void StopReload()
    {
        if (isReloading)
        {
            StopCoroutine(Reload());
            isReloading = false;
            Debug.Log("Cancelled Reload.");
        }
    }

    private void Awake()
    {
        fpsCam = Camera.main;
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        defaultFOV = fpsCam.fieldOfView;
        zoomKey = PlayerManager.instance.player.GetComponent<PlayaMoveScript>().zoomKey;

        defaultSensX = PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedX;
        defaultSensY = PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedY;

        zoomSensX = PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedX * zoomSensModifier;
        zoomSensY = PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedY * zoomSensModifier;
    }


    // Update is called once per frame
    void Update()
    {

        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0 || (Input.GetKey(reloadKey) && currentAmmo < maxAmmo))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && isFullAuto)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
        else if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (canZoom)
        {
            HandleZoom();
        }

    }

    public IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        OnReload?.Invoke(currentAmmo);
        Debug.Log("Reloaded.");
        isReloading = false;
    }

    public void sendAmmo()
    {
        currentAmmo = maxAmmo;
        OnReload?.Invoke(currentAmmo);
    }

    void Shoot()
    {

        currentAmmo--;
        OnShoot?.Invoke(currentAmmo);

        RaycastHit hit;
        for (int n = 0; n < pelletCount; n++)
        {
            float bloomx = Random.Range(-2.5f, 2.5f);
            float bloomy = Random.Range(-2.5f, 2.5f);
            Vector3 bloomVector = new Vector3(bloomx, bloomy);
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward + bloomVector, out hit, range))
            {
                Enemy targetEnemy = hit.transform.GetComponent<Enemy>();
                if (targetEnemy != null)
                    {
                        targetEnemy.TakeDamage(damage/pelletCount);
                    }
                //impact mark here
            } 
        }
    }

    private void HandleZoom()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }

        if (Input.GetKeyUp(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = fpsCam.fieldOfView;

        float startingSensX = PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedX;
        float startingSensY = PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedY;

        float targetSensX = isEnter ? zoomSensX : defaultSensX;
        float targetSensY = isEnter ? zoomSensY : defaultSensY;

        float timeElapsed = 0;

        while (timeElapsed < timeToZoom)
        {
            fpsCam.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedX = Mathf.Lerp(startingSensX, targetSensX, timeElapsed / timeToZoom);
            PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedY = Mathf.Lerp(startingSensY, targetSensY, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        fpsCam.fieldOfView = targetFOV;
        PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedX = targetSensX;
        PlayerManager.instance.player.GetComponent<PlayaMoveScript>().lookSpeedY = targetSensY;

        zoomRoutine = null;
    }
}
