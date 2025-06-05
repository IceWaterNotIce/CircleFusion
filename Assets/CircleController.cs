using UnityEngine;

public class CircleController : MonoBehaviour
{
    public float scaleStep = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // change the color of this circle to a new color according to the scale
        float scale = this.transform.localScale.x;
        float colorValue = Mathf.Clamp01(scale / 5f); // assuming max scale is 5
        GetComponent<SpriteRenderer>().color = new Color(colorValue, 0.5f, 1 - colorValue, 1); // RGB color based on scale
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<CircleController>() != null && other.gameObject.transform.localScale.x == this.transform.localScale.x)
        {
            // get the circle that the distance between the circle and origin point is more less 
            if (Vector2.Distance(other.gameObject.transform.position, Vector2.zero) < Vector2.Distance(this.transform.position, Vector2.zero))
            {
                // destroy the circle that is closer to the origin point
                Destroy(other.gameObject);

                // increase the score
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.AddScore(1);
                }

                this.transform.localScale += new Vector3(scaleStep, scaleStep, 0);
                // change the color of this circle to a new color according to the scale
                float scale = this.transform.localScale.x;
                float colorValue = Mathf.Clamp01(scale / 5f); // assuming max scale is 5
                GetComponent<SpriteRenderer>().color = new Color(colorValue, 0.5f, 1 - colorValue, 1); // RGB color based on scale
            }
            else
            {
                // destroy this circle
                Destroy(this.gameObject);
            }
        }
    }
}
