using Unity.VisualScripting;
using UnityEngine;

public class BallController : MonoBehaviour
{

    [Header("Object properties")]
    //Object
    [SerializeField] private GameObject ball;
    [SerializeField] public float radius = 0.5f;

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

    [Header("Collisions")]
    [SerializeField] private CollisionController collisionController;
    [SerializeField] private bool colliding;

    [Header("Gravity")]
    [SerializeField] float gravityAcceleration = 5;
    [SerializeField] float gravityVelocity = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ball.transform.localScale = new Vector3(2 * radius, 2 * radius, 2 * radius);
        ball.transform.position = position;
        angularVelocity = initialAngularVelocity;
        angle = initialAngle;
        time = 0;
    }

    void FixedUpdate()
    {
        gravityVelocity += gravityAcceleration * Time.fixedDeltaTime;

        (angle, angularVelocity, linearVelocity, position) = motionEquations(angle, angularVelocity, linearVelocity, position);
        position = new Vector3(position.x, position.y - gravityVelocity , position.z);

        ball.transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0);
        ball.transform.position = position;

        float CollisionY;

        (colliding, CollisionY) = collisionController.CollidingWithGround();

        if (colliding)
        {
            position = new Vector3(position.x, CollisionY + radius + 0.01f, position.z);
            gravityVelocity = 0;
        }
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
