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

    // Static Variables
    public static GameManager Instance;

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
}
