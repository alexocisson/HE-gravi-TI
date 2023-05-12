using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PseudoController : MonoBehaviour
{

    InputField input;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputField>();

        input.text = "Player" + Random.Range(1000, 10000);
        myPseudo = input.text;
        input.onValueChanged.AddListener(delegate {
            myPseudo = input.text;
        });
    }

    public static string myPseudo = "";
}
