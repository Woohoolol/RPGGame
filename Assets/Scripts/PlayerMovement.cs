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
    public bool canRecruit;
    public Rigidbody2D rb;
    public GameObject worldManager;
    public Animator anim;
    public GameObject shopkeeper;
    public GameObject npc;
    public int state;
    public int previousState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerSpeed = 3f;
        StartCoroutine(canRecruitProcess());
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
        if(canMove && !SaveManager.instance.dialogueActive && !canBuy)
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
        else
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
        if(canSave && Keyboard.current.zKey.wasPressedThisFrame && worldManager.GetComponent<WorldManager>().mode == 0 && !SaveManager.instance.dialogueActive)
        {
            Debug.Log("Saved game!");
            SaveManager.instance.lastSavedLocation = transform.position;
            Debug.Log(SaveManager.instance.lastSavedLocation);
            if(SaveManager.instance.eventFlags.Contains(3))
            {
                List<string> dialogue = new List<string>{"Hrm... what would this be?", "It's a magic circle. I can utilize this to teleport us in and out of here if we need to escape or leave!", "Great! Just as easy as that?", "Well... we won't remember or keep anything before we interacted with the magic circle.", "Ah...", "Still convenient for us to use if we are in trouble or we need to rest up. Do not fret."};
                List<int> dialogueCharacters = new List<int>{3, 2, 0, 2, 0, 1};
                SaveManager.instance.spawnDialogue(dialogue, characterDialogue: true, dialogueCharacters: dialogueCharacters);
                SaveManager.instance.eventFlags.Remove(3);
            }
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
            else if(!shopkeeper.GetComponent<ShopManager>().activated)
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

    public IEnumerator canRecruitProcess()
    {
        while(true)
        {
            GameObject dialogueCanvas = null;
            if(canRecruit && Keyboard.current.zKey.wasPressedThisFrame && worldManager.GetComponent<WorldManager>().mode == 0 && !SaveManager.instance.dialogueActive)
            {
                if(npc.GetComponent<Character>().stats.characterType == 1)
                {
                    List<string> dialogue = new List<string>{"Well well. It's been a while.", "It has! Will you join me in my adventure?", "Hm sure. It'll give me good practice.", "Glad to hear!"};
                    List<int> dialogueCharacters = new List<int>{1, 0, 1, 0};
                    dialogueCanvas = SaveManager.instance.spawnDialogue(dialogue, characterDialogue: true, dialogueCharacters: dialogueCharacters);
                }
                if(npc.GetComponent<Character>().stats.characterType == 2)
                {
                    List<string> dialogue = new List<string>{"Hello there! Do you happen to be available for some questing?", "Seriously? We haven't done a trip in forever!", "Ah... let me get this and this... not the frog...", "Ready to go!", "Good to have you in the party!"};
                    List<int> dialogueCharacters = new List<int>{0, 2, 2, 2, 0};
                    dialogueCanvas = SaveManager.instance.spawnDialogue(dialogue, characterDialogue: true, dialogueCharacters: dialogueCharacters);
                }
                if(npc.GetComponent<Character>().stats.characterType == 3)
                {
                    List<string> dialogue = new List<string>{"Off to another quest, are we?", "Of course! Would you care to join me?", "Sure. I was looking for some work anyways.", "Great!"};
                    List<int> dialogueCharacters = new List<int>{3, 0, 3, 0};
                    dialogueCanvas = SaveManager.instance.spawnDialogue(dialogue, characterDialogue: true, dialogueCharacters: dialogueCharacters);
                }
                yield return new WaitUntil(() => dialogueCanvas == null);
                CharacterStats stat = new CharacterStats();
                stat.characterType = npc.GetComponent<Character>().stats.characterType;
                stat.currenthp = 100;
                stat.currentmp = 100;
                stat.level = 1;
                stat.exp = 1;
                GameObject baseCharacter = SaveManager.instance.classes[npc.GetComponent<Character>().stats.characterType];
                baseCharacter.GetComponent<Character>().stats = stat;
                SaveManager.instance.playerList.Add(baseCharacter);
                Destroy(npc);
            }
            yield return null;
        }
    }
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("SaveSpot"))
        {
            canSave = true;
        }
        if(collider.gameObject.CompareTag("Teleporter"))
        {
            if(collider.gameObject.GetComponent<Teleporter>().activated)
            {
                transform.position = collider.gameObject.GetComponent<Teleporter>().destinationTeleporter.transform.position + new Vector3(0, 2, 0);
                SaveManager.instance.biome = collider.gameObject.GetComponent<Teleporter>().destinationTeleporterBiome;
            }
            else
            {
                SaveManager.instance.spawnDialogue(new List<string>{"Hmm... this teleporter doesn't seem to be open.", "Maybe you need to do something first."});
            }
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
        if(collision.gameObject.CompareTag("NPCRecruit"))
        {
            canRecruit = true;
            npc = collision.gameObject;
        }
        if(collision.gameObject.CompareTag("Shopkeeper"))
        {
            canBuy = true;
            shopkeeper = collision.gameObject;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("NPCRecruit"))
        {
            canRecruit = false;
            npc = null;
        }
        if(collision.gameObject.CompareTag("Shopkeeper"))
        {
            canBuy = false;
            shopkeeper = null;
        }
    }
}
