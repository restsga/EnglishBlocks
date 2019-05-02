using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour {

    // Constants //
    static public Sprite[] sprites;
    private readonly string[] ALPHABETS ={
        "a","b","c","d","e","f","g","h","i","j","k","l","m",
        "n","o","p","q","r","s","t","u","v","w","x","y","z"
    };
    private const int EMPTY = -1;
    private bool UPDATE = true;
  
    // Datas //
    private bool grounded = false;
    private int letter_id = EMPTY;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    static public void LoadSprites()
    {
        sprites = Resources.LoadAll<Sprite>("a-z");
    }

    public void SetAlphabet(int alphabet_id, bool grounded)
    {
        letter_id = alphabet_id;
        this.grounded = grounded;

        if (UPDATE)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[alphabet_id];
        }
    }

    public int GetAlphabet()
    {
        return letter_id;
    }

    public void SetEmpty()
    {
        letter_id = EMPTY;
        grounded = false;
        GetComponent<SpriteRenderer>().sprite = null;
    }

    public bool IsFill()
    {
        if (letter_id != EMPTY)
        {
            return true;
        }
        return false;
    }

    public void SetGrounded()
    {
        grounded = true;
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    public void SetOutline(bool exist)
    {
        grounded = exist;
        UPDATE = false;
        if (exist)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[26];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = null;
        }
    }
}
