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
    private PlayerModel _player;

    // AI PARAMETERS //
    private const float IntervalBetweenMoves = 2f;
    private readonly Vector3 OffsetFromBuilding = new Vector3(-15f, 0f, 0f);
    /////////////////// 

    private float _intervalSeconds;

    void Awake()
    {
        _player = GetComponent<PlayerModel>();
        Assert.IsNotNull(_player);
    }

    void OnEnable()
    {
        _intervalSeconds = IntervalBetweenMoves;
    }

    void OnDisable()
    {
        this.CancelInvoke();
    }

    void Update()
    {
        // EARLY OUT! //
        if(_player == null || !GameModel.Instance.IsPlaying) return;

        _intervalSeconds -= Time.deltaTime;

        // If the timer runs out, or the AI is at max mana, throw down a card.
        if(_intervalSeconds <= 0f || Mathf.Approximately(_player.Mana, Consts.MaxMana))
        {
            _intervalSeconds = IntervalBetweenMoves;
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
        CardData randomCard = _player.CardState.GetRandomCardFromHand();

        // If we don't have enough mana, just skip playing the card for now.
        if(_player.CanPlayCard(randomCard) && _player.Buildings.Length > 0)
        {
            if(randomCard.IsProjectile)
            {
                // Shoot at a random enemy building.
                var enemy = GameModel.Instance.GetOppositePlayer(_player);
                var randomBuilding = enemy.Buildings[Random.Range(0, enemy.Buildings.Length)];
                if(randomBuilding.Entity != null)
                {
                    _player.PlayCard(randomCard, randomBuilding.Entity.transform.position);
                }
            }
            else
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
}