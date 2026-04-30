using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    public int xDirection;
    public int yDirection;
    public bool canMove;
    public bool canSave;
    public bool canBuy;
    public Rigidbody2D rb;
    public GameObject worldManager;
    public Animator anim;
    public GameObject shopkeeper;
    public int state;
    public int previousState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerSpeed = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.shiftKey.isPressed)
        {
            playerSpeed = 6f;
        }
        else
        {
            playerSpeed = 3f;
        }
        if(canMove && !SaveManager.instance.dialogueActive)
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
            anim.StopPlayback();
            //Horizontal direction takes priority over veritcal
            if(xDirection > 0)
            {
                anim.Play("PlayerWalkingRight");
                state = 0;
                yDirection = 0;
            }
            else if(xDirection < 0)
            {
                anim.Play("PlayerWalkingLeft");
                state = 1;
                yDirection = 0;
            }
            else if(yDirection > 0)
            {
                anim.Play("PlayerWalkingUp");
                state = 2;
            }
            else if(yDirection < 0)
            {
                anim.Play("PlayerWalkingDown");
                state = 3;
            }
            else if(xDirection == 0 && yDirection == 0)
            {
                anim.Play("DefaultAnimation");
                state = 4;
            }
            rb.linearVelocity = new Vector2(xDirection * playerSpeed, yDirection * playerSpeed);
        }
        if(canSave && Keyboard.current.zKey.wasPressedThisFrame && worldManager.GetComponent<WorldManager>().mode == 0)
        {
            Debug.Log("Saved game!");
            SaveManager.instance.lastSavedLocation = transform.position;
            Debug.Log(SaveManager.instance.lastSavedLocation);
            SaveManager.instance.saveGame();
        }
        if(canBuy && Keyboard.current.zKey.wasPressedThisFrame && worldManager.GetComponent<WorldManager>().mode == 0)
        {
            Debug.Log("Opened shop!");
            if(!shopkeeper.GetComponent<ShopManager>().activated)
            {
                shopkeeper.GetComponent<ShopManager>().activateShop();
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("SaveSpot"))
        {
            canSave = true;
        }
        if(collision.gameObject.CompareTag("Shopkeeper"))
        {
            canBuy = true;
            shopkeeper = collision.gameObject;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("SaveSpot"))
        {
            canSave = false;
        }
        if(collision.gameObject.CompareTag("Shopkeeper"))
        {
            canBuy = false;
        }
    }
}
