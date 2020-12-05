using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;

    public static int level = 1;

    public static int cogValue = 4;

    //this paragraph of code is added as of part 2 of the challenge
    public int scoreValue = 0;
    public Text score;
    public Text winText;
    public Text loseText;
    public Text cog;
    public AudioClip musicClipOne;
    public AudioClip musicClipTwo;
    public AudioSource musicSource;

    bool gameOver = false;
    
    public GameObject projectilePrefab;
    public GameObject healthIncrease;
    public GameObject healthDecrease;
    public GameObject backgroundMusic;
    
    public AudioClip throwSound;
    public AudioClip hitSound;

    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        //this paragraph of code is added as of part 2 of the challenge
        scoreValue = 0;
        score.text = "Score: " + scoreValue.ToString();

        cogValue = 4;
        cog.text = "Cogs: " + cogValue.ToString();
     
        audioSource = GetComponent<AudioSource>();

        GameObject backgroundMusicObject = GameObject.FindWithTag("BGMusic");
        if (backgroundMusicObject != null)
        {
            print("Found the object!");
            backgroundMusic = backgroundMusicObject;
        }

        if (backgroundMusicObject == null)
        {
            print("Cannot find the object!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        if (currentHealth == 0) //lose function
        {
            speed = 0;
            gameOver = true;
            loseText.text = "You lose! Press R to restart. Game created by Chase Rook.";
            backgroundMusic.SetActive(false);
            musicSource.clip = musicClipTwo;
            musicSource.Play();
            musicSource.loop = false;

            if (Input.GetKey(KeyCode.R))
            {
                if (gameOver == true)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
                }
            }
        }

        if (scoreValue == 4) //win function
        {
            winText.text = "Talk to Jambi to visit stage two!";

           if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            { 
                // you could probably add your IF statement here
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                     SceneManager.LoadScene("Stage 2");
                     level = 2;
                }
            }
        }
        }

        if (level == 2) //win function level 2
        {
            if (scoreValue == 4)
            {
            winText.text = "You Win! Press R to restart. Game created by Chase Rook.";
            gameOver = true;
            backgroundMusic.SetActive(false);
            musicSource.clip = musicClipOne;
            musicSource.Play();
            musicSource.loop = false;
    
            if (Input.GetKey(KeyCode.R))
            {
                if (gameOver == true)
                {
                    SceneManager.LoadScene("Stage 1");
                    level = 1; // this loads the currently active scene
                }
            }
        }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject projectileObject = Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }

         if (amount > 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject projectileObject = Instantiate(healthIncrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    
    public void Launch()
    {
        if (cogValue > 0)
        {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        
        PlaySound(throwSound);

        cogValue -= 1;
        cog.text = "Cog: " + cogValue.ToString();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    
    //this paragraph of code is added as of part 2 of the challenge
    public void ChangeScore(int amount)
    {
        scoreValue += 1;
        score.text = "Score: " + scoreValue.ToString();
    } 

    public void ChangeCogs(int amount)
    {
        cogValue += 3;
        cog.text = "Cog: " + cogValue.ToString();
    }

    /*
    
        if (scoreValue == 4) //win function
        {
            bool gameOver = true;
            winText.text = "You win!  Press R to restart. Game created by Chase Rook.";
            ChangeMusicVictory();

            if (Input.GetKey(KeyCode.R))
            {
                if (gameOver == true)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
                }
            }
        }
        */
}
