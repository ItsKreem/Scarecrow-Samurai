using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveupanddown : MonoBehaviour
{
    [Header("Movement Settings")]
    public float amplitude = 1f;     // How high it moves
    public float frequency = 1f;     // How fast it moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Sine wave movement
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
    }
}

