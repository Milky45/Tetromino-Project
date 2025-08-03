using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IconHoverInfo : MonoBehaviour
{
    public Animator L_infoAnimator;
    public Animator R_infoAnimator;
    public bool LeftClicked = false;
    public bool RightClicked = false;

    public void LeftOnClickIcon()
    {
        LeftClicked = !LeftClicked;
        if (LeftClicked)
        {
            LeftOnShowPanel();
        }
        else
        {
            LeftOnExitPanel();
        }
    }

    public void RightOnClickIcon()
    {
        RightClicked = !RightClicked;
        if (RightClicked)
        {
            RightOnShowPanel();
        }
        else
        {
            RightOnExitPanel();
        }
    }

    public void RightOnShowPanel()
    {

        Debug.Log("Hovering over icon");
        R_infoAnimator.SetTrigger("Grow");
    }

    public void RightOnExitPanel()
    {
        Debug.Log("Exiting");
        R_infoAnimator.SetTrigger("Shrink");
    }

    public void LeftOnShowPanel()
    {
        
        Debug.Log("Hovering over icon");
        L_infoAnimator.SetTrigger("Grow");
    }

    public void LeftOnExitPanel()
    {
        Debug.Log("Exiting");
        L_infoAnimator.SetTrigger("Shrink");
    }
}
