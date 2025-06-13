using System.Collections;
using UnityEngine;

public class AIHandler : MonoBehaviour
{
    [SerializeField]
    CarHandler carHandler; // Reference to the CarHandler component


    //Collision detection
    [SerializeField]
    LayerMask otherCarsLayerMask;

    [SerializeField]
    MeshCollider meshCollider;

    RaycastHit[] raycastHits = new RaycastHit[1]; 
    bool isCarAhead = false;

    // Lanes
    int drivingLane = 0; // Default lane for the AI car

    //Timing
    WaitForSeconds wait = new WaitForSeconds(0.2f); 

    private void Awake()
    {
        if (CompareTag("Player"))
        {
            Destroy(this); // Destroy this component if the GameObject is tagged as "Player"
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(UpdateLessOftenCO()); 
    }

    // Update is called once per frame
    void Update()
    {
        float accelerationInput = 1.0f;
        float steeringInput = 0.0f;
        

        if (isCarAhead)
        {
            accelerationInput = -1; // Brake if a car is detected ahead
            steeringInput = 0.0f; // No steering input if a car is ahead
        }
        else
        {
            steeringInput = Random.Range(-1f, 1f); // Random steering input because AI cars are not very smart
            if (Random.Range(0f, 1f) < 0.01f) // Randomly change lanes with a 1% chance
            {
                drivingLane = (drivingLane + 1) % Utils.CarLanes.Length; // Cycle through available lanes
            }
        }

        float desiredLanePosition = Utils.CarLanes[drivingLane]; // Get the desired lane position from Utils

        float difference = desiredLanePosition - transform.position.x; // Calculate the difference between the desired lane and current position

        if (Mathf.Abs(difference) > 0.05f)
            steeringInput = 1.0f * difference;
        

        steeringInput = Mathf.Clamp(steeringInput, -1.0f, 1.0f); // Ensure steering input is within bounds
        carHandler.SetInput(new Vector2(steeringInput, accelerationInput)); // Set the input for the car handler

    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            isCarAhead = CheckForCarAhead(); // Check if there is a car ahead
            yield return wait;
        }
    }

    bool CheckForCarAhead()
    {
        meshCollider.enabled = false; // Disable the mesh collider to avoid self-collision
        int numHits = Physics.BoxCastNonAlloc(transform.position, Vector3.one * 0.25f, transform.forward, raycastHits, Quaternion.identity, 2, otherCarsLayerMask);

        meshCollider.enabled = true; // Re-enable the mesh collider
        if (numHits > 0)
        {      
            return true; // A car is detected ahead
        }
        return false; // No car ahead

    }


    // Events handlers for AI car behavior

    private void OnEnable()
    {
        carHandler.SetMaxSpeed(Random.Range(2,4)); // Set a random max speed for the AI car

        drivingLane = Random.Range(0, Utils.CarLanes.Length); // Randomly select a lane from the available lanes in Utils
    }
}
