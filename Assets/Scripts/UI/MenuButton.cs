using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = new Color(0.95f, 0.95f, 0.9f);
    [SerializeField] private Color hoverColor = new Color(1f, 0.85f, 0.4f);
    [SerializeField] private Color pressedColor = new Color(0.8f, 0.7f, 0.3f);
    
    [Header("Animation")]
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float animationSpeed = 10f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    
    private Button button;
    private Image buttonImage;
    private TextMeshProUGUI buttonText;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private AudioSource audioSource;
    
    void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = transform.localScale;
        targetScale = originalScale;
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        
        SetColor(normalColor);
    }
    
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            SetColor(hoverColor);
            targetScale = originalScale * scaleMultiplier;
            PlaySound(hoverSound);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            SetColor(normalColor);
            targetScale = originalScale;
        }
    }
    
    private void OnClick()
    {
        SetColor(pressedColor);
        PlaySound(clickSound);
    }
    
    private void SetColor(Color color)
    {
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
