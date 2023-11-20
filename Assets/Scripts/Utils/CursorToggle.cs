using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorToggle : Singleton<CursorToggle>
{
    Controls.EditorActions controls;
    public bool IsCursorLocked
    {
        get { return Cursor.visible; }
    }

    public bool startLocked = true;
    public bool forceStayUnlocked = false;

    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        controls = new Controls().Editor;

        controls.ToggleCursor.performed += ctx => { ToggleCursor(); };
#endif

        if (startLocked)
            LockCursor();
        else
            UnlockCursor();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void ToggleCursor()
    {
        if (IsCursorLocked)
            UnlockCursor();
        else
            LockCursor();
    }

    public void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LockCursor()
    {
#if UNITY_EDITOR
        if (forceStayUnlocked)
            return;
#endif

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
