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
        Refresh();
    }

    void Msg_OnDied() => Refresh();

    void Refresh() => _anim.SetBool(propertyName, _enemy.isDead);

    public void Insp_DestroyEnemyGameObject() =>
        Destroy(_enemy.gameObject);
}
