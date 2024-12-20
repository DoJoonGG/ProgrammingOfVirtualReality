using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh terrainMesh;
    Mesh waterMesh;

    Vector3[] terrainVertices;
    int[] terrainTriangles;
    Color[] terrainColors;

    public int xSize = 20;
    public int zSize = 20;

    public float strength = 0.3f;

    public Gradient gradient; // ���䳺�� ��� �������

    public float waterHeight = 0.5f; // ������ ����
    public Color waterColor = Color.blue; // ���� ����

    float minTerrainHeight;
    float maxTerrainHeight;

    public Texture2D groundTexture; // �������� �����
    public Material terrainMaterial; // �������� ��� ��������

    void Start()
    {
        terrainMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = terrainMesh;

        // �������� ��������� ��� �������� � ���������
        terrainMaterial = new Material(Shader.Find("Standard"));
        terrainMaterial.mainTexture = groundTexture; // ��������� �������� � ���������

        // ��������� �������� � ���������
        MeshRenderer terrainRenderer = gameObject.AddComponent<MeshRenderer>();
        terrainRenderer.material = terrainMaterial;

        // �������� ���������� ������� ��� ����
        GameObject water = new GameObject("Water");
        water.transform.parent = this.transform;

        // ��������� ������� ����
        MeshRenderer waterRenderer = water.AddComponent<MeshRenderer>();
        Material waterMaterial = new Material(Shader.Find("Standard"))
        {
            color = new Color(0.3f, 0.8f, 0.7f, 0.25f) // ��������� ���� � 25% ��������������
        };
        waterMaterial.SetFloat("_Mode", 3); // ������������� ����� "Transparent"
        waterMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        waterMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        waterMaterial.SetInt("_ZWrite", 0);
        waterMaterial.DisableKeyword("_ALPHATEST_ON");
        waterMaterial.EnableKeyword("_ALPHABLEND_ON");
        waterMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        waterMaterial.renderQueue = 3000; // ���������� ������� ���������� ����� ������������
        waterRenderer.material = waterMaterial;

        // ��������� ���� ����
        MeshFilter waterFilter = water.AddComponent<MeshFilter>();
        waterMesh = new Mesh();
        waterFilter.mesh = waterMesh;

        StartCoroutine(CreateShape());
    }




    private void Update()
    {
        UpdateTerrainMesh();
        UpdateWaterMesh();
    }

    IEnumerator CreateShape()
    {
        terrainVertices = new Vector3[(xSize + 1) * (zSize + 1)];
        terrainColors = new Color[terrainVertices.Length];

        // ����������� �������� �� ����������� ������
        minTerrainHeight = float.MaxValue;
        maxTerrainHeight = float.MinValue;

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * strength, z * strength) * 2f;
                terrainVertices[i] = new Vector3(x, y, z);

                // ��������� �������� � ����������� ������
                if (y > maxTerrainHeight) maxTerrainHeight = y;
                if (y < minTerrainHeight) minTerrainHeight = y;

                i++;
            }
        }

        // ���������� ��� ��������
        terrainTriangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                terrainTriangles[tris + 0] = vert + 0;
                terrainTriangles[tris + 1] = vert + xSize + 1;
                terrainTriangles[tris + 2] = vert + 1;
                terrainTriangles[tris + 3] = vert + 1;
                terrainTriangles[tris + 4] = vert + xSize + 1;
                terrainTriangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;

                yield return new WaitForSeconds(0.01f);
            }
            vert++;
        }

        // ��������� ������� ��� ����� ������� ��������
        for (int i = 0; i < terrainVertices.Length; i++)
        {
            float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, terrainVertices[i].y);
            if (terrainVertices[i].y < waterHeight)
            {
                terrainColors[i] = waterColor; // ���� ���� ����, ��������� ������� ����
            }
            else
            {
                terrainColors[i] = gradient.Evaluate(height); // ����� ���������� ��������
            }
        }

        // �������� UV-���� ��� ��������
        Vector2[] uv = new Vector2[terrainVertices.Length];
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uv[z * (xSize + 1) + x] = new Vector2((float)x / xSize, (float)z / zSize);
            }
        }
        terrainMesh.uv = uv; // ������������� UV-�����

        // ������� ��� ��� ����
        CreateWaterMesh();
    }


    void UpdateTerrainMesh()
    {
        terrainMesh.Clear();

        terrainMesh.vertices = terrainVertices;
        terrainMesh.triangles = terrainTriangles;
        terrainMesh.colors = terrainColors;

        terrainMesh.RecalculateNormals();
    }

    void CreateWaterMesh()
    {
        List<Vector3> waterVertices = new List<Vector3>();
        List<int> waterTriangles = new List<int>();

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // �������� ������� ������� ������ ��������
                int i1 = z * (xSize + 1) + x;
                int i2 = i1 + xSize + 1;
                int i3 = i1 + 1;
                int i4 = i2 + 1;

                // ���������� ������ ����
                Vector3 v1 = new Vector3(x, waterHeight, z);
                Vector3 v2 = new Vector3(x, waterHeight, z + 1);
                Vector3 v3 = new Vector3(x + 1, waterHeight, z);
                Vector3 v4 = new Vector3(x + 1, waterHeight, z + 1);

                // ���������: ���� ��� ������� �������� ���� ����, ��������� ������������
                if (terrainVertices[i1].y < waterHeight || terrainVertices[i2].y < waterHeight ||
                    terrainVertices[i3].y < waterHeight || terrainVertices[i4].y < waterHeight)
                {
                    int vertIndex = waterVertices.Count;

                    waterVertices.Add(v1);
                    waterVertices.Add(v2);
                    waterVertices.Add(v3);
                    waterVertices.Add(v4);

                    // ������������ ��� ����
                    waterTriangles.Add(vertIndex + 0);
                    waterTriangles.Add(vertIndex + 1);
                    waterTriangles.Add(vertIndex + 2);
                    waterTriangles.Add(vertIndex + 2);
                    waterTriangles.Add(vertIndex + 1);
                    waterTriangles.Add(vertIndex + 3);
                }
            }
        }

        waterMesh.Clear();
        waterMesh.vertices = waterVertices.ToArray();
        waterMesh.triangles = waterTriangles.ToArray();
        waterMesh.RecalculateNormals();
    }


    void UpdateWaterMesh()
    {
        // ���� ��������, ���������� ����� ������ ������ �� �������������
    }

    private void OnDrawGizmos()
    {
        if (terrainVertices == null) return;

        for (int i = 0; i < terrainVertices.Length; i++)
        {
            Gizmos.DrawSphere(terrainVertices[i], 0.1f);
        }
    }
}
