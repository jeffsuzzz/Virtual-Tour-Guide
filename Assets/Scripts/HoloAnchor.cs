using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA;

public class HoloAnchor : MonoBehaviour, IInputClickHandler, IInputHandler
{

    public string ObjectAnchorStoreName;
    public bool edit = false;
    WorldAnchorStore anchorStore;

    bool Placing = false;
    // Use this for initialization
    void Start()
    {
        WorldAnchorStore.GetAsync(AnchorStoreReady);
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
        Placing = true;

        Debug.Log("looking for " + ObjectAnchorStoreName);
        string[] ids = anchorStore.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            if (ids[index] == ObjectAnchorStoreName)
            {
                Debug.Log(ids[index]);
                WorldAnchor wa = anchorStore.Load(ids[index], gameObject);
                Placing = false;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        
    }

    public void OnInputDown(InputEventData eventData)
    {

        if (anchorStore == null || !edit)
        {
            return;
        }
        //gameObject.GetComponent<Rigidbody>().isKinematic = false;

        WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();
        if (anchor != null)
        {
            DestroyImmediate(anchor);
        }

        string[] ids = anchorStore.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            if (ids[index] == ObjectAnchorStoreName)
            {
                Debug.Log(ids[index]);
                bool deleted = anchorStore.Delete(ids[index]);
                Debug.Log("deleted: " + deleted);
                break;
            }
        }
    }
    public void OnInputUp(InputEventData eventData)
    {
        if (anchorStore == null || !edit)
        {
            return;
        }

        //gameObject.GetComponent<Rigidbody>().isKinematic = true;
        WorldAnchor attachingAnchor = gameObject.AddComponent<WorldAnchor>();
        if (attachingAnchor.isLocated)
        {
            bool saved = anchorStore.Save(ObjectAnchorStoreName, attachingAnchor);
            Debug.Log("saved: " + saved);
        }
        else
        {
            attachingAnchor.OnTrackingChanged += AttachingAnchor_OnTrackingChanged;
        }
    }

    private void AttachingAnchor_OnTrackingChanged(WorldAnchor self, bool located)
    {
        if (located)
        {
            bool saved = anchorStore.Save(ObjectAnchorStoreName, self);
            Debug.Log("saved: " + saved);
            self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
        }
    }

    /// <summary>
    /// This function should reset the anchor of the model in front of user.
    /// </summary>
    public void SetStartPosition() {

        WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();
        if (anchor != null)
        {
            DestroyImmediate(anchor);
        }

        GameObject camera = GameObject.Find("MixedRealityCamera");
        transform.position = camera.transform.position + 5*camera.transform.forward;
        transform.LookAt(camera.transform);

        // Always fail
        WorldAnchor attachingAnchor = gameObject.AddComponent<WorldAnchor>();
        bool saved = anchorStore.Save(ObjectAnchorStoreName, attachingAnchor);
        if (saved) Debug.Log("saved: " + ObjectAnchorStoreName);
        else Debug.Log("save failed");
        
    }

}