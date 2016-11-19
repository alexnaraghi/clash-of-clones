using UnityEngine;
using VRStandardAssets.Utils;

namespace VRStandardAssets.Examples
{
    // This script shows a simple example of how
    // swipe controls can be handled.
    public class ExampleTouchpad : MonoBehaviour
    {
        [SerializeField] private float m_Torque = 10f;
        [SerializeField] private VRInput m_VRInput;                                        
        [SerializeField] private Rigidbody m_Rigidbody;                                    


        private void OnEnable()
        {
           m_VRInput.OnSwipe.AddListener(HandleSwipe);
        }


        private void OnDisable()
        {
            m_VRInput.OnSwipe.RemoveListener(HandleSwipe);
        }


        //Handle the swipe events by applying AddTorque to the Ridigbody
        private void HandleSwipe(SwipeDirection swipeDirection)
        {
            switch (swipeDirection)
            {
                case SwipeDirection.NONE:
                    break;
                case SwipeDirection.UP:
                    m_Rigidbody.AddTorque(Vector3.right * m_Torque);
                    break;
                case SwipeDirection.DOWN:
                    m_Rigidbody.AddTorque(-Vector3.right * m_Torque);
                    break;
                case SwipeDirection.LEFT:
                    m_Rigidbody.AddTorque(Vector3.up * m_Torque);
                    break;
                case SwipeDirection.RIGHT:
                    m_Rigidbody.AddTorque(-Vector3.up * m_Torque);
                    break;
            }
        }
    }
}