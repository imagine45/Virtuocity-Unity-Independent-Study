using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad;
    
    void onTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player")) {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
  
}
