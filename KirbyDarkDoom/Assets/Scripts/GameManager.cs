/* A basic script that handles global game actions. There will ONLY be one of these when the game spawns.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("General Variables")]
    public int lastSceneIndex;      // Used to remember the last level the player was on

    // Private Variables
    private bool gamePause = false;  // Is the game currently paused?

    // Static Variables
    public static GameManager Instance;

    // Getter
    public bool GamePause {
        get {return gamePause;}
    }

    // Makes sure there is only one in the game at all cost and is persistent
	private void Awake()
	{
		if(Instance == null)
        {
            Instance = gameObject.GetComponent<GameManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
	}

	// Takes the player to the GameOver Screen
	public void GoToGameOver()
    {
        lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("GameOver");
    }

    // Find all interactable GameObjects and "stops" them
    public void PauseGame()
    {
        foreach(BaseEnemy currEnemy in FindObjectsOfType<BaseEnemy>())
        {
            currEnemy.StopEnemy();
        }
        foreach(ExhaleProjectile currProjectile in FindObjectsOfType<ExhaleProjectile>())
        {
            currProjectile.enabled = false;
        }

        if(FindObjectOfType<InhaleHitbox>() != null)
        {
            FindObjectOfType<PlayerController>().StopPlayer();
        }

        if(FindObjectOfType<InhaleHitbox>() != null)
        {
            FindObjectOfType<InhaleHitbox>().enabled = false;
        }
    }

    // Does the opposite of PauseGame, but has the option to give the player their og velicity back
    public void ResumeGame(bool usePlayerOrigVelocity)
    {
        foreach(BaseEnemy currEnemy in FindObjectsOfType<BaseEnemy>())
        {
            currEnemy.ResumeEnemy();
        }
        foreach(ExhaleProjectile currProjectile in FindObjectsOfType<ExhaleProjectile>())
        {
            currProjectile.enabled = true;
        }

        if(FindObjectOfType<InhaleHitbox>() != null)
        {
            FindObjectOfType<PlayerController>().ResumePlayer(usePlayerOrigVelocity);
        }

        if(FindObjectOfType<InhaleHitbox>() != null)
        {
            FindObjectOfType<InhaleHitbox>().enabled = true;
        }
    }
}
