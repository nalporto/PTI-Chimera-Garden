using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float swayAmount = 0.02f;      // How much the weapon moves
    public float smoothAmount = 6f;       // How quickly the weapon moves back

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float moveX = -Input.GetAxis("Mouse X") * swayAmount;
        float moveY = -Input.GetAxis("Mouse Y") * swayAmount;

        Vector3 swayPosition = new Vector3(moveX, moveY, 0);
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            initialPosition + swayPosition,
            Time.deltaTime * smoothAmount
        );
    }
}