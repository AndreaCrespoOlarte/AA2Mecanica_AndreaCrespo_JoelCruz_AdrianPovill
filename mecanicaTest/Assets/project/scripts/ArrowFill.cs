using UnityEngine;
using UnityEngine.UI;

public class ArrowFill : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Image image;

    Vector3 arrowDirection; 
    float fillAmount;

    void Update()
    {
        if(arrowDirection != null && arrowDirection != Vector3.zero)
        {
            image.enabled = true;
            rectTransform.localEulerAngles = arrowDirection;
            image.fillAmount = fillAmount;
        }
        
        image.enabled = false;
    }

    public void SetArrowDirection(Vector3 newArrowDirection)
    {
        arrowDirection = newArrowDirection;
    }

    public void SetFillAmount(float newFillAmount) //Del 0 al 1
    {
        fillAmount = newFillAmount;
    }
}
