using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRStandardAssets.Utils
{
    // This class should be added to any gameobject in the scene
    // that should react to input based on the user's gaze.
    // It contains events that can be subscribed to by classes that
    // need to know about input specifics to this gameobject.
    public class VRInteractiveItem : MonoBehaviour
    {
        public readonly UnityEvent OnOver = new UnityEvent();             // Called when the gaze moves over this object
        public readonly UnityEvent OnOut = new UnityEvent();              // Called when the gaze leaves this object
        public readonly UnityEvent OnClick = new UnityEvent();            // Called when click input is detected whilst the gaze is over this object.
        public readonly UnityEvent OnDoubleClick = new UnityEvent();      // Called when double click input is detected whilst the gaze is over this object.
        public readonly UnityEvent OnUp = new UnityEvent();               // Called when Fire1 is released whilst the gaze is over this object.
        public readonly UnityEvent OnDown = new UnityEvent();             // Called when Fire1 is pressed whilst the gaze is over this object.


        protected bool m_IsOver;


        public bool IsOver
        {
            get { return m_IsOver; }              // Is the gaze currently over this object?
        }


        // The below functions are called by the VREyeRaycaster when the appropriate input is detected.
        // They in turn call the appropriate events should they have subscribers.
        public void Over()
        {
            m_IsOver = true;

            OnOver.Invoke();
        }


        public void Out()
        {
            m_IsOver = false;

            OnOut.Invoke();
        }


        public void Click()
        {
            OnClick.Invoke();
        }


        public void DoubleClick()
        {
            OnDoubleClick.Invoke();
        }


        public void Up()
        {
            OnUp.Invoke();
        }


        public void Down()
        {
            OnDown.Invoke();
        }
    }
}