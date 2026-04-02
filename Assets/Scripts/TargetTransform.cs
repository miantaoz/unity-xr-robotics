using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;


enum Mode
{
    FixedToHand,
    Released,
}

public class TargetTransform : MonoBehaviour
{
    public bool isTeleporting;

    public Transform hand_transform;
    public Material virtual_material;
    public GameObject drone_model;
    public Vector3 DroneExtents = new Vector3(0.3f, 0.12f, 0.4f);
    private Mode mode = Mode.FixedToHand;
    private Vector3 offsetFromGrabbedObject = Vector3.zero;
    private Vector3 grabbedObjectExtents;
    private GameObject grabbedObjectClone;
    private bool hasPayload;

    private TeleportationAnchor tpAnchor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tpAnchor = GetComponent<TeleportationAnchor>();

        GameObject model = Instantiate(drone_model, transform);
        model.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        foreach (Renderer child_renderer in model.GetComponentsInChildren<Renderer>())
        {
            child_renderer.material = virtual_material;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mode == Mode.FixedToHand)
        {
            transform.position = hand_transform.position;
            transform.rotation = hand_transform.rotation;
        }

    }

    private void OnSelect()
    {
        Debug.Log("OnSelect Called");
        if (! isTeleporting)
        {
            if (hasPayload)
            {
                hasPayload = false;
                Destroy(grabbedObjectClone);
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(hand_transform.position, hand_transform.forward, out hit, 100))
                {
                    if (hit.collider.tag == "grab")
                    {
                        grabbedObjectExtents = hit.collider.bounds.extents;
                        offsetFromGrabbedObject = new Vector3(0, grabbedObjectExtents.y + DroneExtents.y, 0);
                        transform.position = hit.collider.bounds.center + offsetFromGrabbedObject;
                        transform.LookAt(transform.position + new Vector3(hand_transform.forward.x, 0, hand_transform.forward.z), Vector3.up);
                        grabbedObjectClone = Instantiate(hit.collider.gameObject, transform);
                        grabbedObjectClone.transform.SetPositionAndRotation(hit.transform.position, hit.transform.rotation);
                        Destroy(grabbedObjectClone.GetComponent<Collider>());
                        grabbedObjectClone.GetComponent<Renderer>().material = virtual_material;

                        hasPayload = true;
                    }
                }

                mode = Mode.Released;
            }
        } else
        {
            tpAnchor.RequestTeleport();
        }
    }

    private void OnActivate()
    {

        Debug.Log("OnActivate Called");
        if (mode == Mode.Released)
        {
            mode = Mode.FixedToHand;
        } else if (mode == Mode.FixedToHand) {
            RaycastHit hit;
            if (Physics.Raycast(hand_transform.position, hand_transform.forward, out hit, 100))
            {
                Vector3 offset = Vector3.zero;
                Vector3 extents;

                if (hasPayload)
                {
                    offset += offsetFromGrabbedObject;
                    extents = grabbedObjectExtents;
                } else
                {
                    extents = DroneExtents;
                }

                if (Vector3.Dot(hit.normal, Vector3.up) > 0.5)
                {
                    transform.LookAt(transform.position + new Vector3(hand_transform.forward.x, 0, hand_transform.forward.z), Vector3.up);
                    offset += hit.normal * extents.y;
                }
                else
                {
                    transform.LookAt(transform.position - new Vector3(hit.normal.x, 0, hit.normal.z), Vector3.up);
                    offset += hit.normal * extents.z;
                }

                transform.position = hit.point + offset;

                mode = Mode.Released;
            }
        }
    }

}
