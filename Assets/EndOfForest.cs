using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfForest : GameManager
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MainMenu();
        }
    }
}
