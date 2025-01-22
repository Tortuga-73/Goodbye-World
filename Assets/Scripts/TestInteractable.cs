using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestInteractable: Interactable
{

    [SerializeField] GameObject newWeapon;

    public override void OnFocus()
    {
        print("Looking At " + gameObject.name);
    }

    public override void OnInteract()
    {
        WeaponSwitch.Instance.AddWeapon(newWeapon);
        this.gameObject.SetActive(false);

        print("Interacted With " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        print("Stopped Looking At " + gameObject.name);
    }
}
