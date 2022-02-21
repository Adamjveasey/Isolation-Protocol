using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLayout", menuName = "ScriptableObjects/Enemies/EnemyLayouts", order = 1)]
public class EnemyLayout : ScriptableObject
{
    [Tooltip("The list of enemies that will be spawned")]
    public GameObject[] m_enemies;

    [Tooltip("The positions that the enemies will be spawned in")]
    public Vector3[]    m_localPositions;
}
