using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AlphabetController : MonoBehaviour
{
    public static AlphabetController Instance;
    
    public bool canReceiveInput;
    public bool inputReceived;
    
    [SerializeField] private bool isUsingSkill = false;
    [SerializeField] private bool attacking = false;
    [SerializeField] private bool blocking = false;

    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private GroundSensor _groundSensor;
    private const float MoveSpeed = 5f;
    private const float BlendMoveSpeedAnimation = 10f;
    private const float JumpForce = 7.5f;
    private bool _grounded = false;
    private float _blockDeltaTime = 0.2f;
    private float _lastDirection = 1; // todo refactor to enum
    private GameObject _airAttackArea;
    private GameObject _attackArea1;
    private GameObject _attackArea2;
    private GameObject _attackArea3;
    private GameObject _ultimateAttackArea;
    
    [Header("Animation")]
    private static readonly int Moving = Animator.StringToHash("Moving");

    private static readonly int Block = Animator.StringToHash("Block");
    private static readonly int HoldBlock = Animator.StringToHash("HoldBlock");

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this) {
            Destroy(Instance.gameObject);
            Instance = this;
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
        Defense();
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
                attacking = true;
                if (canReceiveInput) {
                    inputReceived = true;
                    canReceiveInput = false;
                }
            }
        } else {
            if(Input.GetKeyDown(KeyCode.J)) {
                attacking = true;
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
        if (isUsingSkill || blocking) return;
        float horizontal = Input.GetAxis("Horizontal1");
        if (horizontal * _lastDirection < 0 && attacking) return;
        if(horizontal < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        } else if(horizontal > 0) {
            transform.localScale = new Vector3(1, 1, 1);
        }
        _animator.SetFloat("MoveSpeed", BlendMoveSpeedAnimation * Mathf.Abs(horizontal));  // todo check
        if (horizontal == 0) {
            _animator.SetBool(Moving, false);
        } else {
            _lastDirection = horizontal;
            _animator.SetBool(Moving, true);
        }
        _rb.velocity = new Vector2(horizontal * MoveSpeed, _rb.velocity.y);
    }

    private void Jump()
    {
        if(_grounded)
        {
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
                _animator.CrossFadeInFixedTime("JumpUp", 0.1f);
                _animator.SetBool("IsGrounded", false);
                _grounded = false;
                _rb.velocity = new Vector2(_rb.velocity.x, JumpForce); 
                _groundSensor.Disable(0.2f);
            }
        }
    }

    private void Roll()
    {
        if(_grounded)
        {
            if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.L))
            {
                _animator.Play("Dash");
            }
        }
    }

    private void UseSkill() {
        if (_grounded)
        {
            if(Input.GetKeyDown(KeyCode.I)) {
                _animator.Play("UltimateAttack");
                isUsingSkill = true;
                ActivateAttackPoint(4);
            }
        }
    }

    public void UseSkillDone() {
        isUsingSkill = false;
    }
    
    public void AttackDone() {
        attacking = false;
    }

    private void Death() {
        if(Input.GetKeyDown(KeyCode.P)) {
            _animator.Play("Death");
        }
    }

    private void Defense() {
        if (!_grounded) return;
        if(Input.GetKey(KeyCode.K)) {
            blocking = true;
            _animator.SetBool(Block, true);
            if (_blockDeltaTime < 0) {
                _animator.SetBool(HoldBlock, true);
            } else {
                _blockDeltaTime -= Time.deltaTime;
                _animator.SetBool(HoldBlock, false);
            }
        } else {
            blocking = false;
            _animator.SetBool(Block, false);
            _animator.SetBool(HoldBlock, false);
            _blockDeltaTime = 0.2f;
        }
    }

    public void TakeDamage() {
        _animator.SetTrigger("Take Hit");
        StartCoroutine(TakeDamageCoroutine());
    }

    private IEnumerator TakeDamageCoroutine() {
        _renderer.color = new Color(255, 255, 255, 127);
        yield return new WaitForSeconds(0.2f);
        _renderer.color = new Color(255, 255, 255, 255);
    }
}
