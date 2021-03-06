using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
//using System.Diagnostics;
//using UnityEditor;
//using System.Security.Cryptography.X509Certificates;
//using System.ComponentModel;
using Anima2D;

public class fishMovement : MonoBehaviour {
	/*public class fishInMouth  
	{

		public fishMovement movement;

		public Rigidbody2D rb;

		public fishInMouth ( fishMovement m, Rigidbody2D r)
		{
			movement = m;
			rb = r;
		}

		public fishInMouth()
		{
		//	rb = GetComponent<Rigidbody2D> ();
		}

		public void turnOffMovement()
		{
			movement.enabled = false;
		}



	}*/

	private GameObject puffin;
	public float flightZoneMax = 15f;
	private float escapeZoneMax = 10f;
	public float escapeSpeed= 7f;

	public float swimSpeed = 5f;

	private Vector2 puffinToFish;
	public Rigidbody2D rb2d; // this should make it so I can reference this from my puffin control script
	//private Rigidbody2D puffinrb2d;
	private Vector2 startPosition;
	private Vector2 direction;
	private bool fishInWater;
	private bool fishInAir;
	private float  puffinApproachAngle;
	public float collisionTimer = 0f;
	private Vector2 collisionNormal1;
	private Vector2 collisionNormal2;
	private Vector2 collisionPosition1;
	//private Vector2 collisionPosition2;
	private SpriteRenderer spriteRenderer;
	public SpriteMeshInstance spriteMesh;
	private Animator anim;
	//public fishInMouth fishinmouth;

	private bool inPuffinRange = false;

	public int pointValue = 1;

	//private float puffintoFishRelativeVelocityDir;


	public enum FishMode
	{
		swim,
		swimDown,
		escape,
		escape2, //escape after collision
		stuck,// when puffin has fish cornered
		caught,
		still,
	}

	private FishMode fishState = FishMode.swim;


