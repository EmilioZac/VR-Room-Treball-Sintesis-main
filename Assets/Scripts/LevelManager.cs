using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static float currentSpeed = 3f;

    public void Nivel1()
    {
        SetLevelSpeed(3f);
    }

    public void Nivel2()
    {
        SetLevelSpeed(5f);
    }

    public void Nivel3()
    {
        SetLevelSpeed(10f);
    }

    void SetLevelSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;

        MoveForward[] movers = FindObjectsOfType<MoveForward>();
        foreach (MoveForward m in movers)
        {
            m.speed = newSpeed;
        }
    }
}

