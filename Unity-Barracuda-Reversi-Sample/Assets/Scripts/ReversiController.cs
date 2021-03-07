using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Unity.Barracuda;

public class ReversiController : MonoBehaviour
{
    // Public：SerializeField
    [SerializeField]
    GameObject boardObject = null;

    [SerializeField] 
    GameObject square = null;
    [SerializeField] 
    GameObject blackPiece = null;
    [SerializeField] 
    GameObject whitePiece = null;
    [SerializeField]
    NNModel modelAsset = null;

    // Public：ゲーム状態
    public int blackCount = 0;
    public int whiteCount = 0;
    public bool isGameSet = false;
    
    // Public：石状態
    public enum SQUARE_STATUS
    {
        EMPTY,
        BLACK,
        WHITE,
        INVALID
    }
    public SQUARE_STATUS currentPlayer = SQUARE_STATUS.BLACK;

    // Private：盤面
    private const int WIDTH = 8;
    private const int HEIGHT = 8;
    private SQUARE_STATUS[,] board = new SQUARE_STATUS[WIDTH, HEIGHT];

    // Private：リバーシAI
    private ReversiAI reversiAI;
    
    void Start()
    {
        // 盤面初期化
        InitializeBoard();

        // リバーシ推論用クラス
        reversiAI = new ReversiAI(modelAsset);
    }
    public void InitializeBoard()
    {
        isGameSet = false;
        currentPlayer = SQUARE_STATUS.BLACK;

        board = new SQUARE_STATUS[WIDTH, HEIGHT];
        board[3, 3] = SQUARE_STATUS.WHITE;
        board[3, 4] = SQUARE_STATUS.BLACK;
        board[4, 3] = SQUARE_STATUS.BLACK;
        board[4, 4] = SQUARE_STATUS.WHITE;
        
        CountPiece();
        ShowBoard();
    }

    void ShowBoard()
    { 
        // 盤面初期化
        foreach (Transform child in boardObject.transform) {
            Destroy(child.gameObject);
        }

        // 盤面構築
        for (int y = 0; y < HEIGHT; y++) {
            for (int x = 0; x < WIDTH; x++) {
                int positionId = x + (y * WIDTH);
                GameObject piece = GetPiecePrefab(board[x, y]);
                piece.transform.SetParent(boardObject.transform);
                
                // ボタンコールバック登録
                piece.GetComponent<Button>().onClick.AddListener(
                    () => { SetPiece(positionId.ToString()); }
                );
            }
        }
    }

    public void SetPiece(string positionId)
    {
        // 黒番(プレイヤー)以外の場合、無効
        if (currentPlayer != SQUARE_STATUS.BLACK) {
            return;
        }

        // 指定座標
        int x = int.Parse(positionId) % WIDTH;
        int y = int.Parse(positionId) / WIDTH;
        
        if (board[x, y] == SQUARE_STATUS.EMPTY) {
            Reverse(x, y);
            ShowBoard();

            // 裏返すことが出来た場合、プレイヤー指定箇所がEMPTY以外になっている
            if (board[x, y] == currentPlayer) {
                currentPlayer = (currentPlayer == SQUARE_STATUS.BLACK ? SQUARE_STATUS.WHITE : SQUARE_STATUS.BLACK);

                if (CheckPass()) {
                    currentPlayer = (currentPlayer == SQUARE_STATUS.BLACK ? SQUARE_STATUS.WHITE : SQUARE_STATUS.BLACK);
                    // 両プレイヤーが続けてパスをした場合、ゲーム終了
                    if (CheckPass()) {
                        isGameSet = true;
                    }
                }

                // AIターン
                if (currentPlayer == SQUARE_STATUS.WHITE) {
                    StartCoroutine("InferenceAI");
                }
                CountPiece();
            }
        }
    }

    IEnumerator InferenceAI()
    {
        yield return new WaitForSeconds(1);

        // AIによる盤面推論
        int[] inferenceResults = new int[(WIDTH * HEIGHT)];
        inferenceResults = InferenceReverseAI();
        // 石を置ける個所を確認して置く
        string debugText = "";
        debugText = debugText + "Player : "  + ((int)currentPlayer).ToString() + "\n";
        for (int i = 0; i < inferenceResults.Length; i++) { 
            int predictX = (int)(inferenceResults[i] % WIDTH) + 1;
            int predictY = (int)(inferenceResults[i] / WIDTH) + 1;
            debugText =  debugText + i.ToString() + ": X=" + predictX.ToString() + " Y=" + predictY.ToString() +"\n";
            
            if(SetPieceAI(inferenceResults[i])) {
                break;
            }
        }
        CountPiece();

        Debug.Log(debugText);

        yield break;
    }

