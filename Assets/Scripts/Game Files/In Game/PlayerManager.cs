using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStatus playerStatus;
    public PieceSpawner pieceSpawner;
    public GameDisplay gameDisplay;
    public AudioManager audioManager;
    private void PlayComboSFX(int combo)
    {
        switch (combo)
        {
            case 2: audioManager.PlaySFX(audioManager.clear2); break;
            case 3: audioManager.PlaySFX(audioManager.clear3); break;
            case 4: audioManager.PlaySFX(audioManager.clear4); break;
            case 5: audioManager.PlaySFX(audioManager.clear5); break;
            case 6: audioManager.PlaySFX(audioManager.clear6); break;
            case 7: audioManager.PlaySFX(audioManager.clear7); break;
            case 8: audioManager.PlaySFX(audioManager.clear8); break;
            case 9: audioManager.PlaySFX(audioManager.clear9); break;
            case 10: audioManager.PlaySFX(audioManager.clear10); break;
            case 11: audioManager.PlaySFX(audioManager.clear11); break;
            case 12: audioManager.PlaySFX(audioManager.clear12); break;
            case 13: audioManager.PlaySFX(audioManager.clear13); break;
            default: break;
        }
    }

    public void TryHoldPiece(TetrominoData current, Piece controller)
    {   
        var activePiece = GameObject.Find($"ActivePiece{(playerStatus.isPlayer1 ? "P1" : "P2")}")?.GetComponent<Piece>();

        if (activePiece != null)
            activePiece.Clear();

        if (playerStatus.holdUsed)
        {
            // comboText.color = Color.red;
            // comboText.text = "SWAP LOCKED";
            Debug.Log("Hold already used this turn!");
            return;
        }
        controller.Clear();

        if (pieceSpawner.heldTetromino == null)
        {
            pieceSpawner.heldTetromino = current;
            gameDisplay.LogTetrominoStatus(pieceSpawner.nextTetromino, pieceSpawner.heldTetromino); // Log after hold
            pieceSpawner.SpawnPiece(null);
        }
        else
        {
            TetrominoData temp = pieceSpawner.heldTetromino;
            pieceSpawner.heldTetromino = current;
            gameDisplay.LogTetrominoStatus(pieceSpawner.nextTetromino, pieceSpawner.heldTetromino); // Log after swap
            pieceSpawner.SpawnPiece(temp);
        }

        playerStatus.holdUsed = true;
        //holdDisplayUI.ShowHold(heldTetromino.tetromino);

        Destroy(controller.gameObject);
    }
}
