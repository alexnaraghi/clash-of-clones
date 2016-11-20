using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;

/// <summary>
/// Controls the core game loop/lifecycle.
/// </summary>
[RequireComponent(typeof(GameModel))]
public class GameLoop : MonoBehaviour 
{
    [SerializeField] private MessageUI _messagePrinter;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private Camera _camera;
    [SerializeField] private Animation _cardPanelAnimation;
    [SerializeField] private Animation _timerAnimation;
    [SerializeField] private VRCameraFade _cameraFade;

    // The scene name to load when the game ends.
    [SerializeField] private string _sceneToLoad = "ClashScene";

    private const float INITIAL_ZOOM = 50f;
    private const float GAMEPLAY_ZOOM = 44.51f;

    private GameModel _model;

    private bool _isFadingOut;

    public void RestartGame()
    {
        StartCoroutine(endGame());
    }

    void Awake()
    {
        _model = GetComponent<GameModel>();
    }

    void Start()
    {
        // EARLY OUT! //
        if(_model == null)
        {
            Debug.LogWarning("Game loop requires game model");
            return;
        }

        StartCoroutine(Loop());
    }

    private IEnumerator endGame()
    {
        // EARLY OUT //
        if(_isFadingOut)
        {
            yield break;
        }

        _isFadingOut = true;

        // Wait for the camera to fade out.
        yield return StartCoroutine(_cameraFade.BeginFadeOut(true));

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

        _camera.orthographicSize = INITIAL_ZOOM;

        // Do some camera zoom in effect.
        while(_camera.orthographicSize > GAMEPLAY_ZOOM)
        {
            const float zoomPerSecond = 3f;
            _camera.orthographicSize -= zoomPerSecond * Time.deltaTime;
            yield return null;
        }

        _cardPanelAnimation.gameObject.SetActive(true);
        _cardPanelAnimation.Play();
        _timerAnimation.Play();
        yield return _cardPanelAnimation.isPlaying;

        _messagePrinter.HideMessage();

        // Just use default decks for now.
        _model.InitGame(TestFactory.GetDefaultDeck(), TestFactory.GetDefaultDeck());
        _model.GameStartedEvent.Invoke();
    }

    private IEnumerator RoundInProgress()
    {
        while(!isGameOver())
        {
            _model.SecondsLeft -= Time.deltaTime;

            if(_model.SecondsLeft <= 10f)
            {
                _messagePrinter.PrintMessage(Mathf.Max(1f, Mathf.RoundToInt(_model.SecondsLeft)).ToString());
            }

            yield return null;
        }
        _messagePrinter.HideMessage();

        _model.IsPlaying = false;
    }

    private IEnumerator RoundEnding()
    {
        _model.GameOverEvent.Invoke();
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

        // Do some camera zoom out effect.
        while(_camera.orthographicSize < INITIAL_ZOOM)
        {
            const float zoomPerSecond = 3f;
            _camera.orthographicSize += zoomPerSecond * Time.deltaTime;
            yield return null;
        }

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
        return _model.LeftPlayer.HQ.HP <= 0 || _model.RightPlayer.HQ.HP <= 0 || _model.SecondsLeft <= 0f;
    }

    /// <summary>
    /// Return the winner.  If tie, returns null.
    /// </summary>
    private PlayerModel determineWinner()
    {
        int[] scores = new int[2];

        for(int i = 0; i < 2; i++)
        {
            var player = _model.Players[i];
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
            winner = _model.Players[0];
        }
        else if(scores[0] < scores[1])
        {
            winner = _model.Players[1];
        }

        return winner;
    }
}