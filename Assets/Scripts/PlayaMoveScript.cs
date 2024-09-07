//most of movement script from Comp-3 Interactive player movement tutorial
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class PlayaMoveScript: MonoBehaviour
{
    public bool canMove { get; private set; } = true;
    private bool isSprinting => canSprint && Input.GetKey(sprintKey) && Input.GetKey(KeyCode.W);
    private bool shouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool shouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation;


    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool willSlideOnSlopes = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool canUseHeadbob = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] public KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeSpeed = 8.0f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchTime = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = 3f;
    //[SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;


    //declaring methods for movement

    private void HandleMovmementInput()
    {
        
        currentInput = new Vector2((isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxisRaw("Vertical"), (isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxisRaw("Horizontal"));

        float moveDirectionY = moveDirection.y;
        if (currentInput.x != 0 && currentInput.y != 0)
        {
            moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x * 0.7071f) + (transform.TransformDirection(Vector3.right) * currentInput.y * 0.7071f);
        }
        else
        {
            moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        }
        moveDirection.y = moveDirectionY;
    }

    private void HandleJump()
    {
        if(shouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }

    private void HandleCrouch()
    {
        if(shouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private void HandleHeadbob()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        if (Mathf.Abs(new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude) > 0.1f || (Mathf.Abs(defaultYPos - playerCamera.transform.localPosition.y) > 0.01f))
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);

            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount), playerCamera.transform.localPosition.z);
        }
        else if (playerCamera.transform.localPosition.y != defaultYPos)
        {
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultYPos, playerCamera.transform.localPosition.z);
        }
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.layer == 6 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
    }
    //&& Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer)

    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
        {
            yield break;
        }

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < crouchTime)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/crouchTime);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / crouchTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxisRaw("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -lowerLookLimit, upperLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Mouse X") * lookSpeedX, 0);
    }

    private Vector3 hitPointNormal;
    private bool isSliding
    {
        get
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    private void ApplyFinalMovements()
    {
        if(!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if(willSlideOnSlopes && isSliding)
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        //some conditions will prevent the player from moving
        if (canMove)
        {
            HandleMovmementInput();
            if (canJump)
            {
                HandleJump();
            }

            if (canCrouch)
            {
                HandleCrouch();
            }

            if (canUseHeadbob)
            {
                HandleHeadbob();
            }

            if (canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            HandleMouseLook();
            
            ApplyFinalMovements();
        }
    }
}
