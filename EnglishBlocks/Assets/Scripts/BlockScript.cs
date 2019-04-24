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
    private const string EMPTY = ",";
    private bool UPDATE = true;
  
    // Datas //
    private bool grounded = false;
    private string letter = EMPTY;

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
        letter = ALPHABETS[alphabet_id];
        this.grounded = grounded;

        if (UPDATE)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[alphabet_id];
        }
    }

    public void SetEmpty()
    {
        letter = EMPTY;
        GetComponent<SpriteRenderer>().sprite = null;
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
