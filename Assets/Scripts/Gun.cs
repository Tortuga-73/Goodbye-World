using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class GunScript : MonoBehaviour
{
    // Start is called before the first frame update

    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    public Camera fpsCam;

    private float nextTimeToFire = 0f;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.2f;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float defaultFOV;
    KeyCode zoomKey;
    [SerializeField] private bool canZoom = true;
    private Coroutine zoomRoutine;

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

        if (currentAmmo <= 0)
        {           
            StartCoroutine(Reload());
            return;
        }
        
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (canZoom)
        {
            HandleZoom();
        }

    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        Debug.Log("Reloaded.");
        isReloading = false;
    }

    void Shoot()
    {

        currentAmmo--;
        
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
