using UnityEngine;
using UnityEngine.UI;

public class SimpleDynamicCrosshair : MonoBehaviour {

    private RectTransform reticle; // The RecTransform of reticle UI element.

    public float restingSize;
    public float maxSize;
    public float speed;
    private float currentSize;

    private void Start() {
        reticle = GetComponent<RectTransform>();
        currentSize = restingSize;
    }

    private void Update() {
        // Widen reticle only when Mouse 1 (Fire1) is pressed
        if (Input.GetButton("Fire1")) {
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        } else {
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);
        }

        reticle.sizeDelta = new Vector2(currentSize, currentSize);

    }
}