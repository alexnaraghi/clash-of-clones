using UnityEngine;
using System.Collections;

public class FigurineTutorial : MonoBehaviour 
{
    public bool HasGrabbedFigurinePlatter { get; set; }
    public bool IsSkipping;
    public Light TutorialLight;
    public Light RealLight;

    public float TutorialLightFadeSeconds = 1f;
    public float TutorialDarknessSeconds = 1f;
    public int RealLightNumFlickers = 3;
    public float TutorialFlickerDarknessSeconds = 0.2f;
    public float TutorialFlickerLightSeconds = 0.6f;
    public float TutorialEndSeconds = 1f;

    public IEnumerator StepTutorial()
    {
        // Skip code for testing.
        if(IsSkipping)
        {
            TutorialLight.gameObject.SetActive(false);
            RealLight.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            while(!HasGrabbedFigurinePlatter)
            {
                yield return new WaitForEndOfFrame();
            }
            
            if(TutorialLight != null)
            {
                TutorialLight.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(TutorialDarknessSeconds);

            if(RealLight != null)
            {
                for(int i = 0; i < RealLightNumFlickers; i++)
                {
                    RealLight.gameObject.SetActive(true);

                    yield return new WaitForSeconds(TutorialFlickerDarknessSeconds);

                    RealLight.gameObject.SetActive(false);

                    yield return new WaitForSeconds(TutorialFlickerLightSeconds);
                }

                RealLight.gameObject.SetActive(true);
            }

            SL.Get<MessageUI>().PrintMessage("Destroy Blue!");
            yield return new WaitForSeconds(TutorialEndSeconds);

            // When the tutorial is done, disable it and its children.
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        // Ensure the lights are set up properly for the tutorial.
        if(TutorialLight != null)
        {
            TutorialLight.gameObject.SetActive(true);
        }

        if(RealLight != null)
        {
            RealLight.gameObject.SetActive(false);
        }
    }
}
