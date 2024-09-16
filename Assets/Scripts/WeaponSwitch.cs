using System;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public static WeaponSwitch Instance;

    public static Action<float, float> OnWeaponSwap;
    public static Action stopReload;

    public int selectedWeapon = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SelectWeapon();
    }

    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;
        
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0) 
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            selectedWeapon = 2;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        stopReload?.Invoke();
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                OnWeaponSwap?.Invoke(weapon.gameObject.GetComponent<Gun>().currentAmmo, weapon.gameObject.GetComponent<Gun>().maxAmmo);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    public void AddWeapon(GameObject weaponPrefab)
    {
        if (transform.childCount > 1)
        {
            int i = 0;
            foreach (Transform weapon in transform)
            {
                if (i == selectedWeapon)
                {
                    Destroy(weapon.gameObject);
                }

                i++;
            }
            GameObject newWeapon = Instantiate(weaponPrefab, new Vector3(), Quaternion.identity);
                newWeapon.transform.SetParent(transform, false);
        }
        else if (transform.childCount <= 1)
        {
            GameObject newWeapon = Instantiate(weaponPrefab, new Vector3(), Quaternion.identity);
            newWeapon.transform.SetParent(transform, false);
        }
        SelectWeapon();
    }
}
