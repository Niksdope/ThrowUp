using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class btnController : MonoBehaviour {

    GameObject ball;
    public static int highScoreSurvival = 0;
    public static int highScoreTimeAttack = 0;
	// Use this for initialization
	void Start () {
        ball = GameObject.Find("bb");
	}
	
	// Update is called once per frame
	void Update () {
        ball.transform.Rotate(0, 0, 20 * -Time.deltaTime);
	}

    public void OnSurvival()
    {
        SceneManager.LoadScene("Survival");
    }
    public void OnTimeAttack()
    {
        SceneManager.LoadScene("Time Attack");
    }
    public void OnScores()
    {
        SceneManager.LoadScene("Scores");
    }
}
