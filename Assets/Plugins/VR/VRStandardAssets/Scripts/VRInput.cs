using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRStandardAssets.Utils
{
    //Swipe directions
    public enum SwipeDirection
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    public class SwipeEvent : UnityEvent<SwipeDirection> { }

    // This class encapsulates all the input required for most VR games.
    // It has events that can be subscribed to by classes that need specific input.
    // This class must exist in every scene and so can be attached to the main
    // camera for ease.
    public class VRInput : MonoBehaviour
    {
        public readonly SwipeEvent OnSwipe = new SwipeEvent();   // Called every frame passing in the swipe, including if there is no swipe.
        public readonly UnityEvent OnClick = new UnityEvent();                   // Called when Fire1 is released and it's not a double click.
        public readonly UnityEvent OnDown = new UnityEvent();                    // Called when Fire1 is pressed.
        public readonly UnityEvent OnUp = new UnityEvent();                      // Called when Fire1 is released.
        public readonly UnityEvent OnDoubleClick = new UnityEvent();             // Called when a double click is detected.
        public readonly UnityEvent OnCancel = new UnityEvent();                  // Called when Cancel is pressed.


        [SerializeField] private float m_DoubleClickTime = 0.3f;    //The max time allowed between double clicks
        [SerializeField] private float m_SwipeWidth = 0.3f;         //The width of a swipe

        // Minimum threshhold to consider a move a swipe.
        // Bounds are strange, see this forum comment:
        /*
            reading raw Input.mousePosition.x and Input.mousePosition.y

            A simple tap, or a tap and hold, returns static coordinates,
            no matter where on the pad the tap occurs:
            x: 1280, y: 720
            (note that these map to an exact midpoint of the reported screen resolution of 2560x1440)

            swiping FORWARD (from ear towards visorplate) on the pad spans a range of values from roughly
            1280 on the far back to ~1000 on the far front (-280)

            swiping BACK (from faceplate towards ear) on the pad spans a range of values:
            1280 on the front to ~1600 on the far back (+320)

            swiping DOWN on the pad gives a range of
            720 on the top to ~440 on the bottom (-280)

            swiping UP on the pad gives a range of
            720 on the bottom to ~1040 at the top (+320)
        */
        [SerializeField] private float _minimumSwipeLength = 100f;  

        
        private Vector2 m_MouseDownPosition;                        // The screen position of the mouse when Fire1 is pressed.
        private Vector2 m_MouseUpPosition;                          // The screen position of the mouse when Fire1 is released.
        private float m_LastMouseUpTime;                            // The time when Fire1 was last released.
        private float m_LastHorizontalValue;                        // The previous value of the horizontal axis used to detect keyboard swipes.
        private float m_LastVerticalValue;                          // The previous value of the vertical axis used to detect keyboard swipes.


        public float DoubleClickTime{ get { return m_DoubleClickTime; } }


        private void Update()
        {
            CheckInput();
        }


        private void CheckInput()
        {
            // Set the default swipe to be none.
            SwipeDirection swipe = SwipeDirection.NONE;

            if (Input.GetButtonDown("Fire1"))
            {
                // When Fire1 is pressed record the position of the mouse.
                m_MouseDownPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
                // If anything has subscribed to OnDown call it.
                OnDown.Invoke();
            }

            // This if statement is to gather information about the mouse when the button is up.
            if (Input.GetButtonUp ("Fire1"))
            {
                // When Fire1 is released record the position of the mouse.
                m_MouseUpPosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

                // Detect the direction between the mouse positions when Fire1 is pressed and released.
                swipe = DetectSwipe ();
            }

            // If there was no swipe this frame from the mouse, check for a keyboard swipe.
            if (swipe == SwipeDirection.NONE)
                swipe = DetectKeyboardEmulatedSwipe();

            // If there are any subscribers to OnSwipe call it passing in the detected swipe.
                OnSwipe.Invoke(swipe);

            // This if statement is to trigger events based on the information gathered before.
            if(Input.GetButtonUp ("Fire1"))
            {
                // If anything has subscribed to OnUp call it.
                    OnUp.Invoke();

                // If the time between the last release of Fire1 and now is less
                // than the allowed double click time then it's a double click.
                if (Time.time - m_LastMouseUpTime < m_DoubleClickTime)
                {
                    // If anything has subscribed to OnDoubleClick call it.
                    OnDoubleClick.Invoke();
                }
                else
                {
                    // If it's not a double click, it's a single click.
                    // If anything has subscribed to OnClick call it.
                    OnClick.Invoke();
                }

                // Record the time when Fire1 is released.
                m_LastMouseUpTime = Time.time;
            }

            // If the Cancel button is pressed and there are subscribers to OnCancel call it.
            if (Input.GetButtonDown("Cancel"))
            {
                OnCancel.Invoke();
            }
        }


        private SwipeDirection DetectSwipe ()
        {
            Vector2 velocity = m_MouseUpPosition - m_MouseDownPosition;

            // Get the direction from the mouse position when Fire1 is pressed to when it is released.
            Vector2 swipeData = velocity.normalized;

            if(velocity.magnitude > _minimumSwipeLength)
            {
                // If the direction of the swipe has a small width it is vertical.
                bool swipeIsVertical = Mathf.Abs (swipeData.x) < m_SwipeWidth;

                // If the direction of the swipe has a small height it is horizontal.
                bool swipeIsHorizontal = Mathf.Abs(swipeData.y) < m_SwipeWidth;

                // If the swipe has a positive y component and is vertical the swipe is up.
                if (swipeData.y > 0f && swipeIsVertical)
                    return SwipeDirection.UP;

                // If the swipe has a negative y component and is vertical the swipe is down.
                if (swipeData.y < 0f && swipeIsVertical)
                    return SwipeDirection.DOWN;

                // If the swipe has a positive x component and is horizontal the swipe is right.
                if (swipeData.x > 0f && swipeIsHorizontal)
                    return SwipeDirection.RIGHT;

                // If the swipe has a negative x component and is vertical the swipe is left.
                if (swipeData.x < 0f && swipeIsHorizontal)
                    return SwipeDirection.LEFT;
            }

            // If the swipe meets none of these requirements there is no swipe.
            return SwipeDirection.NONE;
        }


        private SwipeDirection DetectKeyboardEmulatedSwipe ()
        {
            // Store the values for Horizontal and Vertical axes.
            float horizontal = Input.GetAxis ("Horizontal");
            float vertical = Input.GetAxis ("Vertical");

            if (new Vector2(horizontal, vertical).magnitude > _minimumSwipeLength)
            {
                // Store whether there was horizontal or vertical input before.
                bool noHorizontalInputPreviously = Mathf.Abs(m_LastHorizontalValue) < float.Epsilon;
                bool noVerticalInputPreviously = Mathf.Abs(m_LastVerticalValue) < float.Epsilon;

                // The last horizontal values are now the current ones.
                m_LastHorizontalValue = horizontal;
                m_LastVerticalValue = vertical;

                // If there is positive vertical input now and previously there wasn't the swipe is up.
                if (vertical > 0f && noVerticalInputPreviously)
                    return SwipeDirection.UP;

                // If there is negative vertical input now and previously there wasn't the swipe is down.
                if (vertical < 0f && noVerticalInputPreviously)
                    return SwipeDirection.DOWN;

                // If there is positive horizontal input now and previously there wasn't the swipe is right.
                if (horizontal > 0f && noHorizontalInputPreviously)
                    return SwipeDirection.RIGHT;

                // If there is negative horizontal input now and previously there wasn't the swipe is left.
                if (horizontal < 0f && noHorizontalInputPreviously)
                    return SwipeDirection.LEFT;
            }

            // If the swipe meets none of these requirements there is no swipe.
            return SwipeDirection.NONE;
        }
    }
}