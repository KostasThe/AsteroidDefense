using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDrag : MonoBehaviour {
    public Rigidbody2D m_Rigidbody;

    private Vector2 force;
    private Vector2 mouseInitLoc;
    private Vector2 mouseCurrLoc;
    private float startTime;
    private float endTime;

    // Use this for initialization
    void Awake () {
        m_Rigidbody = this.transform.GetComponent<Rigidbody2D>();
	}
	
    void OnMouseDown()
    {
        mouseInitLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startTime = Time.time;
    }
    private void Update()
    {
        Debug.Log(m_Rigidbody.velocity);
    }
    void OnMouseDrag()
    {
        mouseCurrLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //this.transform.position = mouseCurrLoc;
        //force = mouseCurrLoc - mouseInitLoc;
        //mouseInitLoc = mouseCurrLoc;
    }

    void OnMouseUp()
    {
        endTime = Time.time;

        float duration = endTime - startTime;

       // mouseCurrLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition); // not needed?

        Vector2 dir = mouseCurrLoc - mouseInitLoc;
        float distance = dir.magnitude;
        float power = distance / duration;

        //dir.Normalize();

        m_Rigidbody.velocity = dir.normalized * power;
        
    }
}
