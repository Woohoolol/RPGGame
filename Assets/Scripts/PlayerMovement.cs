using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public int playerSpeed;
    public int xDirection;
    public int yDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        xDirection = 0;
        yDirection = 0;
        if(Keyboard.current.leftArrowKey.isPressed)
        {
            xDirection += -1;
        }
        if(Keyboard.current.rightArrowKey.isPressed)
        {
            xDirection += 1;
        }
        if(Keyboard.current.upArrowKey.isPressed)
        {
            yDirection += 1;
        }
        if(Keyboard.current.downArrowKey.isPressed)
        {
            yDirection += -1;
        }
            transform.position += Time.deltaTime * new Vector3(xDirection * playerSpeed, yDirection * playerSpeed, 0);
    }
}
