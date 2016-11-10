using UnityEngine;
using UnityEngine.UI;

public class DebugGridPrintout : MonoBehaviour 
{
    [SerializeField] private Text _text;

    void Update()
    {
        if(_text != null)
        {
            var gridPoint = TerritoryData.GetGridPositionFromMouse();
            _text.text = string.Format("({0}, {1})", gridPoint.X, gridPoint.Y);
        }
    }
}