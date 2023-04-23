using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;

public class GameBehaviour : MonoBehaviour
{
    /* Game objects */
    public GameObject beer;
    public GameObject bubbleRoot;
    public GameObject game_canvas;
    public GameObject input_canvas;
    public GameObject gameover_canvas;
    public GameObject asian_glow;
    public GameObject body;

    /* Constants */
    private const float beerStartX = 0f;
    private const float beerStartY = -14.5f;
    private const float beerStartZ = -1f;

    private const float bubbleStartY = -9f;
    private const float bubbleStartZ = -2f;
    private const string bubbleRootName = "bubble";

    private const float bubbleSpeed = 0.7f;
    private const float bubbleXMin = -11f;
    private const float bubbleXMax = -0.9f;
    private const float bubbleYMax = 10f;
    private const float moveSpeed = 0.5f;

    /* Classes */
    private class Bubble {
        public GameObject obj;
        public int id { get; set; }
        public string word { get; set; }
    }

    public class Score {
        private int numPop;
        private int level;
    }

    /* Lists */
    private List<Bubble> bubbles = new List<Bubble>();
    private List<int> bubbleIds = new List<int>();
    private List<string> wordsList; // contains 84097 words
    private List<string> currentWords = new List<string>();
    private List<int> scoreboard = new List<int>();

    /* Counters */
    private int numBubbles = 0;
    private int numWordsLeft;
    private int numPop = 0;
    private int level = 1;
    private float gameTimer = 20f;
    private float bubbleTimer = 0f;
    
    /* Modifiers */
    private int fade = 0;
    private float glow = 0f;
    private float bubbleMult = 1f;
    private float singleFill = 0.5f; // 29 fails to gameover

    /* Speed */
    private float accumFill = 0f;

    /* Game State */
    private bool isGameOver = false;
    private bool spacePressed = false;
    public static bool isPaused = false;
    
    /* etc */
    private string input;
    private bool hasRan = false;
    
    void Start() {
        // Activate game scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));

        // Get the game objects
        beer = GameObject.Find("beer");
        game_canvas = GameObject.Find("game_canvas");
        input_canvas = GameObject.Find("input_canvas");
        gameover_canvas = GameObject.Find("gameover_canvas");
        gameover_canvas.SetActive(false);
        asian_glow = GameObject.Find("asian_glow");
        body = GameObject.Find("body");

        // Get starting list
        GetWordList();
        RandomizeBubbleID();

        // Reset the position of the beer
        beer.transform.position = new Vector3(beerStartX, beerStartY, beerStartZ);

        // Set isPaused to false
        isPaused = false;

        // Set data
        SetPlayerNum();
    }

    void SetPlayerNum() {
        // Get existing data
        int playerNum = PlayerPrefs.GetInt("PlayerNum");

        // Increment playerNum
        playerNum += 1;

        // Set data
        PlayerPrefs.SetInt("PlayerNum", playerNum);
    }

    void Update() {
        if (isGameOver) GameOver();
        else if (isPaused) {}
        else {
            KeyboardInput();
            Timer();
            ChangePositions();
        }
    }

