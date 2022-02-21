using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A base class for anything that explodes
public class Explodable : MonoBehaviour
{
    [Tooltip("Damage to be applied to whatever is hit")]
    [SerializeField]
    protected int            m_damage;
    
    [Tooltip("Whether it should explode when hit by a projectile")]
    [SerializeField]
    protected bool           m_explodeWhenShot;
    
    [Tooltip("Whether it should explode on a timer")]
    [SerializeField]
    protected bool           m_timed;
    
    [Tooltip("The amount of time after which it will explode in seconds")]
    [SerializeField]
    protected float          m_timer;
    
    [Tooltip("The radius of the explosion")]
    [SerializeField]
    protected float          m_explosionRadius;
    
    [Tooltip("The explosion effect particle system")]
    [SerializeField]
    protected ParticleSystem m_vfx;

    [Tooltip("Explosion audio")]
    public AudioClip         m_explosionSound;
    
    protected void Start()
    {
        // Starts timed explosion if explodable should be timed
        if ( m_timed )
        {
            StartCoroutine( TimedExplodsion() );
        }
    }

    protected void Explode()
    {
        AudioSource.PlayClipAtPoint( m_explosionSound, transform.position );

        // Makes an array of all Collider2D within the determined radius
        Collider2D[] hit = Physics2D.OverlapCircleAll( transform.position, m_explosionRadius );

        // Checks if the array isn't empty
        if ( hit.Length != 0 )
        {
            for ( int i = 0; i < hit.Length; i++ )
            {
                // Checks if collider hit is a character to apply determined damage to their health manager
                if ( hit[i].CompareTag("Player") || hit[i].CompareTag("Enemy") )
                {
                    hit[i].gameObject.GetComponent<HealthManager>().TakeDamage( m_damage );
                }
                // Checks if collider was another explodable if its not a character to activate it
                else if ( hit[i].CompareTag("Explosive") )
                {
                    Destroy( gameObject );
                    hit[i].gameObject.GetComponent<Explodable>().Explode();
                }
            }
        }

        // Unparents particle effects so they play after the object has exploded
        m_vfx.transform.SetParent(null);
        
        m_vfx.Play();
        
        Destroy(gameObject);
    }

    // Waits determined number of seconds before calling Explode()
    protected IEnumerator TimedExplodsion()
    {
        yield return new WaitForSeconds( m_timer );
        Explode();
    }

    // Checks if explodable was hit by a projectile and if it can explode when shot
    private void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.CompareTag("Projectile") && m_explodeWhenShot )
        {
            Explode();
        }
    }
}