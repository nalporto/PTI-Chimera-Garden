using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfigManager : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;
    [SerializeField] private float defaultSensitivity = 5f;
    [SerializeField] private float minSensitivity = 1f;
    [SerializeField] private float maxSensitivity = 10f;
    
    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeValueText;
    [SerializeField] private float defaultVolume = 0.8f;
    
    private const string SENSITIVITY_KEY = "Sensitivity";
    private const string VOLUME_KEY = "Volume";
    
    void Start()
    {
        LoadSettings();
        
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
        
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        
        UpdateUI();
    }
    
    public void LoadSettings()
    {
        float sensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, defaultSensitivity);
        float volume = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);
        
        if (sensitivitySlider != null)
            sensitivitySlider.value = sensitivity;
        
        if (volumeSlider != null)
            volumeSlider.value = volume;
        
        ApplySettings(sensitivity, volume);
    }
    
    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, value);
        PlayerPrefs.Save();
        
        ApplySettings(value, volumeSlider != null ? volumeSlider.value : defaultVolume);
        UpdateUI();
        
        Debug.Log($"[Config] Sensitivity changed to: {value:F1}");
    }
    
    private void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
        
        ApplySettings(sensitivitySlider != null ? sensitivitySlider.value : defaultSensitivity, value);
        UpdateUI();
        
        Debug.Log($"[Config] Volume changed to: {value:F2}");
    }
    
    private void ApplySettings(float sensitivity, float volume)
    {
        AudioListener.volume = volume;
        
        Debug.Log($"[Config] Applied settings - Sensitivity: {sensitivity:F1}, Volume: {volume:F2}");
    }
    
    private void UpdateUI()
    {
        if (sensitivityValueText != null && sensitivitySlider != null)
        {
            sensitivityValueText.text = sensitivitySlider.value.ToString("F1");
        }
        
        if (volumeValueText != null && volumeSlider != null)
        {
            int percentage = Mathf.RoundToInt(volumeSlider.value * 100f);
            volumeValueText.text = percentage.ToString() + "%";
        }
    }
    
    public void ResetToDefaults()
    {
        if (sensitivitySlider != null)
            sensitivitySlider.value = defaultSensitivity;
        
        if (volumeSlider != null)
            volumeSlider.value = defaultVolume;
        
        Debug.Log("[Config] Settings reset to defaults");
    }
    
    public static float GetSensitivity()
    {
        return PlayerPrefs.GetFloat(SENSITIVITY_KEY, 5f);
    }
    
    public static float GetVolume()
    {
        return PlayerPrefs.GetFloat(VOLUME_KEY, 0.8f);
    }
}
