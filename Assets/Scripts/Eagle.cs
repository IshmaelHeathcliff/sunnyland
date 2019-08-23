using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Enemy
{   public float patrolTime;
     public float patrolMoveTime;
 
     private float _remainingPatrolTime;
 
     private float _remainingPatrolMoveTime;
 
     private static readonly int Fly = Animator.StringToHash("Fly");
 
     // Start is called before the first frame update
     protected override void Awake()
     {
         base.Awake(); 
         _remainingPatrolTime = patrolTime;
         _remainingPatrolMoveTime = patrolMoveTime;
     }
     void Start()
     {
         
     }
 
     private void Patrol()
     {
         if (_remainingPatrolTime > 0)
         {
             _animator.SetBool(Fly, true);
             _remainingPatrolTime -= Time.fixedDeltaTime;
             if (_remainingPatrolMoveTime > 0)
             {
                 _remainingPatrolMoveTime -= Time.fixedDeltaTime;
                 _rigidBody2D.velocity = Vector2.left * moveSpeed;
             }
             else
             {
                 _animator.SetBool(Fly, false);
                 _rigidBody2D.velocity = Vector2.zero;
             }
         }
         else
         {
             _spriteRenderer.flipX = !_spriteRenderer.flipX;
             _remainingPatrolTime += patrolTime;
             _remainingPatrolMoveTime += patrolMoveTime;
             moveSpeed *= -1f;
 
         }
         
     }
 
     // Update is called once per frame
     void FixedUpdate()
     {
        Patrol(); 
     }

}
