using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditController : MonoBehaviour {

    GameObject EditMenu;
    GameObject cursor;

    private GameObject[] guides;

    // Use this for initialization
    void Start () {
        cursor = GameObject.Find("Anchor_Cursor");
        guides = GameObject.FindGameObjectsWithTag("Guide");
        EditMenu = GameObject.Find("EditMenu");
        EditMenu.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

    }

    // Allow the user to drag the tour guide model
    public void EnableEdit(bool isEnable)
    {
        foreach(GameObject guide in guides)
        {
            guide.GetComponent<HandDraggable>().enabled = isEnable;
            guide.GetComponent<HoloAnchor>().edit = isEnable;
        }

        if(!isEnable)
            EditMenu.SetActive(false);
    }

    public void OpenMenu()
    {
        // Turn it off and on to reset the menu's position in front of user
        EditMenu.SetActive(false);
        EditMenu.SetActive(true);
        EnableEdit(true);
    }

    // Show or hide the caption dialog
    public void EnableCaption(bool isEnable)
    {
        foreach (GameObject guide in guides)
        {
            guide.GetComponent<AnimationController>().ShowCaption(isEnable);
        }
    }

}
