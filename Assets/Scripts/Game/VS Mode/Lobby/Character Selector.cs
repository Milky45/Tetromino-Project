using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelector : MonoBehaviour
{
    [Header("References")]
    public PlayerReady PlayerReady;
    public int rows = 6;
    public int columns = 6;
    public Transform characterGridParent; // Parent holding the grid
    public GameObject highlightP1;
    public GameObject highlightP2;
    public RightRoleManager roleManagerP2; // Reference to the RoleManager script
    public LeftRoleManager roleManagerP1;

    [Header("P1 Character Prefabs")]
    public GameObject P1Tetro;
    public GameObject P1PackHat;
    public GameObject P1Scorch;
    public GameObject P1Kor;
    public GameObject P1Dodoke;
    public GameObject P1YunJin;

    [Header("P2 Character Prefabs")]
    public GameObject P2Tetro;
    public GameObject P2PackHat;
    public GameObject P2Scorch;
    public GameObject P2Kor;
    public GameObject P2Dodoke;
    public GameObject P2YunJin; 

    public float cellSize = 50f; // Each tile is 50x50 units
    public Vector2 gridOriginP1 = new Vector2(-125f, 125f); // Top-left corner starting point for P1
    public Vector2 gridOriginP2 = new Vector2(-125f, 125f); // Top-right corner starting point for P2

    private Vector2Int selectionP1 = new Vector2Int(0, 0); // Current selected cell for P1
    private Vector2Int selectionP2 = new Vector2Int(5, 0); // Current selected cell for P2

    private P1Controller p1Controls;
    private P2Controller p2Controls;

    public static int P1ChosenCharacter = 0;
    public static int P2ChosenCharacter = 0;

    private bool p1Locked = false;
    private bool p2Locked = false;


    private void Start()
    {
        UpdateP1CharacterDisplay();
        UpdateP2CharacterDisplay();
    }
    private void Awake()
    {
        p1Controls = new P1Controller();
        p2Controls = new P2Controller();

        // Set up P1 input actions
        p1Controls.ChooseMove.MoveLeft.performed += ctx => MoveP1Left();
        p1Controls.ChooseMove.MoveRight.performed += ctx => MoveP1Right();
        p1Controls.ChooseMove.MoveUp.performed += ctx => MoveP1Up();
        p1Controls.ChooseMove.MoveDown.performed += ctx => MoveP1Down();
        p1Controls.ChooseMove.Select.performed += ctx => SelectP1();


        // Set up P2 input actions
        p2Controls.ChooseMove.MoveLeft.performed += ctx => MoveP2Left();
        p2Controls.ChooseMove.MoveRight.performed += ctx => MoveP2Right();
        p2Controls.ChooseMove.MoveUp.performed += ctx => MoveP2Up();
        p2Controls.ChooseMove.MoveDown.performed += ctx => MoveP2Down();
        p2Controls.ChooseMove.Select.performed += ctx => SelectP2();
    }

    private void OnEnable()
    {
        p1Controls.Enable();
        p2Controls.Enable();
    }

    private void OnDisable()
    {
        p1Controls.Disable();
        p2Controls.Disable();
    }

    private void MoveP1Left()
    {
        if (p1Locked) return;
        if (selectionP1.x > 0)
        {
            selectionP1.x--;
            UpdateHighlightPosition(highlightP1, selectionP1, true);
            UpdateP1CharacterDisplay();
        }
    }

    private void MoveP1Right()
    {
        if (p1Locked) return;
        if (selectionP1.x < columns - 1)
        {
            selectionP1.x++;
            UpdateHighlightPosition(highlightP1, selectionP1, true);
            UpdateP1CharacterDisplay();
        }
    }

    private void MoveP1Up()
    {
        if (p1Locked) return;
        if (selectionP1.y > 0)
        {
            selectionP1.y--;
            UpdateHighlightPosition(highlightP1, selectionP1, true);
            UpdateP1CharacterDisplay();
        }
    }

    private void MoveP1Down()
    {
        if (p1Locked) return;
        if (selectionP1.y < rows - 1)
        {
            selectionP1.y++;
            UpdateHighlightPosition(highlightP1, selectionP1, true);
            UpdateP1CharacterDisplay();
        }
    }

    private void MoveP2Left()
    {
        if (p2Locked) return;
        if (selectionP2.x > 0)
        {
            selectionP2.x--;
            UpdateHighlightPosition(highlightP2, selectionP2, false);
            UpdateP2CharacterDisplay();
        }
    }

    private void MoveP2Right()
    {
        if (p2Locked) return;
        if (selectionP2.x < columns - 1)
        {
            selectionP2.x++;
            UpdateHighlightPosition(highlightP2, selectionP2, false);
            UpdateP2CharacterDisplay();
        }
    }

    private void MoveP2Up()
    {
        if (p2Locked) return;
        if (selectionP2.y > 0)
        {
            selectionP2.y--;
            UpdateHighlightPosition(highlightP2, selectionP2, false);
            UpdateP2CharacterDisplay();
        }
    }

    private void MoveP2Down()
    {
        if (p2Locked) return;
        if (selectionP2.y < rows - 1)
        {
            selectionP2.y++;
            UpdateHighlightPosition(highlightP2, selectionP2, false);
            UpdateP2CharacterDisplay();
        }
    }

    private void SelectP1()
    {
        if (!p1Locked)
        {
            if (selectionP1 == new Vector2Int(0, 0))
            {
                P1ChosenCharacter = 1; // Tetro
                p1Locked = true;
                PlayerReady.p1ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP1 == new Vector2Int(1, 0))
            {
                P1ChosenCharacter = 2; // Kor
                p1Locked = true;
                PlayerReady.p1ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP1 == new Vector2Int(2, 0))
            {
                P1ChosenCharacter = 3; // PackHat
                p1Locked = true;
                PlayerReady.p1ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP1 == new Vector2(3, 0))
            {
                P1ChosenCharacter = 4; // Yun Jin
                p1Locked = true;
                PlayerReady.p1ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP1 == new Vector2Int(4, 0))
            {
                P1ChosenCharacter = 5; // Dodoke
                p1Locked = true;
                PlayerReady.p1ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP1 == new Vector2Int(5, 0))
            {
                P1ChosenCharacter = 6; // Scorch
                p1Locked = true;
                PlayerReady.p1ready = true;
                PlayerReady.UpdateStatusText();
            }
            else
            {
                Debug.Log("P1 tried to select an empty cell.");
            }
        }
        else
        {
            if (PlayerReady.p1ready == true)
            {
                P1ChosenCharacter = 0;
                p1Locked = false;
                Debug.Log("P1 deselected their character.");
                PlayerReady.p1ready = false;
                PlayerReady.UpdateStatusText();
                UpdateP1CharacterDisplay();
            }
        }
        UpdateP1CharacterDisplay();
    }

    private void SelectP2()
    {
        if (!p2Locked)
        {
            if (selectionP2 == new Vector2Int(0, 0))
            {
                P2ChosenCharacter = 1; // Tetro
                p2Locked = true;
                PlayerReady.p2ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP2 == new Vector2Int(1, 0))
            {
                P2ChosenCharacter = 2; // Kor
                p2Locked = true;
                PlayerReady.p2ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP2 == new Vector2Int(2, 0))
            {
                P2ChosenCharacter = 3; // PackHat
                p2Locked = true;
                PlayerReady.p2ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP2 == new Vector2Int(3, 0))
            {
                P2ChosenCharacter = 4; // Yun Jin
                p2Locked = true;
                PlayerReady.p2ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP2 == new Vector2Int(4, 0))
            {
                P2ChosenCharacter = 5; // Dodoke
                p2Locked = true;
                PlayerReady.p2ready = true;
                PlayerReady.UpdateStatusText();
            }
            else if (selectionP2 == new Vector2Int(5, 0))
            {
                P2ChosenCharacter = 6; // Scorch
                p2Locked = true;
                PlayerReady.p2ready = true;
                PlayerReady.UpdateStatusText();
            }
            else
            {
                Debug.Log("P2 tried to select an empty cell.");
            }
        }
        else
        {
            if(PlayerReady.p2ready == true)
            {
                P2ChosenCharacter = 0;
                p2Locked = false;
                Debug.Log("P2 deselected their character.");
                PlayerReady.p2ready = false;
                PlayerReady.UpdateStatusText();
                UpdateP2CharacterDisplay();
            }
        }
        UpdateP2CharacterDisplay();
    }

    private void UpdateHighlightPosition(GameObject highlight, Vector2Int selection, bool isP1)
    {
        if (highlight == null)
        {
            Debug.LogError($"Highlight GameObject is null for {(isP1 ? "P1" : "P2")}!");
            return;
        }

        Vector3 currentPos = highlight.transform.localPosition;
        Vector2 gridOrigin = isP1 ? gridOriginP1 : gridOriginP2;
        Vector3 newPosition = new Vector3(
            gridOrigin.x + (selection.x * cellSize),
            gridOrigin.y - (selection.y * cellSize),
            currentPos.z
        );
        highlight.transform.localPosition = newPosition;
        Debug.Log($"{(isP1 ? "P1" : "P2")} highlight moved to: {newPosition}");
    }

    private void UpdateP1CharacterDisplay()
    {
        // Deactivate all
        P1Tetro.SetActive(false);
        P1PackHat.SetActive(false);
        P1Scorch.SetActive(false);
        P1Kor.SetActive(false);
        P1Dodoke.SetActive(false);
        P1YunJin.SetActive(false);

        // Activate based on grid position
        if (selectionP1 == new Vector2Int(0, 0))
        {
            P1Tetro.SetActive(true);
            roleManagerP1.DisplayFool(); // Display Fool role info
        }
        else if (selectionP1 == new Vector2Int(1, 0))
        {
            P1Kor.SetActive(true);
            roleManagerP1.DisplayEnchanter(); // Display Defender role info
        }
        else if (selectionP1 == new Vector2Int(2, 0))
        {
            P1PackHat.SetActive(true);
            roleManagerP1.DisplayEnchanter();
        }
        else if (selectionP1 == new Vector2Int(3, 0))
        {
            P1YunJin.SetActive(true);
            roleManagerP1.DisplayDefender(); // Display Enchanter role info
        }

        else if (selectionP1 == new Vector2Int(4, 0))
        {
            P1Dodoke.SetActive(true);
            roleManagerP1.DisplayFool(); // Display Enchanter role info
        }
        else if (selectionP1 == new Vector2Int(5, 0))
        {
            P1Scorch.SetActive(true);
            roleManagerP1.DisplayDefender();
        }
        else
        {
            roleManagerP1.DisplayNone();
        }

    }

    private void UpdateP2CharacterDisplay()
    {
        P2Tetro.SetActive(false);
        P2PackHat.SetActive(false);
        P2Scorch.SetActive(false);
        P2Kor.SetActive(false);
        P2Dodoke.SetActive(false);
        P2YunJin.SetActive(false);

        if (selectionP2 == new Vector2Int(0, 0))
        {
            P2Tetro.SetActive(true);
            roleManagerP2.DisplayFool();
        }
        else if (selectionP2 == new Vector2Int(1, 0))
        {
            P2Kor.SetActive(true);
            roleManagerP2.DisplayEnchanter();
        }
        else if (selectionP2 == new Vector2Int(2, 0))
        {
            P2PackHat.SetActive(true);
            roleManagerP2.DisplayEnchanter();
        }
        else if (selectionP2 == new Vector2Int(3, 0))
        {
            P2YunJin.SetActive(true);
            roleManagerP2.DisplayDefender();
        }
        else if (selectionP2 == new Vector2Int(4, 0))
        {
            P2Dodoke.SetActive(true);
            roleManagerP2.DisplayFool();
        }
        else if (selectionP2 == new Vector2Int(5, 0))
        {
            P2Scorch.SetActive(true);
            roleManagerP2.DisplayDefender();
        }
    }

}

