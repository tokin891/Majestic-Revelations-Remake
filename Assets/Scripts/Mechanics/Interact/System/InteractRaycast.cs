using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractRaycast : MonoBehaviour
{
    public static InteractRaycast Instance;

    [Header("Details")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Image slightImage;
    [Tooltip("You can see red line in the front of camera")]
    [SerializeField] float range;
    [SerializeField] TMPro.TMP_Text textInfo;

    [Header("Input")]
    [SerializeField] KeyCode interactKey;
    [SerializeField] Color32 slightColorHover;

    private Color32 slightColorStart;

    private void Awake()
    {
        Instance = this;
        slightColorStart = slightImage.color;
    }

    void Update()
    {
        if(Input.GetKeyDown(interactKey))
        {
            if (GetObjectWithInteractSystem() != null)
                GetObjectWithInteractSystem().TryInteract();
        }

        if(GetObjectWithInteractSystem() != null)
            slightImage.color = slightColorHover;
        if(GetObjectWithInteractSystem() == null)       
            slightImage.color = slightColorStart;
    }

    private InteractSystem GetObjectWithInteractSystem()
    {
        RaycastHit hit;
        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit,range))
        {
            if(hit.transform.GetComponent<InteractSystem>())
            {
                return hit.transform.GetComponent<InteractSystem>();
            }
        }    

        return null;
    }
    public RaycastHit GetRaycastObject(float range)
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range))
        {
            return hit;
        }

        return hit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.position + new Vector3(0f, 0f, range));
    }

    public void SetTextInfo(string text, float delay)
    {
        StartCoroutine(SetTextInfo(delay, text));
    }
    private IEnumerator SetTextInfo(float delay, string text)
    {
        textInfo.text = text;
        yield return new WaitForSeconds(delay);
        textInfo.text = "";
    }
}
