using System.Collections;
using System.IO;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Transform gameModel;

    [SerializeField]
    MeshRenderer carMeshRender;

    [SerializeField]
    ExplodeHandler explodeHandler;

    [Header("Visuals")]
    [SerializeField] private Transform visualsPivot;

    //Max limits
    float maxSteerVelocity = 2;
    float maxForwardVelocity = 30;

    //Multipliers
    float accelerationMultiplier = 3;
    float brakeMultiplier = 15;
    float steeringMultiplier = 5;

    //Inputs
    Vector2 input = Vector2.zero;

    // Emissive property (for car lights/brakes)
    int _EmissionColor = Shader.PropertyToID("_EmissionColor");
    Color emissiveColor = Color.white;
    float emissiveColorMultiplier = 0f;

    //Check for exploded state
    bool isExploded = false;

    bool isPlayer = true;

    // Jump feature settings
    [Header("Jump Settings")]
    [SerializeField]
    private float jumpHeight = 1f; // height of the jump
    [SerializeField]
    private float jumpSpeed = 10f; // speed of the jump transition
    [SerializeField]
    private float maxJumpDuration = 1.0f; // maximum time the jump can be charged

    private bool isJumping = false;
    private float currentJumpCharge = 0f;
    private Vector3 originalModelLocalPosition;

    [Header("Jump Cooldown")]
    [field: SerializeField]
    public float jumpCooldown { get; private set; } = 3.0f; // 3 seconds cooldown for jump
    public float currentCooldownTimer { get; private set; } = 0f;
    public bool isJumpInCooldown { get; private set; } = false;


    //Start is called before the first frame update
    private void Start()
    {
        isPlayer = CompareTag("Player");

        // Save the original position of the car model
        if (gameModel != null)
        {
            originalModelLocalPosition = gameModel.localPosition;
        }

    }

    //Update is called once per frame
    private void Update()
    {
        // Check if exploded
        if (isExploded)
            return;

        // Handle jump logic
        HandleJump();


        // Fake car rotation while steering
        if (visualsPivot != null)
        {

            visualsPivot.localRotation = Quaternion.Euler(0, rb.linearVelocity.x * 5f, 0);
        }

        if (carMeshRender != null)
        {
            float desiredCarEmissiveColorMultiplier = 0f;

            if (input.y < 0)
                desiredCarEmissiveColorMultiplier = 3.0f;

            emissiveColorMultiplier = Mathf.Lerp(emissiveColorMultiplier, desiredCarEmissiveColorMultiplier, Time.deltaTime * 4);

            carMeshRender.material.SetColor(_EmissionColor, emissiveColor * emissiveColorMultiplier);
        }
    }

    private void FixedUpdate()
    {
        //isExploded
        if (isExploded)
        {
            //Apply drag to slow parts after explosion
            rb.linearDamping = rb.linearVelocity.z * 0.1f;
            rb.linearDamping = Mathf.Clamp(rb.linearDamping, 1.5f, 10);

            //Change camera view after explosion
            rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(0, 0, transform.position.z), Time.deltaTime * 0.5f));

            return;
        }

        if (input.y > 0)
            Accelerate();
        else
            rb.linearDamping = 0.2f;

        if (input.y < 0)
            Brake();

        Steer();

        //Force the car not to go backwards
        if (rb.linearVelocity.z <= 0)
            rb.angularVelocity = Vector3.zero;

    }

    // Jump handling method
    void HandleJump()
    {
        if (gameModel == null) return;

        if (isJumpInCooldown)
        {
            currentCooldownTimer -= Time.deltaTime;
            if (currentCooldownTimer <= 0)
            {
                currentCooldownTimer = 0;
                isJumpInCooldown = false;
                // Add UI feedback here later
            }
        }

        // Check if the player is allowed to jump
        bool wasCharged = currentJumpCharge > 0;

        // jump if the player is pressing the jump button and not already jumping
        if (isJumping && currentJumpCharge < maxJumpDuration)
        {
            currentJumpCharge += Time.deltaTime;
        }
        // if we release the button or charge is empty, stop jumping
        else if (currentJumpCharge > 0)
        {
            isJumping = false; // stop jumping
            currentJumpCharge -= Time.deltaTime * 2; // goees down slowly
        }

        currentJumpCharge = Mathf.Clamp(currentJumpCharge, 0, maxJumpDuration);

        // If the jump charge is full and we are not in cooldown, set the jump in cooldown state
        if (wasCharged && currentJumpCharge == 0)
        {
            isJumpInCooldown = true;
            currentCooldownTimer = jumpCooldown;
        }

        // Car position logic
        Vector3 targetPosition;
        if (isJumping || currentJumpCharge > 0)
        {
            // High
            targetPosition = originalModelLocalPosition + new Vector3(0, jumpHeight, 0);
        }
        else
        {
            // Low
            targetPosition = originalModelLocalPosition;
        }

        // Move the car model to the target position
        gameModel.localPosition = Vector3.Lerp(gameModel.localPosition, targetPosition, Time.deltaTime * jumpSpeed);
    }


    void Accelerate()
    {
        rb.linearDamping = 0;

        //Make sure we stay below speed limit
        if (rb.linearVelocity.z >= maxForwardVelocity)
            return;

        rb.AddForce(rb.transform.forward * accelerationMultiplier * input.y);
    }

    void Brake()
    {
        if (rb.linearVelocity.z <= 0)
            return;

        rb.AddForce(rb.transform.forward * brakeMultiplier * input.y);
    }

    void Steer()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            //Only allwo player to turn depending on the speed --> cant steer while stationary 
            float speedBasedSteerLimit = rb.linearVelocity.z / 5.0f;
            speedBasedSteerLimit = Mathf.Clamp01(speedBasedSteerLimit); //Returns value between 0 and 1

            rb.AddForce(rb.transform.right * steeringMultiplier * input.x * speedBasedSteerLimit);

            //normalized steer velocity depending on speed 
            float normalizedX = rb.linearVelocity.x / maxSteerVelocity;

            //force magnitude of 1
            normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f);

            //Make sure to stay within turn speed limit
            rb.linearVelocity = new Vector3(normalizedX * maxSteerVelocity, 0, rb.linearVelocity.z);
        }
        //Stabilize car if no sideways input
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(0, 0, rb.linearVelocity.z), Time.fixedDeltaTime * 3);
        }
    }


    public void SetInput(Vector2 inputVector)
    {
        inputVector.Normalize();
        input = inputVector;
        Debug.Log("Input reçu: " + input);
    }

    // Jumping methods
    public void SetJumping(bool jumping)
    {
        if (jumping && isJumpInCooldown)
        {
            return; // If we are in cooldown, do not allow jumping
        }

        // You can only jump if you are not already jumping and you have not reached the maximum jump duration
        if (jumping && currentJumpCharge >= maxJumpDuration) return;

        isJumping = jumping;
    }


    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxForwardVelocity = newMaxSpeed;
    }

    //Coroutine to slow down time on impact
    IEnumerator SlowDownTimeCO()
    {
        while (Time.timeScale > 0.2f)
        {
            Time.timeScale -= Time.deltaTime * 2;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        while (Time.timeScale < 1.0f)
        {
            Time.timeScale += Time.deltaTime;

            yield return null;
        }

        Time.timeScale = 1.0f;
    }

    // Events

    // On collision event management 
    private void OnCollisionEnter(Collision collision)
    {
        //AI cars should not explode on collision with other AI cars
        if (!isPlayer)
        {
            if (collision.gameObject.CompareTag("Untagged"))
                return;
            if (collision.gameObject.CompareTag("CarAI"))
                return;

        }


        Vector3 velocity = rb.linearVelocity;
        explodeHandler.Explode(velocity * 45);

        isExploded = true;

        GameManager.instance.PlayerDied();

        StartCoroutine(SlowDownTimeCO());
    }
}