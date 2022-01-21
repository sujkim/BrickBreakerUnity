using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Game objects
    public Rock rock;
    public Paddle paddle;
    public MLagent agentPaddle;
    public MLrock agentRock;
    public GameObject playerBricks;
    public GameObject agentBricks;
    public GameObject[] levels;

    // On-screen text
    public int playerLives, agentLives;
    public int playerScore, agentScore;
    public Text playerScoreText, agentScoreText;
    public Text playerLivesText, agentLivesText;
    public Text playerLevelText, agentLevelText;
    public TextMeshProUGUI playerLoadLevelText, agentLoadLevelText;

    public GameObject gameOverPanel, gameWonPanel, noLivesPanel, tiedPanel, playerLoadLevelPanel, agentLoadLevelPanel;

    // Game status variables
    public bool playerGameOver, agentGameOver;
    public bool playerGameWon, agentGameWon;
    public bool agentNoLives;
    public bool isMultiplayer;
    public int playerRemainingBricks, agentRemainingBricks;
    GameObject _currentLevel, _agentLevel;
    public bool _isLoading;
    public int playerCurrentLevel, agentCurrentLevel;

    Scene scene;
    public static GameManager Instance { get; private set; }


    // Start is called before the first frame update
    void Start()
    {

        Instance = this;
        playerCurrentLevel = 0;
        agentCurrentLevel = 0;
        scene = SceneManager.GetActiveScene();

        // Single or Multiplayer Mode
        if (scene.name == "2Player") isMultiplayer = true;

        // build scene(s)
        BuildScene("Player");
        playerLivesText.text = "LIVES: " + playerLives;
        playerScoreText.text = "SCORE: " + playerScore;
        UpdateLevel(playerCurrentLevel, "Player");

        if (isMultiplayer)
        {
            BuildScene("Agent");
            agentLivesText.text = "LIVES: " + agentLives;
            agentScoreText.text = "SCORE: " + agentScore;
            UpdateLevel(agentCurrentLevel, "Agent");
        }

        // test load level
        // agentRemainingBricks = -1;
        // playerRemainingBricks = 1;


    }

    private void Update()
    {
        if (Input.GetKey("escape")) QuitGame();
    }

    public void UpdateLives(string player)
    {
        if (player == "Player")
        {
            playerLives--;
            if (playerLives <= 0)
            {
                playerLives = 0;

                if (!isMultiplayer) GameOver();
                else
                {
                    Destroy(rock);
                    CheckGameStatus();
                }
            }
            playerLivesText.text = "LIVES: " + playerLives;
        }
        else
        {
            agentLives--;
            if (agentLives <= 0)
            {
                agentLives = 0;
                noLivesPanel.SetActive(true);
                agentNoLives = true;
                CheckGameStatus();
            }

            agentLivesText.text = "LIVES: " + agentLives;
        }

    }

    public void UpdateScore(int points, string player)
    {
        if (player == "Player")
        {
            playerScore += points;
            playerScoreText.text = "SCORE: " + playerScore;
        }
        else
        {
            agentScore += points;
            agentScoreText.text = "SCORE: " + agentScore;
        }
    }

    private void UpdateLevel(int level, string player)
    {
        if (player == "Player") playerLevelText.text = "LEVEL " + (playerCurrentLevel + 1);
        else agentLevelText.text = "LEVEL " + (agentCurrentLevel + 1);
    }

    public void UpdateBricks(string tag = "Player")
    {
        if (tag == "Player") playerRemainingBricks--;
        else agentRemainingBricks--;
        CheckGameStatus();
    }

    public void BuildScene(string player)
    {
        if (isMultiplayer)
        {
            // instantiate bricks and get brick count for player or agent
            if (player == "Player")
            {
                _currentLevel = Instantiate(levels[playerCurrentLevel], playerBricks.transform, false);
                // _currentLevel = Instantiate(levels[0]);
                playerRemainingBricks = playerBricks.GetComponentsInChildren<Rigidbody2D>().Length;
            }
            else
            {
                _agentLevel = Instantiate(levels[agentCurrentLevel], agentBricks.transform, false);
                // _agentLevel = Instantiate(levels[0]);
                agentRemainingBricks = agentBricks.GetComponentsInChildren<Rigidbody2D>().Length;
            }
        }
        else
        {
            _currentLevel = Instantiate(levels[playerCurrentLevel]);
            playerRemainingBricks = GameObject.FindGameObjectsWithTag("Brick").Length;
        }
    }


    public void CheckGameStatus()
    {

        // Multiplayer game states
        if (isMultiplayer)
        {
            // if player beat all levels first, game won
            if ((playerRemainingBricks <= 0 && playerCurrentLevel == levels.Length - 1) && agentRemainingBricks > 0) GameWon();

            // if agent beat all levels first, game over
            if ((agentRemainingBricks <= 0 && agentCurrentLevel == levels.Length - 1) && !playerGameWon) GameOver();

            // if agent passed a level and player has not yet won, move to next level
            if ((agentRemainingBricks <= 0 && agentCurrentLevel < levels.Length - 1) && !playerGameWon && !_isLoading)
            {
                ResetPosition("Agent");
                StartCoroutine(LoadLevel(2f, "Agent"));
            }

            // if player passed a level and agent has not yet won, move to next level
            if ((playerRemainingBricks <= 0 && playerCurrentLevel < levels.Length - 1) && !agentGameWon && !_isLoading)
            {
                ResetPosition("Player");
                StartCoroutine(LoadLevel(2f, "Player"));
            }

            // if player loses all balls and agent has higher score
            if (playerLives <= 0 && agentScore > playerScore) GameOver();

            // if player loses all balls but has higher score than agent
            if (playerLives <= 0 && playerScore > agentScore) GameWon();

            // if player and agent both lost all balls
            if (playerLives <= 0 && agentLives <= 0)
            {
                if (playerScore > agentScore) GameWon();
                else if (agentScore > playerScore) GameOver();
                else tiedPanel.SetActive(true);
            }
        }

        // single player, load levels 
        else
        {
            // if player beat all levels
            if (playerRemainingBricks <= 0 && playerCurrentLevel == levels.Length - 1) GameWon();
            else if (playerRemainingBricks <= 0 && playerCurrentLevel < levels.Length)
            {
                StartCoroutine(LoadLevel(2f, "Player"));
            }
        }
    }

    IEnumerator LoadLevel(float delay, string player)
    {
        _isLoading = true;

        if (player == "Player")
        {
            // display load level text
            playerLoadLevelPanel.SetActive(true);
            playerLoadLevelText.text = "Level " + (playerCurrentLevel + 2);

            // rock not in play, increase level
            Destroy(_currentLevel);
            rock.inPlay = false;
            playerCurrentLevel++;
            // update level text
            UpdateLevel(playerCurrentLevel, "Player");

            yield return new WaitForSeconds(delay);

            // remove load level panel
            playerLoadLevelPanel.SetActive(false);
        }
        else
        {

            // display load level text
            agentLoadLevelPanel.SetActive(true);
            agentLoadLevelText.text = "Level " + (agentCurrentLevel + 2);

            // rock not in play, increase level
            Destroy(_agentLevel);
            agentRock.MLPlay = false;
            agentCurrentLevel++;
            // update level text
            UpdateLevel(agentCurrentLevel, "Agent");

            yield return new WaitForSeconds(delay);

            // remove load level panel
            agentLoadLevelPanel.SetActive(false);
        }

        // build next level
        BuildScene(player);

        _isLoading = false;

    }

    public void ResetPosition(string player)
    {
        if (player == "Player")
        {
            // freeze rock position
            rock.transform.position = rock.startPos;
            rock.rb.constraints = RigidbodyConstraints2D.FreezeAll;

        }
        else
        {
            // freeze rock position
            agentRock.transform.position = agentRock.startPos;
            agentRock.rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // freeze paddle
            agentPaddle.transform.localPosition = agentPaddle.startPaddle;
            agentPaddle.paddlerb.constraints = RigidbodyConstraints2D.FreezeAll;

        }
    }


    public void PlayAgain()
    {
        SceneManager.LoadScene(scene.name);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void GameWon()
    {
        playerGameWon = true;
        // UpdateHighScore(playerScore);
        gameWonPanel.SetActive(true);
        if (isMultiplayer) agentGameWon = true;

    }


    void GameOver()
    {
        playerGameOver = true;
        // UpdateHighScore(playerScore);
        gameOverPanel.SetActive(true);
        if (isMultiplayer) agentGameOver = true;
    }





}
