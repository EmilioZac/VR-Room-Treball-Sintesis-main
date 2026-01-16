using UnityEngine;

public class DeletePrefabs : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entra tiene tag "Cube"
        if (other.CompareTag("CubeRed"))
        {
            Destroy(other.gameObject);
            //hola
        }
        if (other.CompareTag("CubeBlue"))
        {
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Mine"))
        {
            Destroy(other.gameObject);
        }
    }
}


