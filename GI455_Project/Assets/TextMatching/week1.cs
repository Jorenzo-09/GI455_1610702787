using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class week1 : MonoBehaviour
{
    string[] data = { "Doraemon", "Naruto", "Onepiece", "Dragonball", "Conan", "Pokemon" };
    public Text textData;
    public Text textCheck;

    public void CheckData()
    {
        for (int i = 0; i <= data.Length-1; i++)
        {
            if(textData.text == data[i])
            {              
                textCheck.text = textData.text + "<color=green> is found </color> "; 
                return;
            }
            else if(textData.text != data[i])
            {
                textCheck.text = textData.text + " <color=red> not found </color>";
            }
            
        }
    }
}
