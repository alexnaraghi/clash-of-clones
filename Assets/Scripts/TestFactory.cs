using System.Collections.Generic;

/// <summary>
/// For testing, creates some cards that we can use to form decks.
/// </summary>
public static class TestFactory 
{
    public static List<CardData> GetDefaultPlayerDeck()
    {
        return Config.Instance.GetDeckByName("Deck2");
    }

    public static List<CardData> GetDefaultEnemyDeck()
    {
        return Config.Instance.GetDeckByName("Deck1");
    }
}