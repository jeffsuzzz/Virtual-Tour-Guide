using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingText : MonoBehaviour {

    public float scrollSpeed = 5;

	void OnEnable()
	{
		StartCoroutine(AutoScroll());
	}

    IEnumerator AutoScroll()
    {
        float width = gameObject.GetComponent<TextMesh>().text.Length / 20;
        Vector3 startPos = transform.localPosition;
        float scrollPos = 0, newX;
        while (true)
        {
            newX = (scrollPos == 0)? 0: scrollPos % width;
            transform.localPosition = new Vector3(newX, startPos.y, startPos.z);
            scrollPos += scrollSpeed * 0.1f * Time.deltaTime;
            yield return null;
        }  
    }
        
}
