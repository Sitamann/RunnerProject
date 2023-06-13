using System.Collections;
using UnityEngine;

public class movement1 : MonoBehaviour
{
    public float speed = 5f; // speed of the horizontal movement
    public float Fspeed = 5f; // speed of the horizontal movement
    public float smoothTime = 0.1f; // time it takes to smoothly interpolate the movement
    public float currentMass;
    public float initialMass = 0f; // Initial mass of the player
    private float targetPosition; // the target horizontal position of the spaceship
    private Vector3 velocity; // the current velocity of the 
    public float leftLimit, rightLimit;
    private Animator anim;
    private bool canTrigger = true;
    private float cooldownDuration = 2f;
    private float cooldownTimer = 0f;
    private Rigidbody rb;
    private bool add = false;
    private bool minus = false;
    public GameObject addVFX;
    public GameObject minusVFX;
    public float x = 0f;
    public Transform transformVfx;
    public AudioClip sfxClip;
    public AudioSource audioSource;
    float halfScreenWidth;
    public float direction;
    Vector2 secondTouchPosition;
    Vector2 firstTouchPosition;
    public RectTransform joyStick;
    public Canvas canvas;
    public float S = 5;
    bool istouching = false;


    private void Start()
    {
        
        halfScreenWidth = Screen.width * .5f;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        currentMass = initialMass;
        rb = gameObject.GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 Big = new Vector3(1.953125f, 1.953125f, 1.953125f);
        Vector3 Small = new Vector3(0.512f, 0.512f, 0.512f);
        if (canTrigger)
        {
            if (other.CompareTag("add") && (gameObject.transform.localScale.x < Big.x && gameObject.transform.localScale.y < Big.y && gameObject.transform.localScale.z < Big.z))
            {
                Destroy(other.gameObject);
                rb = gameObject.GetComponent<Rigidbody>();
                rb.mass *= 1.12f;
                gameObject.transform.localScale *= 1.12f;
                canTrigger = false;
                add = true;
                audioSource.PlayOneShot(sfxClip);
            }
            else
            if (other.CompareTag("minus") && (gameObject.transform.localScale.x > Small.x && gameObject.transform.localScale.y > Small.y && gameObject.transform.localScale.z > Small.z))

            {
                Destroy(other.gameObject);
                rb = gameObject.GetComponent<Rigidbody>();
                rb.mass /= 1.12f;
                gameObject.transform.localScale /= 1.12f;

                canTrigger = false;
                minus = true;
                audioSource.PlayOneShot(sfxClip);
            }
            else
            if (other.CompareTag("Limit"))
            {
                Destroy(other.gameObject);
                rb = gameObject.GetComponent<Rigidbody>();
                rb.mass += 6f;
            }
            else
            {
                Destroy(other.gameObject);
                canTrigger = false;

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


        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {

                Touch touch = Input.GetTouch(i);
                Vector2 touchPosition = touch.position;
                if  (touchPosition.x < halfScreenWidth)
                    {
                    for (int y = 0; y < Input.touchCount; y++)
                    {
                        if (touch.phase == TouchPhase.Began )
                        {
                            firstTouchPosition = touch.position;
                            joyStick.gameObject.SetActive(true);

                            Vector2 position;
                            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, touchPosition, canvas.worldCamera, out position);


                            joyStick.transform.position = canvas.transform.TransformPoint(position);
                            istouching = true;
                        }

                        direction = firstTouchPosition.x - touchPosition.x;
                        if (touch.phase == TouchPhase.Ended)
                        {
                            direction = 0;
                            joyStick.gameObject.SetActive(false);
                            istouching = false;
                        }
                    }

                }
                else if(touchPosition.x > halfScreenWidth&&istouching==false)
                {
                    joyStick.gameObject.SetActive(false);
                    direction = 0;
                }
                
            }

        }


        float horizontalInput = -direction;//Input.GetAxisRaw("Horizontal2"); // get the horizontal input
                                           // float horizontalInput = Input.acceleration.x;// ("Horizontal"); // get the horizontal input

        transform.Translate(Vector3.forward * Fspeed * Time.deltaTime);


        anim.SetFloat("Speed", Fspeed / 50);

        // set the target horizontal position based on the input
        targetPosition = transform.position.x + horizontalInput * speed * Time.deltaTime;

        // clamp the target horizontal position between leftLimit and rightLimit
        targetPosition = Mathf.Clamp(targetPosition, leftLimit, rightLimit);

        // smoothly interpolate the current position towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPosition, transform.position.y, transform.position.z), ref velocity, smoothTime);



        if (Fspeed <= 10)
        {
            Fspeed *= 1.000025f;
        }
        float counter=1f;

        if (Fspeed >S+ counter)
        {
            S = Fspeed;
            counter = 0f;
            rb = gameObject.GetComponent<Rigidbody>();
            rb.mass *= 1.7f;
            Debug.Log(rb.mass);
            S = Fspeed;

        }

    }
    private IEnumerator Wait()
    {

        yield return new WaitForSeconds(1f);
        Destroy(GameObject.FindWithTag("addVFX"));
        Destroy(GameObject.FindWithTag("minusVFX"));

    }

}