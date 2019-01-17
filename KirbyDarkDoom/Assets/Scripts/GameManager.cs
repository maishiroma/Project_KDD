/* A basic script that handles global game stuff WIP
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("General Variables")]
    public int lastSceneIndex;

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
