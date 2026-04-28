using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    public int xDirection;
    public int yDirection;
    public bool canMove;
    public bool canSave;
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSpeed = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
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
            rb.linearVelocity = new Vector2(xDirection * playerSpeed, yDirection * playerSpeed);
        }
        if(canSave && Keyboard.current.zKey.wasPressedThisFrame)
        {
            Debug.Log("Saved game!");
            SaveManager.instance.saveGame();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("SaveSpot"))
        {
            canSave = true;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("SaveSpot"))
        {
            canSave = false;
        }
    }
}
