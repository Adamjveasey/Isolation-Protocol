using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Loads the next level when player collides with the level exit
public class NextLevel : MonoBehaviour
{
    // The number of the level that is currently loaded
    private int m_currentLevelNumber;

    private void Start()
    {
        // Gets level number from Level Controller
        m_currentLevelNumber = GameObject.FindGameObjectWithTag("StartRoom").GetComponent<LevelController>().m_levelNumber;
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        // Checks for collision with player
        if ( collision.CompareTag("Player") )
        {
            // Takes player back to Ship Hub upon completion of level 5
            if ( m_currentLevelNumber == 5 )
            {
                SceneManager.LoadScene("Ship Hub");
            }
            else
            {
                // Saves player state in between levels
                DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Player"));

                // Loads next level based on current level number
                SceneManager.LoadScene( "Level " + (m_currentLevelNumber + 1) );

                // Repositions player to start room
                collision.gameObject.transform.position = Vector3.zero;
            }
        }
    }
}
