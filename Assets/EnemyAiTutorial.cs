using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Video;

public class EnemyAiTutorial : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private RawImage rawImage;

    private Animator myAnim;

    public NavMeshAgent agent;

    public Image image;
    public Text text;
    public Swimmer swim;
    public float duration = 2.0f;
    public float durationCam = 2.0f;
    float t = 0f;


    public GameObject cam1;
    public GameObject cam2;
    public GameObject Man;
    public GameObject Player;
    public GameObject Canva2;


    public Transform player;
    public Transform playerMiddle;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    // bool alreadyAttacked;
    // public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;


    private void Awake()
    {
        player = GameObject.Find("Fly").transform;
        agent = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();

        swim = GameObject.Find("Fly").GetComponent<Swimmer>();
        image.GetComponent<Image>().color = new Color32(0,0,0,0);
        // text = GameObject.Find("Text").GetComponent<Text>();
        // text.color = new Color32(0,0,0,0);

        cam1.SetActive(true);
        cam2.SetActive(false);
        Man.SetActive(true);
        Player.SetActive(true);
        Canva2.SetActive(true);

        videoPlayer = GameObject.Find("video").GetComponent<VideoPlayer>();
        rawImage = GameObject.Find("video").GetComponent<RawImage>();
        videoPlayer.Pause();
    }

    private void Update()
    {
        //Check for sight and attack range
        // playerInSightRange = Physics.CheckSphere(playerMiddle.position, sightRange, whatIsPlayer);
        // playerInAttackRange = Physics.CheckSphere(playerMiddle.position, attackRange, whatIsPlayer);

        playerInSightRange = Physics.CheckCapsule(playerMiddle.position, transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckCapsule(playerMiddle.position, transform.position, attackRange, whatIsPlayer);


        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

        if (image.GetComponent<Image>().color == new Color32(255,255,255,255)){
            Invoke("ChangeCam", durationCam);
            videoPlayer.Play();
        }
    }

    private void Patroling()
    {
        myAnim.SetBool("attack", false);

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f){
            walkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 1000f, whatIsGround)){
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        myAnim.SetBool("attack", false);

        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);
        myAnim.SetBool("attack", true);
        
        swim.accSpeed = 0f;
        // image.GetComponent<Image>().color = new Color32(207,44,0,50);

        t += Time.deltaTime / duration;
        image.GetComponent<Image>().color = Color.Lerp(new Color32(255,255,255,0), new Color32(255,255,255,255), t);
        // text.GetComponent<Text>().color = Color.Lerp(new Color32(0,0,0,0), new Color32(0,0,0,255), t);
        

        // transform.LookAt(player);

    //     if (!alreadyAttacked)
    //     {
    //         ///Attack code here
    //         Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
    //         rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
    //         rb.AddForce(transform.up * 8f, ForceMode.Impulse);
    //         ///End of attack code

    //         alreadyAttacked = true;
    //         Invoke(nameof(ResetAttack), timeBetweenAttacks);
    //     }
    }

    // private void ResetAttack()
    // {
    //     alreadyAttacked = false;
    // }

    // public void TakeDamage(int damage)
    // {
    //     health -= damage;

    //     if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    // }
    // private void DestroyEnemy()
    // {
    //     Destroy(gameObject);
    // }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerMiddle.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        // Gizmos.DrawWireCube(playerMiddle.position, new Vector3(1, 1, 1)*attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerMiddle.position, sightRange);
        Gizmos.DrawWireSphere(transform.position, sightRange);
        // Gizmos.DrawWireCube(playerMiddle.position, new Vector3(1, 1, 1)*sightRange);
    }


    private void ChangeCam(){
        cam1.SetActive(false);
        cam2.SetActive(true);
        Man.SetActive(false);
        Player.SetActive(false);
        Canva2.SetActive(true);
    }
}
