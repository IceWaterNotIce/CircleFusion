using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject circlePrefab;
    public float loseY = -5f; // y 座標低於此值則失敗

    private List<GameObject> circles = new List<GameObject>();
    private bool isGameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        // get user mouse input
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (circlePrefab != null)
            {
                GameObject circle = Instantiate(circlePrefab, mousePosition, Quaternion.identity);
                circles.Add(circle);
            }
        }

        // 檢查所有圓圈的 y 座標
        foreach (var circle in circles)
        {
            if (circle != null && circle.transform.position.y < loseY)
            {
                Debug.Log("Player Lose!");
                isGameOver = true;
                break;
            }
        }
    }
}
