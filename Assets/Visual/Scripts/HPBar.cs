using UnityEngine;

public class HPBar : MonoBehaviour
{
    const int NUM_PIXELS_WIDE = 8;
    const float PIXEL_WIDTH = 0.125f;

#pragma warning disable CS0649
    [SerializeField] Transform _multiplierSection;
    [SerializeField] Transform _activeSection;
    [SerializeField] Transform _activeScaler;
#pragma warning restore CS0649

    Enemy _enemy;

    bool _isDirty;

    void OnEnable()
    {
        _enemy = GetComponentInParent<Enemy>();
        _enemy.onLifeChanged += SetDirty;
        SetDirty();
    }
    void Update()
    {
        if (_isDirty)
            Refresh();
    }
    void OnDisable()
    {
        if (_enemy != null)
            _enemy.onLifeChanged -= SetDirty;
    }

    void SetDirty() => _isDirty = true;

    void Refresh()
    {
        _isDirty = false;

        if (_enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        var remainder = _enemy.life % NUM_PIXELS_WIDE;
        _activeSection.gameObject.SetActive(remainder != 0);
        _activeScaler.localScale = new Vector3(remainder, 1, 1);

        var multiplier = _enemy.life / NUM_PIXELS_WIDE;
        _multiplierSection.localScale = new Vector3(NUM_PIXELS_WIDE, multiplier, 1);
        _activeSection.localPosition = new Vector3(0, PIXEL_WIDTH * multiplier, 0);
    }
}
