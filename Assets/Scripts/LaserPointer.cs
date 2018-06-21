using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Color col;

    private int radius = 1;

    public delegate void ClickAction2();
    public static event ClickAction2 setTexture;

    private Texture2D tex;

    //gets the index of the controller device
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // shows the laser from the controller to the hit point
    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hit.point, 0.5f);
        laserTransform.LookAt(hit.point);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }


    // gets the controller steam vr library script to track the controller
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    //set the variables for the laser
    void Start () {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        col = Color.black;
        //tex = new Texture2D(256, 256);
    }

    //updates the scene based on contrller interaction
    void Update() {
        //get the press from the touchpad
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //checks the axis to increment or decrement the radius
            Debug.Log("Press: " + Controller.GetAxis().x + " " + Controller.GetAxis().y);
            if ((Controller.GetAxis().x < 0) && radius > 1)
            {
                radius--;
                Debug.Log(radius + ": decremented");
            }
            else if (Controller.GetAxis().x > 0 && radius < 10)
            {
                radius++;
                Debug.Log(radius + ": incremented");
            } 
        }
        //gets the touch of the touchpad
        else if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //checks axis to change the color of the laser
            if (Controller.GetAxis().y > 0.6 || Controller.GetAxis().y < -0.6)
            {
                //Debug.Log("NoTouchy");
                RaycastHit hit;
                //Debug.Log("Touch: " + Controller.GetAxis().x + " " + Controller.GetAxis().y);
                //use raycasting to get the object hit
                if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
                {
                    if (hit.transform.name == "Plane")
                    {
                        //change the color fo the laser by the pixel of the hit point on the plane
                        tex = hit.transform.gameObject.GetComponent<Renderer>().material.mainTexture as Texture2D;
                        col = tex.GetPixel((int)(hit.textureCoord.x * tex.width), (int)(hit.textureCoord.y * tex.height));
                        laser.GetComponent<Renderer>().material.color = col;
                    }
                    else
                    {
                        //get the color of the material of the object
                        col = hit.transform.gameObject.GetComponent<Renderer>().material.color;
                        laser.GetComponent<Renderer>().material.color = col;
                    }
                    ShowLaser(hit);
                }
            }
        }
        //get the press of the grip
        else if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            //call the event settexture
            Debug.Log("GRIP");
            if (setTexture != null)
                setTexture();
        }
        //get the press of the trigger
        else if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            RaycastHit hit;

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 1000))
            {
                //if the tag of the object is paintblocks return
                Debug.Log("Hit: " + hit.transform.tag);
                if (hit.transform.tag == "PaintBlocks")
                    return;
        
                //call the public function from the hit objects changetexture
                Debug.Log(hit.transform.gameObject.name);
                hit.transform.gameObject.GetComponent<BlockColors>().changeTexture(hit.textureCoord,radius,col);
                ShowLaser(hit);
            }
        }
        else
        {
            laser.SetActive(false);
        }
    }
}
