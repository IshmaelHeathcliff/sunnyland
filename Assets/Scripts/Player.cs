using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    // ==========Jump==========
    public LayerMask ground; // physical operation layer
    public LayerMask ladderTop;
    public LayerMask ladder;
    public Vector2 groundedCheckSize = new Vector2(0.3f, 0.1f);
    public float thrust = 13f; // jump force
    public float jumpTime = 0.2f; // the time the jump force lasts
    
    private Vector2 _groundedCheckPoint ;
    private float _remainingJumpTime;
    private bool _isJumping; // Does the jump force exist
    private bool _isGrounded = true; // Is player grounded

    // ==========Move==========
    public float moveSpeed = 4.0f;
    private bool _canMove = true;
    private Rigidbody2D _rigidBody2D;
    private CircleCollider2D _circleCollider2D;
    private SpriteRenderer _spriteRenderer;
    
    // ==========Crouch==========
    private bool _isCrouching; 
    
    // ==========Climb==========
    private bool _isClimbing;
    private float _climbingX;
    
    // ==========HP===========
    public float maxHp = 100;
    public float HpChangeTime;
    private float _hp;
    private Slider _hpBar;
    
    private bool _isInvincible;
    
    // ==========Hurt==========
    public float hurtTime = 1f;
    public float drawbackTime = 0.5f;
    public float hurtFeedback = 5f; 
    private float _remainingHurtTime;
    private float _remainingDrawbackTime;
    private bool _isDrawback;
    private Vector2 _drawbackDirection;
    
    // ==========Animation==========
    private Animator _animator;


    // replace string names with hash codes
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int JumpDown = Animator.StringToHash("JumpDown");
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Crouch = Animator.StringToHash("Crouch");
    private static readonly int Climb1 = Animator.StringToHash("Climb");
    private static readonly int ClimbStop = Animator.StringToHash("ClimbStop");

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        // ==========Jump==========
        _remainingJumpTime = jumpTime;
        
        // ==========HP==========
        _hp = maxHp;
        _hpBar = GameObject.Find("HpBar").GetComponent<Slider>();
        
        
        // ==========Hurt==========
        _remainingDrawbackTime = drawbackTime;
        _remainingHurtTime = hurtTime;
    }
    // Start is called before the first frame update
    private void Start()
    {
        // ==========Jump==========
        Vector3 position = transform.position;
        _groundedCheckPoint = new Vector2(position.x, position.y - 0.5f);
        
    }

    private void HpChange()
    {
        if (_hp <= 0)
        {
            _hpBar.value = 0;
            GameManager.GameOver();
            this.enabled = false;
        }
        if (_hp < maxHp)
        {
            _hpBar.value = _hp / maxHp;
        }

        if (_hp >= maxHp)
        {
            _hp = maxHp;
            _hpBar.value = 1f;
        }
    }
    
    private void Drawback()
    {
        if (_remainingHurtTime > 0)
        {
            _remainingHurtTime -= Time.fixedDeltaTime;
            if (_remainingDrawbackTime > 0)
            {
                _remainingDrawbackTime -= Time.fixedDeltaTime;
                _rigidBody2D.velocity =  _drawbackDirection * hurtFeedback;
            }
            else
            {
                _rigidBody2D.velocity = Vector2.zero;
            }
        }
        else
        {
            _animator.SetBool(Hurt, false);
            _isDrawback = false;
            _canMove = true;
            _isJumping = false;
            _isInvincible = false;
            _remainingJumpTime = jumpTime;
            _remainingHurtTime += hurtTime;
            _remainingDrawbackTime += drawbackTime;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && !_isInvincible)
        {
            _animator.SetBool(Hurt, true);
            _animator.SetBool(Jump, false);
            _canMove = false;
            _isDrawback = true;
            _isInvincible = true;
            _hp -= other.gameObject.GetComponent<Enemy>().attackDamage;
            _drawbackDirection = new Vector2(transform.position.x < other.transform.position.x ? -1 : 1, 0);
        }

        if (other.gameObject.CompareTag("NextLevel"))
        {
            GameManager.GameOver();
            this.enabled = false;
        }

    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ladder"))
        {
            bool onLadderTop = Physics2D.OverlapBox(_groundedCheckPoint, groundedCheckSize, 0f, ladderTop);
            if (Input.GetKeyDown(KeyCode.DownArrow) && onLadderTop)
            {
                _climbingX = other.transform.position.x;
                _animator.SetBool(Climb1, true);
                _animator.SetBool(Jump, false);
                _rigidBody2D.isKinematic = true;
                _circleCollider2D.isTrigger = true;
                other.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
                _isClimbing = true;
                _isJumping = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Mask"))
        {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Cherry"))
        {
            other.gameObject.SetActive(false);
            _hp += other.gameObject.GetComponent<Food>().HpCure;
        }

        if (other.gameObject.CompareTag("Gem"))
        {
            other.gameObject.SetActive(false);
            _hp = maxHp;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow))
            {
                _climbingX = other.transform.position.x;
                _animator.SetBool(Climb1, true);
                _animator.SetBool(Jump, false);
                _isJumping = false;
                _isClimbing = true;
                _circleCollider2D.isTrigger = true;
                _rigidBody2D.isKinematic = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            bool onLadderTop = Physics2D.OverlapBox(_groundedCheckPoint, groundedCheckSize, 0f, ladderTop);
            if (onLadderTop)
            {
                other.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            }
            _animator.SetBool(Jump, false);
            _animator.SetBool(Climb1, false);
            _isClimbing = false;
            _circleCollider2D.isTrigger = false;
            _rigidBody2D.isKinematic = false;
        }
    }

    private void Creep()
    {
        bool startCrouch = Input.GetKeyDown(KeyCode.DownArrow);
        bool endCrouch = !Input.GetKey(KeyCode.DownArrow);
        if (startCrouch)
        {
            _animator.SetBool(Crouch, true);
            _isCrouching = true;
            moveSpeed /= 2;
            float radius = _circleCollider2D.radius;
            radius /= 2;
            _circleCollider2D.radius = radius;
            _circleCollider2D.offset = new Vector2(0, - radius) + _circleCollider2D.offset;
        }

        if (endCrouch && _isCrouching)
        {
            _animator.SetBool(Crouch, false);
            _isCrouching = false;
            moveSpeed *= 2;
            float radius = _circleCollider2D.radius;
            _circleCollider2D.offset = new Vector2(0,  radius) + _circleCollider2D.offset;
            radius *= 2;
            _circleCollider2D.radius = radius;
        }
    }

    private void Climb()
    {
        if (_isCrouching)
        {
            _animator.SetBool(Crouch, false);
            _isCrouching = false;
            moveSpeed *= 2;
            float radius = _circleCollider2D.radius;
            _circleCollider2D.offset = new Vector2(0,  radius) + _circleCollider2D.offset;
            radius *= 2;
            _circleCollider2D.radius = radius;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _rigidBody2D.MovePosition(new Vector2(_climbingX, transform.position.y - moveSpeed * Time.fixedDeltaTime));
            _animator.SetBool(ClimbStop, false);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            _rigidBody2D.MovePosition(new Vector2(_climbingX, transform.position.y + moveSpeed * Time.fixedDeltaTime));
            _animator.SetBool(ClimbStop, false);
        }
        else
        {
            _animator.SetBool(ClimbStop, true);
        }
        
    }
    private void Move()
    {
        float horizon = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(horizon) < Mathf.Epsilon)
        {
           _rigidBody2D.velocity = new Vector2(0,0);
           _animator.SetBool(Run, false);
        }
        else
        {
            _spriteRenderer.flipX = (horizon < 0);
            _animator.SetBool(Run, true);
            _rigidBody2D.velocity = new Vector2(horizon*moveSpeed, 0);
        }
        
    }

    private void Leap()
    {
        Vector3 position = transform.position;
        _groundedCheckPoint = new Vector2(position.x, position.y - 0.5f);
        bool startJumpForce = Input.GetKeyDown(KeyCode.Space);
        if (startJumpForce && _isGrounded)
        {
            _animator.SetBool(Jump, true);
            _isJumping = true;
            _isGrounded = false;
        }
        if(_isJumping)
        {
            if (_remainingJumpTime > 0)
            {
                _remainingJumpTime -= Time.fixedDeltaTime;
                _rigidBody2D.AddForce(Vector2.up * thrust, ForceMode2D.Impulse);
            }
            else
            {
                _animator.SetTrigger(JumpDown);
                _remainingJumpTime += jumpTime;
                _isJumping = false; 
            }
        }

        // Check if player get grounded
        _isGrounded = Physics2D.OverlapBox(_groundedCheckPoint, groundedCheckSize, 0f, ground);
        if (!_isJumping && _isGrounded)
        {
            _animator.SetBool(Jump, false);
        }
        
    }
    
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_isClimbing)
        {
            Climb();
        }
        
        if (_canMove)
        {
            Creep();
            Move();
            Leap();
        }
        if (_isDrawback)
        {
            Drawback();
        }
    }

    private void Update()
    {
        HpChange();
    }
}
