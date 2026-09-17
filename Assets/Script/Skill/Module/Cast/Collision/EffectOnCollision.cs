using UnityEngine;

// 캐스트 적중 시 SkillEffector에 설정된 COLLISION 이펙트를 재생하거나 종료한다.
[System.Serializable]
public class EffectOnCollision : ICollisionEventModule
{
    [SerializeField] private CollisionTiming _timing = CollisionTiming.ENTER;

    private SkillEffector _effector;

    public CollisionTiming Timing => _timing;

    public void Bind(Skill skill)
    {
        _effector = skill.Effector;
    }

    public void Collision(Collider col)
    {
        _effector.PlayCollisionEffect();
    }
}
