using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BallController : MonoBehaviour
{

    [SerializeField] private PhysicsManager physicsManager;
    [SerializeField] private Camera cam;

    [SerializeField] private Vector2 screenPosition;

    [Header("Shoot parameters")]
    [SerializeField] private float maxForce;
    [SerializeField] private Vector2 startMousePos;
    [SerializeField] private Vector2 endMousePos;

    Vector3 shootDirection;
    float shootVelocity; 

    void Update()
    {

        screenPosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = screenPosition;
        }
        else if(Input.GetMouseButton(0))
        {
            Vector3 dragDelta = startMousePos - screenPosition;

            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            // y = 0 para que no afecte el eje vertical
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 temp = (camForward * dragDelta.y) + (camRight * dragDelta.x);
            shootDirection = temp.normalized;
            shootVelocity = Mathf.Min(temp.magnitude / 20, maxForce);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endMousePos = screenPosition;

            physicsManager.ApplyImpulse(shootDirection * shootVelocity);

            shootDirection = Vector3.zero;
            shootVelocity = 0f;
        }

    }

    public Vector3 GetShootDirection()
    {
        return shootDirection;
    }

    public float GetShootVelocity()
    {
        return shootVelocity;
    }
}
