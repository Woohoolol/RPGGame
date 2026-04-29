using UnityEngine;
using UnityEngine.UI;

    // Creates a simple HP/MP HUD at runtime and binds it to PlayerStats.
    public class StatBars : MonoBehaviour
    {
        public GameObject character;
        public Slider healthSlider;
        public Slider mpSlider;
        void Start()
        {
        }

        void Update()
        {
            healthSlider.value = character.GetComponent<Character>().stats.currenthp;
            mpSlider.value = character.GetComponent<Character>().stats.currentmp;
            healthSlider.maxValue = character.GetComponent<Character>().basemaxhp;
            mpSlider.maxValue = character.GetComponent<Character>().basemaxmp;
        }
    }