	void Start ()
	{
		
		//fishinmouth = new fishInMouth (this, rb2d);
		anim = GetComponent<Animator> ();
		puffin = ClickManager.instance.puffin;   //puffin = GameObject.Find ("Puffin");
		//puffinrb2d = puffin.GetComponent<Rigidbody2D>();
		rb2d = GetComponent<Rigidbody2D> ();
		startPosition = rb2d.position;
		direction = RandomVector2 ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		//GameManager.instance.Fish.Add (this.gameObject);//this adds all Fish to a list in the GameManager to keep track of them
		// i Moved this call to a delegate in the FishTracker Script. I want the Fish to be added even if in puffin mouth.
		
	}


	
	// Update is called once per frame
	void Update ()
	{ 
		//Flip (); /.this is now handled in rotate
		Rotate ();

		// check to see if puffin is close enough to Open his mouth, if so set bool true

		if (!inPuffinRange && puffinToFish.sqrMagnitude < GameManager.instance.puffinToMouthRange * GameManager.instance.puffinToMouthRange)
		{

			// if the fish is in the range of the puffin, add one to the count // to  make mouth open
			GameManager.instance.fishCloseToPuffin = GameManager.instance.fishCloseToPuffin + 1;
			//print (GameManager.instance.fishCloseToPuffin + " Fish in Range" + puffinToFish.sqrMagnitude + " is the closeness. Puffin to Fish is " + puffinToFish + " It is less than Closeness criteria " + GameManager.instance.puffinToMouthRange * GameManager.instance.puffinToMouthRange);
			inPuffinRange = true;
		}

		if (inPuffinRange && puffinToFish.sqrMagnitude > GameManager.instance.puffinToMouthRange * GameManager.instance.puffinToMouthRange)
		{
			// if the fish moves out ofthe range of the puffin, subtract one from the count if count is greater than 0// to  make mouth close
			GameManager.instance.fishCloseToPuffin = GameManager.instance.fishCloseToPuffin > 0 ? GameManager.instance.fishCloseToPuffin - 1: GameManager.instance.fishCloseToPuffin;
			//GameManager.instance.fishCloseToPuffin = GameManager.instance.fishCloseToPuffin - 1;
			inPuffinRange = false;
		}


	

		//countdown timer in seconds. Greater  than 0, counts down to o.
		collisionTimer = collisionTimer > 0 ? collisionTimer- Time.deltaTime : 0f;//this countdowns from fish last hit something

		if((rb2d.position-collisionPosition1).sqrMagnitude > escapeZoneMax*escapeZoneMax) // checks if the fish is farther than the escape xone and if so, reset timer
		{
			collisionTimer = 0f;
		}

		if (puffin != null) // this is so there isn't issues when the puffin is destroyed sometimes
		{
			puffinToFish = (transform.position - puffin.transform.position);
		} else
		{
			puffinToFish = Vector2.one*Mathf.Infinity;
		}
		//distance from fish to puffin
		puffinApproachAngle = Mathf.Atan2 (puffinToFish.y, puffinToFish.x) * Mathf.Rad2Deg;
	
		//if(puffinApproachAngle > 180
		//Vector2.Angle(puffin.transform.position, transform.position);; //the angle between puffin and fish

		//puffintoFishRelativeVelocityDir = Vector2.Angle (puffinrb2d.velocity, rb2d.velocity);// difference in direction from puffin to fish velocity



		if (fishInWater && !fishInAir && fishState != FishMode.swim && fishState !=  FishMode.swimDown && puffinToFish.sqrMagnitude > flightZoneMax*flightZoneMax)
		{
			fishState = setFishState(FishMode.swim);
		}
		if (fishInAir && !fishInWater && fishState != FishMode.still)
		{
			fishState = setFishState(FishMode.still);

		}

		if (fishInAir && fishInWater&& fishState != FishMode.swimDown)
		{
			fishState = setFishState(FishMode.swimDown);
			//fishState = setFishState(FishMode.escape2);
		}


		//print (puffinToFish.magnitude);
		if (puffinToFish.sqrMagnitude < flightZoneMax*flightZoneMax && fishState != FishMode.escape && fishInWater && fishState!= FishMode.escape2 && fishState !=FishMode.stuck) 
		{
			if (collisionTimer != 0f )//&& (puffinApproachAngle > 15f || puffinApproachAngle < -15f)
			{
				fishState = setFishState (FishMode.escape2);
				
			} else
			{
				fishState = setFishState (FishMode.escape);
			}
			//print ("Set Escape");
			//print (puffinApproachAngle);




		}



		switch (fishState)
		{
		case FishMode.swim:
			rb2d.velocity = direction * swimSpeed;
			if ((rb2d.position - startPosition).sqrMagnitude > 20*20)
			{
				//Move in new random direction
				direction = RandomVector2 ();
				startPosition = rb2d.position;
			}
			break;

		case FishMode.swimDown:	
			rb2d.velocity = direction * swimSpeed;
			if ((rb2d.position - startPosition).sqrMagnitude > 10*10)
			{
				fishState = setFishState (FishMode.swim);
			}
			break;

		case FishMode.escape:

			rb2d.velocity = direction * escapeSpeed;
			
			//how to script if the puffin's approach changes too much, to change directions
			//Vector2.
			if (collisionTimer != 0f)
			{
				setFishState (FishMode.escape2);
			}
			else if ( puffinApproachAngle > 30f || puffinApproachAngle <-30f)//change direction based on fish angle
			{
				//print (puffinApproachAngle);
				direction = EscapeVector2 ();
			}


			 //only changes if 5 seconds passed
			{
				//print (puffinApproachAngle);

			}
			//rb2d.AddForce (puffinToFish.normalized * escapeForce);
			break;

		case FishMode.escape2:
			rb2d.velocity = direction * escapeSpeed;//*1.5f;

			if (puffinApproachAngle > 30f || puffinApproachAngle < -30f)
			{
				direction = EscapeVector2AndWall ();
			}

			break;

		case FishMode.stuck:
			rb2d.velocity = direction * escapeSpeed;// * 1.5f;

			if (puffinApproachAngle > 45f || puffinApproachAngle < -45f)
			{
				direction = CornerEscape ();
			}

			if ((rb2d.position - startPosition).sqrMagnitude > 10 * 10)
			{
				setFishState (FishMode.escape);
			}

			break;


		case FishMode.still:
			break;

		case FishMode.caught:
			break;

		default:
			break;

		
		}




	}

