using UnityEngine;
using UnityEngine.UI;

public class ArrowFill : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Transform ballTransform;
    [SerializeField] Image imageFill;
    [SerializeField] Image imageArrow;
    [SerializeField] BallController ballController;

    float maxForce = 20f;

    void Update()
    {
        if (ballController.GetShootDirection() != Vector3.zero && ballController.GetShootVelocity() > 0.01f)
        {
            imageFill.enabled = true;
            imageArrow.enabled = true;

            float angle = Mathf.Atan2(ballController.GetShootDirection().z, ballController.GetShootDirection().x) * Mathf.Rad2Deg;

            rectTransform.rotation = Quaternion.Euler(90f, 0f, angle - 90f);

            imageFill.fillAmount = ballController.GetShootVelocity() / maxForce;
        }
        else
        {
            imageFill.enabled = false;
            imageArrow.enabled = false;
        }
    }
}