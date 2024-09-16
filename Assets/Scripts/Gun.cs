using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using System;

public class Gun : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private bool isFullAuto = false;
    
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    public Camera fpsCam;

    private float nextTimeToFire = 0f;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.2f;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float defaultFOV;
    KeyCode zoomKey;
    KeyCode reloadKey = KeyCode.R;
    [SerializeField] private bool canZoom = true;
    private Coroutine zoomRoutine;

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

    void Shoot()
    {

        currentAmmo--;
        OnShoot?.Invoke(currentAmmo);
        
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Enemy targetEnemy = hit.transform.GetComponent<Enemy>();
            if (targetEnemy != null)
            {
                targetEnemy.TakeDamage(damage);
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

        float timeElapsed = 0;

        while (timeElapsed < timeToZoom)
        {
            fpsCam.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        fpsCam.fieldOfView = targetFOV;
        zoomRoutine = null;
    }
}
