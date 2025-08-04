using UnityEngine;

public class HoldDisplayUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject hold_I;
    public GameObject hold_O;
    public GameObject hold_T;
    public GameObject hold_S;
    public GameObject hold_Z;
    public GameObject hold_J;
    public GameObject hold_L;

    public void ShowHold(TetrominoType type)
    {
        HideAll();

        switch (type)
        {
            case TetrominoType.I: hold_I.SetActive(true); break;
            case TetrominoType.O: hold_O.SetActive(true); break;
            case TetrominoType.T: hold_T.SetActive(true); break;
            case TetrominoType.S: hold_S.SetActive(true); break;
            case TetrominoType.Z: hold_Z.SetActive(true); break;
            case TetrominoType.J: hold_J.SetActive(true); break;
            case TetrominoType.L: hold_L.SetActive(true); break;
        }
    }

    public void HideAll()
    {
        hold_I.SetActive(false);
        hold_O.SetActive(false);
        hold_T.SetActive(false);
        hold_S.SetActive(false);
        hold_Z.SetActive(false);
        hold_J.SetActive(false);
        hold_L.SetActive(false);
    }
}
