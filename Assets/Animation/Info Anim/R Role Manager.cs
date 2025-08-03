using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightRoleManager : MonoBehaviour
{
    public Image currentImage; // Image component to display the current role
    public Sprite FoolSprite;
    public Sprite DefenderSprite;
    public Sprite EnchanterSprite;
    public GameObject Panel;
    public GameObject Icon; // Panel to show role information
    public GameObject IconHolder; // Holder for the icon
    public Toggle toggle; // Toggle to enable or disable the role display
    public IconHoverInfo infoAnimator;

    public TextMeshProUGUI InfoText; // Text component to display role information
    public TextMeshProUGUI RoleText;

    public void DisplayFool()
    {
        ResetDisplay();
        currentImage.sprite = FoolSprite;
        RoleText.text = "Fool";
        InfoText.text = "Fool:\r\n\r\nThis character manipulates the opponent's board.";
    }

    public void DisplayDefender()
    {
        ResetDisplay();
        currentImage.sprite = DefenderSprite;
        RoleText.text = "Defender";
        InfoText.text = "Defender:\r\n\r\nThis character protects the board from attacks.";
    }

    public void DisplayEnchanter()
    {
        ResetDisplay();
        currentImage.sprite = EnchanterSprite;
        RoleText.text = "Enchanter";
        InfoText.text = "Enchanter:\r\n\r\nThis character enhances the player's gameplay.";
    }

    public void ResetDisplay()
    {
        toggle.interactable = true; // Enable the toggle when a role is displayed

        RoleText.gameObject.SetActive(true);
        InfoText.gameObject.SetActive(true); // Hide the info text

        Image panelImage = Panel.GetComponent<Image>();
        Color panelColor = panelImage.color;
        panelColor = new Color(panelColor.r, panelColor.g, panelColor.b, 1f);
        panelImage.color = panelColor;

        SpriteRenderer iconHolderImage = IconHolder.GetComponent<SpriteRenderer>();
        Color iconHolderColor = iconHolderImage.color;
        iconHolderColor = new Color(iconHolderColor.r, iconHolderColor.g, iconHolderColor.b, 1f);
        iconHolderImage.color = iconHolderColor;

        Image iconImage = Icon.GetComponent<Image>();
        Color iconColor = iconImage.color;
        iconColor = new Color(iconColor.r, iconColor.g, iconColor.b, 1f);
        iconImage.color = iconColor;
    }

    public void DisplayNone()
    {
        toggle.interactable = false;

        InfoText.text = ""; // Clear the info text

        RoleText.gameObject.SetActive(false); // Hide the role text
        InfoText.gameObject.SetActive(false); // Hide the info text

        Image panelImage = Panel.GetComponent<Image>();
        Color panelColor = panelImage.color;
        panelColor = new Color(panelColor.r, panelColor.g, panelColor.b, 0f);
        panelImage.color = panelColor;
        
        SpriteRenderer iconHolderImage = IconHolder.GetComponent<SpriteRenderer>();
        Color iconHolderColor = iconHolderImage.color;
        iconHolderColor = new Color(iconHolderColor.r, iconHolderColor.g, iconHolderColor.b, 0f);
        iconHolderImage.color = iconHolderColor;

        Image iconImage = Icon.GetComponent<Image>();
        Color iconColor = iconImage.color;
        iconColor = new Color(iconColor.r, iconColor.g, iconColor.b, 0f);
        iconImage.color = iconColor;
    }

}
