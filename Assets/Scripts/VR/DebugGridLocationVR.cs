using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Prints out a grid location given a tracking location.
/// </summary>
public class DebugGridLocationVR : MonoBehaviour 
{
    [SerializeField] private Text _text;
    [SerializeField] private GameObject _trackedObject;

    void Update()
    {
        if(_text != null && _trackedObject != null)
        {
            var gridPoint = TerritoryData.GetGridPosition(_trackedObject.transform.position);
            _text.text = string.Format("Grid : ({0}, {1})", gridPoint.X, gridPoint.Y);
        }
    }
}
