using UnityEngine;

public class MoveForward : MonoBehaviour
{
    private Vector3 direction = Vector3.back;
    public float speed;

    void Start()
    {
        speed = LevelManager.currentSpeed;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}

