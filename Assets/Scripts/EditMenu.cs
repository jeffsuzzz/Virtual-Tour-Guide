using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Examples.InteractiveElements;	// access button script

public class EditMenu : MonoBehaviour {

    public GameObject buttonPrefab;
    private GameObject[] bottuns;

    // Use this for initialization
    void Start () {
        GameObject[] guides = GameObject.FindGameObjectsWithTag("Guide");
        bottuns = new GameObject[guides.Length];

        // Create buttons for each tour guide models.
        for (int i = 0; i < guides.Length; i++)
        {
            bottuns[i] = Instantiate(buttonPrefab, transform.position, transform.rotation, transform);
            bottuns[i].transform.localRotation *= Quaternion.Euler(0, 180, 0);
            bottuns[i].transform.localPosition += new Vector3(0f, -0.1f * i, 0f);

            // Words shown on button
            bottuns[i].GetComponent<LabelTheme>().Default = guides[i].name;

            // Setup the button's select function.
            Interactive buttonInteractive = bottuns[i].GetComponent<Interactive>();
            buttonInteractive.OnSelectEvents.AddListener(guides[i].GetComponent<HoloAnchor>().SetStartPosition);
        }
    }

    // Called when the attached object is activated.
    void OnEnable()
    {
        // Place the menu in front of the user.
        GameObject camera = GameObject.Find("MixedRealityCamera");
        transform.position = camera.transform.position + camera.transform.forward;

        transform.LookAt(camera.transform);
    }
	
}
