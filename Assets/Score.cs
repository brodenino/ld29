using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
    public int score = 0;

    public int frameScore = 0;

    public Transform scoreText;
    public Transform frameScoreText;

    public float fadeOutDuration = 0.5f;

    public float fadeOutCurrent = 0;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        for (int i = 0; i < frameScore; i++)
        {
            score += (i+1); 
        }


        if (frameScore > 0)
        {
            frameScoreText.guiText.text = "+" + frameScore;
            frameScoreText.guiText.enabled = true;
            fadeOutCurrent = fadeOutDuration;
            scoreText.guiText.text = "Score: " + score;
        }

        if (fadeOutCurrent > 0)
        {
            fadeOutCurrent -= Time.deltaTime;

            var color = frameScoreText.guiText.color;
            color.a = Mathf.Lerp(1, 0, (fadeOutDuration - fadeOutCurrent) / fadeOutDuration);

            if (fadeOutCurrent <= 0)
            {
                frameScoreText.guiText.enabled = false;
            }
            
            frameScoreText.guiText.color = color;
        }

        frameScore = 0;
    }

}
