using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class RockController : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool pulling = false;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
            {
                pulling = false;
            }
        if (pulling)
        {
            Vector3 direction = (transform.position - player.transform.position).normalized;
            Vector2 newPosition = player.transform.position + direction * 1f;
            transform.position = newPosition;
        }
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // check if pulling
            if (Input.GetKey(KeyCode.LeftShift))
            {
                pulling = true;
            }
        }
    }

}
