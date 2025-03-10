using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float slideSpeed;
    public float jumpForce;
    public float gravity = -9.81f;
    public float rotationSpeed = 200f;
    public float mouseSensitivity;
    public float minVerticalAngle = -85f;
    public float maxVerticalAngle = 70f;
    private float verticalRotation = 0f;
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private Animation anim;
    public float maxStamina; // Seconds of sliding
    private float currentStamina;
    private bool isSliding = false;
    private bool isRecovering = false;

    public float staminaRecoveryTime; // Time to refill stamina
    public Slider staminaBar; // Assign in the Inspector

    // References for slapping bones
    public Transform rightShoulder;
    public Transform rightArm;
    private bool isSlapping = false;
    private bool wasSliding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animation>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;
        if (staminaBar) staminaBar.maxValue = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (staminaBar) staminaBar.value = currentStamina;

        ApplyGravity();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float speed = isSliding ? slideSpeed : moveSpeed;
        Vector3 move = characterController.transform.forward * moveInput.y 
                 + characterController.transform.right * moveInput.x;

        characterController.Move(move * speed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // Horizontal rotation (left/right)
        transform.Rotate(Vector3.up * lookInput.x * rotationSpeed * mouseSensitivity * Time.deltaTime);

        // Vertical rotation (up/down) with limits
        verticalRotation -= lookInput.y * rotationSpeed * mouseSensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
        
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput == Vector2.zero)
        {
            anim.Stop("walk");
            anim.Play("idle");
        }
        else
        {
            anim.Stop("idle");
            anim.Play("walk");
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (context.started && !isRecovering && currentStamina > 0)
        {
            isSliding = true;
            anim.Play("run");
            StartCoroutine(SlideDuration());
        }
    }

    private IEnumerator SlideDuration()
    {
        float slideTime = 0f;

        while (slideTime < maxStamina && isSliding)
        {
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                anim.Stop("run");
                anim.Play("walk");
            }
            slideTime += Time.deltaTime;
            yield return null;
        }

        isSliding = false;
        StartCoroutine(StaminaCooldown());
    }

    private IEnumerator StaminaCooldown()
    {
        isRecovering = true;
        yield return new WaitForSeconds(staminaRecoveryTime);

        // Refill stamina over time
        while (currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime;
            yield return null;
        }

        currentStamina = maxStamina;
        isRecovering = false;
    }

    public void OnSlap(InputAction.CallbackContext context)
    {
        if (context.performed && !isSlapping)
        {
            StartCoroutine(PerformSlap());
            // Find all enemies in range (assuming they have the "EnemyBehavior" script attached)
            Collider[] enemiesInRange = Physics.OverlapSphere(rightShoulder.position, 5f); // Adjust radius as needed

            foreach (Collider enemyCollider in enemiesInRange)
            {
                // Check if the enemy has the "EnemyBehavior" script attached
                EnemyBehavior enemyBehavior = enemyCollider.GetComponent<EnemyBehavior>();
                if (enemyBehavior != null)
                {
                    enemyBehavior.OnSlap(); // Call the OnSlap method of each enemy
                }
            }
        }
    }

    private IEnumerator PerformSlap()
    {
        isSlapping = true;

        if (rightShoulder == null || rightArm == null)
        {
            Debug.LogWarning("RightShoulder or RightArm is not assigned!");
            isSlapping = false;
            yield break;
        }

        anim.Stop("idle");
        anim.Stop("walk");

        if (isSliding) 
        {
            wasSliding = true; // Track if player was sliding before slapping
            isSliding = false; // Stop sliding
            anim.Stop("run"); // Stop slide animation
        }
        StopCoroutine(SlideDuration());

        Quaternion startRotationShoulder = rightShoulder.localRotation;
        Quaternion targetRotationShoulder = startRotationShoulder * Quaternion.Euler(-80, -80, -30); 

        Quaternion startRotationArm = rightArm.localRotation;
        Quaternion targetRotationArm = startRotationArm * Quaternion.Euler(-80, -80, -45);

        float elapsedTime = 0f;
        float slapDuration = 0.1f;

        while (elapsedTime < slapDuration)
        {
            rightShoulder.localRotation = Quaternion.Lerp(startRotationShoulder, targetRotationShoulder, elapsedTime / slapDuration);
            rightArm.localRotation = Quaternion.Lerp(startRotationArm, targetRotationArm, elapsedTime / slapDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.05f); // Hold position briefly

        elapsedTime = 0f;
        while (elapsedTime < slapDuration)
        {
            rightShoulder.localRotation = Quaternion.Lerp(targetRotationShoulder, startRotationShoulder, elapsedTime / slapDuration);
            rightArm.localRotation = Quaternion.Lerp(targetRotationArm, startRotationArm, elapsedTime / slapDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (moveInput == Vector2.zero)
        {
            anim.Play("idle"); // Player is standing still
        }
        else
        {
            anim.Play("walk"); // Player was moving
        }

        if (!isRecovering)
        {
            StartCoroutine(StaminaCooldown());
        }

        wasSliding = false; // Reset sliding state
        isSlapping = false; // Slap is finished
    }
}
