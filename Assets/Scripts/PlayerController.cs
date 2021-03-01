using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public CinemachineVirtualCamera virtualCamera = null;

    public bool sprinting;

    public float movementSpeed;

    public float rotationMultiplier = 0.6f;
    public float moveBackMultiplier = 0.4f;
    public float sprintMultiplier = 0.4f;

    private Animator anim;

    // Grounding
    public LayerMask groundOnly;

    private void Awake()
    {
        anim = this.GetComponentInChildren<Animator>();
        virtualCamera.gameObject.SetActive(true);
    }

    void Update()
    {

        if (!isLocalPlayer) { return; }

        // Calc rotation vector
        Vector3 rotVector = this.transform.up * Input.GetAxis("Horizontal") * Time.deltaTime * rotationMultiplier * 250f;
        this.transform.eulerAngles += rotVector;

        // Sprint
        sprinting = Input.GetKey(KeyCode.LeftShift);

        Vector3 movement = Vector3.zero;
        movement = this.transform.forward * Time.deltaTime * movementSpeed * Input.GetAxis("Vertical");
        movement = sprinting ? movement * sprintMultiplier : movement;
        this.transform.position += movement;

        // Animator
        UpdateAnimator();

        // Stick player to terrain
        StickToTerrain();
    }

    private void UpdateAnimator()
    {
        anim.SetFloat("Vertical", Input.GetAxis("Vertical"));
        anim.SetBool("Sprinting", sprinting);
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
}
