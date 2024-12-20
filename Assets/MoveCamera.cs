using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float moveSpeed = 5f; // �������� ������������ ������
    public float lookSensitivity = 2f; // ���������������� ����

    private float rotationX = 0f; // ���� �������� �� ��� X (�����-����)
    private float rotationY = 0f; // ���� �������� �� ��� Y (�����-������)

    void Start()
    {
        // �������� � ��������� ������ ��� �������� ���������� �������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ��������� ������������ ������ � ������� ������ WASD
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // ����������� �����-������ (A � D)
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;   // ����������� ������-����� (W � S)

        transform.Translate(moveX, 0, moveZ);

        // ��������� �������� ������ � ������� ����
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;

        // ����������� ���� �������� �� ��� X (�����-����)
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // ���������� �������� � ������
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
