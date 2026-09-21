using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "Character Skills", menuName = "Character Skills/New Skill")]
public class CharSkills : ScriptableObject
{
    [Header("Cooldown Settings")]
    public float cooldownTime = 35f;
    public float cooldownTimer = 0f;

    [Header("Misc")]
    public int skillCost = 300;
    
    [Header("Skill UI Settings")]
    public Color skillColor;

    [Header("SFX Settings")]
    public AudioClip skillSFX;

    [Header("Character Prefab")]
    public string characterName;
    public int characterID;
    public string characterDescription;
    public string characterRole;
    public GameObject characterPrefab;
    public Animator animator;

    // for tetro skill
    [Header("Blinding Overlay")]
    public GameObject blindOverlay;
    public GameObject blindBall;
    public Animator animBall;
    public SpriteRenderer BO_Renderer;
    public SpriteRenderer ballRenderer;

    // for yun jin skill
    [Header("Rocks Summons Settings")]
    public int maxRockCount = 0;

    [Header("Burn Effect Settings")]
    public int maxBurnStack = 0;
    

}
