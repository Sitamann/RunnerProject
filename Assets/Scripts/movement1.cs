using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement1 : MonoBehaviour
{
    public float speed = 5f; // speed of the horizontal movement
    public float Fspeed = 5f; // speed of the horizontal movement

    public float smoothTime = 0.1f; // time it takes to smoothly interpolate the movement
    public float currentMass;
    public float initialMass = 0f; // Initial mass of the player
    private float targetPosition; // the target horizontal position of the spaceship
    private Vector3 velocity; // the current velocity of the spaceship
    private float currentTiltAngle; // the current tilt angle of the spaceship
    private TextChanger TextChanger;
    public float leftLimit, rightLimit;
    private Animator anim;
    private bool canTrigger = true;
    private float cooldownDuration = 2f;
    private float cooldownTimer = 0f;
    public int y = -2;
    private Rigidbody rb;
    private bool add=false;
    private bool minus = false;


    public GameObject addVFX;
    public GameObject minusVFX;

    public Transform transformVfx;

    private void Start()
    {
        anim = GetComponent<Animator>();

        currentMass = initialMass;
    }
    private void OnTriggerEnter(Collider other)
    {
            if (canTrigger)
        {
            if (other.CompareTag("add") && (gameObject.transform.localScale.magnitude < 8))
            {
                rb = gameObject.GetComponent<Rigidbody>();
                rb.mass *= 2f;
                gameObject.transform.localScale *= 1.25f;
                canTrigger = false;  
                y -= 2;
               Instantiate(addVFX, transformVfx.position, Quaternion.LookRotation(Vector3.up));
                add = true;
            }
            else if (other.CompareTag("minus") && (gameObject.transform.localScale.magnitude > 0.125))
            {
                rb = gameObject.GetComponent<Rigidbody>();
                rb.mass *= 0.5f;
                gameObject.transform.localScale *= 0.8f;
                canTrigger = false;  
                y -= 2;
                Instantiate(minusVFX, transformVfx.position, Quaternion.LookRotation(Vector3.up));
                minus = true;
            }
            StartCoroutine(Wait());
        }

    }
    void Update()
    {
        if (!canTrigger)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= cooldownDuration)
            {
                canTrigger = true;
                cooldownTimer = 0f;
            }
           
            if (add == true)
            {
                Instantiate(addVFX, transformVfx.position, Quaternion.LookRotation(Vector3.up));
                add = false;
            }
            
            if (minus == true)
            {
                Instantiate(minusVFX, transformVfx.position, Quaternion.LookRotation(Vector3.up));
                minus = false;
            }
        }



        float horizontalInput = Input.GetAxisRaw("Horizontal2"); // get the horizontal input
                                                                 // float horizontalInput = Input.acceleration.x;// ("Horizontal"); // get the horizontal input

        transform.Translate(Vector3.forward * Fspeed * Time.deltaTime);


        anim.SetFloat("Speed", Fspeed / 50);

        // set the target horizontal position based on the input
        targetPosition = transform.position.x + horizontalInput * speed * Time.deltaTime;

        // clamp the target horizontal position between leftLimit and rightLimit
        targetPosition = Mathf.Clamp(targetPosition, leftLimit, rightLimit);

        // smoothly interpolate the current position towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPosition, transform.position.y, transform.position.z), ref velocity, smoothTime);
        if (Fspeed <= 20)
        {
            Fspeed *= 1.00005f;
        }
       
    }
    private IEnumerator Wait()
    {

        yield return new WaitForSeconds(1f);
        Destroy(GameObject.FindWithTag("addVFX"));
        Destroy(GameObject.FindWithTag("minusVFX"));

    }

}