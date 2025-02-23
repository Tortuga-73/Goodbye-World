using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpgradeScript : Interactable
{
    public static Action<float> OnAddScore;
    private float speedPrice = -500f;

    public override void OnFocus()
    {
        print("Looking At " + gameObject.name);
    }

    public override void OnInteract()
    {
        if (PlayerManager.instance.player.GetComponent<PlayaStatusScript>().score >= 500)
        {
            OnAddScore?.Invoke(speedPrice);
            PlayerManager.instance.player.GetComponent<PlayaMoveScript>().playerSpeedMultiplier += 0.2f;
        }
        else
        {
            Debug.Log("You do not have enough money to purchase this upgrade.");
        }
    }

    public override void OnLoseFocus()
    {
        print("Stopped Looking At " + gameObject.name);
    }

    
}
