using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SetAnimIsDead : MonoBehaviour
{
    public string propertyName = "Is Dead";

    Animator _anim;
    Enemy _enemy;

    void OnEnable()
    {
        _anim = GetComponent<Animator>();
        _enemy = GetComponentInParent<Enemy>();

        _enemy.onDie += Refresh;
        Refresh();
    }
    void OnDisable()
    {
        if (_enemy != null)
            _enemy.onDie -= Refresh;
    }

    void Refresh() => _anim.SetBool(propertyName, _enemy.isDead);

    public void Insp_DestroyEnemyGameObject() =>
        Destroy(_enemy.gameObject);
}
