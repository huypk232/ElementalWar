using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public SpriteRenderer _renderer;
    public int MaxHp;

    // private Rigidbody2D _rb;
    private bool invisible = false;
    private float invisibleDeltaTime = 0.2f;
    private Animator _animator;
    private int _hp;    
    

    void Start()
    {
        // _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        // _renderer = GetComponent<SpriteRenderer>();
        _hp = MaxHp;
    }

    public void TakeDamage() {
        if (invisible) return;
        else {
            _hp -= 1;
            if (_hp < 0) {
                _animator.Play("Death");
            }
            StartCoroutine(TakeDamageCoroutine());
        }
    }

    private IEnumerator TakeDamageCoroutine() {
        _renderer.color = new Color(127f, 127f, 127f);
        invisible = true;
        yield return new WaitForSeconds(invisibleDeltaTime);
        _renderer.color = new Color(255f, 255f, 255f);
        invisible = false;
    }
}
