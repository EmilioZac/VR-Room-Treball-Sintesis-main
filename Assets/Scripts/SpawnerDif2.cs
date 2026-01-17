using UnityEngine;

public class SpawnerDif2 : MonoBehaviour
{
    public GameObject prefab1;  // primer prefab
    public GameObject prefab2;  // segundo prefab

    public float spawnTime = 2f;   // cada cuánto tiempo spawnea
    private float timer = 0f;

    public BoxCollider spawnArea;    // el Box Collider donde aparecerán

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnTime)
        {
            Spawn();
            timer = 0f;
        }
    }

    void Spawn()
    {
        // decidir si spawnea 1, 2 o 3
        int count = Random.Range(1, 3); // 1, 2 o 3

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = GetRandomPosition();

            // elegir uno de los 3 prefabs
            int choice = Random.Range(1, 3); // 1, 2 o 3

            GameObject selected;

            if (choice == 1)
                selected = prefab1;
            else
                selected = prefab2;

            GameObject go = Instantiate(selected, pos, Quaternion.identity);

            // Buscar MoveForward en el prefab o en sus hijos
            MoveForward mover = go.GetComponentInChildren<MoveForward>();
            if (mover != null)
                mover.speed = LevelManager.currentSpeed;
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 size = spawnArea.size;
        Vector3 center = transform.position + spawnArea.center;

        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, y, z);
    }
}
