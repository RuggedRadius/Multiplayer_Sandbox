using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

namespace Mirror.Examples.Tanks
{
    public class Player : NetworkBehaviour
    {
        [Header("Components")]
        public Rigidbody rb;
        public float movementSpeedBase;
        public Animator animator;
        public CinemachineVirtualCamera virtualCamera = null;

        [Header("Movement")]
        public float rotationSpeed = 100;

        [Header("Firing")]
        public KeyCode shootKey = KeyCode.Space;
        public GameObject projectilePrefab;
        public Transform projectileMount;

        [Header("Game Stats")]
        [SyncVar] public int health;
        [SyncVar] public int score;
        [SyncVar] public string playerName;
        [SyncVar] public bool allowMovement;
        [SyncVar] public bool isReady;

        public bool isDead => health <= 0;
        public TextMesh nameText;

        private void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
            
            virtualCamera.gameObject.SetActive(true);
        }

        void Update()
        {
            if (Camera.main)
            {
                nameText.text = playerName;
                nameText.transform.rotation = Camera.main.transform.rotation;
            }

            // movement for local player
            if (!isLocalPlayer)
                return;

            Debug.Log("ISLocal");

            //Set local players name color to green
            nameText.color = Color.green;

            if (!allowMovement)
                return;

            if (isDead)
                return;

            // rotate
            float horizontal = Input.GetAxis("Horizontal");
            transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);

            // move
            float vertical = Input.GetAxis("Vertical");
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            rb.velocity = forward * Mathf.Max(vertical, 0) * movementSpeedBase;
            animator.SetBool("Moving", rb.velocity != Vector3.zero);

            Debug.Log("forward: " + forward);
            Debug.Log("horizontal: " + horizontal);

            // shoot
            if (Input.GetKeyDown(shootKey))
            {
                CmdFire();
            }
        }

        // this is called on the server
        [Command]
        void CmdFire()
        {
            //GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, transform.rotation);
            //projectile.GetComponent<Projectile>().source = gameObject;
            //NetworkServer.Spawn(projectile);
            //RpcOnFire();
        }

        // this is called on the tank that fired for all observers
        [ClientRpc]
        void RpcOnFire()
        {
            animator.SetTrigger("Shoot");
        }

        public void SendReadyToServer(string playername)
        {
            if (!isLocalPlayer)
                return;

            CmdReady(playername);
        }

        [Command]
        void CmdReady(string playername)
        {
            if (string.IsNullOrEmpty(playername))
            {
                playerName = "PLAYER" + Random.Range(1, 99);
            }
            else
            {
                playerName = playername;
            }

            isReady = true;
        }
    }
}
