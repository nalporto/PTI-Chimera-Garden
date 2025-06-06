using TMPro;
using UnityEngine;

public class DashUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dashText;
    [SerializeField] private PlayerCharacter playerCharacter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCharacter != null && dashText != null)
        {
            // Assuming dashCharges is private, expose a public property in PlayerCharacter if needed
            int dashCount = playerCharacter.GetDashCharges();
            dashText.text = $"Dashes: {dashCount}";
        }
    }
}
