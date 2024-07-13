using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    private Vector3 normalSize;
    public bool isGrounded = true;
    private bool isCrouched = false;
    [SerializeField] private float groundBuffer;
    [SerializeField] private float crouchShrink; 
    [SerializeField] private float vertCrouchOffset; 
    [SerializeField] private float crouchSpeedPenalty;
    [SerializeField] private float airSpeedPenalty;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float jumpPower;

    // Start is called before the first frame update
    void Awake()
    {
        normalSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = transform.position;
        temp.y -= 1;
        RaycastHit2D hit = Physics2D.Raycast(temp, -Vector2.up,groundBuffer);
        if (hit.collider != null) {
            isGrounded = true;
        } else {
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Crouch(true);
        }else if (Input.GetKeyUp(KeyCode.S))
        {
            Crouch(false);
        }

        MoveSide(Input.GetAxisRaw("Horizontal"));

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            Jump();
        }
    }

    private void Crouch(bool hr)
    { // hr short for hold/release
        if (hr == true) { // hr true means started holding
            Vector3 temp = transform.localScale; 
            temp.y *= crouchShrink; 
            transform.localScale = temp; 

            temp = transform.position;
            temp.y -= vertCrouchOffset;
            transform.position = temp;

            isCrouched = true;
        } else { // else (hr false) means release
            transform.localScale = normalSize;
            Vector3 temp = transform.position;
            temp.y += vertCrouchOffset;
            transform.position = temp;

            isCrouched = false;
        }
    }
    private void MoveSide(float direction)
    {
        float multiplier = 1;
        if (isCrouched){
            multiplier *= crouchSpeedPenalty;
        }
        if (!isGrounded) {
            multiplier *= airSpeedPenalty;
        }

        Vector3 temp = GetComponent<Rigidbody2D>().velocity;
        temp.x = baseSpeed*direction*multiplier;
        GetComponent<Rigidbody2D>().velocity = temp;
        
        if (direction != 0) {
            Mirror(direction);
        }
    }
    private void Jump()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up*jumpPower,ForceMode2D.Impulse);
    }
    private void Mirror(float direction)
    { // cant just do localscale.x *= -1 cuz transform.localscale is an object that isn't a vector3, the only way we can alter it though is to convert it first and then apply it
        Vector3 temp = transform.localScale; // storing block of xyz
        temp.x = Mathf.Abs(temp.x) * direction; // changing x        
        transform.localScale = temp; // applying change
    }
}
