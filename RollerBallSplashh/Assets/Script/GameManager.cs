using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] grounds;

    public float groundNumbers;
    void Start()
    {
        grounds = GameObject.FindGameObjectsWithTag("Ground");
        Debug.Log("Par�a say�s� : " + grounds.Length);
    }

    // Update is called once per frame
    void Update()
    {
        groundNumbers = grounds.Length;
    }
}
