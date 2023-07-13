using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsDrag { private set; get; }
    public bool MouseOnObject { private set; get; }

    [Header("Proporties")]
    public Item Index;
    public Slot[] CurrentSlot;
    [SerializeField] bool backToAwakePosition = true;
    [SerializeField] Transform[] points;
    [SerializeField] Slot[] putToSlot;
    [SerializeField] GameObject DestroyAfterPickup;

    private List<Slot> closestSlot = new List<Slot>();
    private Vector2 startPosition;
    private Quaternion startRotation;
    private bool stopLoop = false;

    private void Awake()
    {
        // For Test
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        if (putToSlot != null)
            Put(putToSlot);
    }
    private void Update()
    {
        if (IsDrag)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90f);
                Debug.Log(transform.rotation.eulerAngles);
            }
        }
        if (MouseOnObject)
        {
            if (Input.GetMouseButtonDown(1) && CurrentSlot[0].IsInventorySlot)
            {
                if(Index.nameItem != "Medkit")
                {
                    Inventory.Instance.CurrentUseItem = Index;
                }else if(FindObjectOfType<PlayerMovement>().Health < 100)
                {
                    FindObjectOfType<PlayerMovement>().AddHealth(Random.Range(15, 50));
                    for (int i = 0; i < CurrentSlot.Length; i++)
                    {
                        CurrentSlot[i].IsFree = true;
                    }
                    Destroy(gameObject);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (IsDrag)
        {
            transform.position = Vector2.Lerp(transform.position, Input.mousePosition, 10 * Time.deltaTime);
            FineToPut();
            stopLoop = false;
        }
        else if (!stopLoop)
        {
            StopDrag();
            Debug.Log("Stop Drag");
            stopLoop = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsDrag && eventData.button == PointerEventData.InputButton.Left)
        {
            IsDrag = false;
            Debug.Log("Pointer Up");
            // Put
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsDrag == false && eventData.button == PointerEventData.InputButton.Left)
        {
            IsDrag = true;
        }
        Debug.Log("drag");
    }
    public void StopDrag()
    {
        if (backToAwakePosition)
        {
            PutBack();
        }
        if (closestSlot.Count != points.Length)
        {
            PutBack();
        }
        #region Failed
        for (int i = 0; i < closestSlot.Count; i++)
        {
            if (closestSlot[i].IsFree == false)
            {
                PutBack();
                Debug.Log("Put failed");
                return;
            }
            Debug.Log(closestSlot[i].name + " " + closestSlot[i].IsFree);
        }
        #endregion

        #region Succes
        if (!backToAwakePosition &&
            closestSlot.Count == points.Length)
        {
            Put(closestSlot.ToArray());
        }

        #endregion

        for (int i = 0; i < CurrentSlot.Length; i++)
        {
            CurrentSlot[i].SetColor(true);
        }
    }
    public void PutBack()
    {
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;
    }
    public void FineToPut()
    {
        for (int i = 0; i < closestSlot.Count; i++)
        {
            closestSlot[i].SetColor(true);
        }
        closestSlot.Clear();
        Slot[] slots = FindObjectsOfType<Slot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].CurrentItem == this)
                slots[i].Clear();
        }
        CurrentSlot = null;

        Transform[] position = points;
        for (int i = 0; i < position.Length; i++)
        {
            Slot closest = null;
            float distance = Mathf.Infinity;
            foreach (Slot go in slots)
            {
                Vector2 diff = (Vector2)go.transform.position - (Vector2)position[i].position;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                    if (Vector2.Distance(go.transform.position, position[i].position) < 30f)
                    {
                        closest = go;
                        distance = curDistance;
                        closestSlot.Add(closest);
                        for (int i2 = 0; i2 < closestSlot.Count; i2++)
                        {
                            closestSlot[i2].SetColor(false);
                        }
                    }
                }
            }
        }
    }
    public void Put(Slot[] slots)
    {
        var totalX = 0f;
        var totalY = 0f;
        foreach (var player in slots)
        {
            totalX += player.transform.position.x;
            totalY += player.transform.position.y;
        }
        var centerX = totalX / closestSlot.Count;
        var centerY = totalY / closestSlot.Count;
        transform.position = new Vector2(centerX, centerY);

        for (int i = 0; i < closestSlot.Count; i++)
        {
            closestSlot[i].IsFree = false;
            closestSlot[i].CurrentItem = this;
        }

        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        CurrentSlot = slots;
    }
    public void DestroyAfetrPick() => Destroy(DestroyAfterPickup);
    public void ResetStartPosAndRot()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOnObject = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOnObject = false;
    }
}