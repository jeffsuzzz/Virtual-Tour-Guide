using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

using HoloToolkit.Examples.InteractiveElements;	// access button script

public class AnimationController : MonoBehaviour {

    public GameObject headObject;
	public AudioClip[] audios;  // The audio should be separated into paragraphs
	public GameObject buttonPrefab;
	public TextAsset txtAsset;

    private bool initialState;
    private string[] answerText;
	private GameObject textobj;
	private GameObject[] bottuns;
    private int audio_now = 0;

    // Use this for initialization
    void Start()
    {
		initialState = true;
        HandleDialog ();

		textobj = transform.Find("Dialog").gameObject;
		textobj.SetActive (false);
        ShowCaption(false);
        DisplayOption(false);
    }

    // Update is called once per frame
    void Update()
    {
        // When the audio finishes, play the next audio and dialiog, pop out buttons to interact when everything is done.
        if (!initialState && !this.GetComponent<AudioSource>().isPlaying)
        {
            audio_now++;
            if (audio_now < audios.Length)
            {
                ButtonInteraction(audio_now);
            }
            else
            {
                DisplayOption(true);
                textobj.SetActive(false);
            }
        }
    }

    public void ShowCaption(bool isShow)
    {
        Color newColor = textobj.GetComponent<TextMesh>().color;
        if (isShow)
            newColor.a = 1;
        else
            newColor.a = 0;
        textobj.GetComponent<TextMesh>().color = newColor;
    }

    // Interrupt the animation and dialog immediately
    public void StopAnimation()
    {
        headObject.GetComponent<Animator>().SetTrigger("interrupt");
        GetComponentInChildren<Animator>().SetTrigger("interrupt");
        this.GetComponent<AudioSource>().Stop();
        audio_now = 99999;
        DisplayOption(true);
    }

    // To create more options, add cases and change animator's trigger name
    public void ButtonInteraction(int option)
    {
        PlayDialog(option);
        switch (option)
        {
            case 0:
                headObject.GetComponent<Animator>().SetTrigger("repeat");
                GetComponentInChildren<Animator>().SetTrigger("repeat");
                PlayDialog(0);
                break;
            case 1:
                headObject.GetComponent<Animator>().SetTrigger("para2");
                GetComponentInChildren<Animator>().SetTrigger("para2");
                PlayDialog(1);
                break;
            case 2:
                headObject.GetComponent<Animator>().SetTrigger("para3");
                GetComponentInChildren<Animator>().SetTrigger("para3");
                PlayDialog(2);
                break;
            case 3:
                headObject.GetComponent<Animator>().SetTrigger("para4");
                GetComponentInChildren<Animator>().SetTrigger("para4");
                PlayDialog(3);
                break;
            default:
                break;
        }
    }

	// Show or hide the option buttons
	private void DisplayOption(bool isShow){
		int buttonNum = bottuns.Length;
		for(int i = 0; i < buttonNum; i++){
			bottuns[i].SetActive(isShow);
		}
	}

    // Paly the audio and show the dialog of choice
    private void PlayDialog(int number)
	{
		this.GetComponent<AudioSource>().clip = audios [number];
		this.GetComponent<AudioSource>().Play();
        audio_now = number;

        // activate the text object to make it scroll from the beginning
        textobj.GetComponent<TextMesh>().text = answerText[number];
        textobj.SetActive(false);
        textobj.SetActive(true);

        // Close the options when playing the dialog.
        DisplayOption(false);
    }

    // Tour guide should only activate once, then wait for user input
    private void OnTriggerEnter(Collider col)
    {
        headObject.GetComponent<Animator>().SetBool("inRange", true);
        GetComponentInChildren<Animator>().SetBool("inRange", true);

		// First encounter
		if (initialState) {
            PlayDialog(0);
        }
		initialState = false;
    }

    private void OnTriggerExit(Collider col)
    {
        headObject.GetComponent<Animator>().SetBool("inRange", false);
        GetComponentInChildren<Animator>().SetBool("inRange", false);
    }

	// Read from dialog file and handle the text.
	private void HandleDialog()
	{      
		string txtContent = txtAsset.text;

		// Buttons will be generated automatically according to the text file. Position, rotation, script variables need to set up as well
		string titles = txtContent.Split ('(') [0];
		int buttonNum = titles.Split (',').Length;
		bottuns = new GameObject[buttonNum];
		for(int i = 0; i < buttonNum; i++){
			bottuns [i] = Instantiate (buttonPrefab, transform.position, transform.rotation, transform);
			bottuns [i].transform.localRotation *= Quaternion.Euler(0, 180, 0);
			bottuns [i].transform.localPosition = new Vector3 (0.3f, 0.673f, 0.3f);
            bottuns[i].transform.localPosition += new Vector3(0f, -0.1f * i, 0f);

            // Words shown on button are separated by , in the text file.
            bottuns[i].GetComponent<LabelTheme>().Default = titles.Split (',')[i];
            if (i == buttonNum - 1) {
                bottuns[i].GetComponent<LabelTheme>().Default = '\n' + titles.Split(',')[i];
            }

			// Setup the button's select function
			Interactive buttonInteractive = bottuns[i].GetComponent<Interactive>();
            // local variable is needed for delegate function
            int option = i;
            buttonInteractive.OnSelectEvents.AddListener(delegate { ButtonInteraction(option); });
		}

        // Save each dialog.
		Regex regex_dialog = new Regex (@"\(.*?\)");
		MatchCollection matches = regex_dialog.Matches (txtContent);
        answerText = new string[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            answerText[i] = DialogNewLine(matches[i].Value);
        }
	}

	// in the text file () are used to separate differnt dialog if there are many
	private string DialogNewLine(string dialog){
		dialog = dialog.Replace ("(", "");
		dialog = dialog.Replace (")", "");
		return dialog;
	}
    
}
