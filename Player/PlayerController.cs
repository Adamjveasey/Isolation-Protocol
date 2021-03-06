using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

// Controls most player functions and interactions
// (Anything not marked as someone else's work via comments is Adam's work)
public class PlayerController : CharacterBase
{
    public DictionaryBase dictionary;

    //James' Work

    private static PlayerController playerInstance;

    #region Weapons

    [Header("Weapons")]

    [Tooltip("The position of the weapon on the player.")]
    public GameObject   m_weaponAttachPoint;

    [Tooltip("The weapon held by the player.")]
    public GameObject   m_currentWeapon;

    [Tooltip("The array index of the player's currently equipped weapon.")]
    public int          m_currentWeaponIndex;

    [Tooltip("The array containing the player's carried weapons.")]
    public GameObject[] m_carriedWeapons;

    public bool m_weaponsFree;

    #endregion

    public PlayerHealthManager m_playerHealthManager;

    //End of James' work

    public DroneController m_drone;

    #region Animation

    [Header("Animation")]
    
    // Reference to animator
    private Animator       m_animator;
    
    // Mouse direction relative to player position
    private Vector2        m_mouseDirection;

    #endregion

    #region UI

    [Header("UI")]

    [Tooltip("The HUD Manager")]
    public HUDManager      m_hud;

    [Tooltip("The pause menu canvas")]
    [SerializeField]
    private GameObject     m_pauseMenu;

    #endregion

    #region Dash

    [Header("Dash")]

    [Tooltip("The cooldown time of the dash in seconds")]
    public float        m_dashCooldown;

    [Tooltip("The speed at which the player should dash")]
    [SerializeField]
    private float       m_dashSpeed;

    [Tooltip("The amount of time the dash lasts in seconds")]
    [SerializeField]
    private float       m_dashDuration;

    // Countdown timer for dash duration
    private float       m_dashTimer;

    // Margin of error for dash direction
    private float       m_directionTolerance;

    [Tooltip("Whether or not the player can dash")]
    [HideInInspector]
    public bool         m_canDash;

    [Tooltip("Whether or not the player is currently dashing")]
    [HideInInspector]
    public bool         m_isDashing;

    // The direction the player is dashing in
    private Vector2     m_dashDirection;

    // The possible directions the player can dash in
    private Vector2[]   m_directions;

    #endregion

    //James' Work
    #region MutoSlug Attachments

    [Header("MutoSlug Attachments")]

    [Tooltip("An array of GameObjects that MutoSlugs can attach to.")]
    public GameObject[] m_slugAttachmentPoints;

    [Tooltip("Determines whether or not the player currently has any slugs attached to them.")]
    public bool m_hasAttachedSlugs;

    [Tooltip("The physics layer that the walls sit on, used to detect collisions during dodges.")]
    public LayerMask m_wallLayer;

    #endregion

    #region Currency

    public CurrencyManager m_currencyManager;

    public LayerMask m_interactiveLayer;

    public float m_interactionRange;

    #endregion

    #region AudioLogs

    [Header("AudioLogs")]

    public AudioLogListController m_audioLogList;

    public CanvasController m_audioLogCanvas;

    public bool m_isInMenu = false;

    #endregion

    //End of James' work

    #region Upgrades

    [Header("Upgrades")]

    public bool m_hasDexterityBoost;

    [Tooltip("The player move speed after 'Turbo Legs' upgrade is unlocked")]
    [SerializeField]
    private float m_upgradedMoveSpeed;

    [Tooltip("The amount in seconds that the reload time will be decreased by after 'Dexterity Boost' is unlocked")]
    [SerializeField]
    private float m_reloadTimeDecreaseAmount;

    #endregion

    public analyticsManager m_enemyCounter;

