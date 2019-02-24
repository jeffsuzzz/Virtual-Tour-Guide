using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

using HoloToolkit.Examples.InteractiveElements;	// access button script

public class AnimationController : MonoBehaviour {

    public GameObject headObject;
	public AudioClip[] audios;
	public GameObject buttonPrefab;
	public TextAsset txtAsset;

    private bool initialState;
    private string[] answerText;
	private GameObject textobj;
	private GameObject[] bottuns;

    // Use this for initialization
    void Start()
    {
		initialState = true;
        HandleDialog ();

		textobj = transform.Find("Dialog").gameObject;
		textobj.SetActive (false);
    }

    // Update is called once per frame
    void Update()
    {
        // When the audio finishes, pop out buttons to interact
        if (!initialState && !this.GetComponent<AudioSource>().isPlaying)
        {
			DisplayOption (true);
			textobj.SetActive (false);
        }
        else
        {
			DisplayOption (false);
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

    // To create more options, add cases and change animator's trigger name
    public void ButtonInteraction(int option)
    {
        PlayDialog(option, answerText[option]);
        switch (option)
        {
            case 0:
                headObject.GetComponent<Animator>().SetTrigger("repeat");
                GetComponentInChildren<Animator>().SetTrigger("repeat");
                break;
            case 1:
                headObject.GetComponent<Animator>().SetTrigger("repeat");
                GetComponentInChildren<Animator>().SetTrigger("repeat");
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
	private void PlayDialog(int audio, string text)
	{
		this.GetComponent<AudioSource> ().clip = audios [audio];
		this.GetComponent<AudioSource>().Play();
        textobj.GetComponent<TextMesh>().text = text;
        textobj.SetActive (true);
	}

    private void OnTriggerEnter(Collider col)
    {
        headObject.GetComponent<Animator>().SetBool("inRange", true);
        GetComponentInChildren<Animator>().SetBool("inRange", true);

		// First encounter
		if (initialState) {
            PlayDialog(0, answerText[0]);
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

            // Words shown on button are separated by , in the text file. Additional newline is added for button layout issue
            bottuns[i].GetComponent<LabelTheme>().Default = "\n"+ titles.Split (',')[i];

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
