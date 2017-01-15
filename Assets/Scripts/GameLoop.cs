using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the core game loop/lifecycle.
/// </summary>
public class GameLoop : MonoBehaviour 
{
    [SerializeField] private MessageUI _messagePrinterPrefab;

    private MessageUI _messagePrinter;
    private GameObject _gameOverMenu;

    // The scene name to load when the game ends.
    [SerializeField] private string _sceneToLoad = "ClashScene";

    private const float INITIAL_ZOOM = 50f;
    private const float GAMEPLAY_ZOOM = 44.51f;

    private bool _isFadingOut;

    public void RestartGame()
    {
        StartCoroutine(endGame());
    }

    void Start()
    {
        // EARLY OUT! //
        if(_messagePrinterPrefab == null)
        {
            Debug.LogWarning("Require message printer.");
            return;
        }

        initPrefabs();
        StartCoroutine(Loop());
    }

    private void initPrefabs()
    {
        _messagePrinter = Instantiate(_messagePrinterPrefab);
        _gameOverMenu = _messagePrinter.transform.Find("GameOver").gameObject;
        _gameOverMenu.SetActive(false);
    }

    private IEnumerator endGame()
    {
        // Do something when game ends.

        yield return null;

        SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Single);

    }

    private IEnumerator Loop()
    {
        yield return (RoundStarting());
        yield return (RoundInProgress());
        yield return (RoundEnding());
    }

    private IEnumerator RoundStarting()
    {
        var messageObj = GameObject.Find("CenterMessage");
        if(messageObj != null)
        {
            _messagePrinter = messageObj.GetComponent<MessageUI>();
            if(_messagePrinter != null)
            {
                _messagePrinter.PrintMessage("Clash of Clones");
            }
        }

        // Do a cool effect.

        yield return new WaitForSeconds(2f);

        //_messagePrinter.HideMessage();

        SL.Get<GameModel>().InitGame(TestFactory.GetDefaultPlayerDeck(), TestFactory.GetDefaultEnemyDeck());
        //_model.InitGame(GameSessionData.Instance.EnemyDeck, GameSessionData.Instance.PlayerDeck);

        SL.Get<GameModel>().GameStartedEvent.Invoke();
    }

    private IEnumerator RoundInProgress()
    {
        while(!isGameOver())
        {
            SL.Get<GameModel>().SecondsLeft -= Time.deltaTime;

            if(SL.Get<GameModel>().SecondsLeft <= 10f)
            {
                _messagePrinter.PrintMessage(Mathf.Max(1f, Mathf.RoundToInt(SL.Get<GameModel>().SecondsLeft)).ToString());
            }

            yield return null;
        }

        if(_messagePrinter != null)
        {
            _messagePrinter.HideMessage();
        }

        SL.Get<GameModel>().IsPlaying = false;
    }

    private IEnumerator RoundEnding()
    {
        SL.Get<GameModel>().GameOverEvent.Invoke();
        PlayerModel winner = determineWinner();

        // Print message.
        if(_messagePrinter != null)
        {
            if(winner != null)
            {
                _messagePrinter.PrintMessage(winner.Name + " won!");
            }
            else
            {
                _messagePrinter.PrintMessage("Tie game!");
            }
        }

        // Do a cool effect.

        yield return new WaitForSeconds(3f);

        if(_messagePrinter != null)
        {
            _messagePrinter.HideMessage();
        }

        if(_gameOverMenu != null)
        {
            _gameOverMenu.SetActive(true);
        }
    }

    private bool isGameOver()
    {
        return SL.Get<GameModel>().LeftPlayer.HQ.HP <= 0 
            || SL.Get<GameModel>().RightPlayer.HQ.HP <= 0 
            || SL.Get<GameModel>().SecondsLeft <= 0f;
    }

    /// <summary>
    /// Return the winner.  If tie, returns null.
    /// </summary>
    private PlayerModel determineWinner()
    {
        int[] scores = new int[2];

        for(int i = 0; i < 2; i++)
        {
            var player = SL.Get<GameModel>().Players[i];
            if(player.HQ.HP > 0)
            {
                scores[i]++;

                if(player.TopOutpost.HP > 0)
                {
                    scores[i]++;
                }

                if(player.BottomOutpost.HP> 0)
                {
                    scores[i]++;
                }
            }
        }

        PlayerModel winner = null;

        if(scores[0] > scores[1])
        {
            winner = SL.Get<GameModel>().Players[0];
        }
        else if(scores[0] < scores[1])
        {
            winner = SL.Get<GameModel>().Players[1];
        }

        return winner;
    }
}