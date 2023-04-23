using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedBehaviour : MonoBehaviour
{
    public GameObject input_canvas;
    void Start()
    {
        input_canvas = GameObject.Find("input_canvas");
        input_canvas.SetActive(false);
    }

    public void ResumeClicked() {
        Debug.Log("Game: Resume");

        input_canvas.SetActive(true);
        GameBehaviour.isPaused = false;
        SceneManager.UnloadSceneAsync("PausedScene");
    }

    public void MenuClicked() {
        Debug.Log("Game: Return to menu");

        SceneManager.LoadScene("MenuScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("PausedScene");
        SceneManager.UnloadSceneAsync("GameScene");
    }
}
