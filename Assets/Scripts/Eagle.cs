using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Enemy
{   public float patrolTime;
     public float patrolMoveTime;
 
     private float _remainingPatrolTime;
 
     private float _remainingPatrolMoveTime;
     private float _remainingDiveTime;
 
     // Start is called before the first frame update
     protected override void Awake()
     {
         base.Awake(); 
         _remainingPatrolTime = patrolTime;
         _remainingPatrolMoveTime = patrolMoveTime;
         _remainingDiveTime = patrolMoveTime / 2;
     }
     void Start()
     {
         
     }
 
     private void Patrol()
     {
         if (_remainingPatrolTime > 0)
         {
             _remainingPatrolTime -= Time.fixedDeltaTime;
             if (_remainingPatrolMoveTime > 0)
             {
                 _remainingPatrolMoveTime -= Time.fixedDeltaTime;
                 if (_remainingDiveTime > 0)
                 {
                     _remainingDiveTime -= Time.fixedDeltaTime;
                     _rigidBody2D.velocity = new Vector2(-1 * moveSpeed, -Mathf.Abs(moveSpeed));
                 }
                 else
                 { 
                     _rigidBody2D.velocity = new Vector2(-1 * moveSpeed,  Mathf.Abs(moveSpeed));
                 }
             }
             else
             {
                 _rigidBody2D.velocity = Vector2.zero;
             }
         }
         else
         {
             _spriteRenderer.flipX = !_spriteRenderer.flipX;
             _remainingPatrolTime += patrolTime;
             _remainingPatrolMoveTime += patrolMoveTime;
             _remainingDiveTime += patrolMoveTime / 2;
             moveSpeed *= -1f;
 
         }
         
     }
 
     // Update is called once per frame
     void FixedUpdate()
     {
        Patrol(); 
     }

}
