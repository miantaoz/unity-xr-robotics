using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

public class WrenchRenderer : MonoBehaviour
{
    public string topicName = "/unity/wrench";

    public GameObject forceArrowPrefab;
    public GameObject torqueArrowPrefab;
    private GameObject forceArrow;
    private GameObject torqueArrow;

    public float ForceScale = 0.1f;
    public float TorqueScale = 0.1f;

    public float ForceHighBound = 1f;
    public float TorqueHighBound = 1f;

    public Material RedMaterial;
    public Material GreenMaterial;

    Vector3 force;
    Vector3 torque;

    private ROSConnection ros;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<WrenchMsg>(topicName, ReceiveMessage);

        // actually need to find cg of object to which forces are applied, but its fine this way
        forceArrow = Instantiate(forceArrowPrefab, transform);
        torqueArrow = Instantiate(torqueArrowPrefab, transform);
        forceArrow.transform.localScale = Vector3.zero;
        forceArrow.transform.localPosition = Vector3.zero;
        torqueArrow.transform.localScale = Vector3.zero;
        torqueArrow.transform.localPosition = Vector3.zero;
    }

    void ReceiveMessage(WrenchMsg msg)
    {
        force = msg.force.From<ENU>();
        torque = msg.torque.From<ENU>();

        forceArrow.transform.localScale = Vector3.one * force.magnitude * ForceScale;
        torqueArrow.transform.localScale = Vector3.one * torque.magnitude * TorqueScale;

        if (force.magnitude > ForceHighBound)
        {
            setChildrenMaterial(forceArrow, RedMaterial);
        }
        else
        {
            setChildrenMaterial(forceArrow, GreenMaterial);
        }

        if (torque.magnitude > TorqueHighBound)
        {
            setChildrenMaterial(torqueArrow, RedMaterial);
        }
        else
        {
            setChildrenMaterial(torqueArrow, GreenMaterial);
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
        forceArrow.transform.LookAt(transform.position + force);
        torqueArrow.transform.LookAt(transform.position + torque);

    }
}
