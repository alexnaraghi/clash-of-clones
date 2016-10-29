using System.Collections.Generic;

/// <summary>
/// For testing, creates some cards that we can use to form decks.
/// </summary>
public static class TestFactory 
{
    public static List<CardData> GetDeck1()
    {
        return Config.Instance.GetDeckByName("Deck1");
    }
}