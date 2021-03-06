using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SocialPlatforms;
//using Mono.Cecil.Cil;
//using UnityEditor;

public class EnemyFlyer : MonoBehaviour 
{

	//private CircleCollider2D enemyZone; //parented circle collider that determines approach distance
	private Rigidbody2D rb2d; 
	private GameObject puffin;

	private Vector2 direction;
	private Vector2 startPosition;
	public float patrolSpeed = 5/4f;
	public float pursuitSpeed = 15/4f;
	public float pursuitRange = 20/4f;
	public float maxHeight = 10f;

	private Vector2 enemyToPuffin;
	private float idleTimer = 10f;

	private bool enemyInWater;
	private bool enemyInAir = true;
	private bool changeDir = false;

	private SpriteRenderer spriteRenderer;

	// the enemy checks if puffin is in radius. If so pursue. If out of radius for a certain time

	public enum airEnemyState
	{
		idle,
		patrol,
		pursue,
		goHome,
		waterSurface,
	}
	public airEnemyState state = airEnemyState.patrol;


	// Use this for initialization
	void Start () 
	{
		rb2d = GetComponent<Rigidbody2D> ();	
		//enemyZone = GetComponentInChildren<CircleCollider2D>();
		startPosition = transform.position;
		direction = patrolDirection ();//new Vector2(1f,0f);
		spriteRenderer = GetComponent<SpriteRenderer> ();

		puffin = GameObject.Find ("Puffin");
	}
	
	// Update is called once per frame
	void Update () 
	{
		idleTimer = (idleTimer > 0 && (Vector2)transform.position != startPosition)? idleTimer - Time.deltaTime : 0f;
		enemyToPuffin = (puffin.transform.position - transform.position);

		flipSprite ();
		if (enemyToPuffin.sqrMagnitude < pursuitRange * pursuitRange &&enemyInAir && GameManager.instance.score>0)
		{
			
			state = setEnemyState (airEnemyState.pursue);
			idleTimer = 10f;
			//print ("pursue");
			//print (enemyToPuffin.sqrMagnitude);
		}


		if (idleTimer == 0f && ((Vector2)transform.position - startPosition).sqrMagnitude > 5f)			
		{


			//state = setEnemyState (airEnemyState.goHome);
		}

		/*if (enemyInWater)
		{
			state = setEnemyState (airEnemyState.idle);

		}*/
		switch (state)
		{
		case airEnemyState.idle:
			rb2d.velocity = Vector2.zero;
			if (idleTimer == 0 && ((Vector2)transform.position - startPosition).sqrMagnitude > 5f*5f)
			{
				state = setEnemyState (airEnemyState.goHome);
			}
			break;

		case airEnemyState.patrol:
			rb2d.velocity = direction * patrolSpeed;
			if ((((Vector2)transform.position - startPosition).sqrMagnitude > 10f * 10f) || transform.position.y >= maxHeight)// && changeDir == false)
			{
				state = setEnemyState (airEnemyState.patrol);

				//direction = patrolDirection ();
				//direction = -direction;
				//print ("Switch");
				//changeDir = true;
				//startPosition = rb2d.position;
			}
			if (enemyInWater)
			{
				state = setEnemyState (airEnemyState.goHome);
				//direction.y = 1;
			}
			/*if (((Vector2)transform.position - startPosition).sqrMagnitude < 10f * 10f)
			{
				changeDir = false;
			} */
			break;

		case airEnemyState.pursue: // right now it is normalized. Try to think of another way to get direction, this is expensive?
			direction = enemyToPuffin.normalized;
			rb2d.velocity = direction * pursuitSpeed;
			idleTimer = 5f;
			if (enemyToPuffin.sqrMagnitude > pursuitRange * pursuitRange || GameManager.instance.score== 0)
			{
				state = setEnemyState (airEnemyState.idle);
			}

			if (enemyInWater)
			{
				state = setEnemyState (airEnemyState.waterSurface);
			}
			break;

		case airEnemyState.goHome:
			direction = (startPosition - (Vector2)transform.position).normalized;
			rb2d.velocity = direction*pursuitSpeed;
			if (((Vector2)transform.position - startPosition).sqrMagnitude <1f)//((Vector2)transform.position - startPosition).sqrMagnitude <5f) //(Vector2)transform.position == startPosition   
			{
				state = setEnemyState (airEnemyState.patrol);
			}
			break;


		case airEnemyState.waterSurface:
			if (enemyToPuffin.sqrMagnitude < pursuitRange * pursuitRange && enemyToPuffin.y > 0 && GameManager.instance.score>0)
			{
				state = setEnemyState (airEnemyState.pursue);
			}
			else if(idleTimer ==0f)
			{
				state = setEnemyState (airEnemyState.goHome);
			}
			break;	
		}
	}


	public airEnemyState setEnemyState ( airEnemyState newState)
	{
		switch (newState)
		{
		case airEnemyState.idle:
			//rb2d.gravityScale = 0f;
			rb2d.velocity = Vector2.zero;
			break;

		case airEnemyState.patrol:
			startPosition = rb2d.position;
			direction = patrolDirection ();
			rb2d.velocity = direction * patrolSpeed;
			break;

		case airEnemyState.pursue:
			idleTimer = 10f;
			break;

		case airEnemyState.goHome:
			break;

		case airEnemyState.waterSurface:
			//rb2d.gravityScale = .5f;
			break;	

		}


		return newState;
	}

	public Vector2 patrolDirection()
	{
		Vector2 Rand = new Vector2(0f,0f);
	
		if (transform.position.y >= maxHeight)
		{
			Rand = new Vector2 (UnityEngine.Random.Range (-1.0f, 1.0f), UnityEngine.Random.Range (-1.0f, 0f));
		} else
		{ 
			Rand = new Vector2 (UnityEngine.Random.Range (-1.0f, 1.0f), UnityEngine.Random.Range (-1.0f, 1.0f));
		}
		return Rand.normalized;
	}


	void OnCollisionEnter2D(Collision2D other)
	{
		//fishState = setFishState(FishMode.swim);
		//direction = -direction;


		if (!other.gameObject.CompareTag ("pickup")) //check // other.gameObject.CompareTag ("fishBoundary")
		{
			startPosition = rb2d.position;

		}

		foreach (ContactPoint2D contact in other.contacts)
		{

			if (state != airEnemyState.pursue) //check
			{
				direction = contact.normal;

			}

		}

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		/*
		if (other.gameObject.CompareTag("Player") && enemyInAir)
		{
			print ("Pursue!");
			state = setEnemyState (airEnemyState.pursue);
			idleTimer = 10f;

		}*/

		if (other.name == "Water")
		{
			enemyInWater = true;

		}
		if (other.name == "Air")
		{
			enemyInAir = true;

		}

	}
	void OnTriggerExit2D(Collider2D other)
	{
		/*if (other.gameObject.CompareTag("Player") )
		{
			state = setEnemyState (airEnemyState.idle);

		}*/

		if (other.name == "Water")
		{
			enemyInWater = false;
		}

		if (other.name == "Air")
		{
			enemyInAir = false;

		}
	}

	public void flipSprite()
	{
		if (state == airEnemyState.pursue)
		{
			if (enemyToPuffin.x < 0)
			{
				spriteRenderer.flipX = true;
			} else if (enemyToPuffin.x > 0)
			{
				spriteRenderer.flipX = false;
			}
		} else if (direction.x < 0)
		{
			spriteRenderer.flipX = true;
		}else if (direction.x > 0)
		{
			spriteRenderer.flipX = false;
		}


	}
}
