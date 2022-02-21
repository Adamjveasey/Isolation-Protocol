using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores save data related to the player character
[System.Serializable]
public class PlayerData
{
    // Player's maximum health value
    public int      m_maxHealth;

    // Amount of fuel player is carrying
    public int      m_playerFuel;

    // Names of the weapons player has equipped
    public string[] m_equippedWeapons;

    // Whether player has the drone activated
    public bool     m_isDroneActive;

    // The state of all the drone modes to know which one is active
    public bool[]   m_droneModes;

    public PlayerData(PlayerController player)
    {
        m_maxHealth = player.m_playerHealthManager.m_maxHealth;

        m_playerFuel = player.m_currencyManager.m_fabricatorFuelCount;

        m_equippedWeapons = new string[player.m_carriedWeapons.Length];

        for ( int i = 0; i < player.m_carriedWeapons.Length; i++ )
        {
            m_equippedWeapons[i] = player.m_carriedWeapons[i].name;
        }

        m_isDroneActive = player.m_drone.gameObject.activeSelf;

        m_droneModes = new bool[player.m_drone.m_droneUpgrades.Length];

        for ( int i = 0; i < player.m_drone.m_droneUpgrades.Length; i++ )
        {
            m_droneModes[i] = player.m_drone.m_droneUpgrades[i].enabled;
        }
    }
}
