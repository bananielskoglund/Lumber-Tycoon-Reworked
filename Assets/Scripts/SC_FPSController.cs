using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float crouchingSpeed = 3f;
    public float walkingSpeed = 5f;
    public float jumpSpeed = 7f;
    public float runningSpeed = 7.5f;
    public float gravity = 30.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public bool canMove = true;
    public Animator animator;

    [Header("Axe Settings")]
    public SphereCollider AxeCollider;
    public int LogsCount = 0;
    public int playerBalance = 0;
    public GameObject LogsText;
    public GameObject BalanceText;
    public GameObject StandardAxe, Axe1, Axe2, MarketAxe1, MarketAxe2;
    public GameObject AxeColliderObj;

    [Header("Stamina")]
    public StaminaBar staminaBar;
    public int maxStamina = 100;
    public float currentStamina;
    public float staminaLoss = 0.05f;
    public float staminaGain = 0.05f;

    [Header("Shop")]
    public float raycastRange = 5;
    int onAxe = 1;

    // [Header("Vision Settings")]

    float curSpeedX;
    float curSpeedY;
    bool isCrouching = false;
    bool isRunning = false;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    // [HideInInspector]

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
    }

    void RaycastHandling()
    {
        Vector3 direction = Vector3.forward;

        Ray myRayCast = new Ray(transform.position, transform.TransformDirection(direction * raycastRange));
        Debug.DrawRay(transform.position, transform.TransformDirection(direction * raycastRange));

        if (Physics.Raycast(myRayCast, out RaycastHit hit, raycastRange))
        {
            if (hit.collider.tag == "Shop"){
                // ShopObject.GetComponent<Outline>().enabled = true;
                if (Input.GetKeyDown(KeyCode.B))
                {
                    if(playerBalance >= 10000)
                    {
                        Application.Quit();
                    }
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    playerBalance += 2*LogsCount;
                    LogsCount = 0;
                }
                else if (Input.GetKeyDown(KeyCode.B) && onAxe == 1)
                {
                    if(playerBalance >= 100)
                    {
                        StandardAxe.SetActive(false);
                        MarketAxe1.SetActive(false);
                        Axe1.SetActive(true);
                        AxeColliderObj.GetComponent<AxeMechanics>().LogsFromHit = 5;
                        playerBalance -= 100;
                        onAxe = 2;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.B) && onAxe == 2)
                {
                    if(playerBalance >= 1000)
                    {
                        Axe1.SetActive(false);
                        MarketAxe2.SetActive(false);
                        Axe2.SetActive(true);
                        AxeColliderObj.GetComponent<AxeMechanics>().LogsFromHit = 10;
                        playerBalance -= 1000;
                        onAxe = 3;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.B) && onAxe == 3)
                {
                    if(playerBalance >= 10000)
                    {
                        Application.Quit();
                    }
                }
                
                // else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape)  && shopIsOpen == true)
                // {
                //     Cursor.visible = false;
                //     ShopCanvas.SetActive(false);
                //     shopIsOpen = false;
                //     GetComponent<SC_FPSController>().canMove = true;
                //     Cursor.lockState = CursorLockMode.Locked;
                // }
            }
        }

    }

    void MovementAnimationHandling()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            animator.SetBool("isWalkingForward", true);
        }
        else 
        {
            animator.SetBool("isWalkingForward", false);
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            animator.SetBool("isWalkingBack", true);
        }
        else 
        {
            animator.SetBool("isWalkingBack", false);
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) < Input.GetAxis("Horizontal"))
        {
            animator.SetBool("isWalkingRight", true);
        }
        else 
        {
            animator.SetBool("isWalkingRight", false);
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) < Input.GetAxis("Horizontal")*-1)
        {
            animator.SetBool("isWalkingLeft", true);
        }
        else 
        {
            animator.SetBool("isWalkingLeft", false);
        }
    }

    void MovementHandling()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        curSpeedY = canMove ? walkingSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (canMove){
            if (Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Vertical") > 0))
            {
                if(currentStamina > 0.5){
                    curSpeedX = runningSpeed * Input.GetAxis("Vertical");
                    animator.SetBool("isRunningForward", true);
                    currentStamina -= staminaLoss;
                    staminaBar.SetStamina(currentStamina);
                }
                else{
                    curSpeedX = walkingSpeed * Input.GetAxis("Vertical");
                    animator.SetBool("isWalkingForward", true);
                    animator.SetBool("isRunningForward", false);
                }
            }
            else{
                curSpeedX = walkingSpeed * Input.GetAxis("Vertical");
                animator.SetBool("isRunningForward", false);
                if (currentStamina < 100){
                    currentStamina += staminaGain;
                    staminaBar.SetStamina(currentStamina);
                }
            }

            if (Input.GetKey(KeyCode.LeftControl) && characterController.isGrounded)
            {
                curSpeedX = crouchingSpeed * Input.GetAxis("Vertical");
                // playerTransform.position = new Vector3(playerTransform.position.x, transform.position.y-0.3f, playerTransform.position.z);
                isCrouching = true;
            }
            else{
                // playerTransform.position = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
                isCrouching = false;
            }
        }
        else
        {
            curSpeedX = 0;
        }

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !isCrouching)
        // !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Jump Idle")
        {
            moveDirection.y = jumpSpeed;
            // animator.SetBool("isJumping", true);
        }
        else
        {
            moveDirection.y = movementDirectionY;
            // animator.SetBool("isJumping", false);
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    void Chopping()
    {
        animator.SetBool("isChopping", false);      
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Chop 1"))
        {
            canMove = false;
            AxeCollider.enabled = true;
        }
        else
        {
            AxeCollider.enabled = false;
            canMove = true;
            if (Input.GetMouseButton(0))
            {
                animator.SetBool("isChopping", true);
            }
        }
    }

    void InterfaceHandling()
    {
        LogsText.GetComponent<TextMeshProUGUI>().text = LogsCount+"x";
        BalanceText.GetComponent<TextMeshProUGUI>().text = "$"+playerBalance;
    }

    void Update()
    {
        MovementAnimationHandling();
        MovementHandling();
        Chopping();
        InterfaceHandling();
        RaycastHandling();
    }
}