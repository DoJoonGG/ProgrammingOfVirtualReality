using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    public int cloudCount = 20; // Количество облаков
    public float cloudAreaSize = 50f; // Площадь облаков
    public Vector2 cloudHeightRange = new Vector2(10f, 20f); // Высотный диапазон облаков
    public GameObject cloudPrefab; // Префаб облака
    private List<GameObject> clouds = new List<GameObject>();

    public Vector3 terrainCenter = new Vector3(0f, 0f, 0f);

    void Start()
    {
        for (int i = 0; i < cloudCount; i++)
        {
            CreateCloud();
        }
    }

    void CreateCloud()
    {
        Vector3 position = new Vector3(
            terrainCenter.x + Random.Range(-cloudAreaSize / 2, cloudAreaSize / 2),
            Random.Range(cloudHeightRange.x, cloudHeightRange.y),
            terrainCenter.z + Random.Range(-cloudAreaSize / 2, cloudAreaSize / 2)
        );

        GameObject cloud = Instantiate(cloudPrefab, position, Quaternion.identity);
        cloud.transform.parent = this.transform;

        // Генерация случайного масштаба от 0.1 до 1
        float randomScale = Random.Range(0.1f, 1f);
        cloud.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        clouds.Add(cloud);
    }

    void Update()
    {
        foreach (var cloud in clouds)
        {
            cloud.transform.position += new Vector3(0.01f, 0, 0);

            if (cloud.transform.position.x > terrainCenter.x + cloudAreaSize / 2)
            {
                cloud.transform.position = new Vector3(terrainCenter.x - cloudAreaSize / 2, cloud.transform.position.y, cloud.transform.position.z);
            }
        }
    }
}
