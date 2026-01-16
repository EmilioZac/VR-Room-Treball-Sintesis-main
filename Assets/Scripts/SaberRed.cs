using UnityEngine;

public class SaberRed : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entra tiene tag "Cube"
        if (other.CompareTag("CubeRed"))
        {
            GameManager.Puntuacion ++;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Mine"))
        {
            GameManager.Puntuacion --;
            Destroy(other.gameObject);
        }
    }
}


