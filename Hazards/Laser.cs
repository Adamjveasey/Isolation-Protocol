using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Activated laser unit
public class Laser : MonoBehaviour
{
    // Checks for collision with player and damages them
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<HealthManager>().TakeDamage(1);
        }
    }
}
