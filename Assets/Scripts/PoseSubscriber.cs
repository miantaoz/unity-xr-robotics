using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

public class PoseSubscriber : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/unity/vehicle_pose";
    public bool IsInterpolated = true;
    
    // Speed of interpolation (higher = faster catch up)
    public float interpolationSpeed = 10f; 

    // Define hidden position (Made static readonly so other fields can use it if needed, 
    // though initializing in Start is safer)
    private static readonly Vector3 hiddenPosition = Vector3.up * 100f;
    
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseMsg>(topicName, ReceiveMessage);
        
        // Initialize target to current position so we don't snap to (0,0,0) on start
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    bool isNaN(PointMsg p)
    {
        return double.IsNaN(p.x) || double.IsNaN(p.y) || double.IsNaN(p.z);
    }

    void ReceiveMessage(PoseMsg msg)
    {
        // 1. Data Validation
        if (isNaN(msg.position))
        {
            targetPosition = hiddenPosition;
            return; // Fixed missing semicolon
        }

        // 2. Update the TARGET
        targetPosition = msg.position.From<FLU>();
        targetRotation = msg.orientation.From<FLU>();
    }

    void Update()
    {
        // LOGIC CHECK:
        // If Interpolation is ON
        // AND we are NOT currently hidden (transform != hidden)
        // AND we are NOT going to hidden (target != hidden)
        // -> Then Interpolate
        if (IsInterpolated && transform.position != hiddenPosition && targetPosition != hiddenPosition)
        {
            float step = interpolationSpeed * Time.deltaTime;
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, step);
            
            // Use Slerp for rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
        }
        else
        {
            // Else: Snap directly.
            // This happens if:
            // 1. Interpolation is False
            // 2. We are currently hidden (Snap to valid target to reappear instantly)
            // 3. We are targeting hidden (Snap to hidden to disappear instantly)
            transform.position = targetPosition;
            transform.rotation = targetRotation;
        }
    }
}
