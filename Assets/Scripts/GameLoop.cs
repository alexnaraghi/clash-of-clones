using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the core game loop/lifecycle.
/// </summary>
public class GameLoop : MonoBehaviour 
{
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
        StartCoroutine(Loop());
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
        yield return SL.Get<FigurineTutorial>().StepTutorial();

        SL.Get<GameModel>().InitGame(TestFactory.GetDefaultEnemyDeck(), TestFactory.GetDefaultPlayerDeck());
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
                SL.Get<MessageUI>().PrintMessage(Mathf.Max(1f, Mathf.RoundToInt(SL.Get<GameModel>().SecondsLeft)).ToString());
            }

            yield return null;
        }

        SL.Get<MessageUI>().HideMessage();
        SL.Get<GameModel>().IsPlaying = false;
    }

    private IEnumerator RoundEnding()
    {
        SL.Get<GameModel>().GameOverEvent.Invoke();
        PlayerModel winner = determineWinner();

        // Print message.
        if(winner != null)
        {
            SL.Get<MessageUI>().PrintMessage(winner.Name + " won!");
        }
        else
        {
            SL.Get<MessageUI>().PrintMessage("Tie game!");
        }

        // Do a cool effect.

        yield return new WaitForSeconds(3f);

        SL.Get<MessageUI>().ShowGameOverUI();
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