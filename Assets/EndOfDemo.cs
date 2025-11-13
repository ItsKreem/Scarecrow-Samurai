using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfDemo : GameManager
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MainMenu();
        }
    }

}
