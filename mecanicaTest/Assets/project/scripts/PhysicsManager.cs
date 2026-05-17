using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PhysicsManager : MonoBehaviour
{
    [Header("Ball Properties")]
    public Transform ball;
    public float mass = 2f; //kg, no es realista pero se siente bien
    public float radius = 0.25f; //m, tampoco es realista, pero si no no se ve un papoi

    [Header("Environment & Aerodynamics")]
    public float gravity = 9.81f;
    public float airDensity = 1.225f; 
    public float dragCoefficient = 0.47f; 

    [SerializeField] private Vector3 velocity = Vector3.zero;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private float currentFriction;
    [SerializeField] private Vector3 currentGroundNormal;

    public List<Transform> groundRectangles;
    public List<Transform> obstacles;


    private void Start()
    {
        ball.transform.localScale = 2 * radius * Vector3.one;
    }

    
    void FixedUpdate()
    {
        CheckCollisions();

        Vector3 forces = Vector3.zero;
        Vector3 gravityForce = Vector3.down * (mass * gravity);
        float currentEffectiveMass = mass;

        if (isGrounded)
        {
            //F paralela = mg * sin(theta)
            Vector3 parallelGravity = Vector3.ProjectOnPlane(gravityForce, currentGroundNormal);

            //F normal = mg * cos(theta)
            float cosTheta = Mathf.Clamp(Vector3.Dot(Vector3.up, currentGroundNormal), 0f, 1f);
            float normalForceMagnitude = mass * gravity * cosTheta;

            //F friction del torque tau = -mu * Fnormal * r 
            float frictionMagnitude = currentFriction * normalForceMagnitude;

            forces += parallelGravity;

            if (velocity.magnitude > 0.01f)
            {
                //aplicamos friccion en la direccion contraria al movimiento
                forces += -velocity.normalized * frictionMagnitude;
            }
            else if (parallelGravity.magnitude < frictionMagnitude)
            {
                forces -= parallelGravity;
                velocity = Vector3.zero;
            }

            currentEffectiveMass = mass + 2/5 * mass;

            RotateBall();
        }
        else
        {
            forces += gravityForce;
        }

        if (ball.position.y > 1f)
        {
            float crossSectionalArea = Mathf.PI * radius * radius;
            float dragForceMag = 0.5f * airDensity * velocity.sqrMagnitude * dragCoefficient * crossSectionalArea;
            forces += -velocity.normalized * dragForceMag;
        }

        Vector3 acceleration = forces / currentEffectiveMass;
        velocity += acceleration * Time.fixedDeltaTime;

        if (isGrounded)
        {
            velocity = Vector3.ProjectOnPlane(velocity, currentGroundNormal);
        }

        ball.position += velocity * Time.fixedDeltaTime;
    }

    private void RotateBall()
    {
        float speed = velocity.magnitude;
        if (speed > 0.001f)
        {
            //w = v/r 
            float angularVelocityRad = speed / radius;
            float angularVelocityDeg = angularVelocityRad * Mathf.Rad2Deg;

            //encontramos el axis de rotacion segun la normal y el movimiento
            Vector3 rotationAxis = Vector3.Cross(currentGroundNormal, velocity.normalized);

            //aplicar rotacion
            ball.Rotate(rotationAxis, angularVelocityDeg * Time.fixedDeltaTime, Space.World);
        }
    }

    public void ApplyImpulse(Vector3 force)
    {
        velocity += force / mass;
    }

    private void CheckCollisions()
    {
        isGrounded = false;
        currentGroundNormal = Vector3.up;

        foreach (Transform ground in groundRectangles)
        {
            if (CheckSphereBoxCollision(ball.position, radius, ground, out Vector3 hitNormal, out float penetration, out bool isOnTopFace))
            {
                //subimos a la bola fuera del suelo
                ball.position += hitNormal * penetration;

                if (isOnTopFace)
                {
                    isGrounded = true;
                    currentGroundNormal = ground.up;

                    if (ground.CompareTag("Grass")) currentFriction = 0.4f;
                    else if (ground.CompareTag("Ice")) currentFriction = 0.1f;
                    else if (ground.CompareTag("Sand")) currentFriction = 0.8f;//hemos subido la friccin de la arena para que se note mas el contraste
                }
                else
                {
                    //SI no schocamos son el lado de un suelo
                    velocity = Vector3.ProjectOnPlane(velocity, hitNormal);
                }
            }
        }

        foreach (Transform obstacle in obstacles)
        {
            //ignoramos isOnTopFace porque solo queremos rebotar 
            if (CheckSphereBoxCollision(ball.position, radius, obstacle, out Vector3 hitNormal, out float penetration, out _))
            {
                ball.position += hitNormal * penetration;
                float e = obstacle.CompareTag("Elastic") ? 0.8f : obstacle.CompareTag("Sand") ? 0.2f : 0.5f;
                velocity = Vector3.Reflect(velocity, hitNormal) * e;
            }
        }
    }

    private bool CheckSphereBoxCollision(Vector3 spherePos, float sphereRadius, Transform box, out Vector3 normal, out float penetration, out bool isOnTopFace)
    {
        normal = Vector3.zero;
        penetration = 0f;
        isOnTopFace = false;

        //Distancia entre centros
        Vector3 toBall = spherePos - box.position;

        //usamos el dot para ignorar la rotacion a la hora de calcular las colisionse
        float distX = Vector3.Dot(toBall, box.right);
        float distY = Vector3.Dot(toBall, box.up);
        float distZ = Vector3.Dot(toBall, box.forward);

        //usamos lossyScale para prevenir bugs
        Vector3 halfExtents = box.lossyScale / 2f;

        //check de si stamos en el espacio x z que ocupa el suelo
        bool isWithinX = Mathf.Abs(distX) <= halfExtents.x;
        bool isWithinZ = Mathf.Abs(distZ) <= halfExtents.z;
        bool isAboveCenter = distY > 0; //mirar que noe stemos por debajo

        //calculamos el punto mas cercano para calcular el pushback
        float closestX = Mathf.Clamp(distX, -halfExtents.x, halfExtents.x);
        float closestY = Mathf.Clamp(distY, -halfExtents.y, halfExtents.y);
        float closestZ = Mathf.Clamp(distZ, -halfExtents.z, halfExtents.z);

        //reconstruimos la posicion a worldPosition 
        Vector3 worldClosestPoint = box.position
                                  + (box.right * closestX)
                                  + (box.up * closestY)
                                  + (box.forward * closestZ);

        float distance = Vector3.Distance(spherePos, worldClosestPoint);

        if (distance < sphereRadius)
        {
            normal = (spherePos - worldClosestPoint).normalized;
            if (normal == Vector3.zero) normal = box.up;
            penetration = sphereRadius - distance;

            if (isWithinX && isWithinZ && isAboveCenter)
            {
                isOnTopFace = true;
            }

            return true;
        }

        return false;
    }
}