using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TestDontDestroy : MonoBehaviour
{
    public GameObject DontDestroyObjects;
    public GameObject player;
    public GameObject cameras;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(DontDestroyObjects);
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(cameras);
        Invoke("TestScene", 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestScene()
    {
        SceneManager.LoadScene("TestScene");
    }
}
