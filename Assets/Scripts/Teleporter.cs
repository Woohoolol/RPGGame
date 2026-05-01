using UnityEngine;

public class Teleporter : MonoBehaviour
{
    //Set in inspector
    public GameObject destinationTeleporter;
    public int destinationTeleporterBiome;
    public bool activated;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activated)
        {
            platformRotate(); 
        }
    }

    public void platformRotate()
    {
        float rotationSpeed = 10;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

}
