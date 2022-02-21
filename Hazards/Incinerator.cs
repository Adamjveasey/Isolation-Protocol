using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : MonoBehaviour
{
    [Tooltip("Whether the incinerator is on")]
    public bool           m_on;

    [Tooltip("The fire particle system")]
    public ParticleSystem m_vfx;
    
    [Tooltip("The open gate object")]
    public GameObject     m_openGate;

    [Tooltip("The closed gate object")]
    public GameObject     m_closedGate;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine( On() );
    }

    private IEnumerator On()
    {
        m_on = true;
        m_vfx.Play();
        m_openGate.SetActive( true );
        m_closedGate.SetActive( false );
        yield return new WaitForSeconds( 3 );
        StartCoroutine( Off() );
    }
    // By calling the two coroutines inside each other the incinerator is looped in an on & off state
    private IEnumerator Off()
    {
        m_on = false;
        m_vfx.Stop();
        m_closedGate.SetActive( true );
        m_openGate.SetActive( false );
        yield return new WaitForSeconds( 3 );
        StartCoroutine( On() );
    }

    // Sets character on fire and activates status effect
    private void OnTriggerStay2D( Collider2D collision )
    {
        if ( collision.CompareTag("Player") || collision.CompareTag("Enemy") )
        {
            CharacterBase collisionChar = collision.GetComponent<CharacterBase>();
            
            if ( m_on && !collisionChar.m_isBurning )
            {
                collisionChar.m_isOnFire = true;
                collisionChar.m_onFireParticles.Play();
            }
        }
    }
}
