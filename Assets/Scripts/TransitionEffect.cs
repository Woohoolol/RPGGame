using UnityEngine;
public class TransitionEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponent<Animator>().Play("EndTransition");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
