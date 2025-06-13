using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private CarHandler carHandler;
    private PlayerControls controls;
    private Vector2 movementInput;


    void Awake()
    {
        if (!CompareTag("Player"))
        {
            Destroy(this); // Destroy this component if the GameObject is not tagged as "Player"
            return;
        }

        controls = new PlayerControls();
        controls.Gameplay.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => movementInput = Vector2.zero;
        controls.Gameplay.Reset.performed += ctx => GameManager.instance.RestartGame();
        controls.Gameplay.Jump.performed += ctx => carHandler.SetJumping(true);
        controls.Gameplay.Jump.canceled += ctx => carHandler.SetJumping(false);
    }

    private void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Playing)
        {
            carHandler.SetInput(Vector2.zero);
            return;
        }
        carHandler.SetInput(movementInput);
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}