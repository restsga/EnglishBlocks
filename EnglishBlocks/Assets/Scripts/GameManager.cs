using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class States
{
    public int now = ERROR;
    public float timer = 0f;

    public const int
        ERROR = 0,
        FALLING = 1,
        FALL_ANIMATION = 2,
        CLEAN_ANIMATION = 3;

    public float fallInterval = 1f;
    public float fallAnimationInterval= 0.5f;
}

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

    // 実験用
    private const float GAMESPEED = 1f;
    

    // Prefabs //
    // ブロックのPrefabオブジェクト
    public GameObject blockPrefab;

    // GameDatas //
    // ブロックのデータ配列
    private GameObject[,] blocks;
    // 落下中のブロックの座標
    private FallingBlock[] fallingBlocks = { new FallingBlock(), new FallingBlock() };
    // 現在の状態(アニメーションなど)
    private States states = new States();

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
        states.timer += Time.deltaTime*GAMESPEED;

        switch (states.now)
        {
            case States.FALLING:
                if (states.timer >= states.fallInterval)
                {
                    states.timer = 0f;
                    FallControledBlock();
                }
                break;

            case States.FALL_ANIMATION:
                if (states.timer >= states.fallAnimationInterval)
                {
                    states.timer = 0f;
                    FreeFallAnimation();
                }
                break;

            case States.CLEAN_ANIMATION:
                CreateBlock();
                break;
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
        int[] createPos = { blocks.GetLength(0) / 2-1, blocks.GetLength(1)-2 };

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

            //落下状態に移行
            states.now = States.FALLING;
        }
    }

    // ブロックを落下させる
    private void FallControledBlock()
    {
        // 2つのブロックのどちらも接地条件を満たしていない
        if (blocks[fallingBlocks[0].x,fallingBlocks[0].y-1].GetComponent<BlockScript>().IsGrounded() == false&&
            blocks[fallingBlocks[1].x, fallingBlocks[1].y - 1].GetComponent<BlockScript>().IsGrounded() == false)
        {
            for (int i = 0; i < 2; i++)
            {
                // 落下
                Fall(fallingBlocks[i].x, fallingBlocks[i].y);
                // 座標を更新
                fallingBlocks[i].y--;
            }
        }
        // 接地条件を満たした
        else
        {
            states.now = States.FALL_ANIMATION;
        }
    }

    // 自由落下アニメーション
    private void FreeFallAnimation()
    {
        //落下フラグ
        bool fall=false;

        for(int x = 1; x < blocks.GetLength(0) - 1; x++)
        {
            for(int y = 1; y < blocks.GetLength(1) - 1; y++)
            {
                // 空のブロックでない
                if (blocks[x, y].GetComponent<BlockScript>().IsFill())
                {
                    // 下にあるブロックが接地されている
                    if(blocks[x, y - 1].GetComponent<BlockScript>().IsGrounded())
                    {
                        // 接地する
                        blocks[x, y].GetComponent<BlockScript>().SetGrounded();
                    }
                    // 下にあるブロックが接地されていない
                    else
                    {
                        // 落下
                        Fall(x, y);

                        fall = true;
                    }
                }
            }
        }

        // 落下したブロックが存在しない
        if (fall == false)
        {
            states.now = States.CLEAN_ANIMATION;
        }
    }

    // ブロックを1マス下に移動
    private void Fall(int x,int y)
    {
        // ブロックの種類を保存
        int letter_id = blocks[x, y].GetComponent<BlockScript>().GetAlphabet();
        // 現在の座標を空に
        blocks[x, y].GetComponent<BlockScript>().SetEmpty();
        // ひとつ下の座標を更新
        blocks[x, y - 1].GetComponent<BlockScript>().SetAlphabet(letter_id, false);
    }
}
