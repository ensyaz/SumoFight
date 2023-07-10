using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public LayerMask sumoLayer;

    [Header("Attributes")]
    [SerializeField] private float collectRadius;
    [SerializeField] private float wrestleRadius;
    [SerializeField] private float hitForce;
    [SerializeField] private float massIncreasePerFood;
    [SerializeField] private float forceIncreasePerFood;
    [SerializeField] private float sizeGrowthPerFood;

    [Header("Prefabs")]
    [SerializeField] private Transform groundTransform;

    [Header("ScriptableObjects")]
    [SerializeField] private GameModeSO gameModeSO;
    [SerializeField] private GameStateSO gameStateSO;

    [Header("Scripts")]
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private FrontAreaDetector frontAreaDetector;

    public enum AIState
    {
        Patrol,
        Collect,
        Wrestle
    }
    public AIState currentState = AIState.Patrol;
    private NavMeshAgent agent;
    private GameObject nearestFood;
    private GameObject weakestSumo;
    private Rigidbody rb;
    private Animator animator;
    
    private LayerMask foodLayer;
    private Vector3 destination;

    private bool isDestinationSet = false;
    private bool isFoodDestinationSet = false;
    private float nearestDistance = float.MaxValue;
    private float aiMaxDestination;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        foodLayer = LayerMask.GetMask("Food");

        aiMaxDestination = (groundTransform.localScale.x - 3) / 2;
    }

    private void OnEnable()
    {
        frontAreaDetector.OnHit += OnHit;

        gameModeSO.OnGameEnd += OnGameEnd;
    }

    private void OnDisable()
    {
        frontAreaDetector.OnHit -= OnHit;

        gameModeSO.OnGameEnd -= OnGameEnd;
    }

    private void Update()
    {
        if (gameStateSO.currentState == GameStateSO.State.GameStart && agent.enabled)
        {
            switch (currentState)
            {
                case AIState.Patrol:
                    Patrol();
                    break;
                case AIState.Collect:
                    Collect();
                    break;
                case AIState.Wrestle:
                    Wrestle();
                    break;
                default: 
                    break;
            } 
        }
    }

    

    private void Patrol()
    {
        animator.SetBool("isWalking", true);

        // If there is a skinnier opponent than AI around, change state to Wrestle
        Collider[] sumoColliders = Physics.OverlapSphere(transform.position, wrestleRadius, sumoLayer);
        weakestSumo = GetSkinniestSumo(sumoColliders);
        if (sumoColliders.Length > 0 && weakestSumo != null)
        {
            currentState = AIState.Wrestle;
            // When AI goes its destination and state changes, isDestinationSet gets stuck on true. To neglect it, set to false
            isDestinationSet = false;
        }
        // If is there any food in the range of collectRadius then change the current state to Collect
        else if (Physics.CheckSphere(transform.position, collectRadius, foodLayer))
        {
            currentState = AIState.Collect;
            isDestinationSet = false;
        }
        else
        {
            // If AI has no destination point to go, set a destination
            if (isDestinationSet == false)
            {
                destination = GetRandomLocation();
                agent.destination = destination;
                isDestinationSet = true;
            }
            // If AI has arrived to its destination, set isDestinationSet to false to get a new destination
            else if (IsDestinationArrived())
            {
                isDestinationSet = false;
            }
        }
    }

    private void Collect()
    {
        // Get the information about foodLayer around AI along a sphere centered to player with radius of collectRadius
        Collider[] foodColliders = Physics.OverlapSphere(transform.position, collectRadius, foodLayer);
        // If there is a food around AI 
        if (foodColliders.Length > 0)
        {
            // If someone took the food before
            nearestFood = null;

            if (isFoodDestinationSet == false)
            {
                GetNearestFood(foodColliders);
                agent.destination = nearestFood.transform.position;
                isFoodDestinationSet = true;
            }

            else if (nearestFood == null && isFoodDestinationSet == true)
            {
                nearestDistance = float.MaxValue;
                isFoodDestinationSet = false;
            }
        }
        // If there is no food to collect around AI, set the state to Patrol
        else
            currentState = AIState.Patrol;
    }

    private void Wrestle()
    {
        // Chase the oppenent till it is out
        if (!weakestSumo.activeSelf)
        {
            currentState = AIState.Patrol;
        }
        transform.LookAt(weakestSumo.transform.position);
        if (weakestSumo.transform.position.x < aiMaxDestination && weakestSumo.transform.position.z < aiMaxDestination)
            agent.destination = weakestSumo.transform.position;
    }
    // This method invokes when OnHit action fired by FrontAreaDetecter
    private void OnHit(Collider col)
    {
        animator.SetTrigger("isHitting");
        col.GetComponent<Rigidbody>().AddForce(transform.forward * hitForce);
        if (col.name == "Player")
            col.GetComponent<PlayerController>().OnGotHit();
        else
            col.GetComponent<AIController>().OnGotHit();
    }
    // This method invokes when oppenent hit to this object
    public void OnGotHit()
    {
        animator.SetBool("isWalking", false);
        animator.SetTrigger("isGettingHit");
        isDestinationSet = false;
        isFoodDestinationSet = false;
        nearestDistance = float.MaxValue;

        nearestFood = null;
        agent.enabled = false;
    }
    // It is connected to FallAI animation
    private void OnGetUp()
    {
        agent.enabled = true;
        currentState = AIState.Patrol;
    }

    private void OnGameEnd()
    {
        this.gameObject.SetActive(false);
    }

    // Find nearest food around AI
    private void GetNearestFood(Collider[] foodCollider)
    {
        float distance;

        foreach (Collider collider in foodCollider)
        {
            distance = Vector3.Distance(transform.position, collider.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestFood = collider.gameObject;
            }
        }
    }

    private GameObject GetSkinniestSumo(Collider[] sumoColliders)
    {
        foreach (Collider sumo in sumoColliders)
        {
            if (rb.mass > sumo.GetComponent<Rigidbody>().mass)
                return sumo.gameObject;
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        float maxSizeIncrease = 4;
        // Get food and increase size, mass and force
        if (other.CompareTag("Food"))
        {
            nearestFood = null;
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
            gameObject.SetActive(false);
            objectPool.inactiveAICount++;
        }
            
    }

    private Vector3 GetRandomLocation()
    {
        Vector3 randomLocation = (UnityEngine.Random.insideUnitCircle * aiMaxDestination);

        randomLocation.z = randomLocation.y;
        randomLocation.y = transform.position.y;

        return randomLocation;
    }

    private bool IsDestinationArrived()
    {
        return (Mathf.Abs(transform.position.x - destination.x) <= 0.1f && Mathf.Abs(transform.position.z - destination.z) <= 0.1f);
    }
}
