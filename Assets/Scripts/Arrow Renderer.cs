using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

public class ArrowRenderer : MonoBehaviour
{
    public string topicName = "/unity/vector";

    public GameObject ArrowPrefab;
    private GameObject arrow;
    public float Scale = 0.1f;
    public float HighBound = 5;

    public Material RedMaterial;
    public Material GreenMaterial;

    Vector3 vector;

    private ROSConnection ros;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<Vector3Msg>(topicName, ReceiveMessage);

        // actually need to find cg of object to which forces are applied, but its fine this way
        arrow = Instantiate(ArrowPrefab, transform);
        arrow.transform.localPosition = Vector3.zero;
        arrow.transform.localScale = Vector3.zero;
    }

    void ReceiveMessage(Vector3Msg msg)
    {
        vector = msg.From<ENU>();
        arrow.transform.localScale = Vector3.one * vector.magnitude * Scale;
        if (vector.magnitude > HighBound)
        {
            setChildrenMaterial(arrow, RedMaterial);
        } else
        {
            setChildrenMaterial(arrow, GreenMaterial);
        }

    }

    void setChildrenMaterial(GameObject gameObject, Material material)
    {
        foreach (Renderer child_renderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            child_renderer.material = material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        arrow.transform.LookAt(transform.position + vector);

    }
}
