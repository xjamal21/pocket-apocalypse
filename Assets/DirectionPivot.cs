using UnityEngine;
using UnityEngine.InputSystem;

public class DirectionIndicator : MonoBehaviour
{
    public Transform directionPivot;
    public float rotationSpeed = 720f;

    void Update()
    {
        if (directionPivot == null || Keyboard.current == null) return;

        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY -= 1f;

        Vector2 moveDirection = new Vector2(moveX, moveY);

        if (moveDirection != Vector2.zero)
        {
            // 1. Calculate the target angle
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // 2. Smoothly rotate toward the target angle
            directionPivot.rotation = Quaternion.RotateTowards(
                directionPivot.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