    private bool SetPieceAI(int positionId)
    {
        int x = positionId % WIDTH;
        int y = positionId / WIDTH;

        int[] inferenceResults = new int[(WIDTH * HEIGHT)];
        
        if (board[x, y] == SQUARE_STATUS.EMPTY) {
            Reverse(x, y);
            ShowBoard();

            // 裏返すことが出来た場合、プレイヤー指定箇所がEMPTY以外になっている
            if (board[x, y] == currentPlayer) {
                currentPlayer = (currentPlayer == SQUARE_STATUS.BLACK ? SQUARE_STATUS.WHITE : SQUARE_STATUS.BLACK);

                if (CheckPass()) {
                    currentPlayer = (currentPlayer == SQUARE_STATUS.BLACK ? SQUARE_STATUS.WHITE : SQUARE_STATUS.BLACK);
                    // 両プレイヤーが続けてパスをした場合、ゲーム終了
                    if (CheckPass()) {
                        isGameSet = true;
                    }
                }

                // AIターン
                if (currentPlayer == SQUARE_STATUS.WHITE) {
                    StartCoroutine("InferenceAI");
                }
                CountPiece();
                return true;
            }
        }

        return false;
    }

    void Reverse(int x, int y)
    {
        int[] directionXArray = {-1, 0, 1};
        int[] directionYArray = {-1, 0, 1};
        foreach (int directionY in directionXArray) {
            foreach (int directionX in directionXArray) {
                if (!(directionX == 0 && directionY == 0)) {
                    Reverse_(x, y, directionX, directionY);
                }
            }
        }
    }

    void Reverse_(int x, int y, int directionX, int directionY)
    {
        int reverseCount = 0;
        int checkX = x + directionX; 
        int checkY = y + directionY;
        
        while (checkX < WIDTH && checkX >= 0 && checkY < HEIGHT && checkY >= 0) {
            if (board[checkX, checkY] == currentPlayer) {
                checkX -= directionX;
                checkY -= directionY;
                while (!(checkX == x && checkY == y)) {
                    board[checkX, checkY] = currentPlayer;
                    checkX -= directionX;
                    checkY -= directionY;

                    reverseCount++;
                }
                if (reverseCount > 0) {
                    board[x, y] = currentPlayer;
                }
                break;
            }
            else if (board[checkX, checkY] == SQUARE_STATUS.EMPTY) {
                break;
            }

            checkX += directionX;
            checkY += directionY;
        }
    }

    bool CheckPass()
    {
        for (int y = 0; y < HEIGHT; y++) {
            for (int x = 0; x < WIDTH; x++) {
                if (board[x, y] == SQUARE_STATUS.EMPTY) {
                    SQUARE_STATUS[,] tempBoard = new SQUARE_STATUS[WIDTH, HEIGHT]; 
                    Array.Copy(board, tempBoard, board.Length);

                    Reverse(x, y);

                    // 1つでも裏返すことが出来ていればパス不成立
                    if (board[x, y] == currentPlayer) {
                        board = tempBoard;
                        return false;
                    }
                }
            }
        }
        return true;
    }

    void CountPiece()
    {
        blackCount = 0;
        whiteCount = 0;
    
        for (int y = 0; y < HEIGHT; y++) {
            for (int x = 0; x < WIDTH; x++) {
                switch (board[x, y]) {
                    case SQUARE_STATUS.BLACK:
                        blackCount++;
                        break;
                    case SQUARE_STATUS.WHITE:
                        whiteCount++;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    int[] InferenceReverseAI()
    {
        float[] input = new float[(WIDTH * HEIGHT)];
        int[] resultIds = new int[(WIDTH * HEIGHT)];

        // モデル入力データ構築
        for (int y = 0; y < HEIGHT; y++) {
            for (int x = 0; x < WIDTH; x++) {
                int inputValue = ((int)board[x, y]);
                if (currentPlayer == SQUARE_STATUS.WHITE) {
                    switch(inputValue) {
                        case (int)(SQUARE_STATUS.BLACK):
                            inputValue = (int)(SQUARE_STATUS.WHITE);
                            break;
                        case (int)(SQUARE_STATUS.WHITE):
                            inputValue = (int)(SQUARE_STATUS.BLACK);
                            break;
                        default:
                            break;
                    }
                }
                input[(x + (y * WIDTH))] = (float)(inputValue);
            }
        }
        for (int i = 0; i < resultIds.Length; ++i) { resultIds[i] = i; }
        
        // 推論
        var scores = reversiAI.Inference(input);

        // スコア順にソート
        Array.Sort(scores, resultIds);
        Array.Reverse(scores);
        Array.Reverse(resultIds);

        return resultIds;
    }

    GameObject GetPiecePrefab(SQUARE_STATUS status)
    {
        GameObject prefab;
        switch (status) {
            case SQUARE_STATUS.EMPTY:
                prefab = Instantiate(square);
                break;
            case SQUARE_STATUS.BLACK:
                prefab = Instantiate(blackPiece);
                break;
            case SQUARE_STATUS.WHITE:
                prefab = Instantiate(whitePiece);
                break;
            default:
                prefab = null;
                break;
        }
        return prefab;
    }
}
