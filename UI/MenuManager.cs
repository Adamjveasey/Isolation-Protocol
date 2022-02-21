using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Manages all functions UI buttons need to call
public class MenuManager : MonoBehaviour
{
    [Tooltip("The 'are you sure' object")]
    public GameObject m_areYouSurePanel;

    // Takes player to the ship hub scene
    public void ShipHub()
    {
        // Removes player object from DontDestroyOnLoad if they are in the scene
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            SceneManager.MoveGameObjectToScene(GameObject.FindGameObjectWithTag("Player"), SceneManager.GetActiveScene());
        }
        
        SceneManager.LoadScene("Ship Hub");
        Time.timeScale = 1;
    }

    // Takes player to tutorial scene
    public void Tutorial( )
    {
        SceneManager.LoadScene( "TutorialScene" );
    }

    // Quits game
    public void Quit()
    {
        Application.Quit();
    }

    // Deletes the saved data from the player's computer files
    public void ClearSaveData()
    {
        SaveSystem.DeletePlayerData();
        SaveSystem.DeleteFabricatorData();
    }

    public void AreYouSure()
    {
        m_areYouSurePanel.SetActive(true);
    }

    public void Cancel()
    {
        m_areYouSurePanel.SetActive(false);
    }

    // Resumes game when paused
    public void Resume()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    // Takes player to main menu
    public void MainMenu()
    {
        SceneManager.MoveGameObjectToScene(GameObject.FindGameObjectWithTag("Player"), SceneManager.GetActiveScene());
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }

    // Takes player to our feedback form url
    public void LinkToSurvey()
    {
        Application.OpenURL("https://forms.gle/d38QM6iLT82SNn3A7");
    }
}
