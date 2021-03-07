using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    Text text;
    ReversiController reversiController;

    void Start()
    {
        reversiController = GameObject.Find("ReversiController").GetComponent<ReversiController>();
        text = this.GetComponent<Text>();
    }

    void Update()
    {
        string displayText = "";

        if (!(reversiController.isGameSet)) {
            switch (reversiController.currentPlayer) {
                case ReversiController.SQUARE_STATUS.BLACK:
                    displayText = "黒(プレイヤー)\nの番です。";
                    break;
                case ReversiController.SQUARE_STATUS.WHITE:
                    displayText = "白(AI)\nの番です。";
                    break;
                default:
                    break;
            }
        } else {
            displayText = "ゲーム終了";
        }

        displayText = displayText + "\n";
        displayText = displayText + "\n";
        displayText = displayText + "黒：" + reversiController.blackCount.ToString();
        displayText = displayText + "\n";
        displayText = displayText + "白：" + reversiController.whiteCount.ToString();

        text.text = displayText;
    }
}
