using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PhysicsObject
{
    Vector3 origLocalScale;
    public bool frozen = false;
    float launch = 1;
    [SerializeField] float launchRecovery;
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    public bool recovering;
    public float recoveryCounter;
    public float recoveryTime = 2;
    [SerializeField] GameObject graphic;
    [SerializeField] Animator animator;

    public RaycastHit2D ground;

    static Player instance;

    public static Player Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<Player>();
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        origLocalScale = transform.localScale;
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        ground = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -Vector2.up);
        launch += (0 - launch) * Time.deltaTime * launchRecovery;

        if (!frozen)
        {
            move.x = Input.GetAxis("Horizontal") + launch;

            if (Input.GetButtonDown("Jump") && grounded)
            {
                velocity.y = jumpTakeOffSpeed;
                //PlayJumpSound();
                //PlayStepSound();
            }

            if (move.x > 0.01f)
            {
                if (graphic.transform.localScale.x < 0)
                {
                    graphic.transform.localScale = new Vector3(origLocalScale.x, transform.localScale.y, transform.localScale.z);
                }
            }
            else if (move.x < -0.01f)
            {
                if (graphic.transform.localScale.x > 0)
                {
                    graphic.transform.localScale = new Vector3(-origLocalScale.x, transform.localScale.y, transform.localScale.z);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("attack");
            }
        }
        else
        {
            launch = 0;
        }

        if (recovering)
        {
            recoveryCounter += Time.deltaTime;
            if (recoveryCounter >= recoveryTime)
            {
                recoveryCounter = 0;
                recovering = false;
            }
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs (velocity.x) / maxSpeed);
        targetVelocity = move * maxSpeed;
    }
}
