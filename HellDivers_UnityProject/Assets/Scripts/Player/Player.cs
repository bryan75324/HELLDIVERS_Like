using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StratagemController))]
[RequireComponent(typeof(Weapon_Battle))]
[RequireComponent(typeof(PlayerAnimationsContorller))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CameraController))]
public class Player : MonoBehaviour
{
    public PlayerInfo Info { get; set; }
    public PlayerParts Parts { get; set; }
    public GameObject Display { get; private set; }
    public Animator Anima { get; private set; }

    private void Awake()
    {
        this.tag = "Player";

        //Display = Resources.Load("Characters/Ch00/ch01") as GameObject;
        //Display = Instantiate(Display, this.transform);
        //Display.transform.position -= Vector3.up;

        //if (Display != null)
        //{
            Anima = Display.GetComponent<Animator>();
            Parts = Display.GetComponent<PlayerParts>();

            Camera.main.GetComponent<CameraController>().m_Player = Display.transform;
        //}
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}