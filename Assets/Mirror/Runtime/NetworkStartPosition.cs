using UnityEngine;

namespace Mirror
{
    /// <summary>
    /// This component is used to make a gameObject a starting position for spawning player objects in multiplayer games.
    /// <para>This object's transform will be automatically registered and unregistered with the NetworkManager as a starting position.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkStartPosition")]
    [HelpURL("https://mirror-networking.com/docs/Components/NetworkStartPosition.html")]
    public class NetworkStartPosition : MonoBehaviour
    {
        public void Awake()
        {
            RaycastHit hit;
            Ray ray = new Ray(this.transform.position + (Vector3.up * 100), -this.transform.up);
            LayerMask layer = 1024;
            float distance = 1000f;
            float startingHeight = 10f;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, distance, layer, QueryTriggerInteraction.Ignore))
            {
                this.transform.position = new Vector3(hit.point.x, hit.point.y + startingHeight, hit.point.z);
            }
            else
            {
                Debug.LogWarning("No ground position found for network starting position");
            }

            NetworkManager.RegisterStartPosition(transform);
        }

        public void OnDestroy()
        {
            NetworkManager.UnRegisterStartPosition(transform);
        }
    }
}
