using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject Prefab;
    public SpawnMode mode;
    public string input;
   
    public float period = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        if (mode == SpawnMode.Once)
        {
            Instantiate(Prefab, transform);
        }
    }

    // Update is called once per frame
    private float nextActionTime = 0.0f;
    void Update()
    {
        if ((mode == SpawnMode.Timer) && Time.time > nextActionTime)
        {
            nextActionTime += period;
            Instantiate(Prefab, transform);
        }
        if (mode == SpawnMode.OnTrigger && Input.GetButtonDown(input))
        {
            Instantiate(Prefab, transform);
        }
    }

}

public enum SpawnMode {
    Once,
    Timer,
    OnTrigger
}
