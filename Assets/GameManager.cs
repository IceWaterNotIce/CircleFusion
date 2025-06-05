using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem; // 新增

public class GameManager : MonoBehaviour
{
    public GameObject circlePrefab;
    public float loseY; // y 座標低於此值則失敗

    public float spawnY; // 圓圈生成的初始 y 座標
    public float spawnMinX; // 生成圓圈的最小 x
    public float spawnMaxX; // 生成圓圈的最大 x

    public int spawnMaxSize = 3; // 圓圈生成的最大大小

    private List<GameObject> circles = new List<GameObject>();
    private bool isGameOver = false;

    public static event Action OnGameOver; // 新增事件
    public static event Action<int> OnScoreChanged; // 分數變動事件

    public int score = 0; // 分數

    public GameObject CurrentCircle; // 當前圓圈

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateCircle(new Vector2(0, spawnY)); // 初始生成一個圓圈
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        // get user mouse input
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DropCircle(); // 釋放當前圓圈
            StartCoroutine(WaitAndCreateCircle(mousePosition)); // 使用協程等待後生成新圓圈
        }

        if (CurrentCircle != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 circlePos = CurrentCircle.transform.position;
            // 限制 x 在 spawnMinX 和 spawnMaxX 之間
            circlePos.x = Mathf.Clamp(mouseWorldPos.x, spawnMinX + CurrentCircle.transform.localScale.x / 2, spawnMaxX - CurrentCircle.transform.localScale.x / 2);
            CurrentCircle.transform.position = circlePos;
        }

        // 檢查所有圓圈的 y 座標
        foreach (var circle in circles)
        {
            if (circle != null && circle.transform.position.y > loseY)
            {
                Debug.Log("Player Lose!");
                isGameOver = true;
                OnGameOver?.Invoke(); // 觸發事件
                break;
            }
        }
    }
    private void CreateCircle(Vector2 mousePosition)
    {
        if (circlePrefab != null)
        {
            float clampedX = Mathf.Clamp(mousePosition.x, spawnMinX, spawnMaxX);
            Vector3 spawnPos = new Vector3(clampedX, spawnY, 0);
            GameObject circle = Instantiate(circlePrefab, spawnPos, Quaternion.identity);

            // 隨機大小 int
            int randomSize = UnityEngine.Random.Range(1, spawnMaxSize + 1);
            circle.transform.localScale = new Vector3(randomSize, randomSize, 1);
            circle.GetComponent<Rigidbody2D>().gravityScale = 0;
            circles.Add(circle);

            CurrentCircle = circle; // 設置當前圓圈為新生成的圓圈
        }


    }

    private void DropCircle()
    {
        if (CurrentCircle != null)
        {
            Rigidbody2D rb = CurrentCircle.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 1; // 啟用重力
            }
            CurrentCircle.GetComponent<CircleCollider2D>().enabled = true; // 啟用碰撞器
            CurrentCircle = null; // 清除當前圓圈
        }

    }

    // 在 Scene 視窗繪製 loseY 線
    // 協程：等待一段時間後生成新圓圈
    private System.Collections.IEnumerator WaitAndCreateCircle(Vector2 mousePosition)
    {
        yield return new WaitForSeconds(1f);
        CreateCircle(mousePosition);
    }

    // 在 Scene 視窗繪製 loseY 線
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float xMin = -100f;
        float xMax = 100f;

        // 取得攝影機可見範圍
        Camera cam = Camera.main;
        Vector3 camLeft = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, Mathf.Abs(cam.transform.position.z)));
        Vector3 camRight = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, Mathf.Abs(cam.transform.position.z)));
        Vector3 camTop = cam.ViewportToWorldPoint(new Vector3(0.5f, 1, Mathf.Abs(cam.transform.position.z)));
        Vector3 camBottom = cam.ViewportToWorldPoint(new Vector3(0.5f, 0, Mathf.Abs(cam.transform.position.z)));
        xMin = camLeft.x;
        xMax = camRight.x;


        Vector3 start = new Vector3(xMin, loseY, 0);
        Vector3 end = new Vector3(xMax, loseY, 0);
        Gizmos.DrawLine(start, end);

        // spawnY 線
        Gizmos.color = Color.green;
        Vector3 spawnStart = new Vector3(xMin, spawnY, 0);
        Vector3 spawnEnd = new Vector3(xMax, spawnY, 0);
        Gizmos.DrawLine(spawnStart, spawnEnd);

        // spawnMinX 與 spawnMaxX 豎線
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(spawnMinX, camTop.y, 0), new Vector3(spawnMinX, camBottom.y, 0));
        Gizmos.DrawLine(new Vector3(spawnMaxX, camTop.y, 0), new Vector3(spawnMaxX, camBottom.y, 0));

        // 標示 loseY 線、spawnY 線、spawnMinX、spawnMaxX
#if UNITY_EDITOR
        float labelX = xMin;
        float loseYLabelY = loseY + 0.2f;
        float spawnYLabelY = spawnY + 0.2f;
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.Label(
            new Vector3(labelX, loseYLabelY, 0),
            "Lose Y (" + loseY.ToString("F2") + ")"
        );
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.Label(
            new Vector3(labelX, spawnYLabelY, 0),
            "Spawn Y (" + spawnY.ToString("F2") + ")"
        );
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.Label(
            new Vector3(spawnMinX, spawnY + 0.5f, 0),
            "spawnMinX"
        );
        UnityEditor.Handles.Label(
            new Vector3(spawnMaxX, spawnY + 0.5f, 0),
            "spawnMaxX"
        );
#endif
    }

    public void AddScore(int value)
    {
        score += value;
        OnScoreChanged?.Invoke(score);
    }

    public int GetScore()
    {
        return score;
    }
}
