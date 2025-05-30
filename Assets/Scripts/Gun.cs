using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using System;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] AudioSource gunshot;
    [SerializeField] private ParticleSystem muzzleFlash;
    
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private bool isFullAuto = false;
    
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    [SerializeField] private bool isShotgun = false;
    public int pelletCount = 8;

    public Camera fpsCam;

    private float nextTimeToFire = 0f;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.2f;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float zoomSensModifier = 0.5f;
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
        gunshot = this.GetComponent<AudioSource>();
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
        gunshot = this.GetComponent<AudioSource>();


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
        gunshot.Play();
        muzzleFlash.Play();
        
        if (isShotgun)
        {
            RaycastHit hit;
            for (int n = 0; n < pelletCount; n++)
            {
                float bloomx = Random.Range(-.05f, .05f);
                float bloomy = Random.Range(-.05f, .05f);
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
        else
        {
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
