using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class FallingBlock
{
    public int x;
    public int y;
    public int letter_id;
}

public class GameManager : MonoBehaviour
{
    // Constants //
    private readonly int[] BLOCK_COUNTS = { 8, 12 };
    private const float BLOCK_SIZE = 0.13f;
    

    // Prefabs //
    // ブロックのPrefabオブジェクト
    public GameObject blockPrefab;

    // GameDatas //
    // ブロックのデータ配列
    private GameObject[,] blocks;
    // 落下中のブロックの座標
    private FallingBlock[] fallingBlocks = { new FallingBlock(), new FallingBlock() };

    // プロトタイプ用の仮のタイマー
    private float timer = 0f;

    // Use this for initialization
    void Start()
    {
        // Sprite(アルファベットが描かれたブロックの画像)の読み込み
        BlockScript.LoadSprites();
        // フィールド全体のブロックを生成
        CreateSprites(BLOCK_COUNTS[0], BLOCK_COUNTS[1]);

        // 落下してくるブロックを生成
        CreateBlock();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer -= 1f;
            FallBlock();
        }
    }

    // フィールド全体のブロックを生成
    private void CreateSprites(int w, int h)
    {
        // 壁を生成するために値を+2(上下左右各+1)
        w += 2;
        h += 2;

        // ブロックのデータ配列の大きさを決定
        blocks = new GameObject[w, h];

        // ブロックを生成して配列に格納
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                blocks[x, y] = Instantiate(blockPrefab,
                    new Vector3((x + 0.5f - w / 2f) * BLOCK_SIZE, (y + 0.5f - h / 2f) * BLOCK_SIZE),
                    Quaternion.identity);
            }
        }

        // 左右の壁を生成
        for (int y = 0; y < h; y++)
        {
            blocks[0, y].GetComponent<BlockScript>().SetOutline(true);
            blocks[w - 1, y].GetComponent<BlockScript>().SetOutline(true);
        }
        // 床と見えない天井部分を生成
        for (int x = 0; x < w; x++)
        {
            blocks[x, 0].GetComponent<BlockScript>().SetOutline(true);
            blocks[x, h-1].GetComponent<BlockScript>().SetOutline(false);
        }
    }

    // 落下してくるブロックを生成
    private void CreateBlock()
    {
        // 落下してくるブロックが生成される場所
        int[] createPos = { BLOCK_COUNTS[0] / 2, BLOCK_COUNTS[1] };

        // 生成される場所が埋まっていない(ゲームオーバーではない)
        if (blocks[createPos[0], createPos[1]].GetComponent<BlockScript>().IsGrounded() == false)
        {
            // 落下中のブロックとして生成 //
            // 生成座標をコピー
            fallingBlocks[0].x = createPos[0];
            fallingBlocks[0].y = createPos[1];
            fallingBlocks[1].x = createPos[0];
            fallingBlocks[1].y = createPos[1] + 1;
            // アルファベットを決定
            fallingBlocks[0].letter_id = XOR128.Next(26);
            fallingBlocks[1].letter_id = XOR128.Next(26);

            // 落下中のブロックを表示
            blocks[fallingBlocks[0].x, fallingBlocks[0].y].
                GetComponent<BlockScript>().SetAlphabet(fallingBlocks[0].letter_id,false);
            blocks[fallingBlocks[1].x, fallingBlocks[1].y].
                GetComponent<BlockScript>().SetAlphabet(fallingBlocks[1].letter_id,false);
        }
    }

    // ブロックを落下させる
    private void FallBlock()
    {
        // 2つのブロックのどちらも接地条件を満たしていない
        if (blocks[fallingBlocks[0].x,fallingBlocks[0].y-1].GetComponent<BlockScript>().IsGrounded() == false&&
            blocks[fallingBlocks[1].x, fallingBlocks[1].y - 1].GetComponent<BlockScript>().IsGrounded() == false)
        {
            // 現在ブロックがある座標からブロックを削除
            blocks[fallingBlocks[0].x, fallingBlocks[0].y].GetComponent<BlockScript>().SetEmpty();
            blocks[fallingBlocks[1].x, fallingBlocks[1].y].GetComponent<BlockScript>().SetEmpty();
            // ブロックを移動(落下)
            fallingBlocks[0].y--;
            fallingBlocks[1].y--;
            // 新たな座標で表示
            blocks[fallingBlocks[0].x, fallingBlocks[0].y].
                GetComponent<BlockScript>().SetAlphabet(fallingBlocks[0].letter_id,false);
            blocks[fallingBlocks[1].x, fallingBlocks[1].y].
                GetComponent<BlockScript>().SetAlphabet(fallingBlocks[1].letter_id, false);
        }
    }
}
