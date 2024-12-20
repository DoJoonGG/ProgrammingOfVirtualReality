using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость передвижения камеры
    public float lookSensitivity = 2f; // Чувствительность мыши

    private float rotationX = 0f; // Угол вращения по оси X (вверх-вниз)
    private float rotationY = 0f; // Угол вращения по оси Y (влево-вправо)

    void Start()
    {
        // Скрываем и блокируем курсор для удобного управления камерой
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Обработка передвижения камеры с помощью клавиш WASD
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // Перемещение влево-вправо (A и D)
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;   // Перемещение вперед-назад (W и S)

        transform.Translate(moveX, 0, moveZ);

        // Обработка вращения камеры с помощью мыши
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;

        // Ограничение угла вращения по оси X (вверх-вниз)
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // Применение вращения к камере
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
