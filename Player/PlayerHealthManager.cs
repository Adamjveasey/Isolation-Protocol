using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

// Manages the player character's health and specific methods needed
public class PlayerHealthManager : HealthManager
{
    [Tooltip("The player HUD canvas")]
    [SerializeField]
    private GameObject  m_playerHUD;
    
    [Tooltip("The death UI canvas")]
    [SerializeField]
    private GameObject  m_deathUI;
    
    public GameObject   m_bloodEffect;            // Lewis' code.
    public AudioClip    m_playerDamagedFeedback;   // Lewis' code.

    public enum playerState { alive, dead, downed } // James' code

    public playerState  m_currentPlayerState;       // James' cpde

    [Tooltip("Whether combat armour is active")]
    public bool         m_hasCombatArmour;

    public override void Start()
    {
        base.Start();
        FabricatorEventListener.current.onPlayerUpgradeUnlock += HealthUpgrades;
    }

    // Lowers player health by amount given if vulnerable
    public override void TakeDamage( int damage )
    {
        if ( m_isVulnerable )
        {
            // Removes combar armour when hit
            if ( m_hasCombatArmour )
            {
                m_hasCombatArmour = false;
            }
            else
            {
                StartCoroutine(PlayerDamageFeedBack()); // Lewis' code. See co-routine below
                
                m_currentHealth -= damage;

                // Makes player invulnerable for short time after getting damaged
                m_isVulnerable = false;
                StartCoroutine( DamagedInvunerability( ) );
            }
        }

        // Call player death method when out of health
        if( m_currentHealth <= 0 )
        {
            if (  !GetComponentInChildren<DroneController>() || !gameObject.GetComponentInChildren<DefibMode>( ).enabled || !gameObject.GetComponentInChildren<DefibMode>( ).m_canDefibPlayer )
            {
                Die( );
                SaveSystem.SavePlayer(gameObject.GetComponent<PlayerController>());
            }
            else if ( gameObject.GetComponentInChildren<DefibMode>( ).m_canDefibPlayer )
            {
                m_currentPlayerState = playerState.downed;
                gameObject.GetComponentInChildren<DefibMode>( ).AttemptDefibrillation( );
            }
        }
        
    }

    // Deactivates player and sets up death screen
    private void Die()
    {
        // Removes player from DontDestroyOnLoad
        SceneManager.MoveGameObjectToScene( gameObject, SceneManager.GetActiveScene() );

        // Lewis' code, this tells the analytic Manager to trigger a method to send off data when the player dies 
        analyticsEventManager.analytics?.playerDeathMethod(); 

        m_deathUI.SetActive( true );
        
        m_currentPlayerState = playerState.dead;
    }

    public void IncreaseMaxHealth( int amount )
    {
        m_maxHealth += amount;
        m_currentHealth = m_maxHealth;
    }

    public void HealthUpgrades( string itemName )
    {
        switch ( itemName )
        {
            case "Combat Armour":
                m_hasCombatArmour = true;
                break;
            case "Vitality Increase":
                IncreaseMaxHealth(2);
                break;
            case "Stamina Stim":
                IncreaseMaxHealth(2);
                break;
            default:
                break;
        }
    }

    // Makes player invulnerable for a short time
    IEnumerator DamagedInvunerability()
    {
        yield return new WaitForSeconds(0.75f);
        m_isVulnerable = true;
    }

    IEnumerator PlayerDamageFeedBack()
    {
        if (gameObject.name == "Player")
        {
            AudioSource.PlayClipAtPoint(m_playerDamagedFeedback, transform.position);
            m_bloodEffect.SetActive(true);
            yield return new WaitForSecondsRealtime(0.4f);
            m_bloodEffect.SetActive(false);
        }
    } // Lewis' code. Activates a blood animation on the player when damaged
}
