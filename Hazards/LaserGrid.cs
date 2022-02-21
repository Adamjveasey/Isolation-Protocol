using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Multiple laser controller
public class LaserGrid : MonoBehaviour
{
    [Tooltip("Individual lasers")]
    [SerializeField]
    private GameObject[] m_lasers;
    
    [Tooltip("Laser powering up visual effect")]
    [SerializeField]
    private GameObject[] m_powerUpVfx;

    [Tooltip("The amount of time in seconds the laser spends powering up")]
    public float m_timePowerUp;

    [Tooltip("The amount of time in seconds the laser spends on")]
    public float m_timeOn;

    [Tooltip("The amount of time in seconds the laser spends off")]
    public float m_timeOff;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine( PowerUp() );
    }
    
    private IEnumerator On()
    {
        for ( int i = 0; i < m_lasers.Length; i++ )
        {
            m_powerUpVfx[i].SetActive( false );
            m_lasers[i].SetActive( true );
        }

        yield return new WaitForSeconds( m_timeOn );

        StartCoroutine( Off() );
    }

    private IEnumerator Off()
    {
        for ( int i = 0; i < m_lasers.Length; i++ )
        {
            m_lasers[i].SetActive( false );
        }

        yield return new WaitForSeconds( m_timeOff );

        StartCoroutine( PowerUp() );
    }

    private IEnumerator PowerUp()
    {
        for ( int i = 0; i < m_powerUpVfx.Length; i++ )
        {
            m_powerUpVfx[i].SetActive( true );
        }

        yield return new WaitForSeconds( m_timePowerUp );

        StartCoroutine( On() );
    }
}
