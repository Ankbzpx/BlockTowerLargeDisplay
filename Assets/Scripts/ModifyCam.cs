using UnityEngine;
using System.Collections;

public class ModifyCam : MonoBehaviour {
    public float speed = 3f;
    public float heightDifference = 2.5f;
	float defaultHeight = 12f;

    void Start()
    {
        defaultHeight = gameObject.transform.position.y;
        transform.position = new Vector3(0f, defaultHeight, 0f);
    }
	
	// Update is called once per frame
    //modify the camera height based on the maximum height of the cube
	void FixedUpdate () {
        float _maxH = GameControl.MaximunHeight();

        //if (_maxH > 3.5f)
        //{
            Vector3 _delH = new Vector3(0f, _maxH + heightDifference, 0f);
            transform.position = Vector3.Lerp(transform.position, _delH, 1);
        //}
        //else
        //{
        //    transform.position = new Vector3(0f, _maxH, 0f);
        //}
    }
}