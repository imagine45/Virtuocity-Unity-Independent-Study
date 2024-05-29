using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class intro1ToChapter1 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameObject.Find("Power Room").GetComponent<powerRoomScript>().isClicked())
            {
                GetComponent<Animator>().SetTrigger("end");
            }
        }
    }

    public void loadScene()
    {
        SceneManager.LoadScene("Chapter1");
    }

}
