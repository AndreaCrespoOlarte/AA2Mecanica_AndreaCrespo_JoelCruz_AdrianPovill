using TMPro;
using UnityEngine;

public class BallController : MonoBehaviour
{

    [SerializeField] private PhysicsManager physicsManager;
    [SerializeField] private Camera cam;

    [SerializeField] private Vector2 screenPosition;

    [Header("Shoot parameters")]
    [SerializeField] private float maxForce;
    [SerializeField] private Vector2 startMousePos;
    [SerializeField] private Vector2 endMousePos;
    
    void Update()
    {

        screenPosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = screenPosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endMousePos = screenPosition;

            Vector3 dragDelta = startMousePos - endMousePos;

            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            //y = 0 para qu e no salga volando
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 temp = (camForward * dragDelta.y) + (camRight * dragDelta.x);
            Vector3 shootDirection = temp.normalized;
            float shootVelocity = Mathf.Min(temp.magnitude / 5, maxForce);

            Debug.Log("dir: " + shootDirection + " mag: " + shootVelocity + " vector: " + temp);

            physicsManager.ApplyImpulse(shootDirection * shootVelocity);
        }

    }
}