    // Sets up initial values of variables and references not set in inspector
    private void Start()
    {
        // Adds PlayerUpgrades() to functions call onPlayerUpgradeUnlock event
        FabricatorEventListener.current.onPlayerUpgradeUnlock += PlayerUpgrades;
     
        m_playerHealthManager = GetComponent<PlayerHealthManager>();
        m_healthManager = GetComponent<PlayerHealthManager>();
        m_animator      = GetComponent<Animator>();
        m_dashTimer      = m_dashDuration;
        m_canDash       = true;
        m_isDashing     = false;
        m_directionTolerance = 0.5f;
        m_directions    = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            new Vector2(-1, 1),     // up-left
            new Vector2(1, 1),      // up-right
            new Vector2(-1, -1),    // down-left
            new Vector2(1, -1)      // down-right
        };

        DontDestroyOnLoad(this.gameObject);

        if( playerInstance == null )
        {
            playerInstance = this;
        }
        else
        {
            Destroy( gameObject );
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if ( m_playerHealthManager.m_currentPlayerState == PlayerHealthManager.playerState.alive && Time.timeScale != 0 )
        {
            // Assigns directional velocity to correct input axes
            m_directionalVelocity.x = Input.GetAxisRaw( "Horizontal" );
            m_directionalVelocity.y = Input.GetAxisRaw( "Vertical" );

            // Gets mouse position within screen space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if ( !m_isStunned )
            {
                // Calculates mouse direction relative to the player's position
                m_mouseDirection = mousePos - transform.position;

                Animate();
                Dash();
            }

            // James' work

            if (!m_isInMenu)
            {

                CheckForInteractables();

                if ( m_weaponsFree )
                {

                    if ( Input.GetMouseButton( 0 ) && !m_isInMenu && !m_isStunned )
                    {
                        m_currentWeapon.GetComponent<GunBase>( ).FireWeapon( );
                    }
                    if ( Input.GetMouseButtonUp( 0 ) && !m_isInMenu )
                    {
                        m_currentWeapon.GetComponent<GunBase>( ).StopFiring( );
                    }

                    float scrollWheelValue = Input.GetAxis("Mouse ScrollWheel");

                    if ( scrollWheelValue != 0 )
                    {
                        float indexModifier = scrollWheelValue * 10;
                        SwapWeapon( ( int )indexModifier );
                    }

                    if ( Input.GetKeyDown( KeyCode.R ) && !m_isStunned )
                    {
                        m_currentWeapon.GetComponent<GunBase>( ).ReloadWeapon( );
                    }
                }
            }

            if ( Input.GetKeyDown( KeyCode.Tab ) )
            {
                m_audioLogCanvas.ToggleCanvas( );
                m_isInMenu = !m_isInMenu;
            }

            if ( Input.GetKeyDown( KeyCode.Escape ) )
            {
                m_pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }

            // End of James' work

            if ( m_playerHealthManager.m_currentPlayerState == PlayerHealthManager.playerState.dead )
            {
                gameObject.GetComponent<AnalyticsEventTracker>().enabled = true; // enables the event tracker, it'll send the playtest data to the server
            }
        }
    }

    // Handles updating physical interactions
    protected override void FixedUpdate()
    {
        if ( m_isDashing && !m_knockedBack )
        {
            // Moves rigidbody with dash parameters when player is dashing
            m_characterRigidBody.velocity = m_dashDirection.normalized * m_dashSpeed * Time.deltaTime;

            //James' work

            if( Physics2D.Raycast( transform.position, m_dashDirection, 1.0f, m_wallLayer ) == true )
            {
                KillAttachedSlugs( );
            }

        }
        else if( m_knockedBack )
        {
            SimulateKnockback( );
        }

        //End of James' work

        else if( !m_isInMenu )
        {
            // Moves player normally
            Move( );
        }
    }

    // James' work

    public void ReplenishAmmo( )
    {

        GunBase gunBase = m_currentWeapon.GetComponent<GunBase>();

        gunBase.m_currentCarriedAmmo = gunBase.m_maxAmmoCapacity;

        gunBase.UpdateUIElements( );

    }

