using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
	bool gameHasEnded = false;
    public bool GameEnded { get { return gameHasEnded; } }
    public bool GameWon { get; private set; }
    public bool GameLost { get; private set; }

    public float restartDelay = 1f;

    public GameObject competeLevelUI;
    public GameObject instructionsUI;
    public Button restartButton;
    public TextMeshProUGUI endMessage;
    public string successmessage = "Escaped!";
    public string failmessage = "Died!";
    public RectTransform messageRT, buttonRT;
    public float animationTime = 1.5f;
    public AnimationCurve smoothCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    private void Start()
    {
        restartButton.onClick.AddListener(Restart);
        GameWon = false;
        GameLost = false;
    }

    public void WinGame()
    {
        if (gameHasEnded == false)
        {
            GameWon = true;
            StartCoroutine(ShowEndUI(successmessage));
        }
    }

    public void EndGame()
	{
        if (gameHasEnded == false)
        {
            GameLost = true;
            StartCoroutine(ShowEndUI(failmessage));
        }
    }

    IEnumerator ShowEndUI(string message)
    {
        endMessage.text = message;
        gameHasEnded = true;
        instructionsUI.SetActive(false);
        Vector2 messageEndPos = messageRT.anchoredPosition;
        Vector2 messageStartPos = messageEndPos + (Vector2.up * (150 + messageRT.sizeDelta.y));
        Vector2 buttonEndPos = buttonRT.anchoredPosition;
        Vector2 buttonStartPos = buttonEndPos - (Vector2.up * (150 + messageRT.sizeDelta.y));

        messageRT.anchoredPosition = messageStartPos;
        buttonRT.anchoredPosition = buttonStartPos;
        competeLevelUI.SetActive(true);

        float t = 0;
        float dt = 1f / animationTime;
        while (t <= 1)
        {
            messageRT.anchoredPosition = Vector2.LerpUnclamped(messageStartPos, messageEndPos, smoothCurve.Evaluate(t));
            buttonRT.anchoredPosition = Vector2.LerpUnclamped(buttonStartPos, buttonEndPos, smoothCurve.Evaluate(t));

            t += dt * Time.deltaTime;
            yield return null;
        }

        messageRT.anchoredPosition = messageEndPos;
        buttonRT.anchoredPosition = buttonEndPos;
    }

    void Restart()
    {
        int sceneId = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneId);
    }
}
