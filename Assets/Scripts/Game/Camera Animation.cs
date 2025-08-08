using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public Animator cameraAnimator;

    public void SineIn()
    {
        if (cameraAnimator != null)
        {
            cameraAnimator.SetTrigger("Sine In"); // Trigger the sine in animation
            Debug.Log("Camera sine in animation triggered.");
        }
    }

    public void SineOut()
    {
        if (cameraAnimator != null)
        {
            cameraAnimator.SetTrigger("Sine Out"); // Trigger the sine out animation
            Debug.Log("Camera sine out animation triggered.");
        }
    }

}
