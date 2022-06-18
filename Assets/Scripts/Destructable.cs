using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public AudioSource FallForWall;
    public GameObject explosion;
    public int TimeForDestroy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage()
    {
        if (FallForWall != null)
        { FallForWall.Play(); }
        if (explosion != null)
        {
            var fx = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
            Destroy(fx, 2);
        }
        Destroy(gameObject, TimeForDestroy);
    }
}
