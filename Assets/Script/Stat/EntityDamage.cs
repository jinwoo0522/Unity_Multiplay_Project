using Unity.Netcode;
using UnityEngine;

public abstract class EntityDamage : MonoBehaviour , IDamagable
{
    public bool _isHit {get; set;}
    public bool _isDead {get; set;}
    // 마지막 피격 정보 — 피격 반응이 읽는다 (쓰기는 Hit이 독점)
    public IDamagable.DamageInfo _damageInfo {get; private set;}

    [SerializeField] private float _fInvulnerableDuration = 0.55f;
    protected Stat _stat;
    protected CrowdController _crowdController;
    private float _fInvulnerableUntil;

    protected virtual void Awake()
    {
        _stat = GetComponent<Stat>();
        _crowdController = GetComponent<CrowdController>();
        // 사망 판정은 HP 변경을 구독해 처리한다 — Hit을 거치지 않는 감소(낙사·도트)도 함께 잡힌다
        _stat.StatChanged += HandleStatChanged;
    }

    protected virtual void OnDestroy()
    {
        _stat.StatChanged -= HandleStatChanged;
    }

    protected virtual void Update()
    {
        if (_stat.IsServer == false || _fInvulnerableUntil == 0f) return;
        if (Time.time < _fInvulnerableUntil) return;

        _fInvulnerableUntil = 0f;
        _isHit = false;
    }

    public void Hit(IDamagable.DamageInfo damageInfo)
    {
        if(_isDead == true || Time.time < _fInvulnerableUntil) return;

        // 피격 반응이 읽어야 하므로 데미지 적용보다 먼저 보관한다
        _damageInfo = damageInfo;

        if (_stat.TakeDamage(damageInfo.Damage, damageInfo.Attacker) <= 0f) return;
        _fInvulnerableUntil = Time.time + _fInvulnerableDuration;

        // 에어본·빙결 중에는 데미지만 넣고 경직 반응은 만들지 않는다
        // 여기서 막지 않으면 CC가 풀린 뒤 밀린 플래그가 살아나 뒤늦게 HIT이 재생된다
        if(_crowdController.IsApply(CrowdController.CC_TAG.AIRBORNE) == true) return;
        if(_crowdController.IsApply(CrowdController.CC_TAG.FREEZE) == true) return;

        _isHit = true;

        OnHit(damageInfo);
    }

    // 경직이 성립한 뒤에만 불린다 — CC로 막힌 타격은 여기까지 오지 않는다
    protected abstract void OnHit(IDamagable.DamageInfo damageInfo);

    private void HandleStatChanged(Stat.STAT_TAG tag)
    {
        if(tag != Stat.STAT_TAG.HP) return;

        bool isDead = _stat.Get_Stat(Stat.STAT_TAG.HP) <= 0f;

        bool isJustDied = _isDead == false && isDead == true;
        _isDead = isDead;

        // 사망으로 넘어가는 순간에만 CC를 전부 해제한다 — 시체에 빙결 머티리얼·에어본 이동이 남지 않게
        if(isJustDied == true)
        {
            _crowdController.RestoreAll();
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
    }

    private void OnDisable()
    {
        _fInvulnerableUntil = 0f;
        _isHit = false;
        _isDead = false;
        _damageInfo = default;
    }
}
