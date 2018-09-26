using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwap : MonoBehaviour
{
    public Image image;

    public int spriteIndex;
    public List<Sprite> spriteList;
    
	public void SwapSprites()
    {
        this.spriteIndex++;

        if (this.spriteIndex >= this.spriteList.Count)
        {
            this.spriteIndex = 0;
        }

        this.image.sprite = this.spriteList[this.spriteIndex];
    }
}
