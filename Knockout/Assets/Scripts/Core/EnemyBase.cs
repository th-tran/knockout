using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class EnemyBase : PhysicsObject
{
    [SerializeField] enum EnemyType {Bug, Zombie};
    [SerializeField] EnemyType enemyType;
    public AudioSource audioSource;
    AnimatorFunctions animatorFunctions;
    public float maxSpeed = 7;
    float launch = 1;
    [SerializeField] float launchPower = 10;
    public float direction = 1;
    [SerializeField] public float changeDirectionEase = 1;
    float directionSmooth = 1;
    public float jumpTakeOffSpeed = 7;
    public bool jump = false;
    float playerDifference;
    public AudioClip stepSound;
    public AudioClip jumpSound;
    public bool recovering;
    public float recoveryCounter;
    public float recoveryTime = 2;
    bool followPlayer;
    public float followRange;
    [SerializeField] LayerMask layerMask;

    [SerializeField] GameObject graphic;
    [SerializeField] Animator animator;
    [SerializeField] bool jumping;
    RaycastHit2D ground;
    [SerializeField] Vector2 rayCastOffset;
    RaycastHit2D rightWall;
    RaycastHit2D leftWall;
    RaycastHit2D rightLedge;
    RaycastHit2D leftLedge;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animatorFunctions = GetComponent<AnimatorFunctions>();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        playerDifference = Player.Instance.gameObject.transform.position.x - transform.position.x;
        directionSmooth += (direction - directionSmooth) * Time.deltaTime * changeDirectionEase;
        if (!Player.Instance.frozen && !recovering)
        {
            move.x = 1 * directionSmooth;

            // Flip the graphic depending on the speed
            if (move.x > 0.01f)
            {
                if (graphic.transform.localScale.x == -1)
                {
                    graphic.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
            }
            else if (move.x < -0.01f)
            {
                if (graphic.transform.localScale.x == 1)
                {
                    graphic.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }
            }

            // Check floor type
            ground = Physics2D.Raycast(transform.position, -Vector2.up);
            Debug.DrawRay(transform.position, -Vector2.up, Color.green);

            // Check if player is within range to follow
            if (enemyType == EnemyType.Zombie)
            {
                if ((Mathf.Abs(playerDifference) < followRange))
                {
                    followPlayer = true;
                }
                else
                {
                    followPlayer = false;
                }
            }

            if (followPlayer)
            {
                if (playerDifference < 0)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
            }
            else
            {
                // Allow enemy to instantly change direction when not following player
                directionSmooth = direction;
            }

            // Check for walls
            rightWall = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.right, 1f, layerMask);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, Color.yellow);

            if (rightWall.collider != null)
            {
                if (!followPlayer)
                {
                    direction = -1;
                }
                else
                {
                    Jump();
                }
            }

            leftWall = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.left, 1f, layerMask);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, Color.blue);

            if (leftWall.collider != null)
            {
                if (!followPlayer)
                {
                    direction = 1;
                }
                else
                {
                    Jump();
                }
            }

            // Check for ledges
            if (!followPlayer)
            {
                rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down, .5f);
                Debug.DrawRay(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down, Color.blue);
                if (rightLedge.collider == null)
                {
                    direction = -1;
                }

                leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down, .5f);
                Debug.DrawRay(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down, Color.blue);

                if (leftLedge.collider == null)
                {
                    direction = 1;
                }
            }

        // Recovery after being hit
        }
        else if (recovering)
        {
            recoveryCounter += Time.deltaTime;
            move.x = launch;
            launch += (0 - launch) * Time.deltaTime;
            if (recoveryCounter >= recoveryTime)
            {
                recoveryCounter = 0;
                recovering = false;
            }
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        targetVelocity = move * maxSpeed;
    }

    public void Jump()
    {
        if (grounded)
        {
            velocity.y = jumpTakeOffSpeed;
            PlayJumpSound();
        }
    }

    public void PlayStepSound()
    {
        audioSource.pitch = Random.Range(0.6f, 1f);
        audioSource.PlayOneShot(stepSound);
    }

    public void PlayJumpSound()
    {
        audioSource.pitch = Random.Range(0.6f, 1f);
        audioSource.PlayOneShot(jumpSound);
    }

    public void Hit(int launchDirection)
    {
        animator.SetTrigger("hurt");
        velocity.y = launchPower;
        launch = launchDirection * (launchPower/5);
        recoveryCounter = 0;
        recovering = true;
    }
}