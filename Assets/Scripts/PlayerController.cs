using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("State")]
    public bool sprinting;
    public bool jumping;
    public bool isGrounded;

    [Header("Movement Settings")]
    public float movementSpeed;
    public float jumpPower;
    public float rotationMultiplier = 0.6f;
    public float moveBackMultiplier = 0.4f;
    public float sprintMultiplier = 0.4f;

    [Header("Controller Mappings")]
    public KeyCode mapJump;
    public KeyCode mapBuild;

    [Header("Grounded Settings")]
    public float groundedDistance;
    public LayerMask groundOnly;
    Ray ray;


    private Animator anim;
    private Rigidbody rb;
    [HideInInspector] public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        anim = this.GetComponentInChildren<Animator>();
        rb = this.GetComponent<Rigidbody>();
        virtualCamera.gameObject.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(IsGrounded());
        Debug.Log("GroundOnly == " + groundOnly.value);
    }

    void Update()
    {
        if (!isLocalPlayer) { return; }

        // Sprint
        sprinting = Input.GetKey(KeyCode.LeftShift);

        // Animator
        UpdateAnimator();
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer) { return; }

        // Apply movement and rotation vectors
        this.transform.eulerAngles += CalculateRotationVector();
        this.transform.position += CalculateMovementVector();

        if (!jumping)
        {
            // Jump
            if (Input.GetKeyDown(mapJump))
            {
                jumping = true;
                isGrounded = false;
                StartCoroutine(jump());
            }

            //// Stick to terrain otherwise
            //if (!isGrounded)
            //{
            //    StickToTerrain();
            //}
        }
    }

    

    private IEnumerator IsGrounded()
    {
        RaycastHit hit;
        Vector3 originOffset = Vector3.up * 0.01f;

        while (true)
        {
            ray = new Ray(this.transform.position + originOffset, -this.transform.up);

            if (Physics.Raycast(ray.origin, ray.direction, out hit, groundedDistance, groundOnly, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            yield return null;
        }
    }

    private IEnumerator jump()
    {
        Debug.Log("Jump");

        rb.velocity += CalculateJumpVector();        

        anim.SetTrigger("Jump");

        while (!isGrounded)
        {
            yield return null;
        }

        jumping = false;
        yield return null;
    }

    private Vector3 CalculateJumpVector()
    {
        return Vector3.up * Time.deltaTime * jumpPower;
    }

    private Vector3 CalculateMovementVector()
    {
        Vector3 movement = this.transform.forward * Time.deltaTime * movementSpeed * Input.GetAxis("Vertical");
        return sprinting ? movement * sprintMultiplier : movement;
    }

    private Vector3 CalculateRotationVector()
    {
        return this.transform.up * Input.GetAxis("Horizontal") * Time.deltaTime * rotationMultiplier * 250f;
    }

    private void UpdateAnimator()
    {
        anim.SetFloat("Vertical", Input.GetAxis("Vertical"));
        anim.SetBool("Sprinting", sprinting);
        anim.SetBool("Grounded", isGrounded);
    }

    private void StickToTerrain()
    {
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(transform.position.x, 1000f, transform.position.z), Vector3.down);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 2000f, groundOnly, QueryTriggerInteraction.Ignore))
        {
            this.transform.position = new Vector3(this.transform.position.x, hit.point.y, this.transform.position.z);
        }
        else
        {
            Debug.LogWarning("Could not find terrain to stick player to!");
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(ray.origin, ray.direction * groundedDistance, Color.red);
    }
}
