using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// The save data for the fabricator upgrades
[System.Serializable]
public class FabricatorData
{
    // Which drone upgrades have been unlocked
    public bool[] m_droneUpgradesUnlocks;

    // Which exo suit upgrades have been unlocked
    public bool[] m_exoSuitUpgradesUnlocks;

    // Which weapons have been unlocked
    public bool[] m_weaponUnlocks;

    public FabricatorData( FabricatorUpgradeListGenerator droneList, FabricatorUpgradeListGenerator exoSuitList, FabricatorStoreProduct[] weaponList )
    {
        m_droneUpgradesUnlocks = new bool[droneList.m_buttonObjectReferences.Length];
        m_exoSuitUpgradesUnlocks = new bool[exoSuitList.m_buttonObjectReferences.Length];
        m_weaponUnlocks = new bool[weaponList.Length];

        for ( int i = 0; i < droneList.m_buttonObjectReferences.Length; i++ )
        {
            m_droneUpgradesUnlocks[i] = droneList.m_buttonObjectReferences[i].GetIsUnlocked();
        }

        for ( int i = 0; i < exoSuitList.m_buttonObjectReferences.Length; i++ )
        {
            m_exoSuitUpgradesUnlocks[i] = exoSuitList.m_buttonObjectReferences[i].GetIsUnlocked();
        }

        for ( int i = 0; i < weaponList.Length; i++ )
        {
            m_weaponUnlocks[i] = weaponList[i].GetIsUnlocked();
        }
    }
}
