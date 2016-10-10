using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameState
/// </summary>
public class GameState : MonoBehaviour 
{
    // Singleton instance.
    public static GameState Instance;

    [Range(0, 1)]
    public int LocalPlayerNum;

    public float SecondsLeft;

    public Player LeftPlayer;
    public Player RightPlayer;

    [NonSerialized] public Player[] Players;

    public bool IsPlaying;

    public Player MyPlayer
    {
        get
        {
            return Players[LocalPlayerNum];
        }
    }

    public Player EnemyPlayer
    {
        get
        {
            return Players[LocalPlayerNum == 0 ? 1 : 0];
        }
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        // EARLY OUT! //
        if(LeftPlayer == null || RightPlayer == null) return;

        Players = new Player[2];
        Players[0] = LeftPlayer;
        Players[1] = RightPlayer;

        InitGame(TestFactory.CreateDeck(), TestFactory.CreateDeck());
    }

    public void InitGame(List<CardDefinition> leftDeck, List<CardDefinition> rightDeck)
    {
        LeftPlayer.Init("Computer", leftDeck);
        RightPlayer.Init("Alex", rightDeck);

        SecondsLeft = Consts.MatchStartSeconds;
        IsPlaying = true;
    }

    public Player GetOppositePlayer(Player player)
    {
        Player opposite;
        if(MyPlayer == player)
        {
            opposite = EnemyPlayer;
        }
        else
        {
            opposite = MyPlayer;
        }
        return opposite;
    }

    void Update()
    {
        if(IsPlaying)
        {
            SecondsLeft -= Time.deltaTime;

            if(LeftPlayer.HQ.HP <= 0 || RightPlayer.HQ.HP <= 0 || SecondsLeft <= 0f)
            {
                IsPlaying = false;
                Player winner = determineWinner();

                // Print message.
                var message = GameObject.Find("CenterMessage");
                if(message != null)
                {
                    var printer = message.GetComponent<MessagePrinter>();
                    if(printer != null)
                    {
                        if(winner != null)
                        {
                            printer.PrintMessage(winner.Name + " won!");
                        }
                        else
                        {
                            printer.PrintMessage("Tie game!");
                        }
                    }

                    this.Invoke(restart, 3f);
                }
            }
        }
    }

    /// <summary>
    /// Return the winner.  If tie, returns null.
    /// </summary>
    private Player determineWinner()
    {
        int[] scores = new int[2];

        for(int i = 0; i < 2; i++)
        {
            var player = Players[i];
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

        Player winner = null;

        if(scores[0] > scores[1])
        {
            winner = Players[0];
        }
        else if(scores[0] < scores[1])
        {
            winner = Players[1];
        }

        return winner;
    }

    private void restart()
    {
        SceneManager.LoadScene("ClashScene", LoadSceneMode.Single);
    }
}