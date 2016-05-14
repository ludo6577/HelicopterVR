using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CockipScript : MonoBehaviour {

    public Transform LevelBack;
    public Image PowerBar;

    private Vector3 initialPosition;
    private Vector3 initialRotation;

    private HelicopterController helicopter;

	// Use this for initialization
	void Start () {
        helicopter = GetComponentInParent<HelicopterController>();
        initialPosition = LevelBack.transform.localPosition;
        initialRotation = LevelBack.transform.localRotation.eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        var pos = LevelBack.transform.localPosition;
        var rotX = helicopter.transform.rotation.x;
        LevelBack.transform.localPosition = new Vector3(pos.x, initialPosition.y + rotX * 0.1f, pos.z);

        var rot = LevelBack.transform.localRotation.eulerAngles;
        var rotZ = helicopter.transform.rotation.z;
        LevelBack.transform.localRotation = Quaternion.Euler(rot.x, rot.y, initialRotation.z + rotZ * 60f);

        PowerBar.fillAmount = Mathf.InverseLerp(0, helicopter.MaxEngineForce, helicopter.EngineForce);
    }
}
