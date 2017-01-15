using UnityEngine;
using System.Collections;

public class FigurineTutorial : MonoBehaviour 
{
    public bool HasGrabbedFigurinePlatter { get; set; }

    public IEnumerator StepTutorial()
    {
        while(!HasGrabbedFigurinePlatter)
        {
            yield return new WaitForEndOfFrame();
        }

        // When the tutorial is done, disable it and its children.
        gameObject.SetActive(false);
    }
}
