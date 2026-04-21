using UnityEngine;
public class WorldManager : MonoBehaviour
{
    public GameObject player;
    public GameObject background;
    public float encounterRate;
    private float encounterModifier;
    private float encounterRequirement;
    public int exp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        encounterModifier = 1;
        encounterRequirement = Random.Range(10, 20);
        exp = 5;
        Instantiate(background, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        Instantiate(background, new Vector3(-19.20f, 0, 0), Quaternion.Euler(0, 0, 0));
        Instantiate(background, new Vector3(19.20f, 0, 0), Quaternion.Euler(0, 0, 0));

    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetComponent<PlayerMovement>().xDirection != 0 || player.GetComponent<PlayerMovement>().yDirection != 0)
        {
            encounterRate += Time.deltaTime * encounterModifier;
        }
        if(encounterRate >= encounterRequirement)
        {
            encounterRate = 0;
            encounterRequirement = Random.Range(10, 20);
            //SaveManager.instance is the static thing we can call
            SaveManager.instance.gameData.battleSpawn = 1;
            SaveManager.instance.switchToBattleScene();
        }
    }

}
