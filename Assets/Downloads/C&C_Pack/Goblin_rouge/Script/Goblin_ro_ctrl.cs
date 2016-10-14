using UnityEngine;
using System.Collections;

public class Goblin_ro_ctrl : MonoBehaviour {
	
	
	private Animator anim;
	private CharacterController controller;
	private bool battle_state;
	public float speed = 6.0f;
	public float runSpeed = 1.7f;
	public float turnSpeed = 60.0f;
	public float gravity = 20.0f;
	private Vector3 moveDirection = Vector3.zero;
	
	
	
	// Use this for initialization
	void Start () {
		
		anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController> ();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		
		if (Input.GetKey("2")) //battle_idle
		{
			anim.SetInteger("battle", 1);
			battle_state = true;
			
		}
		if (Input.GetKey("1")) 			//idle
		{
			anim.SetInteger("battle", 0);
			battle_state = false;
		}
		if (Input.GetKey ("up")) {		 //moving
			if (battle_state == false)
			{
				anim.SetInteger ("moving", 1);//walk
				runSpeed = 1.0f;
			} else 
			{
				anim.SetInteger ("moving", 2);//run
				runSpeed = 2.6f;
			}
			
			
		} else {
			anim.SetInteger ("moving", 0);
		}

				
		if (Input.GetKeyDown("m")) //defence_start
		{
			anim.SetInteger("moving",6);
		}


		
		if (Input.GetKeyDown("p")) // defence_start
		{
			anim.SetInteger("moving", 8);
		}
		if (Input.GetKeyUp("p")) // defence_end
		{
			anim.SetInteger("moving", 9);
		} 


		if (Input.GetMouseButtonDown (0)) //attack
		{
			anim.SetInteger("moving", 3);
		}
		if (Input.GetMouseButtonDown (1)) //alt attack1
		{
			anim.SetInteger("moving", 4);
		}
		if (Input.GetMouseButtonDown (2)) //alt attack2
		{
			anim.SetInteger("moving", 5);
		}
		
		if (Input.GetKeyDown("space")) //jump
		{
			anim.SetInteger("moving", 7);
		}
		
		
		if (Input.GetKeyDown("o")) //die_1
		{
			anim.SetInteger("moving", 12);
		}
		if (Input.GetKeyDown("i")) //die_2
		{
			anim.SetInteger("moving", 13);
		}
		
		if (Input.GetKeyDown("u")) //defence
		{
			int n = Random.Range(0,2);
			if (n == 0) {
				anim.SetInteger("moving", 10);
			} else {anim.SetInteger("moving", 11);}
		}
		
		
		
		
		if(controller.isGrounded)
		{
			moveDirection=transform.forward * Input.GetAxis ("Vertical") * speed * runSpeed;
			
		}
		float turn = Input.GetAxis("Horizontal");
		transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
		controller.Move(moveDirection * Time.deltaTime);
		moveDirection.y -= gravity * Time.deltaTime;
		
	}
}

