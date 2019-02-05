/*  This script handles all of the events that occur on special screens.
 *  They will be toggled on which ones to use depending on the selected Enum
 *  This should only be used on whatver screens that are listed in the enums.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ScreenType {
    TITLE_SCREEN,
    RESULT_SCREEN,
    GAMEOVER_SCREEN
}


public class ScreenActions : MonoBehaviour {

    public ScreenType typeOfScreen;

    // Goto the specified scene
    public void GoToScene(int sceneIndex)
    {
        if(typeOfScreen == ScreenType.GAMEOVER_SCREEN)
        {
            // If we are on the GameOver Screen, we ignore the passed in parameter
            SceneManager.LoadScene(GameManager.Instance.lastSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    // Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
