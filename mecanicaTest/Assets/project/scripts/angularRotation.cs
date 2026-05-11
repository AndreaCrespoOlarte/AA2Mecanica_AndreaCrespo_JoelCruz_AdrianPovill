using UnityEngine;

public class angularRotation : MonoBehaviour
{

    [Header("Object properties")]
    //Object
    [SerializeField] private GameObject ball;
    [SerializeField] private float radius = 0.5f;

    [Header("Movement Properties")]
    //Dynamic properties
    [SerializeField] private Vector3 position;
    [SerializeField] private float linearVelocity;
    [SerializeField] private float angularVelocity;
    [SerializeField] private float angle;
    [SerializeField] private Vector3 dir;
    [SerializeField] private float frictionCoefficient;

    [Header("Starting Conditions")]

    //Initial conditions
    public float initialAngularVelocity = -4.6f;
    public float angularAcceleration= 0.015f;
    public float initialAngle = -4.6f;

    //Time properties
    public float stepTime = 0.1f;
    public float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ball.transform.localScale = new Vector3(2 * radius, 2 * radius, 2 * radius);
        position = Vector3.zero; 
        ball.transform.position = position;
        angularVelocity = initialAngularVelocity;
        angle = initialAngle;
        time = 0;
    }

    void FixedUpdate()
    {
        (angle, angularVelocity, linearVelocity, position) = motionEquations(angle, angularVelocity, linearVelocity, position);

        ball.transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0);
        ball.transform.position = position;
    }

    (float, float, float, Vector3) motionEquations(float oldAngle, float oldAngularVelocity, float oldLinearVelocity, Vector3 oldPosition)
    {
        float newAngle, newAngularVelocity, newLinearVelocity;
        Vector3 newPosition;


        float frictionDecel = 0;

        if (Mathf.Abs(oldAngularVelocity) != 0)
            frictionDecel = (2.5f * frictionCoefficient * 9.81f) / radius;

        float netAngularAcceleration = angularAcceleration - Mathf.Sign(angularVelocity) * frictionDecel;

        newAngle = oldAngle + oldAngularVelocity * stepTime + 0.5f * netAngularAcceleration * stepTime * stepTime;
        newAngularVelocity = oldAngularVelocity + netAngularAcceleration * stepTime;

        if (Mathf.Abs(newAngularVelocity) < 0.6f)
            newAngularVelocity = 0;

        newLinearVelocity = newAngularVelocity * radius * stepTime;
        newPosition = oldPosition + dir * newLinearVelocity * stepTime;

        time += stepTime;

        return (newAngle, newAngularVelocity, newLinearVelocity, newPosition);
    }

    public void SetDirection(Vector3 newDir)
    {
        dir = newDir;
    }

    public void SetAngularVelicty(float newAngularVelocity)
    {
        angularVelocity = newAngularVelocity;
    }
}
