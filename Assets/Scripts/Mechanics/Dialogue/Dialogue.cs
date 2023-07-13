using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public TMPro.TMP_Text textNPC;
    public TMPro.TMP_Text textPlayer;

    public Button[] allOption;

    private float timeToClose;

    private void Awake()
    {
        timeToClose = Time.time + 4f;
    }
    private void Update()
    {
        if (allOption[0].gameObject.activeSelf == false && timeToClose <= Time.time)
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                Continue();
    }
    public void Continue() => GetComponentInParent<DialogueSystem>().ContinueText();
}
