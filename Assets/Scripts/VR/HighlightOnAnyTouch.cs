using UnityEngine;
using VRTK;
using VRTK.Highlighters;
using System.Collections;
using System;

public class HighlightOnAnyTouch : MonoBehaviour 
{
    public Color HighlightColor;

    [Range(0f, 1f)]
    public float HapticPulseStrength;

    private VRTK_InteractTouch _touch;
    private VRTK_ControllerActions _actions;
    private CustomHoverHighlight _highlight;

    private void OnEnable()
    {
        StartCoroutine(WaitForModel());
    }


        private IEnumerator WaitForModel()
        {
            while (transform.parent.name == "[VRTK]")
            {
                yield return null;
            }

            _actions = GetComponent<VRTK_ControllerActions>();
            _touch = GetComponent<VRTK_InteractTouch>();
            _highlight = transform.parent.GetComponentInChildren<CustomHoverHighlight>();

            // EARLY OUT! //
            if(_actions == null )
            {
                Debug.LogWarning("VRTK_BaseHighlighter and VRTK_ControllerActions must be attached.");
                yield break;
            }

            _touch.ControllerTouchInteractableObject += onTouched;
            _touch.ControllerUntouchInteractableObject += onUntouched;
            _highlight.Initialize(1);
        }

    private void OnDestroy()
    {
        if(_touch != null)
        {
            _touch.ControllerTouchInteractableObject -= onTouched;
            _touch.ControllerUntouchInteractableObject -= onUntouched;
        }
    }

    private void onTouched(object sender, ObjectInteractEventArgs e)
    {
        _actions.TriggerHapticPulse(HapticPulseStrength);
        _highlight.ShowHighlight();
    }

    private void onUntouched(object sender, ObjectInteractEventArgs e)
    {
        _actions.TriggerHapticPulse(HapticPulseStrength);
        _highlight.HideHighlight();
    }
}
