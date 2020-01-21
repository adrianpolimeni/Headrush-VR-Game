 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    

    Animator animator;
    public float xVelocity, yVelocity;
    private float speed = 0.02f;
    private Transform transform;
    public Transform Target;
    public float ShootingSkill, MovementSkill;
    bool isReloading = false;
    int clip = 8;
    float t = 0;
    bool isMoving = false;
    Vector3 StartPos, TargetPos;
    float duration = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        transform = GetComponent<Transform>();
        animator.enabled = true;
        Ragdoll(false);
        InvokeRepeating("AiMovement", 1f-MovementSkill, 2.0f);
       // InvokeRepeating("AiShooting", 1f-ShootingSkill, Mathf.Lerp(0.2f,0.5f,1f-ShootingSkill));

    }

    void Update()
    {
       UpdateMovement();
       UpdateAnimation();
       float temp = NextGaussian();
      

    }


    void AiMovement()
    {
        //float distance = (transform.position - TargetPos).magnitude;
        //float curPos = Mathf.Lerp(0f, distance,);

        if (!isMoving)
        {
            float r = Random.Range(0f, 1f);
            if (r < MovementSkill)
            { // Move Frequecy
                TargetPos = RandomPosition();
                StartPos = transform.position;
                isMoving = true;

            }
        }


    }

    private Vector3 RandomPosition()
    {
        float x = Random.Range(-10f, 10f);
        float y = 0f;
        float z = Target.position.z + Mathf.Lerp(Random.Range(0f, 5f), Random.Range(10f, 15f),  1f - MovementSkill);
        return new Vector3(x, y, z);
    }


    void AiShooting()
    {
        if (isReloading)
            return;

        float r = Random.Range(0f, 1f);
        if (r < Mathf.Lerp(0.2f,1f,ShootingSkill)) { // Shoot Frequency
            //Shoot
            clip--;
            if (clip == 0)
                StartCoroutine(ReloadCoroutine());
        }

        
    }

    void Shoot()
    {



    }


    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(Mathf.Lerp(1.5f,4f,1f-ShootingSkill));
        clip = 8;
        isReloading = false;
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

        return (gVal+3f)/6f; // Return between 0-1
    }

    private void UpdateMovement()
    {

        Vector3 temp = Target.position;
        temp[1] = 0f;
        Target.position = temp;
        transform.LookAt(Target);
        
        if (isMoving)
        {
            Vector3 next = Vector3.MoveTowards(transform.position, TargetPos, speed);
            Vector3 velocity = (next - transform.position);
            float a = Vector3.Angle(transform.forward, new Vector3(0f, 0f, 1f));
            velocity = Quaternion.Euler(0f, a, 0f) * velocity;
            Debug.Log(transform.forward);
            
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
        

        /*
        Vector3 moveDirection = new Vector3(xVelocity, 0.0f, yVelocity);
        moveDirection *= speed;
        transform.position += moveDirection;

        if (transform.position.x > 1f) {
            Debug.Log(Time.time);
        }
        */




    }

    private void UpdateAnimation() {
        animator.SetFloat("xVelocity", xVelocity);
        animator.SetFloat("yVelocity", yVelocity);
    } 

    void Ragdoll(bool ragdoll)
    {
        Collider[] col = GetComponentsInChildren<Collider>();
        Rigidbody[] rb = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < col.Length; i++)
        {
            rb[i].isKinematic = !ragdoll;
            for (int j = 0; j < col.Length; j++)
            {
                if (i != j)
                    Physics.IgnoreCollision(col[i], col[j], ragdoll);
            }
        }
        animator.enabled = !ragdoll;
    }
}