    public void ToggleIsInMenu( )
    {
        m_isInMenu = !m_isInMenu;
    }

    public void ToggleWeaponsFree( )
    {
        m_weaponsFree = !m_weaponsFree;
    }

    // End of James' work

    protected void Animate()
    {
        // Changes animation via Blend Tree so player is always 'facing' the mouse direction
        m_animator.SetFloat( "Horizontal", m_mouseDirection.x );
        m_animator.SetFloat( "Vertical", m_mouseDirection.y );

        // Trasitions between Walk and Idle animations if directional velocity is bigger than 0.01
        m_animator.SetFloat( "Speed", m_directionalVelocity.sqrMagnitude );

        // Changes held weapon position and sorting order depending on direction player is facing
        // (Needs further work)
        string clipName = m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        // Moves weapon to match positions with players hand according to animation direction
        if ( clipName == "IdleRight" || clipName == "WalkRight" )
        {
            m_weaponAttachPoint.transform.localPosition = new Vector3( 0f, 0f, 0f );
            m_currentWeapon.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        else if ( clipName == "IdleLeft" || clipName == "WalkLeft" )
        {
            m_weaponAttachPoint.transform.localPosition = new Vector3( 0f, -0.3f, 0f );
            m_currentWeapon.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
        else if ( clipName == "IdleUp" || clipName == "WalkUp" )
        {
            m_weaponAttachPoint.transform.localPosition = new Vector3( -0.3f, -0.25f, 0f );
            m_currentWeapon.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        else if ( clipName == "IdleDown" || clipName == "WalkDown" )
        {
            m_weaponAttachPoint.transform.localPosition = new Vector3( 0.3f, -0.25f, 0f );
            m_currentWeapon.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
    }

    // Checks for input and activates dash
    protected void Dash()
    {
        // Picks dash direction based on directional velocity
        for (int i = 0; i < m_directions.Length; i++)
        {
            if (Mathf.Abs(m_directionalVelocity.normalized.x - m_directions[i].x) < m_directionTolerance && Mathf.Abs(m_directionalVelocity.normalized.y - m_directions[i].y) < m_directionTolerance)
            {
                m_dashDirection = m_directions[i];
            }
        }

        if (m_canDash)
        {
            if (Input.GetMouseButtonDown(1) && !m_isInMenu)
            {
                m_isDashing = true;
                m_canDash = false;
                m_healthManager.m_isVulnerable = false;     // Makes player invulnerable while dashing

                // Puts player out if they are burning
                if (m_isOnFire)
                {
                    m_isOnFire = false;
                    m_onFireParticles.Stop();
                }
            }
        }

        if (m_isDashing)
        {
            // Resets everything and starts cooldown once dash is over
            if (m_dashTimer <= 0)
            {
                m_isDashing = false;
                m_dashTimer = m_dashDuration;
                m_characterRigidBody.velocity = Vector2.zero;
                m_dashDirection = Vector2.zero;
                m_healthManager.m_isVulnerable = true;
                m_hud.DashCooldown();
                StartCoroutine(DashCooldown());
            }
            else
            {
                m_dashTimer -= Time.deltaTime;
            }
        }
    }

    // Impedes player from dashing again before cooldown time is over
    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(m_dashCooldown);
        m_canDash = true;
        m_hud.m_dashOnCooldown = false;
    }
    
    // Disables player dash
    public IEnumerator DisableDash( float disableDuration )
    {
        m_canDash = false;

        yield return new WaitForSeconds( disableDuration );

        m_canDash = true;

    }

    public void PlayerUpgrades(string itemName)
    {
        switch (itemName)
        {
            case "Turbo Legs":
                IncreaseMoveSpeed();
                break;
            case "Dexterity Boost":
                DecreaseReloadTime();
                break;
            default:
                break;
        }
    }

    public void IncreaseMoveSpeed()
    {
        m_moveSpeed = m_upgradedMoveSpeed;
    }

    public void DecreaseReloadTime()
    {
        for (int i = 0; i < m_carriedWeapons.Length; i++)
        {
            if (m_carriedWeapons[i].GetComponent<GunBase>().m_reloadTime > 0)
            {
                m_carriedWeapons[i].GetComponent<GunBase>().m_reloadTime -= m_reloadTimeDecreaseAmount;
            }
        }
    }

    //James' Work
    public bool AttachNewSlug( GameObject newSlug )
    {

        //Local boolean used to determine whether or not this slug has been attached to the player
        bool hasAttached = false;

        //Loops through all of the attachment points on the player. If one is free, the slug is attached to it and hasAttached is set to true
        foreach( GameObject attachPoint in m_slugAttachmentPoints )
        {
            if( attachPoint.transform.childCount == 0 )
            {
                newSlug.transform.parent = attachPoint.transform;

                hasAttached = true;

                m_hasAttachedSlugs = true;

            }
        }

        //Returns the value of hasAttached so that the slug can determine whether or not to run the next set of logic
        return hasAttached;

    }

    private void KillAttachedSlugs( )
    {
        //Loops through each attachment point in the array, destroying the children of any of them
        foreach( GameObject attachPoint in m_slugAttachmentPoints )
        {
            if( attachPoint.transform.childCount != 0 )
            {
                //Kills the slug attached to this point
                attachPoint.transform.GetChild( 0 ).gameObject.GetComponent<MutoSlug>( ).Die( );
            }
        }
    }

    public void SwapWeapon( int indexModifier )
    {

        if ( CanSwapWeapon( ) )
        {

            bool swappedWeapon = false;

            int newIndex = m_currentWeaponIndex + indexModifier;

            while ( !swappedWeapon )
            {

                if ( newIndex < 0 )
                {
                    newIndex = m_carriedWeapons.Length - 1;
                }
                else if ( newIndex >= m_carriedWeapons.Length )
                {
                    newIndex = 0;
                }

                if ( m_carriedWeapons[newIndex] != null )
                {
                    m_currentWeapon.SetActive( false );

                    m_currentWeapon = m_carriedWeapons[newIndex];

                    m_currentWeaponIndex = newIndex;

                    m_currentWeapon.SetActive( true );

                    swappedWeapon = true;

                }

                if ( !swappedWeapon )
                {
                    newIndex += indexModifier;
                }

            }
        }

        m_currentWeapon.GetComponent<GunBase>( ).UpdateUIElements( );

    }

    private bool CanSwapWeapon( )
    {

        int numWeapons = 0;

        for(int i = 0; i < m_carriedWeapons.Length; i++ )
        {
            if(m_carriedWeapons[i] != null )
            {
                numWeapons++;
            }
        }

        if(numWeapons > 1 )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckForInteractables( )
    {

        RaycastHit2D interactableObject = Physics2D.Raycast(transform.position, m_mouseDirection, m_interactionRange, m_interactiveLayer);

        Debug.DrawRay( transform.position , m_mouseDirection * m_interactionRange );

        if ( interactableObject )
        {
            InteractableObject interactable = interactableObject.collider.gameObject.GetComponent<InteractableObject>( );

            interactable.m_isBeingLookedAt = true;

            interactable.m_playerController = this;

            if ( Input.GetKeyDown( KeyCode.E ) )
            {
                interactable.Activated( this );
    
            }
        }

        

    }

    public void ToggleHUD( )
    {
        m_hud.GetComponent<CanvasController>().ToggleCanvas();
    }



    //End of James' work


    private void OnTriggerEnter2D(Collider2D collision) // Lewis' code
    {
        {
            if (collision.gameObject.layer == 12) // checks for doorcollider
            {
                analyticsManager.roomsCrossed++;
            }
        }
    }
}