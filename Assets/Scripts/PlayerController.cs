using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public GameObject AirAttackArea;
    public Transform _attackPoint;
    public GameObject AttackArea2;
    public GameObject AttackArea3;
    public GameObject UltimateAttackArea;
    public bool canReceiveInput;
    public bool inputReceived;

    // ? public
    public bool _isUsingSkill = false;
    public bool _attacking = false;
    public bool _blocking = false;

    private Rigidbody2D _rb;
    private Animator _animator;
    private GroundSensor _groundSensor;
    private float _moveSpeed = 5f;
    private float _blendMoveSpeedAnimation = 10f;
    private float _jumpForce = 7.5f;
    private bool _grounded = false;
    private float _blockDeltaTime = 0.2f;
    private float _lastDirection = 1; // todo refactor to enum

    private void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundSensor = transform.Find("Ground Sensor").gameObject.GetComponent<GroundSensor>();
    }

    void Update()
    {
        CheckGround();
        Move();
        Jump();
        Roll();
        AttackCombo();
        UseSkill();
        Death();
        Block();
    }

    private void CheckGround()
    {
        if (!_grounded && _groundSensor.State()) {
            _grounded = true;
            _animator.SetBool("IsGrounded", true);
        }

        if(_grounded && !_groundSensor.State()) {
            _grounded = false;
        }
    }

    private void AttackCombo() {
        if(_grounded)
        {
            if(Input.GetKeyDown(KeyCode.J)) {
                _attacking = true;
                if (canReceiveInput) {
                    inputReceived = true;
                    canReceiveInput = false;
                }
            }
        } else {
            if(Input.GetKeyDown(KeyCode.J)) {
                _attacking = true;
                _animator.Play("AirAttack");
                ActivateAttackPoint(0);
            }
        }        
    }

    public void ChangeReceiveInputStatus() {
        canReceiveInput = !canReceiveInput;
    }

    public void ActivateAttackPoint(int index) {
        if (index == 0) {
            AirAttackArea.SetActive(true);
        } else if (index == 1) {
            _attackPoint.gameObject.SetActive(true);
        } else if (index == 2) {
            AttackArea2.SetActive(true);
        } else if (index == 3) {
            AttackArea3.SetActive(true);
        } else {
            UltimateAttackArea.SetActive(true);
        }
    }
    
    public void DeactivateAttackPoint(int index) {
        if (index == 0) {
            AirAttackArea.SetActive(false);
        } else if (index == 1) {
            _attackPoint.gameObject.SetActive(false);
        } else if (index == 2) {
            AttackArea2.SetActive(false);
        } else if (index == 3) {
            AttackArea3.SetActive(false);
        } else {
            UltimateAttackArea.SetActive(false);
        }
    }

    private void Move()
    {
        if (_isUsingSkill || _blocking) return;
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal * _lastDirection < 0 && _attacking) return;
        if(horizontal < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        } else if(horizontal > 0) {
            transform.localScale = new Vector3(1, 1, 1);
        }
        _animator.SetFloat("MoveSpeed", _blendMoveSpeedAnimation * Mathf.Abs(horizontal));  // todo check
        if (horizontal == 0) {
            _animator.SetBool("Moving", false);
        } else {
            _lastDirection = horizontal;
            _animator.SetBool("Moving", true);
        }
        _rb.velocity = new Vector2(horizontal * _moveSpeed, _rb.velocity.y);
    }

    private void Jump()
    {
        if(_grounded)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            {
                _animator.CrossFadeInFixedTime("JumpUp", 0.1f);
                _animator.SetBool("IsGrounded", false);
                _grounded = false;
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce); 
                _groundSensor.Disable(0.2f);
            }
        }
    }

    private void Roll()
    {
        if(_grounded)
        {
            if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                _animator.Play("Dash");
            }
        }
    }

    private void UseSkill() {
        if(Input.GetKeyDown(KeyCode.I)) {
            _animator.Play("UltimateAttack");
            _isUsingSkill = true;
            ActivateAttackPoint(4);
        }
    }

    public void UseSkillDone() {
        _isUsingSkill = false;
    }
    
    public void AttackDone() {
        _attacking = false;
    }

    private void Death() {
        if(Input.GetKeyDown(KeyCode.P)) {
            _animator.Play("Death");
        }
    }

    private void Block() {
        if (!_grounded) return;
        if(Input.GetKey(KeyCode.K)) {
            _blocking = true;
            _animator.SetBool("Block", true);
            if (_blockDeltaTime < 0) {
                _animator.SetBool("HoldBlock", true);
            } else {
                _blockDeltaTime -= Time.deltaTime;
                _animator.SetBool("HoldBlock", false);
            }
        } else {
            _blocking = false;
            _animator.SetBool("Block", false);
            _animator.SetBool("HoldBlock", false);
            _blockDeltaTime = 0.2f;
        }
    }

    public void TakeDamage() {
        _animator.Play("TakeHit");
    }
}