	//returns a random unit vector
	public Vector2 RandomVector2()
	{
		Vector2 Rand = new Vector2 (UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
		return Rand.normalized;
	}

	public Vector2 DownVector2()
	{
		Vector2 Rand = new Vector2 (UnityEngine.Random.Range(-.5f, .5f), UnityEngine.Random.Range(-1.0f, -.5f));
		return Rand.normalized;
	}

	public Vector2 EscapeVector2()
	{
		Vector2 baseDir = puffinToFish.normalized;
		Vector2 Rand = new Vector2 (UnityEngine.Random.Range(baseDir.x -.5f,baseDir.x + .5f), UnityEngine.Random.Range(baseDir.y -.5f,baseDir.y + .5f));
		return Rand.normalized;
	}

	public Vector2 EscapeVector2AndWall()// This takes into account the location of the puffin after colliding with wall to escape
	{
		Vector2 baseDir = -collisionNormal1;
		Vector2 Rand = puffinToFish.normalized - baseDir;
		//Vector2 Rand = new Vector2 (UnityEngine.Random.Range(baseDir.x -.5f,baseDir.x + .5f), UnityEngine.Random.Range(baseDir.y -.5f,baseDir.y + .5f));


		//trying something new
		//Rand = (collisionPosition1- puffin.transform.position).


		//Debug.DrawRay (rb2d.transform.position, -Rand*4f, Color.white, 1f);
		Debug.DrawLine (rb2d.position, rb2d.position + (Rand * 4f), Color.yellow, .1f);
		Debug.DrawLine (rb2d.position, puffin.transform.position, Color.green, .1f);
		Debug.DrawLine (rb2d.position, collisionPosition1, Color.red, .1f);
		return Rand.normalized;
	}

	public Vector2 EscapeVector2Angle()
	{
		float angle = puffinApproachAngle;
		angle = angle + UnityEngine.Random.Range (-15f, 15f);
		Vector2 Rand = new Vector2();
		Rand.y = Mathf.Sin(angle) * puffinToFish.magnitude;
		Rand.x = Mathf.Cos(angle) * puffinToFish.magnitude;
		return Rand.normalized;
	}

	public Vector2 CornerEscape()
	{
		Vector2 baseDir = -collisionNormal1 -collisionNormal2;
		Vector2 Rand = puffinToFish.normalized - baseDir;
		Debug.DrawRay (rb2d.transform.position, Rand*4f, Color.white, 1f);
		return Rand.normalized;
	}

	// This function handles the transitions to make sure something only happens once.
	public FishMode setFishState( FishMode newState)
	{
		switch (newState)
		{
		case FishMode.swim:
			startPosition = rb2d.position;
			direction = RandomVector2 ();
			break;

		case FishMode.swimDown:	
			startPosition = rb2d.position;
			direction = DownVector2 ();
			break;

		case FishMode.escape:
			startPosition = rb2d.position;
			//rb2d.velocity = Vector2.zero;
			direction = EscapeVector2 ();
			//rb2d.AddForce (direction * escapeForce);
			break;

		case FishMode.escape2: // this is called when a fish has collided in the last 5 sends and puffin is pursuing
			
			startPosition = rb2d.position;
			//rb2d.velocity = Vector2.zero;
			direction = EscapeVector2AndWall ();
			break;


		case FishMode.stuck:
			startPosition = rb2d.position;
			direction = CornerEscape ();	

			break;

		case FishMode.still:
			//rb2d.velocity = Vector2.zero;

			break;

		case FishMode.caught:
			break;

		default:
			break;
		}
		//print ("The Fish's state is now " + newState + " Collision timer is " + collisionTimer);
		return newState;

	}

	public void Flip ()
	{
		if (direction.x < -.3f)
		{
			spriteRenderer.flipX = true;
		}
		else if (direction.x > .3f)
		{
			spriteRenderer.flipX = false;
		}
	}

	public void Rotate () // might be unneccessary and expensive
	{

		float angle = Mathf.Atan2 (rb2d.velocity.y, rb2d.velocity.x) * Mathf.Rad2Deg;
		rb2d.MoveRotation (Mathf.LerpAngle (rb2d.rotation, angle, 20f * Time.deltaTime));


		
		if (rb2d.rotation < 0)
		{
			rb2d.rotation = rb2d.rotation + 360;
		} else if (rb2d.rotation > 360)
		{
			rb2d.rotation = rb2d.rotation - 360;
		}
		if (Mathf.Abs(rb2d.rotation) > 90 && Mathf.Abs(rb2d.rotation) < 270)
		{
			Vector3 localScale = rb2d.transform.localScale;
			localScale.y = Mathf.Abs(localScale.y)* -1f; //return the negative scale
			rb2d.transform.localScale = localScale;

			//rb2d.transform.localScale = new Vector3 (1f, -1f, 1f);
			//spriteRenderer.flipY = true;
		} else if (Mathf.Abs(rb2d.rotation) < 90 || Mathf.Abs(rb2d.rotation) > 270)
		{
			Vector3 localScale = rb2d.transform.localScale;
			localScale.y = Mathf.Abs(localScale.y); //return the positive scale
			rb2d.transform.localScale = localScale;

			//rb2d.transform.localScale = new Vector3 (1f, 1f, 1f);
			//spriteRenderer.flipY = false;
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		//fishState = setFishState(FishMode.swim);
		//direction = -direction;


		if (!other.gameObject.CompareTag ("pickup")) //check // other.gameObject.CompareTag ("fishBoundary")
		{
			startPosition = rb2d.position;
			//print ("collision");
			if (collisionTimer > 4f)
			{
				//print ("Stuck in corner");
				fishState = setFishState (FishMode.stuck);
			}
			collisionTimer = 5f;
		}

		foreach (ContactPoint2D contact in other.contacts)
		{
			//collisionPosition2 = collisionPosition1;
			collisionPosition1 = contact.point;
			collisionNormal2 = collisionNormal1;
			collisionNormal1 = contact.normal;
			if (fishState != FishMode.escape && fishState != FishMode.escape2 && fishState != FishMode.stuck) //check
			{
				direction = collisionNormal1;
				print ("Fish is not stuck, or escaping");
			}
			Debug.DrawRay (contact.point, contact.normal, Color.magenta, 2f);
			// Do something with normal here
		}

	}

	

		//this handles ths bools that show fish location
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "Water")
		{
			fishInWater = true;

		}
		if (other.name == "Air")
		{
			fishInAir= true;
			rb2d.velocity = Vector2.zero;

		}

		

	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.name == "Water")
		{
			fishInWater = false;
		}

		if (other.name == "Air")
		{
			fishInAir = false;

		}

	}

	void OnDisable()
	{
		anim.SetTrigger ("Ate");

		spriteMesh.sortingOrder = GameManager.instance.score + 2;
		gameObject.tag = "deadFish";
		//consider deactivation the collider as well
	}
	void OnEnable()
	{
		if (anim != null)
		{
			anim.SetTrigger ("Swim");
			StartCoroutine ("reTagFish");
		}
	}

	IEnumerator reTagFish ()
	{
		print ("tag Fish called");
		print ("inPuffin range is "+ inPuffinRange);
		/*if (!inPuffinRange)
		{
			gameObject.tag = "pickup";
			yield break;
		}*/
		while (inPuffinRange)
		{
			print ("in Puffin Range");
			yield return new WaitForFixedUpdate();

		}
		if (!inPuffinRange)
		{
			gameObject.tag = "pickup";
			print ("changed tag" + gameObject.tag.ToString());
			print (fishState);
			yield break;
		}
	}

}
