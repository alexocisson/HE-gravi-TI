using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverComponent : MonoBehaviour
{

    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine("Fade");
        text.color = new Color(1, 0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Fade()
    {
        for (float ft = 1f; ft >= 0; ft -= 0.05f)
        {
            Color c = text.color;
            c.a = ft;
            text.color = c;
            yield return new WaitForSeconds(.1f);
        }
        Destroy(gameObject);
    }
}
