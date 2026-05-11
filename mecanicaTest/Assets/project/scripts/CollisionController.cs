using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public struct ColliderBox
    {
        public Vector3 corner1; //Abajo izquierda
        public Vector3 corner2; //Arriba derecha
    };

    [Header("Ball")]
    [SerializeField] private Transform ball;
    [SerializeField] private BallController BC;
    [SerializeField] private bool colliding;

    [Header("Grounds")]

    


    [SerializeField] private List<Transform> grounds = new List<Transform>();
    [SerializeField] private List<ColliderBox> colliders = new List<ColliderBox>();
   
    void Start()
    {
        foreach (Transform t in grounds)
        {
            
            ColliderBox c = new ColliderBox();
            
            c.corner1 = new Vector3(t.transform.position.x - t.localScale.x/2, 
                t.transform.position.y - t.localScale.y / 2, 
                t.transform.position.z - t.localScale.z / 2);
            c.corner2 = new Vector3(t.transform.position.x + t.localScale.x / 2, 
                t.transform.position.y + t.localScale.y / 2, 
                t.transform.position.z + t.localScale.z / 2);

            colliders.Add(c);
        }
    }

    public (bool, float) CollidingWithGround()
    {
        foreach (ColliderBox c in colliders)
        {
            if (((ball.position.x + BC.radius) > c.corner1.x && (ball.position.x + BC.radius) <= c.corner2.x ||
            (ball.position.x - BC.radius) > c.corner1.x && (ball.position.x - BC.radius) <= c.corner2.x)
            &&
            ((ball.position.y + BC.radius) > c.corner1.y && (ball.position.y + BC.radius) <= c.corner2.y ||
            (ball.position.y - BC.radius) > c.corner1.y && (ball.position.y - BC.radius) <= c.corner2.y)
            &&
            ((ball.position.z + BC.radius) > c.corner1.z && (ball.position.z + BC.radius) <= c.corner2.z ||
            (ball.position.z - BC.radius) > c.corner1.z && (ball.position.z - BC.radius) <= c.corner2.z))
            {
                Debug.Log("Colliding!!!!!!! Now fixing position :)");
                return (true, c.corner2.y);
            }
        }
        
        return (false, -1);
    }

}
