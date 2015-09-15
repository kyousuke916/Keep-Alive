using UnityEngine;
using System.Collections;

public class TestDragDrop : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 100f;

    void Start()
    {

    }

    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

}
