using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CockipScript : MonoBehaviour {

    public Camera MiniMapCamera;
    private Quaternion initialMiniMapRotation; 

    public Transform LevelBack;
    private Vector3 initialLevelPosition;
    private Vector3 initialLevelRotation; 
    
    public Image PowerBar;


    private HelicopterController helicopter;

	// Use this for initialization
	void Start () {

        helicopter = GetComponentInParent<HelicopterController>();
        initialMiniMapRotation = MiniMapCamera.transform.rotation;
        initialLevelPosition = LevelBack.transform.localPosition;
        initialLevelRotation = LevelBack.transform.localRotation.eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        MiniMapCamera.transform.rotation = initialMiniMapRotation;
        MiniMapCamera.transform.position = transform.position + new Vector3(0f, 10f, 0f);

        var pos = LevelBack.transform.localPosition;
        var rotX = helicopter.transform.rotation.x;
        LevelBack.transform.localPosition = new Vector3(pos.x, initialLevelPosition.y + rotX * 0.1f, pos.z);

        var rot = LevelBack.transform.localRotation.eulerAngles;
        var rotZ = helicopter.transform.rotation.z;
        LevelBack.transform.localRotation = Quaternion.Euler(rot.x, rot.y, initialLevelRotation.z + rotZ * 60f);

        PowerBar.fillAmount = Mathf.InverseLerp(0, helicopter.MaxEngineForce, helicopter.EngineForce);
    }
}
