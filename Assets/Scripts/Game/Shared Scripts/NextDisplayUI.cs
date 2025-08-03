using UnityEngine;

public class NextDisplayUI : MonoBehaviour
{
    [Header("One GameObject per Tetromino")]
    public GameObject next_I;
    public GameObject next_O;
    public GameObject next_T;
    public GameObject next_S;
    public GameObject next_Z;
    public GameObject next_J;
    public GameObject next_L;

    public void ShowNext(TetrominoType type)
    {
        HideAll();

        switch (type)
        {
            case TetrominoType.I: next_I.SetActive(true); break;
            case TetrominoType.O: next_O.SetActive(true); break;
            case TetrominoType.T: next_T.SetActive(true); break;
            case TetrominoType.S: next_S.SetActive(true); break;
            case TetrominoType.Z: next_Z.SetActive(true); break;
            case TetrominoType.J: next_J.SetActive(true); break;
            case TetrominoType.L: next_L.SetActive(true); break;
        }
    }

    public void HideAll()
    {
        next_I.SetActive(false);
        next_O.SetActive(false);
        next_T.SetActive(false);
        next_S.SetActive(false);
        next_Z.SetActive(false);
        next_J.SetActive(false);
        next_L.SetActive(false);
    }
}
