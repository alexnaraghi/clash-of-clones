using UnityEngine;
using System.Collections;

public enum SliderDimension
{
    X,
    Y,
    Z
}


/// <summary>
/// A slider using the transform of a 3D object.
/// </summary>
public class TransformSlider : MonoBehaviour 
{
    [SerializeField] private SliderDimension Direction;

    private Vector3 _fullLocalPosition;
    private Vector3 _fullLocalScale;

    public float Value
    {
        get
        {
            float fullScale = getSlidingScale(_fullLocalScale);

            // EARLY OUT! //
            if(fullScale == 0f)
            {
                return 0f;
            }

            return getSlidingScale(transform.localScale) / fullScale;
        }
        set
        {
            var fullSlidingScale = getSlidingScale(_fullLocalScale);
            float desiredScale = fullSlidingScale * value;
            setSlidingScale(desiredScale);
            setSlidingPosition(_fullLocalPosition, desiredScale - fullSlidingScale);
        }
    }

    // Use this for initialization
    private void Awake () 
    {
        _fullLocalPosition = transform.localPosition;
        _fullLocalScale = transform.localScale;
    }

    private float getSlidingScale(Vector3 scale)
    {
        switch(Direction)
        {
            case SliderDimension.X:
                return scale.x;
            case SliderDimension.Y:
                return scale.y;
            case SliderDimension.Z:
                return scale.z;
        }

        return 0f;
    }

    private void setSlidingScale(float scale)
    {
        var old = transform.localScale;
        switch(Direction)
        {
            case SliderDimension.X:
                transform.localScale = new Vector3(scale, old.y, old.z);
                break;
            case SliderDimension.Y:
                transform.localScale = new Vector3(old.x, scale, old.z);
                break;
            case SliderDimension.Z:
                transform.localScale = new Vector3(old.x, old.y, scale);
                break;
        }
    }

    private void setSlidingPosition(Vector3 originalPosition, float slidingDelta)
    {
        var old = transform.localPosition;
        switch(Direction)
        {
            case SliderDimension.X:
                transform.localPosition = originalPosition + new Vector3(slidingDelta, old.y, old.z);
                break;
            case SliderDimension.Y:
                transform.localPosition = originalPosition + new Vector3(old.x, slidingDelta, old.z);
                break;
            case SliderDimension.Z:
                transform.localPosition = originalPosition + new Vector3(old.x, old.y, slidingDelta);
                break;
        }
    }
}
