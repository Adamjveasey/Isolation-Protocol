using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPuddle : MonoBehaviour
{
    [Tooltip("Electricity particle system")]
    public ParticleSystem   m_particles;

    [Tooltip("Whether the puddle is currently electrified")]
    public bool             m_electrified;
    
    [Tooltip("Electric crackle audio")]
    public AudioClip        m_crackleSound;
    
    // Whether something is caught on the puddle when its electrified
    private bool            m_caught;

    // The character base of whatever is caught in the puddle
    private CharacterBase   m_caughtThing;

    private void Start()
    {
        m_electrified = false;
        StartCoroutine( NotElectrified() );
    }

    private void Update()
    {
        // Calls Stun status effect if something is caught in the puddle while its electrified
        if( m_electrified && m_caught )
        {
            
            StartCoroutine( m_caughtThing.Stun() );
        }
    }

    private IEnumerator Electrified()
    {
        m_electrified = true;
        
        m_particles.Play();

        AudioSource.PlayClipAtPoint( m_crackleSound, transform.position );
        
        yield return new WaitForSeconds( 1 );
        
        StartCoroutine( NotElectrified() );
    }

    // By calling the two coroutines inside each other the puddle is looped in an on & off state

    private IEnumerator NotElectrified()
    {
        m_electrified = false;
        
        yield return new WaitForSeconds( 3 );
        
        StartCoroutine( Electrified() );
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.CompareTag("Player") || collision.CompareTag("Enemy") )
        {
            m_caught = true;
            m_caughtThing = collision.GetComponent<CharacterBase>();
        }
    }

    private void OnTriggerExit2D( Collider2D collision )
    {
        if( collision.CompareTag("Player") || collision.CompareTag("Enemy") )
        {
            m_caught = false;
        }
    }
}
