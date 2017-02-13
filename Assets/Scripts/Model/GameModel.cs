using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Root object of all game-state.  Adding/removing entities, player managemment, lifecycle
/// should be done through here.  Although, perhaps we can break out lifecycle if it gets unweildy.
/// </summary>
public class GameModel : MonoBehaviour 
{
    // The parent of all board objects
    public GameObject BoardRoot;

    [Range(0, 1)] 
    public int LocalPlayerNum;

    public float SecondsLeft;

    public PlayerModel LeftPlayer;
    public PlayerModel RightPlayer;

    /// <summary>
    /// Convenience array for iterating on players.
    /// </summary>
    [NonSerialized] public PlayerModel[] Players;

    /// <summary>
    /// If the game currently in progress?
    /// </summary>
    public bool IsPlaying;

    public UnityEvent GameStartedEvent;
    public UnityEvent GameOverEvent;

    /// <summary>
    /// The human controlled player.
    /// </summary>
    public PlayerModel MyPlayer
    {
        get
        {
            return Players[LocalPlayerNum];
        }
    }

    /// <summary>
    /// The AI.
    /// </summary>
    public PlayerModel EnemyPlayer
    {
        get
        {
            return Players[LocalPlayerNum == 0 ? 1 : 0];
        }
    }

    void Awake()
    {
        // EARLY OUT! //
        if (LeftPlayer == null || RightPlayer == null)
        {
            Debug.LogWarning("Need two players to have a working game state.");
            return;
        }

        // EARLY OUT! //
        if(BoardRoot == null)
        {
            Debug.LogWarning("Need board root to have a working game state.");
            return;
        }

        Players = new PlayerModel[2];
        Players[0] = LeftPlayer;
        Players[1] = RightPlayer;
    }

    public void InitGame(List<CardData> leftDeck, List<CardData> rightDeck)
    {
        LeftPlayer.Init("Computer", leftDeck);
        RightPlayer.Init("You", rightDeck);

        SecondsLeft = Consts.MatchStartSeconds;
        IsPlaying = true;
    }

    /// <summary>
    /// Get the player that is not the given player.
    /// </summary>
    public PlayerModel GetOppositePlayer(PlayerModel player)
    {
        PlayerModel opposite;
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
}