    void KeyboardInput() {
        // If user presses ENTER, check if the input matches the word
        if (Input.GetKeyDown(KeyCode.Return)) {
            Debug.Log("Game: Enter is pressed. User input is " + input);

            if (numBubbles == 0) Debug.Log("Game: No bubble yet.");
            else if (input == "") Debug.Log("Game: No input.");
            else CheckFailure(input);
        }
        // If user presses ESC, pause the game
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("PausedScene", LoadSceneMode.Additive);
            isPaused = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            isGameOver = true;
        }
    }

    void Timer() {
        // Create new bubble when bubble timer runs out
        bubbleTimer -= Time.deltaTime * bubbleMult;
        if (bubbleTimer <= 0) {
            bubbleTimer = 5.0f;
            InstantiateBubble();
        }

        // Speed up when game timer runs out
        gameTimer -= Time.deltaTime;
        if (gameTimer <= 0) {
            gameTimer = 20.0f;
            bubbleMult += 0.5f;
            level += 1;
            LevelUp();
        }
    }

    void ChangePositions() {
        // If the beer fills all the way
        if (beer.transform.position.y >= 0.0f) {
            // Game over
            Debug.Log("Beer: Reached max position. GAME OVER");
            isGameOver = true;
        }

        // Fill the beer to the accumulated fill
        if (beer.transform.position.y <= beerStartY + accumFill)
            Fill();
       
        // Move the bubbles
        FloatBubble();
    }

    void Fill() {
        // Fill the beer by 0.05
        beer.transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);  
    }

    void LevelUp() {
        Debug.Log("Game: Level up!");

        // Instantiate level up text
        GameObject levelup = Instantiate(GameObject.Find("levelup_text"));
        
        // Position level up
        levelup.transform.position += new Vector3(0, 13f, 0);
        
        // Destroy after animation
        levelup.GetComponent<DestroyAfterAnimation>().enabled = true;

        // Set level text
        TMP_Text text = GameObject.Find("level_text").GetComponent<TMP_Text>();
        text.SetText("Level: " + level);
    }

    void RandomizeBubbleID() {
        for (int i = 0; i < 10; i++) {
            if (i < 7) bubbleIds.Add(1);
            else bubbleIds.Add(2);
        }
        Debug.Log("Bubble: Starting ids are " + string.Join(", ", bubbleIds));
    }

    void InstantiateBubble() {
        // Get root bubble object
        int bubbleId = bubbleIds[Random.Range(0,9)];
        string bubbleObjName = bubbleRootName + bubbleId;
        Debug.Log("Bubble: New " + bubbleObjName + " created");
        bubbleRoot = GameObject.Find(bubbleObjName);

        // Create new bubble
        GameObject bubbleObj = Instantiate(bubbleRoot);
        numBubbles += 1;
        
        // Position bubble
        bubbleObj.transform.position = new Vector3(Random.Range(bubbleXMin, bubbleXMax), bubbleStartY, bubbleStartZ);

        // Change text
        GameObject bubbleText = bubbleObj.transform.GetChild(0).GetChild(0).gameObject;
        TMP_Text text = bubbleText.GetComponent<TMP_Text>();
        GetRandomWord();
        text.SetText(currentWords[numBubbles-1]);
        text.color = new Color32(60, 60, 60, (byte) (255 - fade)); // Fade text

        // Instantiate new Bubble
        Bubble bubble = new Bubble();
        bubble.obj = bubbleObj;
        bubble.id = bubbleId;
        bubble.word = currentWords[numBubbles-1];

        // Add to list
        bubbles.Add(bubble);

        Debug.Log("Bubble: Current number of bubbles: " + numBubbles + "\n" + "bubbles: " + string.Join(", ", bubbles.Select(b => string.Format("{0}({1})", b.obj, b.word))));
    }

    void DisplayPop() {
        GameObject scoreText = GameObject.Find("score_text");
        TMP_Text text = scoreText.GetComponent<TMP_Text>();
        text.SetText("Popped Bubbles: " + numPop);
    }

    void PopBubble(int index) {
        // Deactivate bubble object
        bubbles[index].obj.SetActive(false);

        // Pop bubble
        GameObject popRoot = GameObject.Find("pop" + bubbles[index].id);
        GameObject pop = Instantiate(popRoot);
        pop.transform.position = bubbles[index].obj.transform.position;
        pop.GetComponent<DestroyAfterAnimation>().enabled = true;

        // Destroy bubble
        DestroyBubble(index);
    }

    void PopAllBubble() {
        for (int i = 0; i < numBubbles; i++)
            PopBubble(i);
    }

    void DestroyBubble(int index) {
        Destroy(bubbles[index].obj);
        
        // Remove bubble
        bubbles.RemoveAt(index);

        // Remove word
        currentWords.RemoveAt(index);

        numBubbles -= 1;
    }

    void DestroyAllBubble() {
        for (int i = 0; i < numBubbles; i++)
            DestroyBubble(i);
    }
    
    void FloatBubble() {
        // Move all bubbles upwards
        for (int i = 0; i < numBubbles; i++) {
            if (bubbles[i].obj.transform.position.y <= bubbleYMax) {
                bubbles[i].obj.transform.position += new Vector3(0, bubbleSpeed * Time.deltaTime * bubbleMult, 0);
            } else {
                // Pop bubble
                DestroyBubble(i);

                // Fill beer
                accumFill += singleFill;

                // Fade bubble
                fade += 8;
                
                AsianGlow();
            }
        }
    }

    void GetWordList() {
        TextAsset txt = (TextAsset)Resources.Load("wordsList");
        string[] words = txt.text.Split("\n"[0]);
        wordsList = new List<string>(words);
        numWordsLeft = wordsList.Count;
    }

    void GetRandomWord() {
        // Get random index in the words list
        int index = Random.Range(0, numWordsLeft-1);
        string word = wordsList[index];
        wordsList.RemoveAt(index);
        numWordsLeft -= 1;
        currentWords.Add(word);
        Debug.Log("Words: Current words are " + string.Join(", ", currentWords));
    }

    void CheckFailure(string s) {
        // Check if string matches the current word list
        bool isMatched = currentWords.Contains(s);

        // Is a failure
        if (!isMatched) {
            accumFill += singleFill;
            fade += 8;
            AsianGlow();
            Debug.Log("Game: No match!");
        } else { // Not a failure. Pop the bubble and add to score
            int matchedIndex = currentWords.FindIndex(a => a.Contains(s));
            PopBubble(matchedIndex);
            if (fade != 0)
                fade -= 8;
            numPop += 1;
            DisplayPop();
        }
    }

    void AsianGlow() {
        // Increase opacity of asian glow
        glow += 0.05f;
        asian_glow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, glow);
    }
    
    public void ReadInput(string s) {
        input = s.Trim().ToLower();
    }

    void GameOver() {
        if (!hasRan) {
            // Destroy all bubble
            if (numBubbles > 0) 
                DestroyAllBubble();

            // Activate/deactivate game components
            input_canvas.SetActive(false);
            gameover_canvas.SetActive(true);
            body.SetActive(false);

            // Save latest score
            SaveLatestScore();

            // Set scoreboard
            SetScoreboard();

            hasRan = true;
        }

        // If space is pressed, open menu
        if (Input.GetKeyDown(KeyCode.Space) && !spacePressed) {
            Debug.Log("Game: Space is pressed. Return to menu");
            GameObject.Find("beer").GetComponent<AudioSource>().Play();
            spacePressed = true;
        } 
        if (spacePressed) {      
            // Empty the beer
            if (beer.transform.position.y >= beerStartY) {
                beer.transform.position -= new Vector3(0, 14.5f * Time.deltaTime, 0);
            }
            // When fully emptied, set isMenu to true
            if (Math.Round(beer.transform.position.y,1) <= beerStartY) {
                // Reset the position of the beer
                beer.transform.position = new Vector3(beerStartX, beerStartY, beerStartZ);

                // Activate/deactivate game components    
                body.SetActive(true);

                // Load menu scene
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("GameScene");
            }
        }
    }

    void SaveLatestScore() {
        // Save score to latest score data
        PlayerPrefs.SetInt("LatestScore", numPop);
    }

    void SetScoreboard() {
        // Get scoreboard
        TMP_Text scoreboardText = GameObject.Find("scoreboard_text").GetComponent<TMP_Text>();

        // Sort scores
        SortScore();

        // Set text
        String msg = "";
        if (scoreboard[0] == numPop)
            msg = "<size=45><b>Amazing! You are in the FIRST place :)</b></size>\n";
        else 
            msg = "<size=45><b>You did not reach the highscore ;-;</b></size>\n";

        String ranking = "";
        if (scoreboard.Count >= 1)
            ranking = "<color=#d1b000><b>1st Place:</b></color> " + scoreboard[0] + " bubbles\n";
        if (scoreboard.Count >= 2)
            ranking = ranking + "<color=#aaa9ad><b>2nd Place:</b></color> " + scoreboard[1] + " bubbles\n";
        if (scoreboard.Count >= 3)
            ranking = ranking + "<color=#b87333><b>3rd Place:</b></color> " + scoreboard[2] + " bubbles";

        scoreboardText.SetText(msg + ranking);

    }

    void SortScore() {
        int playerNum = PlayerPrefs.GetInt("PlayerNum");

        for (int i = 1; i < playerNum; i++) {
            int score = PlayerPrefs.GetInt("Score" + i);
            scoreboard.Add(score);
        }

        scoreboard.Add(numPop);

        scoreboard.Sort();
        scoreboard.Reverse();

        Debug.Log(string.Join(", ", scoreboard));
    }
}
