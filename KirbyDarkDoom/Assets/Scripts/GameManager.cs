/* A basic script that handles global game actions. There will ONLY be one of these when the game spawns.
 *  This specifically controls respawning enemies and blocks, level transition speficic actions, and game pauses
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("General Variables")]
    public int lastSceneIndex;                              // Used to remember the last level the player was on
    public int goalSceneIndex;                              // The index of the scene that is used to go to the goal scene

    // Private Variables
    private bool gamePause = false;  // Is the game currently paused?
    private BaseEnemy[] listOfAssociatedEnemies;             // Keeps track of all of the enemies the GameManager is associated with
    private BlockHealth[] listOfAssociatedBlocks;            // Keeps track of all of the blocks the GameManager is associated with

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
        // We take all of the enemies and blocks that are currently active and save their references
        // This should only save the first area enemies since they are the ones that are currently activated.
        // Make sure only the first area's enemies anc blocks are activated

        listOfAssociatedEnemies = FindObjectsOfType<BaseEnemy>();
        listOfAssociatedBlocks = FindObjectsOfType<BlockHealth>();
    }

    // Takes the player to the Result Screen
    public void GoToGoalScene()
    {
        SceneManager.LoadScene("ResultScreen");
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

    // When called, reasssociates the passed enemy list and block list with the GameManager
    public void AssociateNewEnemyAndBlocks(GameObject newEnemyList, GameObject newBlockList)
    {
        // If either list is deactivated, we activate the and then we connect their contents to the GameManager
        if(newEnemyList != null)
        {
            newEnemyList.SetActive(true);
            listOfAssociatedEnemies = newEnemyList.GetComponentsInChildren<BaseEnemy>(true);
        }
        if(newBlockList != null)
        {
            newBlockList.SetActive(true);
            listOfAssociatedBlocks = newBlockList.GetComponentsInChildren<BlockHealth>(true);
        }
    }

    // Respawns all of the enemies that are associated with the GameManager
    public void RespawnAssociatedEnemies()
    {
        foreach(BaseEnemy currEnemy in listOfAssociatedEnemies)
        {
            if(currEnemy != null)
            {
                currEnemy.GetComponent<BaseHealth>().Respawn();
            }
        }
    }

    // Respawns all of the destructable blocks that are associated with the GameManager
    public void RespawnAssociatedBlocks()
    {
        foreach(BlockHealth currBlock in listOfAssociatedBlocks)
        {
            if(currBlock != null)
            {
                currBlock.Respawn();
            }
        }
    }

    // When called, removed all of the enemies that are currently associated with this GameManager
    public void DefeatAssociatedEnemies()
    {
        foreach(BaseEnemy currEnemy in listOfAssociatedEnemies)
        {
            if(currEnemy != null)
            {
                currEnemy.GetComponent<BaseHealth>().DyingAction();
            }
        }
    }

    // When called, removed all of the blocks that are currently associated with this GameManager
    public void DefeatAssociaedBlocks()
    {
        foreach(BlockHealth currBlock in listOfAssociatedBlocks)
        {
            if(currBlock != null)
            {
                currBlock.DyingAction();
            }
        }
    }
}
