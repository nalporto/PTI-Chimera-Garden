using System.Numerics;
using UnityEngine;

public class CameraSpring : MonoBehaviour
{
    [Min(0.01f)]
    [SerializeField] private float halfLife = 0.12f;           // was 0.075f, higher = slower spring
    [Space]
    [SerializeField] private float frequency = 12f;            // was 18f, lower = less bouncy
    [Space]
    [SerializeField] private float angularDisplacement = 1.1f; // was 2f, lower = less tilt

    [SerializeField] private float linearDisplacement = 0.025f;// was 0.05f, lower = less movement
    private UnityEngine.Vector3 _springPosition;
    private UnityEngine.Vector3 _springVelocity;
    public void Initialize()
    {
        _springPosition = transform.position;
        _springVelocity = UnityEngine.Vector3.zero;
    }

    public void UpdateSpring(float deltaTime,UnityEngine.Vector3 up)
    {
        transform.localPosition = UnityEngine.Vector3.zero;

        Spring(ref _springPosition, ref _springVelocity, transform.position, halfLife, frequency, deltaTime);

        var localSpringPosition = _springPosition - transform.position;
        var springHeight = UnityEngine.Vector3.Dot(localSpringPosition, up);

        transform.localEulerAngles = new UnityEngine.Vector3(-springHeight * angularDisplacement, 0f, 0f);
        transform.localPosition = localSpringPosition * linearDisplacement;
    }


    private static void Spring(ref UnityEngine.Vector3 current, ref UnityEngine.Vector3 velocity, UnityEngine.Vector3 target, float halfLife, float frequency, float timeStep)
    {
        var dampingRatio = -Mathf.Log(0.5f) / (frequency * halfLife);
        var f = 1.0f + 2.0f * timeStep * dampingRatio * frequency;
        var oo = frequency * frequency;
        var hoo = timeStep * oo;
        var hhoo = timeStep * hoo;
        var detInv = 1.0f / (f + hhoo);
        var detX = f * current + timeStep * velocity + hhoo * target;
        var detV = velocity + hoo * (target - current);
        current = detX * detInv;
        velocity = detV * detInv;
    }
   
}
