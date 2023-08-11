using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public static ArrowController instance;
    public bool canReceiveInput;
    public bool inputReceived;

    // ? public
    public bool _isUsingSkill = false;
    public bool _attacking = false;
    public bool _blocking = false;

    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private GroundSensor _groundSensor;
    private float _moveSpeed = 5f;
    private float _blendMoveSpeedAnimation = 10f;
    private float _jumpForce = 7.5f;
    private bool _grounded = false;
    private float _blockDeltaTime = 0.2f;
    private float _lastDirection = 1; // todo refactor to enum
    private GameObject _airAttackArea;
    private GameObject _attackArea1;
    private GameObject _attackArea2;
    private GameObject _attackArea3;
    private GameObject _ultimateAttackArea;

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
        _renderer = GetComponent<SpriteRenderer>();
        _groundSensor = transform.Find("Ground Sensor").gameObject.GetComponent<GroundSensor>();
        _airAttackArea = transform.Find("Air Attack Area").gameObject;
        _attackArea1 = transform.Find("Attack Area 1").gameObject;
        _attackArea2 = transform.Find("Attack Area 2").gameObject;
        _attackArea3 = transform.Find("Attack Area 3").gameObject;
        _ultimateAttackArea = transform.Find("Ultimate Attack Area").gameObject;
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
            if(Input.GetKeyDown(KeyCode.Keypad4)) {
                _attacking = true;
                if (canReceiveInput) {
                    inputReceived = true;
                    canReceiveInput = false;
                }
            }
        } else {
            if(Input.GetKeyDown(KeyCode.Keypad4)) {
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
            _airAttackArea.SetActive(true);
        } else if (index == 1) {
            _attackArea1.SetActive(true);
        } else if (index == 2) {
            _attackArea2.SetActive(true);
        } else if (index == 3) {
            _attackArea3.SetActive(true);
        } else {
            _ultimateAttackArea.SetActive(true);
        }
    }
    
    public void DeactivateAttackPoint(int index) {
        if (index == 0) {
            _airAttackArea.SetActive(false);
        } else if (index == 1) {
            _attackArea1.SetActive(false);
        } else if (index == 2) {
            _attackArea2.SetActive(false);
        } else if (index == 3) {
            _attackArea3.SetActive(false);
        } else {
            _ultimateAttackArea.SetActive(false);
        }
    }

    private void Move()
    {
        if (_isUsingSkill || _blocking) return;
        float horizontal = Input.GetAxis("Horizontal2");
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
            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad0))
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
            if(Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                _animator.Play("Dash");
            }
        }
    }

    private void UseSkill() {
        if(Input.GetKeyDown(KeyCode.Keypad8)) {
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
        if(Input.GetKeyDown(KeyCode.Keypad9)) {
            _animator.Play("Death");
        }
    }

    private void Block() {
        if (!_grounded) return;
        if(Input.GetKey(KeyCode.Keypad5)) {
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
        // if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")){
        //     _animator.Play("TakeHit"); 
        // }
        StartCoroutine(TakeDamageCoroutine());
    }

    private IEnumerator TakeDamageCoroutine() {
        _renderer.color = new Color(255, 255, 255, 127);
        yield return new WaitForSeconds(0.2f);
        _renderer.color = new Color(255, 255, 255, 255);
    }
}
