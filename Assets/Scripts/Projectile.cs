using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* using UnityEngine.UI; */

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    private EnemyControllerBoss enemyControllerBoss;

/*
    private int scoreValue = 0;
    public Text score;


    void start()
    {
        score.text = "Score: " + scoreValue.ToString();
    }
*/
    
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    
    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }
    
    void Update()
    {
        if(transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController e = other.collider.GetComponent<EnemyController>();
        if (e != null)
        {
            /*
            scoreValue += 1;
            score.text = "Score: " + scoreValue.ToString();
            SetScoreText ();
            */
            e.Fix();  
        }

        EnemyControllerDifficult f = other.collider.GetComponent<EnemyControllerDifficult>();
        if (f != null)
        {
            /*
            scoreValue += 1;
            score.text = "Score: " + scoreValue.ToString();
            SetScoreText ();
            */
            f.Fix();
        }

        EnemyControllerBoss g = other.collider.GetComponent<EnemyControllerBoss>();        
        if (g != null)
        {
           g.Damage();
        }

/*
        void SetScoreText()
        {
        if (scoreValue == 9)
            {
            }
        }
*/
        Destroy(gameObject);
    }
}
