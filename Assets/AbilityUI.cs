using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUI : MonoBehaviour
{
    public Player player;
    public List<AbilitySlot> abilities;

    public GameObject abilityTemplate; 

    // Start is called before the first frame update
    void Start()
    {
        Component[] components = player.GetComponents(typeof(AbilitySlot));
        foreach(AbilitySlot component in components) {
            abilities.Add(component);
        }

        // Spawn AbilityIcons
        foreach(AbilitySlot component in abilities) {
            GameObject iconObject = Instantiate(abilityTemplate, transform);
            iconObject.GetComponent<AbilityIcon>().Setup(component);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
