using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;
using UnityEngine.UIElements;

public class PoseSubscriber : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/unity/vehicle_pose";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.Subscribe<PoseMsg>(topicName, ReceiveMessage);
    }

    bool isNaN(PointMsg p)
    {
        return double.IsNaN(p.x) || double.IsNaN(p.y) || double.IsNaN(p.z);
    }

    void ReceiveMessage(PoseMsg msg)
    {

        if (isNaN(msg.position))
        {
            transform.position = Vector3.up * 100f;
            return;
        }
        
        transform.position = Vecto3.Lerp(transform.position, msg.position.From<FLU>(), 0.3f);
        transform.rotation = Quaternion.Lerp(transform.rotation, msg.orientation.From<FLU>(), 0.3f);


    }

    // Update is called once per frame
    void Update()
    {

    }
}
