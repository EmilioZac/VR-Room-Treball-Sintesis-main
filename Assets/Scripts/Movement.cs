using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;  // Dirección del movimiento
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.Self);
    }
    void Start()
    {
        speed = LevelManager.currentSpeed;
    }

}
