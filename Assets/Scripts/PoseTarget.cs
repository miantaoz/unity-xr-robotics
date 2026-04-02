using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;

public class PoseTarget : MonoBehaviour
{
    public bool IsLevel;
    public Material virtual_material;
    public Material virtual_material_touching;
    public GameObject model;
    public GameObject child;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject parent = transform.parent.gameObject;

        // add child that contains posepublisher and model
        child = new GameObject("target");
        child.transform.SetParent(transform);
        child.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        // add PosePublisher
        PosePublisher publisher = child.AddComponent<PosePublisher>();
        publisher.topicName = parent.GetComponent<PoseSubscriber>().topicName + "_target";
        publisher.enabled = false;

        // add model
        model = Instantiate(parent.GetNamedChild("model"));
        model.transform.SetParent(child.transform, false);
        foreach (Renderer model_renderer in model.GetComponentsInChildren<Renderer>())
        {
            model_renderer.material = virtual_material;
        }

        foreach (MaterialInit material_init in model.GetComponentsInChildren<MaterialInit>())
        {
            Destroy(material_init);
        }

        // add leveling in case it is level
        if (IsLevel)
        {
            child.AddComponent<StayLevel>();
        }

        child.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (Renderer model_renderer in model.GetComponentsInChildren<Renderer>())
        {
            model_renderer.material = virtual_material_touching;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (Renderer model_renderer in model.GetComponentsInChildren<Renderer>())
        {
            model_renderer.material = virtual_material;
        }
    }

}
