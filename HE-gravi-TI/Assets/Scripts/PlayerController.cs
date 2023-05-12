using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float jumpForce = 1000f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private Transform groundCheck;

    private LayerMask layerGround;
    private bool isGrounded;
    private Rigidbody2D body;
    private LifeComponent life;
    private bool isFacingRight = true;

    private Vector3 velocity = Vector3.zero;

    NetworkVariable<Vector3> sharedVelocity;
    NetworkVariable<Vector3> sharedPosition;

    NetworkVariableColor sharedColor;

    public NetworkVariableString sharedPseudo;

    private AudioSource audioSource;
    public AudioClip dropClip;
    public AudioClip hitClip;
    public AudioClip dieClip;

    int maxShiftAvailable = 2;
    NetworkVariableInt nbShift;
    int nextShiftTimer = 0;

    public GameObject shift1;
    public GameObject shift2;



    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        life = GetComponent<LifeComponent>();
        layerGround = LayerMask.NameToLayer("Terrain");
        //sharedVelocity = new NetworkVariable<Vector3>(Vector3.zero);
        sharedVelocity = new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, Vector3.zero);
        sharedPosition = new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, transform.position);
        sharedPseudo = new NetworkVariableString(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, "_");
        sharedColor = new NetworkVariableColor(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, Color.red);
        nbShift = new NetworkVariableInt(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, maxShiftAvailable);
        sharedPseudo.OnValueChanged += ChangePseudo;
        sharedPosition.OnValueChanged += SyncPosition;
        sharedColor.OnValueChanged += SyncColor;
        audioSource = GetComponent<AudioSource>();
        nbShift.OnValueChanged += updateShift;
    }

    private void Start()
    {
        if (IsLocalPlayer)
        {
            sharedPseudo.Value = PseudoController.myPseudo + " IsServer: " + IsServer;
            transform.position = SceneController.singleton.getSpawnPoint();
            sharedPosition.Value = transform.position;
            sharedColor.Value = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
    }

    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            isGrounded = false;

            //Check the if the player touch the ground
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f, 1 << layerGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    isGrounded = true;
                }
            }
        }
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log(nbShift);
                if (nbShift.Value > 0)
                {
                    changeGravityServerRpc();
                    nbShift.Value--;
                    nextShiftTimer += 10000;
                }
            }

            if (nextShiftTimer > 0)
            {
                nextShiftTimer--;
                if (nextShiftTimer % 1000 == 0)
                {
                    nbShift.Value++;
                }
            }

            // inputs
            float move = Input.GetAxis("Horizontal");
            bool jump = false;
            if (Input.GetKeyDown("space"))
            {
                jump = true;
            }


            Vector3 targetVelocity = new Vector2(move * 10f, body.velocity.y);
            // Smooth the velocity until the max velocity
            body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref velocity, movementSmoothing);
            

            // Flip to the right direction
            if (move > 0 && !isFacingRight)
            {
                //Flip();
            }
            else if (move < 0 && isFacingRight)
            {
                //Flip();
            }

            // Jump
            if (isGrounded && jump)
            {
                isGrounded = false;
                body.AddForce(new Vector2(0f, jumpForce * body.gravityScale));
            }
            sharedVelocity.Value = body.velocity;

            // If leave the map
            if (Mathf.Abs(transform.position.x) > 87 || Mathf.Abs(transform.position.y) > 87)
            {
                life.hit(100);
            }

            if (Time.frameCount % 500 == 0)
            {
                sharedPosition.Value = transform.position;
            }

        }
        else
        {
            body.velocity = sharedVelocity.Value;
        }
    }



    [ServerRpc]
    private void changeGravityServerRpc()
    {
        changeGravityClientRpc();
    }

    [ClientRpc]
    private void changeGravityClientRpc() {
        changeGravity();
    }
    //TODO
    /// <summary>
    /// When player is hit, gravity really fast until next obstacle
    /// 
    /// </summary>
    private void changeGravity()
    {
        body.gravityScale *= -1;
        Vector3 theScale = transform.localScale;
        theScale.y *= -1;
        transform.localScale = theScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Lava")
        {
            //Destroy(this.gameObject);
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        if (Mathf.Abs(body.velocity.y) > 0.5)
        {
            audioSource.PlayOneShot(dropClip, 0.3f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Lava")
        {
            //Destroy(this.gameObject);
            body.WakeUp();
            Hit(0.5f);
            
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Lava")
        {
            //Destroy(this.gameObject);
            audioSource.Stop();
        }
    }

    private void OnDestroy()
    {
        audioSource.PlayOneShot(dieClip, 0.3f);
    }


    [ClientRpc]
    public void HitAndGravityChangeClientRpc(float dammage)
    {
        Hit(dammage);
        changeGravity();
    }


    public void Hit(float dammage)
    {
        life.hit(dammage);
        audioSource.PlayOneShot(hitClip);
    }

    void ChangePseudo(string previous, string value)
    {
        Transform pseudo = transform.Find("lifebarContainer/pseudo");
        pseudo.GetComponent<TextMesh>().text = value;
    }

    void SyncPosition(Vector3 previous, Vector3 value)
    {
        if (!IsLocalPlayer) {
            this.transform.position = value;
        }
    }

    void SyncColor(Color previous, Color value)
    {
        MeshRenderer renderer = transform.Find("Body").GetComponent<MeshRenderer>();

        renderer.material.color = value;
    }

    void updateShift(int previous, int value)
    {
        if (value == 2)
        {
            shift1.SetActive(true);
            shift2.SetActive(true);
        }
        else if (value == 1)
        {
            shift1.SetActive(true);
            shift2.SetActive(false);
        }
        else if (value == 0)
        {
            shift1.SetActive(false);
            shift2.SetActive(false);
        }
    }
}
