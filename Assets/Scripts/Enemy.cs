using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;
    private float xVelocity, yVelocity;
    private float speed = 0.05f;
    private Transform transform;
    GameObject Player;
    public float ShootingSkill, MovementSkill;
    public float AttackDistance = 20f; // Not zero 
    bool isMoving = false;
    bool isPassive = true;
    public bool isAlive = true;
    Vector3 InitPos;
    Vector3 TargetPos;
    public BlasterEnemy blaster;
    public int health = 100;

    void Start()
    {
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        // Start animation at random times
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
        MovementSkill = NextGaussian();
        ShootingSkill = NextGaussian();
        InitPos = transform.position;
        Player = MainManager.Player;
        Ragdoll(false);
    }

    void Update()
    {
        if (isPassive && Vector3.Distance(Player.transform.position, transform.position) < AttackDistance && MainManager.GameStarted) {
            isPassive = false;
            
            if (transform.position.z < 52.5f)
                InvokeRepeating("AiMovement", Mathf.Lerp(0f, 3f, Random.value), 0.5f);
            InvokeRepeating("AiShooting", Mathf.Lerp(0.5f, 2f, Random.value), 0.5f);
        }
        UpdateMovement();
        UpdateAnimation();
    }

    public void OnTriggerEnter(Collider other)
    {
        isMoving = false;
        try
        {
            TargetPos = transform.position - other.transform.position;
        }
        catch (System.Exception e) { }
    }

    void AiMovement()
    {
        if (!isMoving && !isPassive && isAlive)
        {
            float r = Random.Range(0f, 1f);
            if (r < MovementSkill)
            { // Move Frequecy
                TargetPos = RandomPosition();
                isMoving = true;
            }
        }
    }

    private Vector3 RandomPosition()
    {
        float x = Random.Range(-5f, 5f);
        float y = 0f;
        float z = InitPos.z + Mathf.Lerp(0f, Random.Range(-5f, 5f), MovementSkill);
        return new Vector3(x, y, z);
    }

    private float NextGaussian()
    {
        float gVal, val1, val2;
        do
        {
            val1 = Random.Range(0.0f, 1.0f);
            val2 = Random.Range(0.0f, 1.0f);
            gVal = Mathf.Sqrt(-2.0f * Mathf.Log(val1)) * Mathf.Sin(2.0f * Mathf.PI * val2);
        } while (Mathf.Abs(gVal) > 3f);

        return (gVal + 3f) / 6f; // Return between 0-1
    }

    void AiShooting()
    {
        if (!Player.GetComponent<Player>().isAlive || MainManager.GameFinished)
            return;
        float r = Random.Range(0f, 1f);
        if (r < ShootingSkill) { // Shoot Frequency
            blaster.Shoot();
        }
        float rangeFactor = 1f - (Vector3.Distance(transform.position, Player.transform.position) / AttackDistance); // 1 - 25
        if ( ShootingSkill + rangeFactor > 2f * NextGaussian()) {
            Player.GetComponent<Player>().OnShot();
        }
    }
    

    private void UpdateMovement()
    {
        Vector3 temp = Player.transform.position;
        temp[1] = 0f;
        Player.transform.position = temp;
        transform.LookAt(Player.transform);
        
        if (isMoving)
        {
            Vector3 next = Vector3.MoveTowards(transform.position, TargetPos, speed);
            Vector3 velocity = (next - transform.position); 
            float a = Vector3.Angle(transform.forward, new Vector3(0f, 0f, 1f));
            velocity = Quaternion.Euler(0f, a, 0f) * velocity;
            transform.position = next;
            xVelocity = velocity.x/speed;
            yVelocity = velocity.z/speed;
            isMoving = (transform.position - TargetPos).magnitude > 0.1f;
        }
        else
        {
           xVelocity /= 2f;
           yVelocity /= 2f;
        }
    }

    private void UpdateAnimation() {
        animator.SetFloat("xVelocity", xVelocity);
        animator.SetFloat("yVelocity", yVelocity);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isPassive", isPassive);
    } 

    void Ragdoll(bool ragdoll)
    {
        Collider[] col = GetComponentsInChildren<Collider>();
        Rigidbody[] rb = GetComponentsInChildren<Rigidbody>();
        for(int i=0;i<rb.Length;i++)
            rb[i].isKinematic = !ragdoll;

        for (int i = 0; i < col.Length; i++)
        { 
            if (col[i].isTrigger)
                continue;
            for (int j = 0; j < col.Length; j++)
                if (i != j)
                    Physics.IgnoreCollision(col[i], col[j], ragdoll);
        }
        animator.enabled = !ragdoll;
        CancelInvoke();
    }

    void OnHit(RaycastHit hit) {
        health -= 40;
        if (health < 0f) {
            Ragdoll(true);
            isAlive = false;
            hit.rigidbody.AddForce(transform.TransformDirection(Vector3.forward) * 30f, ForceMode.Impulse);
        }
    }

    void OnHeadHit()
    {
        if (!isAlive)
            return;
        Player p = Player.GetComponent<Player>();
        // Set player's health to droids health
        p.Health = health;
        p.MoveTo(transform.position); 
        transform.position = Player.transform.position;
        Ragdoll(true);
        isAlive = false;
    }
}
