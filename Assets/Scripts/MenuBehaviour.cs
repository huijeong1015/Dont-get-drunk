using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour
{
    /* Game Object */
    public GameObject beer;
    public GameObject key_text;
    public GameObject start_text;
    public GameObject asian_glow;
    public GameObject lip;
    public GameObject arm;
    
    /* Constants */
    private const float beerStartX = 0.0f;
    private const float beerStartY = -14.5f;
    private const float beerStartZ = -1.0f;   
   
    /* States */
    private bool spacePressed = false;
    private bool fillEmpty = false; // fill = false; empty = true 
    
    void Start()
    {
        beer = GameObject.Find("beer");
        key_text = GameObject.Find("key_text"); 
        start_text = GameObject.Find("start_text");
        asian_glow = GameObject.Find("asian_glow");
        lip = GameObject.Find("lip");
        arm = GameObject.Find("r_arm");

        key_text.SetActive(true);
        start_text.SetActive(false);

        asian_glow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        lip.GetComponent<Animator>().enabled = false;
        arm.GetComponent<Animator>().enabled = false;

        SaveScore();
    }

    void SaveScore() {
        int sessionCount = PlayerPrefs.GetInt("unity.player_session_count");
        bool keyExists = PlayerPrefs.HasKey("PlayerNum");

        if (sessionCount == 0 && !keyExists) {
            PlayerPrefs.SetInt("LatestScore", 0);
            PlayerPrefs.SetInt("PlayerNum", 0);
        } else {
            int latestScore = PlayerPrefs.GetInt("LatestScore");
            int playerNum = PlayerPrefs.GetInt("PlayerNum");

            // Set latest score
            PlayerPrefs.SetInt("Score" + playerNum, latestScore);

            // Save data
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        // Press space to start game
        if (Input.GetKeyDown(KeyCode.Space) && !spacePressed) {
            Debug.Log("Game: Space is pressed. Game Start!");
            key_text.SetActive(false);
            start_text.SetActive(true);
            beer.GetComponent<AudioSource>().Play();
            spacePressed = true;
        }
        if (spacePressed)
            SpacePressed();
    }

    void SpacePressed() {
        if (!fillEmpty) {
            // Fill the beer
            if (beer.transform.position.y <= 0.0f) {
                beer.transform.position += new Vector3(0, 15f * Time.deltaTime, 0);
            }
            // When fully filled, activate/deactivate game components
            if (Math.Round(beer.transform.position.y,1) >= 0.0f) {
                fillEmpty = true;
            }
        } else {
            // Empty the beer
            if (beer.transform.position.y >= beerStartY) {
                beer.transform.position -= new Vector3(0, 15f * Time.deltaTime, 0);
            }
            // When fully emptied, set isMenu to false
            if (Math.Round(beer.transform.position.y,1) <= beerStartY) {
                beer.transform.position = new Vector3(beerStartX, beerStartY, beerStartZ);
                lip.GetComponent<Animator>().enabled = true;
                arm.GetComponent<Animator>().enabled = true;
                LoadGameScene();
            }
        }
    }

    void LoadGameScene() {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MenuScene");
    }
}
