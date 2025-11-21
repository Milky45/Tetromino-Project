using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class CharacterSelect : MonoBehaviour
{
    public bool isPlayer1 = true;
    public bool ready = false;
    public static bool currentlyPlaying;
    public bool isSolo = false;

    public bool isTeamBattle = false;
    public bool isCharPrimPicked = false;
    public bool isCharSecPicked = false;

    public CharacterDisplay characterDisplay;

    [Header("Grid Settings")]
    public int rows = 2;
    public int cols = 4;
    public int currentRow = 0;
    public int currentCol = 0;
    public int playerCtr = 0;
    public int primCharIndex;
    public int secCharIndex;

    public TextMeshProUGUI P1state;
    public TextMeshProUGUI P2state;
    public LobbyManager lobbyManager;
    public bool isSkin = false;

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
        playerCtr = allSelectors.Length;

        if (playerCtr == 1)
        {
            isPlayer1 = true;
            //Debug.Log($"Assigned as Player 1");
            gameObject.tag = "P1";
            gameObject.name = "Player 1";
            characterDisplay = lobbyManager.charDisplayP1;
        }
        else
        {
            isPlayer1 = false;
            //Debug.Log($"Assigned as Player 2");
            gameObject.tag = "P2";
            gameObject.name = "Player 2";
            characterDisplay = lobbyManager.charDisplayP2;

        }
        PlayerLobby.playerCount++;
        PlayerLobby.UpdateLobbyPanels();
        isTeamBattle = characterDisplay.isTeamBattle;


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
        if (!isSolo)
        {
            P2state = GameObject.Find("P2 state text").GetComponent<TextMeshProUGUI>();
        }
        
    }

    private void Update()
    {
        if (!ready)
        {
            if (primCharIndex != 8 && isCharPrimPicked)
            {
                characterDisplay.charDisplay1[8].SetActive(false);
            }
            else if (primCharIndex == 8 && isCharPrimPicked)
            {
                characterDisplay.charDisplay1[8].SetActive(true);
            }

            if (secCharIndex != 8 && isCharSecPicked)
            {
                characterDisplay.charDisplay2[8].SetActive(false);
            }
            else if (secCharIndex == 8 && isCharSecPicked)
            {
                characterDisplay.charDisplay2[8].SetActive(true);
            }
        }
        

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

    private void HideSkin()
    {
        if (isTeamBattle && isCharPrimPicked && !isCharSecPicked && secCharIndex != 8)
        {
            if (characterDisplay.charDisplay2.Length > 8 && characterDisplay.charDisplay2[8] != null && characterDisplay.charDisplay2[8].activeSelf)
            {
                characterDisplay.charDisplay2[8].SetActive(false);
            }
        }
        if (characterDisplay.charDisplay1.Length > 8 && characterDisplay.charDisplay1[8] != null && characterDisplay.charDisplay1[8].activeSelf && primCharIndex != 8)
        {
            characterDisplay.charDisplay1[8].SetActive(false);
        }

    }

    private void HighlightCurrentSlot()
    {
        HideSkin();
        // If moving away from Scorch, hide the skin
        if (currentCol != 2 || currentRow != 0)
        {
            if (characterDisplay.charDisplay1.Length > 8 && characterDisplay.charDisplay1[8] != null) characterDisplay.charDisplay1[8].SetActive(false);
            if (characterDisplay.charDisplay2.Length > 8 && characterDisplay.charDisplay2[8] != null) characterDisplay.charDisplay2[8].SetActive(false);
            isSkin = false;
        }
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int idx = r * cols + c;
                if (isTeamBattle && isCharPrimPicked && !isCharSecPicked)
                {
                    if (playerTag != null && idx < playerTag.Length && playerTag[idx] != null)
                    {
                        playerTag[idx].enabled = r == currentRow && c == currentCol;
                    }
                    // Activate the corresponding character display GameObject
                    if (characterDisplay != null && characterDisplay.charDisplay2 != null && idx < characterDisplay.charDisplay2.Length && characterDisplay.charDisplay2[idx] != null)
                    {
                        //Debug.Log("Displaying Secondary Character");
                        characterDisplay.charDisplay2[idx].SetActive(r == currentRow && c == currentCol);
                    }
                }
                
                if (!isCharPrimPicked && !isCharSecPicked)
                {
                    if (playerTag != null && idx < playerTag.Length && playerTag[idx] != null)
                    {
                        playerTag[idx].enabled = r == currentRow && c == currentCol;
                    }
                    // Activate the corresponding character display GameObject
                    if (characterDisplay != null && characterDisplay.charDisplay1 != null && idx < characterDisplay.charDisplay1.Length && characterDisplay.charDisplay1[idx] != null)
                    {
                        //Debug.Log("Displaying Primary Character");
                        characterDisplay.charDisplay1[idx].SetActive(r == currentRow && c == currentCol);
                    }
                }
                
            }
        }
    }

    private void NextSkin()
    {
        if (ready == true) return;
        if (currentCol == 2 && currentRow == 0) // prime scorch
        {
            if (isTeamBattle && isCharPrimPicked && !isCharSecPicked)
            {
                NextSkinTeam();
                return;
            }
            isSkin = true;
            characterDisplay.charDisplay1[2].SetActive(false);
            characterDisplay.charDisplay1[8].SetActive(true);
        }
    }
    
    private void NextSkinTeam()
    {
        if (ready == true) return;
        if(currentCol == 2 && currentRow == 0) // prime scorch
        {
            isSkin = true;
            characterDisplay.charDisplay2[2].SetActive(false);
            characterDisplay.charDisplay2[8].SetActive(true);
        }
    }

    private void PreviousSkin()
    {
        if (ready == true) return;
        
        if (currentCol == 2 && currentRow == 0) // normal scorch
        {
            if (isTeamBattle && isCharPrimPicked)
            {
                PreviousSkinTeam();
                return;
            }
            isSkin = false;
            characterDisplay.charDisplay1[2].SetActive(true);
            characterDisplay.charDisplay1[8].SetActive(false);
        }
    }
    
    private void PreviousSkinTeam()
    {
        if (ready == true) return;
        if(currentCol == 2 && currentRow == 0) // normal scorch
        {
            isSkin = false;
            characterDisplay.charDisplay2[2].SetActive(true);
            characterDisplay.charDisplay2[8].SetActive(false);
        }
    }

    private void GoBack()
    {
        if (ready || isCharPrimPicked)
        {
            CancelSelection();
            return;
        }
        //Debug.Log("Back to previous menu");
    }

    private void CancelSelection()
    {
        if (currentlyPlaying) return;
        if (isTeamBattle)
        {
            if (isCharPrimPicked && isCharSecPicked)
            {
                isCharSecPicked = false;
            }
            else if (isCharPrimPicked && !isCharSecPicked)
            {
                Debug.Log("reset primary");
                isCharPrimPicked = false;
            }
        }
        else
        { isCharPrimPicked = false; }
        ready = false;
        //Debug.Log("Selection cancelled. Player can reselect a character.");
        // Optionally, update UI or reset highlights here
        if ((primCharIndex == 8) || (secCharIndex == 8))
        {
            // currentCol = 2;
            // currentRow = 0;
            isSkin = false;
            if (isCharPrimPicked && !isCharSecPicked)
            {
                characterDisplay.charDisplay2[8].SetActive(false);
            }
            else if (!isCharPrimPicked && !isCharSecPicked)
            {
                characterDisplay.charDisplay1[8].SetActive(false);
            }

        }
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
        //Debug.Log("Paused character select");
    }

    private void OpenRoom()
    {
        if (ready == true) return;
        //Debug.Log("Room settings opened");
    }

    private void MoveLeft()
    {
        if (ready == true) return;
        currentCol = (currentCol - 1 + cols) % cols;
        //Debug.Log($"Moved Left: Row={currentRow}, Col={currentCol}");
        HighlightCurrentSlot();
        
    }

    private void MoveRight()
    {
        if (ready == true) return;
        currentCol = (currentCol + 1) % cols;
        //Debug.Log($"Moved Right: Row={currentRow}, Col={currentCol}");
        HighlightCurrentSlot();
    }

    private void MoveUp()
    {
        if (ready == true) return;
        currentRow = (currentRow - 1 + rows) % rows;
        //Debug.Log($"Moved Up: Row={currentRow}, Col={currentCol}");
        HighlightCurrentSlot();
    }

    private void MoveDown()
    {
        if (ready == true) return;
        currentRow = (currentRow + 1) % rows;
        //Debug.Log($"Moved Down: Row={currentRow}, Col={currentCol}");
        HighlightCurrentSlot();
    }

    private void ConfirmSelection()
    {
        if (currentlyPlaying) return;
        if (ready == true) return;
        if (isTeamBattle && isCharPrimPicked && !isCharSecPicked)
        {
            if (isSkin == true && currentCol == 2 && currentRow == 0)
            {
                secCharIndex = 8;
            }
            else
            {
                secCharIndex = currentRow * cols + currentCol;
            }
            if (!CheckIfCharIsValid(secCharIndex))
            {
                StartCoroutine(PulseCharacterSelection("#ff4040ff", true, secCharIndex));
                return;
            }
            isCharSecPicked = true;
            StartCoroutine(PulseCharacterSelection("#40ffa0ff", true, secCharIndex));
        }
        else if (isCharPrimPicked && !isCharSecPicked)
        {
            if (isTeamBattle && isSkin == true && currentCol == 2 && currentRow == 0)
            {
                primCharIndex = 8;
                isCharPrimPicked = true;
            }
            else
            {
                primCharIndex = currentRow * cols + currentCol;
                isCharPrimPicked = true;
            }
            StartCoroutine(PulseCharacterSelection("#40ffa0ff", false, primCharIndex));
        }
        else
        {
            if (isSkin == true && currentCol == 2 && currentRow == 0)
            {
                primCharIndex = 8;
                isCharPrimPicked = true;
            }
            else
            {
                primCharIndex = currentRow * cols + currentCol;
                isCharPrimPicked = true;
            }
            StartCoroutine(PulseCharacterSelection("#40ffa0ff", false, primCharIndex));
        }
        if (isTeamBattle)
        {
            CheckIfReady();
            return;
        }


        ready = true; // Lock in the selection
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
    
    private bool CheckIfCharIsValid(int secChar)
    {
        if (secChar == primCharIndex)
        {
            return false;
        }

        int[] prohibitedIndices = new int[4] { 0, 3, 6, 8 };
        for (int i = 0; i < prohibitedIndices.Length; i++)
        {
            if (primCharIndex == prohibitedIndices[i])
            {
                for (int j = 0; j < prohibitedIndices.Length; j++)
                {
                    if (secChar == prohibitedIndices[j])
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private void CheckIfReady()
    {
        if (isTeamBattle)
        {
            if (isCharPrimPicked && isCharSecPicked)
            {
                ready = true;
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
        }
    }

    private IEnumerator PulseCharacterSelection(string color, bool isSec, int idx)
    {
        float fadeDuration = 1f;
        GameObject targetDisplayObject = null;
        if (isSec == true)
        {
            targetDisplayObject = characterDisplay.charDisplay2[idx];
        }
        else if (isSec == false)
        {
            targetDisplayObject = characterDisplay.charDisplay1[idx];
        }

        if (targetDisplayObject == null) yield break;

        SpriteRenderer sr = targetDisplayObject.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) yield break;
        Color originalColor = sr.color;
        Color targetColor;
        if (!ColorUtility.TryParseHtmlString(color, out targetColor))
        {
            targetColor = new Color(118f / 255f, 55f / 255f, 0f / 255f, 1f);
        }
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            if (sr != null)
            {
                sr.color = Color.Lerp(targetColor, originalColor, t);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = originalColor; // Return to original color
    }

    private void SetReadyUI(bool isReady)
    {
        var targetLabel = isPlayer1 ? P1state : P2state;
        if (targetLabel == null) return;

        targetLabel.text = isReady ? "READY" : "NOT READY";
        targetLabel.color = isReady ? Color.green : Color.red;
    }
}
