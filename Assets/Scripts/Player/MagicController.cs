using UnityEngine;

public class MagicController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Shooter shooter;
    [SerializeField] private GameObject gunObject; // Assign your gun GameObject here
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyDrainRate = 10f;

    [Header("Audio/Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string imbueAnimTrigger = "Imbue";
    [SerializeField] private AudioClip imbueSFX;
    [SerializeField] private AudioSource audioSource;

    public enum MagicType { None, Fire }
    public MagicType currentMagic = MagicType.None;

    private float currentEnergy;
    private bool isImbuing = false;
    private Outline gunOutline;

    void Start()
    {
        currentEnergy = maxEnergy;

        // Get or add QuickOutline component
        if (gunObject != null)
        {
            gunOutline = gunObject.GetComponent<Outline>();
            if (gunOutline == null)
                gunOutline = gunObject.AddComponent<Outline>();
            gunOutline.enabled = false;
            gunOutline.OutlineColor = Color.red; // Set to fiery color
            gunOutline.OutlineWidth = 8f;        // Adjust width as desired
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && !isImbuing && currentEnergy > 0f)
        {
            StartImbue(MagicType.Fire);
        }
        else if ((Input.GetKeyUp(KeyCode.G) || currentEnergy <= 0f) && isImbuing)
        {
            StopImbue();
        }

        if (isImbuing)
        {
            currentEnergy -= energyDrainRate * Time.deltaTime;
            if (currentEnergy <= 0f)
            {
                currentEnergy = 0f;
                StopImbue();
            }
        }
    }

    void StartImbue(MagicType type)
    {
        isImbuing = true;
        currentMagic = type;

        // Enable QuickOutline
        if (gunOutline != null)
            gunOutline.enabled = true;

        if (animator != null && !string.IsNullOrEmpty(imbueAnimTrigger))
            animator.SetTrigger(imbueAnimTrigger);
        if (imbueSFX != null && audioSource != null)
            audioSource.PlayOneShot(imbueSFX);

        if (shooter != null)
            shooter.SetMagicImbue(type);
    }

    void StopImbue()
    {
        isImbuing = false;
        currentMagic = MagicType.None;

        // Disable QuickOutline
        if (gunOutline != null)
            gunOutline.enabled = false;

        if (shooter != null)
            shooter.SetMagicImbue(MagicType.None);
    }

    public float GetCurrentEnergy() => currentEnergy;
    public float GetMaxEnergy() => maxEnergy;
    public bool IsImbuing() => isImbuing;
    public MagicType GetCurrentMagic() => currentMagic;
}
