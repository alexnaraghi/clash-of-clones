using UnityEngine;

/// <summary>
/// Constants.
/// </summary>
public static class Consts
{
    public const string ImagePath = "CardImages/";
    public const string UnitsPath = "Units/";
    public const string UnitGhostsPath = "Ghosts/";

    public const string SpawnClockPrefabPath = "UI/SpawnClock";

    public const int DeckSize = 8;
    public const int HandSize = 4;
    public const float MaxMana = 10f;
    public const float StartMana = 6f;
    public const float ManaRechargePerSecond = 0.6f;

    public const float GridCellWidth = 5f;
    public const float GridCellHeight = 5f;

    // The number of cells in the accessible grid.
    public const int GridWidth = 19;
    public const int GridHeight = 19;

    // Let's just assume for now that building health is the same every match.
    public const int OutpostStartHealth = 1000;
    public const int HQStartHealth = 1700;
    public const float MatchStartSeconds = 60f * 3f;

    public static readonly Vector3 SpawnClockOffset = new Vector3(0f, 12f, 0f);

    /// <summary>
    /// Threshhold on a dot product of our direction and the target direction, 
    /// how close to dead on should we be aiming to fire a shot?
    /// The closer to 0, the more dead on we need to be before firing.
    /// </summary>
    public const float directionThreshholdForProjectileShot = 0.01f;

    /// <summary>
    /// For group spawners, there is a concept of spawning entities in groups of circles.
    /// The number of entities in the smallest circles defines how dense the units will be packed.
    /// </summary>
    public const int SpawnerNumEntitiesInCircle = 3;

    public const float AirUnitHeight = 15f;

    // Letting people test made it clear that sometimes people actually reach a little under the board to
    // place units.  We should allow this, add some area underneath the board to allow it.
    public const float PlacementBufferY = -5f;
}