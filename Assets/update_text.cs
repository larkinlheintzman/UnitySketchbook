using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class update_text : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      GetComponent<TextMesh>().text = "0.0, 0.0";
    }

    // Update is called once per frame
    void Update()
    {
      GetComponent<TextMesh>().text = string.Concat(Input.GetAxis("Horizontal").ToString(), " ", Input.GetAxis("Vertical").ToString());
      // transform.parent = Camera.main.transform;
    }
}
