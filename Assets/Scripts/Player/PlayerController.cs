using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float hitForce;
    [SerializeField] private float sizeGrowthPerFood;
    [SerializeField] private float forceIncreasePerFood;
    [SerializeField] private float massIncreasePerFood;
   
    [Header("ScriptableObjects")]
    [SerializeField] private GameModeSO gameModeSO;
    [SerializeField] private GameStateSO gameStateSO;

    [Header("Scripts")]
    [SerializeField] private FrontAreaDetector frontAreaDetector;
    [SerializeField] private ObjectPool objectPool;

    private PlayerInputMap playerInput;
    private Animator animator;
    private Rigidbody rb;
   
    private enum PlayerState
    {
        None,
        Idle,
        Move,
    }

    private PlayerState currentState = PlayerState.Idle;
    private Vector3 movementInput;
    private Vector3 velocity = Vector3.zero;
    
    private void Awake()
    {
        // New input system script's object
        playerInput = new PlayerInputMap();
        animator = GetComponent<Animator>();    
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        gameModeSO.OnGameStart += OnGameStart;
        gameModeSO.OnGameWin += OnGameWin;
        gameModeSO.OnGameLost += OnGameLost;

        playerInput.Player.Move.performed += OnTouchPerformed;
        playerInput.Player.Move.canceled += OnTouchCanceled;

        frontAreaDetector.OnHit += OnHit;
    }

    private void OnDisable()
    {
        gameModeSO.OnGameStart -= OnGameStart;
        gameModeSO.OnGameWin -= OnGameWin;
        gameModeSO.OnGameLost -= OnGameLost;

        playerInput.Player.Move.performed -= OnTouchPerformed;
        playerInput.Player.Move.canceled -= OnTouchCanceled;

        frontAreaDetector.OnHit -= OnHit;
    }

    private void Update()
    {
        if (gameStateSO.currentState == GameStateSO.State.GameStart)
        {
            if (currentState == PlayerState.Idle)
            {
                SetPlayerIdle();
            }
            if(currentState == PlayerState.Move)
            {
                Move();
            }
        } 
    }
    // Enable input when game start
    private void OnGameStart()
    {
        playerInput.Enable();
    }
    // Sets the appropriate position, rotation, animation when game wins
    private void OnGameWin()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        animator.SetBool("isWalking", false);
        animator.SetTrigger("isWin");
        playerInput.Disable();
    }
    // Sets the appropriate position, rotation, animation when game lost
    private void OnGameLost()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        animator.SetBool("isWalking", false);
        animator.SetTrigger("isLost");
        playerInput.Disable();
    }
    // Get input and set state when player touch joystick
    private void OnTouchPerformed(InputAction.CallbackContext obj)
    {
        currentState = PlayerState.Move;
        movementInput = new Vector3(obj.ReadValue<Vector2>().x, 0f, obj.ReadValue<Vector2>().y);
    }
    // Change state when player let the joystick
    private void OnTouchCanceled(InputAction.CallbackContext obj)
    {
        currentState = PlayerState.Idle;
    }
    // Moves the player according to input
    private void Move()
    {
        animator.SetBool("isWalking", true);

        transform.Translate(movementInput * Time.deltaTime * speed, Space.World);

        if (movementInput != Vector3.zero)
            gameObject.transform.forward = Vector3.SmoothDamp(gameObject.transform.forward, movementInput, ref velocity, rotationSpeed);
    }

    private void SetPlayerIdle()
    {
        animator.SetBool("isWalking", false);
    }
    // This method invokes when OnHit action fired by FrontAreaDetecter
    private void OnHit(Collider col)
    {
        animator.SetTrigger("isHitting");
        col.GetComponentInParent<Rigidbody>().AddForce(transform.forward * hitForce);
        col.GetComponentInParent<AIController>().OnGotHit();
    }
    // This method invokes when oppenent hit to this object
    public void OnGotHit()
    {
        animator.SetTrigger("isGettingHit");
        playerInput.Disable();
    }
    // It is connected to FallPlayer animation
    private void OnGetUp()
    {
        playerInput.Enable();
    }

    private void OnTriggerEnter(Collider other)
    {
        float maxSizeIncrease = 4;
        // Get food and increase size, mass and force
        if (other.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);
            objectPool.disabledFoodCount++;

            if (transform.localScale.x < maxSizeIncrease)
                transform.localScale = new Vector3(transform.localScale.x + sizeGrowthPerFood, transform.localScale.y + sizeGrowthPerFood, transform.localScale.z + sizeGrowthPerFood);
            hitForce += forceIncreasePerFood;
            rb.mass += massIncreasePerFood;
        }
        // When collision happens with lose collider
        if (other.CompareTag("Lose"))
        {
            gameModeSO.RaiseOnGameEnd();
            gameModeSO.RaiseOnGameLost();
        }
    }

    private bool Equal(float a, float b, float tolerance)
    {
        return (Mathf.Abs(a - b) <= tolerance);
    }
}
