using UnityEngine;

public class SaveSpot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        platformRotate();
    
    }


    public void platformRotate()
    {
        float rotationSpeed = 10;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
