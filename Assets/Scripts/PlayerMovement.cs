using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
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
        if(canSave && Keyboard.current.zKey.wasPressedThisFrame && worldManager.GetComponent<WorldManager>().mode == 0 && !SaveManager.instance.dialogueActive)
        {
            Debug.Log("Saved game!");
            SaveManager.instance.lastSavedLocation = transform.position;
            Debug.Log(SaveManager.instance.lastSavedLocation);
            SaveManager.instance.saveGame();
            SaveManager.instance.spawnDialogue(new List<string>{"Saved data successfully!"});
        }
        if(canBuy && Keyboard.current.zKey.wasPressedThisFrame && worldManager.GetComponent<WorldManager>().mode == 0 && !SaveManager.instance.dialogueActive)
        {
            Debug.Log("Opened shop!");
            if(SaveManager.instance.eventFlags.Contains(2))
            {
                List<string> dialogue = new List<string>{"Hi, who is this?", "It's me! The shopkeeper! Tee-hee. Want to buy any wares here?", "That's my friend from childhood days. You'll give us a discount... right?", "Hey! Why'd you tell them that! Anyhow, you won't get prices better than this!", "She is definitely upcharging us.", "(Sigh.... why are all merchants like this.)"};
                List<int> dialogueCharacters = new List<int>{0, 4, 1, 4, 1, 3};
                SaveManager.instance.spawnDialogue(dialogue, characterDialogue: true, dialogueCharacters: dialogueCharacters);
                SaveManager.instance.eventFlags.Remove(2);
            }
            else
            {
                List<string> dialogue = new List<string>{"Welcome back! Want to buy something?"};
                List<int> dialogueCharacters = new List<int>{4};
                SaveManager.instance.spawnDialogue(dialogue, characterDialogue: true, dialogueCharacters: dialogueCharacters);
            }
            if(!shopkeeper.GetComponent<ShopManager>().activated)
            {
                shopkeeper.GetComponent<ShopManager>().activateShop();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("SaveSpot"))
        {
            canSave = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("SaveSpot"))
        {
            canSave = false;
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject.CompareTag("Shopkeeper"))
        {
            canBuy = true;
            shopkeeper = collision.gameObject;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Shopkeeper"))
        {
            canBuy = false;
        }
    }
}
