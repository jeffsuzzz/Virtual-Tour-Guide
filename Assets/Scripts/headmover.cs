using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headmover : MonoBehaviour {
    public GameObject spine;
	// Update is called once per frame
	void Update () {
        this.transform.parent = spine.transform;
	}
}
