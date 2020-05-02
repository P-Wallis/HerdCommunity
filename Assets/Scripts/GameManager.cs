using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	bool gameHasEnded = false;

	public float restartDelay = 1f;

    public GameObject competeLevelUI;

    public void CompleteLevel()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            //Debug.Log("YAY!!!!!");
            competeLevelUI.SetActive(true);
        }
    }

    public void EndGame()
	{
        if (gameHasEnded == false)
        {
			gameHasEnded = true;
			Debug.Log("Game Over");
            Invoke("Restart", restartDelay);
        }
    }

    void Restart()
    {
        int sceneId = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneId);
    }
}
