using UnityEngine;

public class DefeatScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void newGame()
    {
        SaveManager.instance.newGame();
        StartCoroutine(SaveManager.instance.switchToScene("WorldScene"));
    }
    public void loading()
    {
        SaveManager.instance.loadGame();
        StartCoroutine(SaveManager.instance.switchToScene("WorldScene"));
    }

    public void mainMenu()
    {
        StartCoroutine(SaveManager.instance.switchToScene("MainMenuScene"));
    }

    public void exit()
    {
        Application.Quit();
    }
}
