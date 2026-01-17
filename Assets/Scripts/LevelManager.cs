using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static float currentSpeed = 1f;
    public GameObject spawner; // arrastra aquí tu objeto Spawner

    private SpawnerDif1 sd1;
    private SpawnerDif2 sd2;
    private SpawnerDif3 sd3;

    void Start()
    {
        sd1 = spawner.GetComponent<SpawnerDif1>();
        sd2 = spawner.GetComponent<SpawnerDif2>();
        sd3 = spawner.GetComponent<SpawnerDif3>();

        sd1.enabled = false;
        sd2.enabled = false;
        sd3.enabled = false;
    }

    public void Nivel1()
    {
        SetLevelSpeed(1f);
        sd1.enabled = true;
    }

    public void Nivel2()
    {
        SetLevelSpeed(2f);
        sd2.enabled = true;
    }

    public void Nivel3()
    {
        SetLevelSpeed(8f);
        sd3.enabled = true;
    }

    void SetLevelSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
        MoveForward[] movers = FindObjectsOfType<MoveForward>();
        foreach (MoveForward m in movers)
            m.speed = newSpeed;
    }
}


