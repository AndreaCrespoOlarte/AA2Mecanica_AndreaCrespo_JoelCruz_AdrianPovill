using TMPro;
using UnityEngine;

public class BallController : MonoBehaviour
{

    [SerializeField] private PhysicsManager physicsManager;
    [SerializeField] private Camera cam;

    [SerializeField] private bool freeShot;

    [SerializeField] private Vector2 screenPosition;

    [Header("Shoot parameters")]
    [SerializeField] private float maxForce;
    [SerializeField] private Vector2 startMousePos;
    [SerializeField] private Vector2 endMousePos;
    [SerializeField] private TextMeshProUGUI shotsText;
    private int shots;

    private void Start()
    {
        shots = 0;
        shotsText.text = "Shots: " + shots;
    }

    void Update()
    {
        if (!freeShot && physicsManager.GetBallVelocity() > 0.5f) return;

        screenPosition = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = screenPosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endMousePos = screenPosition;

            physicsManager.SetShotPosition();

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
            shots++;
            shotsText.text = "Shots: " + shots;
        }

    }
}
