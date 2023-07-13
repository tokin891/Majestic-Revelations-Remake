using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactManager : MonoBehaviour
{
    public static ImpactManager Instance;

    [Header("Create Impact")]
    [SerializeField] Impact[] impactField;
    [SerializeField] GameObject defulatImpact;

    private void Awake()
    {
        Instance = this;
    }
    public void CreateImpact(RaycastHit hit)
    {
        if (hit.transform == null)
            return;

        foreach(Impact one in impactField)
        {
            if(!one.GetFromChildren)
            {
                if (one.MaterialObject.mainTexture == hit.transform.GetComponent<Renderer>().material.mainTexture)
                {
                    GameObject effect = Instantiate(one.ImpactObject, hit.point, Quaternion.LookRotation(hit.normal));
                    effect.transform.SetParent(hit.transform);
                    Destroy(effect, 5f);

                    return;
                }
            }else
            {
                if (one.MaterialObject.mainTexture == hit.transform.GetComponentInChildren<Renderer>().material.mainTexture)
                {
                    GameObject effect = Instantiate(one.ImpactObject, hit.point, Quaternion.LookRotation(hit.normal));
                    effect.transform.SetParent(hit.transform);
                    Destroy(effect, 5f);

                    return;
                }
            }
        }

        GameObject effect1 = Instantiate(defulatImpact, hit.point, Quaternion.LookRotation(hit.normal));
        effect1.transform.SetParent(hit.transform);
        Destroy(effect1, 5f);
    }

    [System.Serializable]
    public class Impact
    {
        public Material MaterialObject;
        public GameObject ImpactObject;
        public bool GetFromChildren;
    }
}
