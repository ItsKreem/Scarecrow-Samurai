using UnityEngine;

public class MoveLeftRight : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 3f;   // How far left/right from start
    public float moveSpeed = 2f;      // Movement speed

    private Vector3 startPos;
    private bool movingRight = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float step = moveSpeed * Time.deltaTime;

        if (movingRight)
        {
            transform.Translate(Vector3.right * step);

            if (transform.position.x >= startPos.x + moveDistance)
                movingRight = false;
        }
        else
        {
            transform.Translate(Vector3.left * step);

            if (transform.position.x <= startPos.x - moveDistance)
                movingRight = true;
        }
    }
}

