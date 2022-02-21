using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles fabricator events
public class FabricatorEventListener : MonoBehaviour
{
    // current event listener
    public static FabricatorEventListener current;

    private void Awake()
    {
        current = this;
    }

    // Event called when a fabricator product is unlocked
    public event Action onFabricatorProductUnlock;
    public void FabricatorProductUnlock()
    {
        if(onFabricatorProductUnlock != null)
        {
            onFabricatorProductUnlock( );
        }
    }

    // Event called when a player upgrade is unlocked
    public event Action<string> onPlayerUpgradeUnlock;
    public void PlayerUpgradeUnlock(string itemName)
    {
        if(onPlayerUpgradeUnlock != null)
        {
            onPlayerUpgradeUnlock(itemName);
        }
    }
}
