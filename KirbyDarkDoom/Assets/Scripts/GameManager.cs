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
    private NormalEnemyHealth[] listOfAllEnemiesInLevel;    // Keeps track of all of the enemies in the level

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

    // We tell this GameObject to listen for new scene changes
	private void OnEnable()
	{
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

    // If for any reason our GameManger is disabled, we make sure we stop listening for new level changes
	private void OnDisable()
	{
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

    // This function is called when the GameManager detects a new scene
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // We take all of the enemies that are in the level and save their references
        // TODO: This might be sloppy, may need to refine depending on player's location
        listOfAllEnemiesInLevel = FindObjectsOfType<NormalEnemyHealth>();
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

        if(FindObjectOfType<PlayerController>() != null)
        {
            FindObjectOfType<PlayerController>().StopPlayer();
        }

        if(FindObjectOfType<InhaleHitbox>() != null)
        {
            FindObjectOfType<InhaleHitbox>().enabled = false;
        }
    }

    // Does the opposite of PauseGame, but has the option to give the player and enemies their og velicity back
    public void ResumeGame(bool usePlayerOrigVelocity, bool useEnemiesOrigVelocity)
    {
        foreach(BaseEnemy currEnemy in FindObjectsOfType<BaseEnemy>())
        {
            currEnemy.ResumeEnemy(useEnemiesOrigVelocity);
        }
        foreach(ExhaleProjectile currProjectile in FindObjectsOfType<ExhaleProjectile>())
        {
            currProjectile.enabled = true;
        }

        if(FindObjectOfType<PlayerController>() != null)
        {
            FindObjectOfType<PlayerController>().ResumePlayer(usePlayerOrigVelocity);
        }

        if(FindObjectOfType<InhaleHitbox>() != null)
        {
            FindObjectOfType<InhaleHitbox>().enabled = true;
        }
    }

    // Respawns all of the enemies in the current level
    public void RespawnAllEnemies()
    {
        foreach(NormalEnemyHealth currEnemy in listOfAllEnemiesInLevel)
        {
            currEnemy.Respawn();
        }
    }
}
