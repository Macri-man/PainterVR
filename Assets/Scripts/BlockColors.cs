using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColors : MonoBehaviour {

    private Texture2D tex;


    //sets the texture and then sets the reference to the gamobject texture to be the new texture
    void Start () {
        tex = new Texture2D(256, 256);
        this.gameObject.GetComponent<Renderer>().material.mainTexture = tex;
    }

    //Subscribe to the event texture
    void OnEnable()
    {
        LaserPointer.setTexture += resetTexture;
    }


    //unsubscribe to the event setTexture
    void OnDisable()
    {
        LaserPointer.setTexture -= resetTexture;
    }

    //Cahges the color of the texture of the walls by using a version of the bresenham circle function
    public void changeTexture(Vector2 texcoord,int radius, Color col)
    {
        Debug.Log("Change Texture: " + this.gameObject.name);
        int x, y, posx, negx, posy, negy, decision;
        //Gets the cetnerx and cetnery position of the circle
        int centerx = (int)(texcoord.x * tex.width);
        int centery = (int)(texcoord.y * tex.height);
        Color32[] tempArray = tex.GetPixels32();
        for (x=0; x<=radius; x++)
        {
            decision = (int)Mathf.Ceil(Mathf.Sqrt(radius * radius - x * x));
            for (y=0;y<= decision; y++)
            {
                posx = centerx + x;
                negx = centerx - x;
                posy = centery + y;
                negy = centery - y;
                tempArray[posy*tex.width + posx] = col;
                tempArray[posy*tex.width + negx] = col;
                tempArray[negy*tex.width + posx] = col;
                tempArray[negy*tex.width + negx] = col;
            }
        }
        //sets and applies the new pixels to the texture
        tex.SetPixels32(tempArray);
        tex.Apply();
    }

    //resets the texture to gray by looping through and setting the pixel value
    void resetTexture()
    {
        Debug.Log("resetTexture");
        Color32[] tempArray = tex.GetPixels32();
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                tempArray[y * tex.width + x] = Color.gray;
            }
        }
        tex.SetPixels32(tempArray);
        tex.Apply();
    }

}
