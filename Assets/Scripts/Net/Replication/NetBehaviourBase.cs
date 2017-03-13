using UnityEngine;

public abstract class NetBehaviourBase : MonoBehaviour 
{
    [HideInInspector]
    public NetObject NetObject;

    /// <summary>
    /// The number of states to hold onto.
    /// </summary>
    private const int BufferedStateCount = 32;
    public readonly CircularBuffer<object> PreviousStates = new CircularBuffer<object>(BufferedStateCount);

    /// <summary>
    /// The last known accepted state of a networked behavior.
    /// Everything that occured to the game object after this represents a predicted state.
    /// </summary>
    public object LatestState
    {
        get
        {
            return PreviousStates.Back();
        }
    }

    public int Id
    {
        get;
        private set;
    }

    public virtual void Init(int id)
    {
        Id = id;
    }

    /// <summary>
    /// Apply a new replicated state to the object.  Update the last replicated state.
    /// </summary>
    /// <param name="state">The deserialized state.</param>
    public virtual void Apply(object state)
    {
        PreviousStates.PushBack(state);
        Rollback();
    }

    /// <summary>
    /// Is the current state of the object different than the latest state, or in other words, has the
    /// predicted state diverged from the latest state?
    /// </summary>
    public abstract bool HasDelta();

    /// <summary>
    /// Rolls this behavior back to the last replicated state.
    /// </summary>
    public abstract void Rollback();

    /// <summary>
    /// Get the current state from prediction-land.  This should only be used on host behaviors or behaviors
    /// with "local authority", and sent to other clients.  A predicted sample from a non-authoritative client
    /// doesn't reflect anything "true" about the networked state, only what the client thinks could happen.
    /// </summary>
    public abstract object Sample();

    private void Start () 
    {
        NetObject = GetComponent<NetObject>();
    }
}
