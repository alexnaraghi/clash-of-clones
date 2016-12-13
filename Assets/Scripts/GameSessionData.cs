using UnityEngine;
using System.Collections.Generic;

public class GameSessionData : MonoBehaviour 
{
    public List<CardData> PlayerDeck;
    public List<CardData> EnemyDeck;

    public static GameSessionData Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("More than one card config exists!  Destroying...");
            Destroy(this);
        }
    }
}
