using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {
    
    void OnBecameInvisible() //Note: Takes into account every camera including the EDITOR view.. This shouldn't though cause any problems when this is built( even though it's weird in the dev process). For case of simplicity I used OnBecameInvisible
    {
        this.gameObject.SetActive(false);   
    }

}
