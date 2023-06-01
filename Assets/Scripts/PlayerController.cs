using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    enum enDirection { North, East, West};

    public AudioClip[] soundFXClips;
    public Text distanceText;
    public Text coinText;
    public Text bestDistance;
    public Text bestCoins;
    public GameObject deathMenu;
    public bool mobileEnabled;


    CharacterController characterController;
    Vector3 playerVector;
    enDirection playerDirection = enDirection.North;
    enDirection playerNextDirection = enDirection.North;
    Animator anim;
    BridgeControl bridgeControl;
    AudioSource audioSource;
    Gestures gestures;

    int coinsCollected = 0;
    int coinsCollectedBest;
    int distanceRun = 0;
    int distanceRunBest;

    float playerStartSpeed = 10f;
    float playerSpeed;
    float gValue = -40f;
    float translationFactor = 10f;
    float jumpForce = 15f;
    float timer = 0;
    float distance = 0;
    float verticalSpeed = 0f;
    float translationFactorMobile = 7f;

    bool canTurnRight = false; 
    bool canTurnLeft = false;
    bool isDead = false;   
    bool isJumping = false;
   
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        playerSpeed = playerStartSpeed;
        characterController = this.GetComponent<CharacterController>();
        anim = this.GetComponent<Animator>();
        bridgeControl = GameObject.Find("BridgeManager").GetComponent<BridgeControl>();
        playerVector = new Vector3(0, 0, 1) * playerSpeed * Time.deltaTime;
        audioSource = this.GetComponent<AudioSource>();
        deathMenu.SetActive(false);
        distanceRunBest = PlayerPrefs.GetInt("highscoreD");
        coinsCollectedBest = PlayerPrefs.GetInt("highscoreC");
        gestures = this.GetComponent<Gestures>();

    }

    
    void Update()
    {
        PlayerLogic();
        distanceText.text = distanceRun.ToString();
        coinText.text = " X " + coinsCollected.ToString();
    }

    void PlayerLogic()
    {
        if (isDead)
            return;

        if (!characterController.enabled) { characterController.enabled = true; }

        timer += Time.deltaTime;

        playerSpeed += 0.1f * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.E) && canTurnRight || gestures.swipeRight && canTurnRight)
        {
            switch(playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.East;
                    this.transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;

                case enDirection.West:
                    playerNextDirection = enDirection.North;
                    this.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
            gestures.swipeRight = false;
            audioSource.PlayOneShot(soundFXClips[6], 0.4f);
                           
        }

        if (Input.GetKeyDown(KeyCode.Q) && canTurnLeft || gestures.swipeLeft && canTurnLeft)
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.West;
                    this.transform.rotation = Quaternion.Euler(0, -90, 0);
                    break;

                case enDirection.East:
                    playerNextDirection = enDirection.North;
                    this.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
            gestures.swipeLeft = false;
            audioSource.PlayOneShot(soundFXClips[6], 0.4f);
        }

        
        playerDirection = playerNextDirection;

        if(playerDirection == enDirection.North)
        {
            playerVector = Vector3.forward * playerSpeed * Time.deltaTime;
        }

        else if (playerDirection == enDirection.East)
        {
            playerVector = Vector3.right * playerSpeed * Time.deltaTime;
        }

        else if (playerDirection == enDirection.West)
        {
            playerVector = Vector3.left * playerSpeed * Time.deltaTime;
        }

        // Horizontal Movement of the Player
        switch(playerDirection)
        {
            case enDirection.North:
                if(mobileEnabled) 
                {
                    if(Input.acceleration.x > 0.2f || Input.acceleration.x < -0.2f)
                    {
                        playerVector.x = Input.acceleration.x * translationFactorMobile * Time.deltaTime;
                    }
                    
                }
                else { playerVector.x = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime; }                
                break;

            case enDirection.East:
                if (mobileEnabled) 
                {
                    if (Input.acceleration.x > 0.2f || Input.acceleration.x < -0.2f)
                    {
                        playerVector.z = -Input.acceleration.x * translationFactorMobile * Time.deltaTime;
                    }
                     
                }
                else { playerVector.z = -Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime; }
                break;

            case enDirection.West:
                if (mobileEnabled)
                {
                    if (Input.acceleration.x > 0.2f || Input.acceleration.x < -0.2f)
                    {
                        playerVector.z = Input.acceleration.x * translationFactorMobile * Time.deltaTime;
                    }

                }
                else { playerVector.z = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime; }
                break;
        }

        if(Input.GetKeyDown(KeyCode.DownArrow) && characterController.isGrounded || gestures.swipeDown && characterController.isGrounded)
        {
            DoSliding();
            gestures.swipeDown = false;
        }

        void DoSliding()
        {
            
            characterController.height = 1f;
            characterController.center = new Vector3(0, 0.5f, 0);
            characterController.radius = 0;

            StartCoroutine(ReEnableCC());
            audioSource.PlayOneShot(soundFXClips[5], 0.4f);
            anim.SetTrigger("isSliding");
            
            
        }

        IEnumerator ReEnableCC()
        {
            yield return new WaitForSeconds(0.5f);

            characterController.height = 1.75f;
            characterController.center = new Vector3(0, 0.9f, 0);
            characterController.radius = 0.25f;

        }


        // Vertical Movement of the Player
        if (characterController.isGrounded)
        {
            verticalSpeed = -0.2f;
        }
        else
        {
           verticalSpeed += gValue * Time.deltaTime;
        }

       
        if (Input.GetKeyDown(KeyCode.UpArrow) && characterController.isGrounded || gestures.swipeUp && characterController.isGrounded)
        {
            audioSource.PlayOneShot(soundFXClips[3], 0.4f);
            anim.SetTrigger("isJumping");
            isJumping = true;
            verticalSpeed = jumpForce;
            gestures.swipeUp = false;
        }

        if(isJumping)
        {
            verticalSpeed += gValue * Time.deltaTime;
            playerVector.y = verticalSpeed * Time.deltaTime;
            if(verticalSpeed <= 0f)
            {
                isJumping = false;
            }
        }

        else { playerVector.y = verticalSpeed * Time.deltaTime; }

        if(this.transform.position.y < -1.9f)
        {
            isDead = true;
            audioSource.PlayOneShot(soundFXClips[2], 0.4f);
            anim.SetTrigger("isTripping");
        }

        characterController.Move(playerVector);
        distance = playerSpeed * timer;
        distanceRun = (int)distance;

    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {


        if(hit.gameObject.tag == "LCorner")
        {
            canTurnLeft = true;
        }

        else if (hit.gameObject.tag == "RCorner")
        {
            canTurnRight = true;
        }

        else
        {
            canTurnLeft = false;
            canTurnRight = false;
            gestures.swipeRight = false;
            gestures.swipeLeft = false;
        }

        
        if (hit.gameObject.tag == "Obstacle")
        {           

            if (characterController.isGrounded)
            {
                isDead = true;
                anim.SetTrigger("isTripping");
                audioSource.PlayOneShot(soundFXClips[1], 0.4f);
            }

            SaveScore();
        }
    }

    private void OnGUI()
    {
        if(isDead)
        {
            //if(GUI.Button(new Rect(0.4f * Screen.width, 0.6f * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height), "RESTART"))
            //{
            //    DeathEvent();
            //}

            deathMenu.SetActive(true);
        }

        //GUI.Label(new Rect(10, 10, 100, 20), "Coins: " + coinsCollected.ToString());
        //GUI.Label(new Rect(10, 40, 150, 20), "Best Coins: " + PlayerPrefs.GetInt("highscoreC").ToString());
        //GUI.Label(new Rect(10, 70, 100, 20), "Distance: " + distanceRun.ToString());
        //GUI.Label(new Rect(10, 100, 150, 20), "Best Distance: " + PlayerPrefs.GetInt("highscoreD").ToString());
    }

    public void DeathEvent()
    {
        deathMenu.SetActive(false);
        bestCoins.text = "";
        bestDistance.text = "";
        characterController.enabled = false;
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
        playerDirection = enDirection.North;
        playerNextDirection = enDirection.North;
        playerSpeed = playerStartSpeed;
        playerVector = Vector3.forward * playerSpeed * Time.deltaTime;
        coinsCollected = 0;
        timer = 0;
        bridgeControl.CleanTheScene();
        anim.SetTrigger("isRespawned");
        isDead = false;
    }

    void FootStepEventA()
    {
       audioSource.PlayOneShot(soundFXClips[0], 1f);     
    }

    void FootStepEventB()
    {
        audioSource.PlayOneShot(soundFXClips[0], 1f);
    }

    void JumpLandEvent()
    {
        audioSource.PlayOneShot(soundFXClips[4], 0.4f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Coin")
        {
            Destroy(col.gameObject);
            audioSource.PlayOneShot(soundFXClips[7], 0.4f);
            coinsCollected += 1;
        }
    }

    void SaveScore()
    {
        if(coinsCollected > coinsCollectedBest)
        {
            coinsCollectedBest = coinsCollected;
            PlayerPrefs.SetInt("highscoreC", coinsCollectedBest);
            PlayerPrefs.Save();
            bestCoins.text = "Congratulations!!! Best Coin Score : " + coinsCollected.ToString();
        }

        if (distanceRun > distanceRunBest)
        {
            distanceRunBest = distanceRun;
            PlayerPrefs.SetInt("highscoreD", distanceRunBest);
            PlayerPrefs.Save();
            bestDistance.text = "Congratulations!!! Distance Score : " + distanceRun.ToString() + " M";
        }
    }

    public void ExitAapp()
    {
        SceneManager.LoadScene("Menu");
    }
       
}
