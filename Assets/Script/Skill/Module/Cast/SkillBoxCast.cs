using System.Collections.Generic;
using UnityEngine;

// 박스 범위 내 대상을 서버에서 수집하고 충돌 이벤트 모듈을 실행한다.
[System.Serializable]
public class SkillBoxCast : ISkillModule
{
    [SerializeField] private Vector3 _vOffset;
    [SerializeField] private Vector3 _vHalfExtents;
    [SerializeField] private LayerMask _targetMask;
    [SerializeReference, SubclassSelector] private List<ICollisionEventModule> _onHit = new List<ICollisionEventModule>();

    private Skill _skill;
    private Collider[] _hitBuffer;
    private readonly HashSet<Collider> _previousHits = new HashSet<Collider>();
    private readonly HashSet<Collider> _currentHits = new HashSet<Collider>();

    public void Bind(Skill skill)
    {
        _skill = skill;
        _hitBuffer = new Collider[16];

        for (int i = 0; i < _onHit.Count; ++i)
            _onHit[i].Bind(skill);
    }

    public void Enter()
    {
        _previousHits.Clear();
        _currentHits.Clear();
    }

    public void ServerTick(float fTimeDelta)
    {
        Vector3 vCenter = _skill.transform.position + _skill.transform.rotation * _vOffset;
        int iCount = Physics.OverlapBoxNonAlloc(
            vCenter, _vHalfExtents, _hitBuffer, _skill.transform.rotation, _targetMask);

        _currentHits.Clear();

        for (int i = 0; i < iCount; ++i)
        {
            Collider col = _hitBuffer[i];

            if (col.transform.root == _skill.Owner.transform) continue;

            _currentHits.Add(col);
            bool isEntered = _previousHits.Contains(col) == false;

            for (int j = 0; j < _onHit.Count; ++j)
            {
                if (_onHit[j].Timing == CollisionTiming.ENTER && isEntered == false) continue;
                _onHit[j].Collision(col);
            }
        }

        _previousHits.Clear();
        _previousHits.UnionWith(_currentHits);
    }
}
