using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditController : MonoBehaviour {

    public GameObject UserPosition;
    GameObject EditMenu;
    //GameObject cursor;

    private GameObject[] guides;

    // Use this for initialization
    void Start () {
        //cursor = GameObject.Find("Anchor_Cursor");
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

    public void InterruptAnimation()
    {
        foreach (GameObject guide in guides)
        {
            guide.GetComponent<AnimationController>().StopAnimation();
        }
    }

    public void RepeatDialog(int num)
    {
        // Find the closest tour guide and tell it to repeat the nth paragraph.
        GameObject closest = guides[0];
        float closest_distance = 999999999;
        foreach (GameObject guide in guides)
        {
            float distance = Vector3.Distance(UserPosition.transform.position, guide.transform.position);
            if (distance < closest_distance)
            {
                closest = guide;
                closest_distance = distance;
            }
        }
        closest.GetComponent<AnimationController>().ButtonInteraction(num);
    }
}
