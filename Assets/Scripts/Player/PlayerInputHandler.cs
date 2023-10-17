using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputHandler : MonoBehaviour
{
    Controls.GameplayActions controls;

    bool isRunning;
    bool isBlocking;

    [Header("Movement")]
    public UnityEvent<Vector2> look = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> move = new UnityEvent<Vector2>();
    public UnityEvent run;
    public UnityEvent climb;

    [Header("Actions")]
    public UnityEvent interact;
    public UnityEvent attack;
    public UnityEvent blockStart;
    public UnityEvent blockEnd;
    public UnityEvent dropProp;
    public UnityEvent throwProp;

    void Awake()
    {
        controls = new Controls().Gameplay;

        controls.Climb.started += ctx => climb.Invoke();
        controls.Run.started += ctx => isRunning = true;
        controls.Run.canceled += ctx => isRunning = false;

        controls.Interact.started += ctx => interact.Invoke();
        controls.Attack.started += ctx => attack.Invoke();
        controls.Block.started += ctx => blockStart.Invoke();
        controls.Block.canceled += ctx => blockEnd.Invoke();
        controls.Drop.performed += ctx => dropProp.Invoke();
        controls.Throw.performed += ctx => throwProp.Invoke();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        look.Invoke(controls.Look.ReadValue<Vector2>());
        move.Invoke(controls.Movement.ReadValue<Vector2>());

        if (isRunning)
            run.Invoke();
    }
}
