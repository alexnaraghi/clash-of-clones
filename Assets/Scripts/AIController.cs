using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// He who controls the robots controls mankind.
///
/// Attach to a GameObject with a Player script.
/// </summary>
public class AIController : MonoBehaviour 
{
    /// <summary>
    /// The player to control.
    /// </summary>
    [SerializeField] private Player _player;

    // AI PARAMETERS //
    private const float IntervalBetweenMoves = 2f;
    private readonly Vector3 OffsetFromBuilding = new Vector3(-15f, 0f, 0f);
    /////////////////// 
    
    void Awake()
    {
        _player = GetComponent<Player>();
        Assert.IsNotNull(_player);
    }

    void OnEnable()
    {
        // every couple seconds, try to throw down a card.
        this.InvokeRepeating(tryPlaceUnit, IntervalBetweenMoves, IntervalBetweenMoves);
    }

    void OnDisable()
    {
        this.CancelInvoke();
    }

    void Update()
    {
        // EARLY OUT! //
        if(_player == null) return;

        // If the AI is at max mana, throw down a card.
        if(Mathf.Approximately(_player.Mana, Consts.MaxMana))
        {
            tryPlaceUnit();
        }
    }

    /// <summary>
    /// Right now, we place a random unit, in front of one of our buildings.  If we don't have mana,
    /// just skip this attempt.
    /// </summary>
    private void tryPlaceUnit()
    {
        // EARLY OUT! //
        if(_player == null) return;

        // Choose card.  Random for now.
        CardDefinition randomCard = _player.CardState.GetRandomCardFromHand();

        // If we don't have enough mana, just skip playing the card for now.
        if(_player.CanPlayCard(randomCard) && _player.Buildings.Length > 0)
        {
            // Choose a position.  Let's just do right in front of a static friendly building at random.
            int rand = Random.Range(0, _player.Buildings.Length);

            var building = _player.Buildings[rand];

            if(randomCard != null && building != null && building.Entity != null && building.Entity.HP >= 0)
            {
                _player.PlayCard(randomCard, building.Entity.transform.position + OffsetFromBuilding);
            }
        }
    }
}