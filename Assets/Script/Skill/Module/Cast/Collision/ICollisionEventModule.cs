using UnityEngine;

// 캐스트 반응 시점 — 진입 1회(ENTER) / 겹친 동안 매 프레임(STAY)
public enum CollisionTiming { ENTER, STAY }

public interface ICollisionEventModule
{
    CollisionTiming Timing { get; }
    void Bind(Skill skill);
    void Collision(Collider col);
}
