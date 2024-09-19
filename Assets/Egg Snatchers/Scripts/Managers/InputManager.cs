using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [Header("elements")]
    [SerializeField] private MobileJoystick joystick;

    private void Awake()
    {
        if(instance == null)
        { 
            instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public Vector2 GetMovevector ()=> joystick.GetMoveVector();
}
