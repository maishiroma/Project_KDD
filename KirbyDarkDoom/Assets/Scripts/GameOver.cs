/*  This script handles actions taken from the Game Over Screen
 * 
 */

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    // Restarts the player from the last scene they were at when they died
    public void Restart()
    {
        SceneManager.LoadScene(GameManager.Instance.lastSceneIndex);
    }

    // Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
