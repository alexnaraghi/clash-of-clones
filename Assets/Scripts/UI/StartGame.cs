using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour 
{
    public DeckSelection Selection;

    public void Begin()
    {
        if(Selection != null && Selection.IsDeckComplete())
        {
            var deck = Selection.GetDeckList();
            GameSessionData.Instance.PlayerDeck = deck;
            GameSessionData.Instance.EnemyDeck = TestFactory.GetDefaultEnemyDeck();
            SceneManager.LoadScene("ClashScene");
        }
        else
        {
            // TODO: Tell the player to select the right number of cards.
        }
    }
}
