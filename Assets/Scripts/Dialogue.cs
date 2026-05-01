using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem; 
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int textIndex;
    public Image avatar;
    public List<int> characterType;
    public List<string> texts;
    public bool characterDialogue;
    public Sprite test;
    public Animator anim;
    public bool skipActivated = false;
    public bool finishedDialogue = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        texts = new List<string>();
    }
    void Start()
    {
        anim = GetComponent<Animator>();  
        text = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        avatar = transform.GetChild(1).gameObject.GetComponent<Image>();
        transform.parent.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        activateDialogue(); 
        textIndex = 0;
        if(texts.Count == 0)
        {
            text.SetText("No text!");
            finishedDialogue = true;
        }
        else
        {
            if(characterDialogue)
            {
                Sprite characterSprite = SaveManager.instance.portraits[characterType[textIndex]].GetComponent<SpriteRenderer>().sprite;
                avatar.sprite = characterSprite;
            }
            else
            {
                avatar.enabled = false;
            }
            StartCoroutine(textAppear(texts[textIndex]));
            textIndex += 1;
        }
    }

    public void activateDialogue()
    {
        gameObject.SetActive(true);
        anim.Play("DialoguePopUp");
    }

    public IEnumerator deactivateDialogue()
    {
        anim.Play("DialogueDisappear");
        yield return new WaitForSeconds(0.25f);
        Destroy(transform.parent.gameObject);  
    }
    //If dialogue gets destroyed by a scene change make sure we can still move
    public void OnDestroy()
    {
        Debug.Log("WHAT");
        SaveManager.instance.dialogueActive = false;
    }
    // Update is called once per frame
    void Update()
    {
        SaveManager.instance.dialogueActive = true;
        if(Keyboard.current.zKey.wasPressedThisFrame)
        {
            if(textIndex < texts.Count)
            {
                if(finishedDialogue)
                {
                    if(characterDialogue)
                    {
                        Sprite characterSprite = SaveManager.instance.portraits[characterType[textIndex]].GetComponent<SpriteRenderer>().sprite;
                        avatar.sprite = characterSprite;
                    }
                    else
                    {
                        avatar.enabled = false;
                    }
                    StartCoroutine(textAppear(texts[textIndex]));
                    textIndex += 1;
                }
                else
                {
                    skipActivated = true;
                }
            }
            else
            {
                //Might still be in the middle of the last text, finish it before closing
                if(!finishedDialogue)
                {
                    skipActivated = true;
                }
                else
                {
                    StartCoroutine(deactivateDialogue());
                }

            }
        }
    }

    public IEnumerator textAppear(string selectedText)
    {
        for(int i = 0; i < selectedText.Length; i++)
        {
            finishedDialogue = false;
            if(skipActivated)
            {
                i = selectedText.Length - 1;
                skipActivated = false;
            }
            text.SetText(selectedText.Substring(0, i+1));
            yield return new WaitForSeconds(0.02f);
        }
        finishedDialogue = true;
    }
}
