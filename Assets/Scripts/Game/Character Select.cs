using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public bool isPlayer1 = true;
    public bool ready = false;
    public static bool currentlyPlaying;

    public CharacterDisplay characterDisplay;

    [Header("Grid Settings")]
    public int rows = 2;
    public int cols = 4;
    private int currentRow = 0;
    private int currentCol = 0;
    public int selectedCharacterIndex;
    public TextMeshProUGUI P1state;
    public TextMeshProUGUI P2state;

    public LobbyManager lobbyManager;
    

    [Header("Tags")]
    [SerializeField] private SpriteRenderer[] playerTag = new SpriteRenderer[8];
    public Transform[,] slotTransforms = new Transform[2, 4];

    private Vector2Int[,] characterArr = new Vector2Int[2, 4];

    [Header("Input")]
    public PlayerInput playerInput;
    InputAction moveLeft;
    InputAction moveRight;
    InputAction moveUp;
    InputAction moveDown;
    InputAction confirmAction;
    InputAction backAction;
    InputAction pauseAction;
    InputAction roomAction;
    InputAction randomAction;
    InputAction nextSkinAction;
    InputAction previousSkinAction;

    public void Awake()
    {
        // make this game obgject persist across scenes
        DontDestroyOnLoad(gameObject);
        ready = false;
        SetReadyUI(false);

        lobbyManager = GameObject.Find("Camera").GetComponent<LobbyManager>();

        // Detect number of CharacterSelect instances in the scene
        var allSelectors = FindObjectsByType<CharacterSelect>(FindObjectsSortMode.None);
        if (allSelectors.Length == 1)
        {
            isPlayer1 = true;
            Debug.Log($"Assigned as Player 1");
            gameObject.tag = "P1";
            gameObject.name = "Player 1";
            characterDisplay = GameObject.Find("Character Holder P1").GetComponent<CharacterDisplay>();
        }
        else
        {
            isPlayer1 = false;
            Debug.Log($"Assigned as Player 2");
            gameObject.tag = "P2";
            gameObject.name = "Player 2";
            characterDisplay = GameObject.Find("Character Holder P2").GetComponent<CharacterDisplay>();
        }
        PlayerLobby.playerCount++;
        PlayerLobby.UpdateLobbyPanels();
        // Auto-assign SpriteRenderers with tags in the format 'P1 (character name) Tag' or 'P2 (character name) Tag'
        string[] characterNames = { "Tetro", "Packhat", "Scorch", "Dodoke", "Yun Jin", "Null", "Ethan", "Random" };
        string playerTagPrefix = isPlayer1 ? "P1" : "P2";
        for (int i = 0; i < characterNames.Length; i++)
        {
            string nameToFind = $"{playerTagPrefix} {characterNames[i]} Tag";
            foreach (var sr in FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
            {
                if (sr.gameObject.name == nameToFind)
                {
                    playerTag[i] = sr;
                    //Debug.Log($"Assigned SpriteRenderer with GameObject name '{nameToFind}' to playerTag[{i}]");
                    break;
                }
            }
        }
        playerInput = GetComponent<PlayerInput>();
        moveLeft = playerInput.actions["Left"];
        moveRight = playerInput.actions["Right"];
        moveUp = playerInput.actions["Up"];
        moveDown = playerInput.actions["Down"];
        confirmAction = playerInput.actions["Confirm"];
        backAction = playerInput.actions["Back"];
        pauseAction = playerInput.actions["Pause"];
        roomAction = playerInput.actions["Room Settings"];
        randomAction = playerInput.actions["Random"];
        nextSkinAction = playerInput.actions["Next Skin"];
        previousSkinAction = playerInput.actions["Previous Skin"];

        moveLeft.performed += ctx => MoveLeft();
        moveRight.performed += ctx => MoveRight();
        moveUp.performed += ctx => MoveUp();
        moveDown.performed += ctx => MoveDown();

        confirmAction.performed += ctx => ConfirmSelection();
        backAction.performed += ctx => GoBack();
        pauseAction.performed += ctx => TogglePause();
        roomAction.performed += ctx => OpenRoom();
        //randomAction.performed += ctx => SelectRandomCharacter();
        nextSkinAction.performed += ctx => NextSkin();
        previousSkinAction.performed += ctx => PreviousSkin();
    }

    private void Start()
    {
        P1state = GameObject.Find("P1 state text").GetComponent<TextMeshProUGUI>();
        P2state = GameObject.Find("P2 state text").GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        playerInput.actions.Enable();
        HighlightCurrentSlot();
    }

    private void OnDisable()
    {
        playerInput.actions.Disable();
    }



    private void HighlightCurrentSlot()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int idx = r * cols + c;
                if (playerTag != null && idx < playerTag.Length && playerTag[idx] != null)
                {
                    playerTag[idx].enabled = (r == currentRow && c == currentCol);
                }
                // Activate the corresponding character display GameObject
                if (characterDisplay != null && characterDisplay.characterDisplay != null && idx < characterDisplay.characterDisplay.Length && characterDisplay.characterDisplay[idx] != null)
                {
                    characterDisplay.characterDisplay[idx].SetActive(r == currentRow && c == currentCol);
                }
            }
        }
    }


    private void NextSkin()
    {
        if (ready == true) return;
        Debug.Log("Next skin for selected character");
    }

    private void PreviousSkin()
    {
        if (ready == true) return;
        Debug.Log("Previous skin for selected character");
    }

    private void GoBack()
    {
        if (ready)
        {
            CancelSelection();
            return;
        }
        Debug.Log("Back to previous menu");
    }

    private void CancelSelection()
    {
        if (currentlyPlaying) return;
        ready = false;
        Debug.Log("Selection cancelled. Player can reselect a character.");
        // Optionally, update UI or reset highlights here
        HighlightCurrentSlot();
        SetReadyUI(false);
        if (isPlayer1)
        {
            LobbyManager.p1Ready = false;
        }
        else
        {
            LobbyManager.p2Ready = false;
        }

        lobbyManager.ReadyBtn();
    }

    private void TogglePause()
    {
        if (ready == true) return;
        Debug.Log("Paused character select");
    }

    private void OpenRoom()
    {
        if (ready == true) return;
        Debug.Log("Room settings opened");
    }

    private void MoveLeft()
    {
        if (ready == true) return;
        currentCol = (currentCol - 1 + cols) % cols;
        Debug.Log($"Moved Left: Row={currentRow}, Col={currentCol}");
        HighlightCurrentSlot();
    }

    private void MoveRight()
    {
        if (ready == true) return;
        currentCol = (currentCol + 1) % cols;
        Debug.Log($"Moved Right: Row={currentRow}, Col={currentCol}");
        HighlightCurrentSlot();
    }

    private void MoveUp()
    {
        if (ready == true) return;
        currentRow = (currentRow - 1 + rows) % rows;
        Debug.Log($"Moved Up: Row={currentRow}, Col={currentCol}");
        HighlightCurrentSlot();
    }

    private void MoveDown()
    {
        if (ready == true) return;
        currentRow = (currentRow + 1) % rows;
        Debug.Log($"Moved Down: Row={currentRow}, Col={currentCol}");
        HighlightCurrentSlot();
    }

    private void ConfirmSelection()
    {
        if (currentlyPlaying) return;
        if (ready == true) return;
        selectedCharacterIndex = currentRow * cols + currentCol;
        ready = true; // Lock in the selection
        Debug.Log($"Confirmed Selection: Character Index={selectedCharacterIndex}");
        SetReadyUI(true);
        if (isPlayer1)
        {
            LobbyManager.p1Ready = true;
        }
        else
        {
            LobbyManager.p2Ready = true;
        }

        lobbyManager.ReadyBtn();
    }

    private void SetReadyUI(bool isReady)
    {
        var targetLabel = isPlayer1 ? P1state : P2state;
        if (targetLabel == null) return;

        targetLabel.text = isReady ? "READY" : "NOT READY";
        targetLabel.color = isReady ? Color.green : Color.red;
    }
}
