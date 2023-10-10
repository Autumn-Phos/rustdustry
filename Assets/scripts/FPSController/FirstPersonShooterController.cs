using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonShooterController : MonoBehaviour
{
    public float standingHeight = 2.0f; // Высота стоящего персонажа
    public float crouchingHeight = 1.5f; // Высота присевшего персонажа
    public float moveSpeedRunning = 7f; // Скорость перемещения во время бега
    public float moveSpeedStanding = 4.5f; // Скорость перемещения в стоячем положении
    public float moveSpeedCrouching = 2.7f; // Скорость перемещения в присевшем положении
    public float mouseSensitivity = 2f; // Чувствительность мыши
    public float jumpForce = 1.2f; // Сила прыжка
    public float gravity = 5f; // Гравитация
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    private bool switchJumping;
    private bool isRunning;
    private bool isGrounded;
    private bool isCrouching;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Захватываем курсор
    }

    void Update()
    {
        // Получаем ввод от клавиатуры
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Получаем ввод от мыши для вращения камеры
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Поворачиваем игрока по горизонтали (влево/вправо)
        transform.Rotate(Vector3.up * mouseX);

        // Поворачиваем камеру по вертикали (вверх/вниз), с ограничением угла обзора
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // Проверяем, находится ли игрок на земле
        isGrounded = characterController.isGrounded;

        // Применяем гравитацию
        if (isGrounded){
            verticalVelocity = -gravity * Time.deltaTime;
        }
        else{
            verticalVelocity -= gravity * Time.deltaTime;
        }
        
        // Проверяем нажата ли кнопка для прыжка
        if (Input.GetButtonDown("Jump"))
        {
            switchJumping = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            switchJumping = false;
        }
        
        // Производим прыжок, если нажата клавиша прыжка и персонаж находится на земле
        if (isGrounded && switchJumping)
        {
            verticalVelocity = jumpForce;
        }

        // Управление приседанием
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = true;
            characterController.height = crouchingHeight;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
            characterController.height = standingHeight;
        }

        // Управление бегом
        if (Input.GetButtonDown("Run"))
        {
            isRunning = true;
        }
        else if (Input.GetButtonUp("Run"))
        {
            isRunning = false;
        }

        // Двигаем игрока вперед/назад и влево/вправо
        Vector3 moveDirection = transform.TransformDirection(new Vector3(horizontalInput, verticalVelocity, verticalInput));
        characterController.Move(moveDirection * (isRunning && !isCrouching ? moveSpeedRunning : (isCrouching ? moveSpeedCrouching : moveSpeedStanding)) * Time.deltaTime);
    }
}
