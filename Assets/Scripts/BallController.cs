using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour {

    private Camera cam;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Boolean ballClicked = false; // Checks if the gameObject was clicked
    private Boolean ballThrown = false; // Prevent ball from being thrown mid-air
    private Boolean ballFalling = false; // Check if the gO is falling (in order to re-enable the collider)
    private Boolean basketMovingRight = true; // Check basket direction for score = 10+
    private Rigidbody2D rb;
    private Animator anim;
    private Renderer rend;
    public BoxCollider2D floor;
    public GameObject basket;
    public GameObject hoop;
    public GameObject trig;

    private int basketWait = 8;
    private int score;
    private int highScore = btnController.highScoreSurvival;
    public Text scoreText;
    public Text highScoreText;

    // Use this for initialization
    void Start () {
        // If no camera is found in the scene, set cam variable as the main camera
        if (cam == null)
        {
            cam = Camera.main;
        }

        // Setting the screen orientation to be portrait
        Screen.orientation = ScreenOrientation.Portrait;

        // Initializing everything
        Physics2D.gravity = new Vector2(0, -20f);
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rend = gameObject.GetComponent<Renderer>();
        GameObject fl = GameObject.Find("bck");
        floor = fl.GetComponent<BoxCollider2D>();
        basket = GameObject.Find("basket");
        hoop = GameObject.Find("hoop");
        trig = GameObject.Find("Score Trigger");

        score = 0;
        setScoreText();
        setHighScoreText();
	}
	
	// Update is called once per frame
	void Update () {

        // Back button functionality
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }

        if (Input.GetMouseButtonDown(0) && gameObjectClicked() && !ballThrown)
        {
            ballClicked = true;
            Vector3 rawData = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosition = new Vector2(rawData.x, rawData.y);
        }
        if (Input.GetMouseButtonUp(0) && ballClicked == true)
        {
            Vector3 rawData = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPosition = new Vector2(rawData.x, rawData.y);
            Vector2 delta = endPosition - startPosition;

            float dist = Mathf.Sqrt(Mathf.Pow(delta.x, 2) + Mathf.Pow(delta.y, 2));
            float angle = Mathf.Atan(delta.y / delta.x) * (180.0f / (float)Math.PI);

            if (startPosition.y < endPosition.y)
            {
                if (angle < 0) angle = angle * -1;
                Debug.Log("Distance: " + dist + " Angle: " + angle);

                if (dist >= 0.5f && angle > 10 && !ballThrown)
                {
                    // Do ball throw animation
                    anim.SetBool("thrown", true);
                    rb.isKinematic = false;
                    throwBall(delta);
                }
                else
                {
                    ballClicked = false;
                }
            }
            else
            {
                ballClicked = false;
            }
        }

        float yPos = transform.position.y;

        if (yPos >= 2.6 && !gameObject.GetComponent<CircleCollider2D>().enabled)
        {
            gameObject.GetComponent<CircleCollider2D>().enabled = true;
            rend.sortingOrder = 3;
        }
        
        // Checks if the ball is outside of the screen (phone sized)
        if (gameObject.transform.localPosition.x < -6 || gameObject.transform.localPosition.x > 6 || gameObject.transform.localPosition.y < -8)
        {
            // If outside borders and ballFalling is not set to true by scoring, reset score to 0
            if (ballFalling) 
            {
                if (score > highScore)
                {
                    highScore = score;
                    btnController.highScoreSurvival = highScore;
                    setHighScoreText();
                }

                score = 0;
                setScoreText();
                ballFalling = false;
            }

            anim.SetBool("thrown", false);
            gameObject.transform.localScale = new Vector3(1, 1, 1); // Return ball to original size

            respawn(); // Reset the ball position
        }

        if (score >= 10)
        {
            // Check if basket is at the border of the canvas (phone aspect)
            if (basket.transform.localPosition.x > 1.14)
            {
                basketWait++;
                basketMovingRight = false;
            }
            else if (basket.transform.localPosition.x < -1.14)
            {
                basketWait++;
                basketMovingRight = true;
            }

            // transform the basket + hoop accordingly if 8 frames have passed at edge
            if (score < 20)
            {
                if (basket.transform.localPosition.x <= 1.14 && basketMovingRight && basketWait == 8)
                {
                    moveBasket(0.03f);
                }
                else if (basket.transform.localPosition.x >= -1.14 && !basketMovingRight && basketWait == 8)
                {
                    moveBasket(-0.03f);
                }
            }
            else if (score >= 20)
            {
                if (basket.transform.localPosition.x <= 1.14 && basketMovingRight && basketWait == 8)
                {
                    moveBasket(0.06f);
                }
                else if (basket.transform.localPosition.x >= -1.14 && !basketMovingRight && basketWait == 8)
                {
                    moveBasket(-0.06f);
                }
            }
            
        }
    }

    void setHighScoreText()
    {
        highScoreText.text = "Highscore: " + highScore.ToString();
    }

    // Method to set the score in the game (nothing if score is 0)
    void setScoreText()
    {
        if (score > 0)
        {
            scoreText.text = score.ToString();
        }
        else
        {
            scoreText.text = "";
        }
    }

    // Game difficulty method (moves the basket)
    void moveBasket(float x)
    {
        if (x == 0)
        {
            basket.transform.localPosition = new Vector2(0, basket.transform.localPosition.y);
            hoop.transform.localPosition = new Vector2(0, hoop.transform.localPosition.y);
            trig.transform.localPosition = new Vector2(0, trig.transform.localPosition.y);
        }
        else
        {
            basket.transform.localPosition = new Vector2(basket.transform.localPosition.x + x, basket.transform.localPosition.y);
            hoop.transform.localPosition = new Vector2(hoop.transform.localPosition.x + x, hoop.transform.localPosition.y);
            trig.transform.localPosition = new Vector2(trig.transform.localPosition.x + x, trig.transform.localPosition.y);

            // Make the basket wait 8 frames at each edge
            if (basket.transform.localPosition.x > 1.14)
            {
                basketWait = 0;
            }
            else if (basket.transform.localPosition.x < -1.14)
            {
                basketWait = 0;
            }
        }
    }

    // Reset all them values, mmmm
    void respawn()
    {
        rb.isKinematic = true; // Keep ball steady until it's thrown
        ballThrown = false;
        rend.sortingOrder = 4;
        floor.enabled = true;
        gameObject.GetComponent<CircleCollider2D>().enabled = true;

        if (score == 0)
        {
            moveBasket(0);
            basketWait = 8;
            basketMovingRight = true;
            gameObject.transform.localPosition = new Vector2(0f, -3.48f);
        }
        else
        {
            gameObject.transform.localPosition = new Vector2(UnityEngine.Random.Range(-2.4f, 2.4f), -3.66502f);
        }
        
    }

    // Unity method to detect trigger collision (I use it to +1 the score)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Score"))
        {
            Debug.Log("Gool");
            score++;
            setScoreText();
            ballFalling = false;
            floor.enabled = false;
        }
    }

    // A method to check if the ball was clicked before we throw it at any random swipe
    private bool gameObjectClicked()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePosition);

        if (hit)
        {
            Debug.Log("Ball clicked");
            return true;
        }
        else
        {
            Debug.Log("Ball not clicked");
            return false;
        }
    }

    // AddForce to the ball equivalent to the swipe you made in the X direction, and a constant force in Y
    private void throwBall(Vector2 delta)
    {
        ballThrown = true;
        ballFalling = true;
        floor.enabled = false;

        Debug.Log("Throw ball");

        Vector2 throwForce = new Vector2(delta.x * 10, 48f);
        rb.AddForce(throwForce, ForceMode2D.Impulse);

        // Disable the balls collider so it doesn't hit the hoop on the way up 
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }
}
