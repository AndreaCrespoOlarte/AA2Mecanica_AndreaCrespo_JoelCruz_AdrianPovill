using UnityEngine;

public class CameraControl : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private Transform ballTransform;

    [SerializeField] private Vector3 offset;

    [SerializeField] private float rotationSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam.transform.position = ballTransform.position + offset;
        cam.transform.LookAt(ballTransform.position);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;

            offset = Quaternion.AngleAxis(mouseX, Vector3.up) * offset;
        }


        cam.transform.position = ballTransform.position + offset;
        cam.transform.LookAt(ballTransform.position);
    }
}
