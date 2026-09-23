# Unity 프로젝트 코드 컨벤션

이 문서는 프로젝트 전반의 코드 스타일과 설계 규칙을 정리한다.
신규 코드는 이 컨벤션을 따르고, 기존 코드 수정 시에도 점진적으로 맞춰간다.
---

## 1. 네이밍 규칙

| 대상 | 규칙 | 예시 |
| --- | --- | --- |
| 클래스 / 구조체 | PascalCase | `Skill`, `StateMachine`, `GameManager` |
| 인터페이스 | `I` 접두사 + PascalCase | `ISkill`, `IState`, `IEntityMovement` |
| public 프로퍼티 | PascalCase, 헝가리안 미적용 | `Number`, `Type`, `Data` |
| `int` 지역/멤버 | `i` 접두사 | `iNum`, `iCount` |
| `float` 지역/멤버 | `f` 접두사 | `fTime`, `fSpeed` |
| `double` 지역/멤버 | `d` 접두사 | `dDouble` |
| `bool` 지역/멤버 | `is` 접두사 | `isBool`, `isActive` |
| `Vector2` / `Vector3` 지역/멤버 | `v` 접두사 | `vDir`, `vCustomDir` |
| 멤버 변수 | `_` 접두사 | `_iNum`, `_fTime`, `_isActive` |
| 클래스 타입 멤버 객체 | `_` + 소문자 시작 | `_hitter`, `_effector` |
| enum 값(태그) | 대문자만 사용 | `IDLE`, `WALK`, `ATTACK_START` |

- **식별자 스펠링이 틀리면 반드시 올바르게 수정한다.** (예: `Clinet` → `Client`, `Animatior` → `Animator`)
---

## 2. 접근 제어 / 은닉화

- **접근 지정자(`public` / `protected` / `private`)는 반드시 명시한다.** 생략해 기본값에 의존하지 않는다.
- **변수·함수 모두 접근 수준이 넓은 순서로 나열한다: `public` → `protected` → `private`.**
- **`[SerializeField]` 는 선언과 한 줄에 붙여 쓴다.** 어트리뷰트와 필드를 두 줄로 나누지 않는다.
  - O: `[SerializeField] private EntityEffector _effector;`
  - X: `[SerializeField]` 와 `private EntityEffector _effector;` 를 두 줄로 분리


## 3. 주석 / 언어

- 모든 주석·커밋 메시지·문서는 **한국어**로 작성한다.
- 주석은 가장 중요한 것만 쓰고 많이 쓰지 않는다.

## 4. 코드 작성 및 설계 규칙

- Inspector에 노출할 필드는 `[SerializeField]`와 `private` 또는 `protected`를 사용한다. 스크립트 내부에 `public` 변수를 선언하지 않는다.
- 내부 코드에 null 체크를 추가하지 않는다. 사용자 입력과 외부 API 등 시스템 경계에서만 유효성을 검증한다.
- **게임 규칙이나 여러 곳에서 공유하는 상수값을 메서드 본문에 숫자·문자열 리터럴로 직접 쓰지 않는다.** 의미가 드러나는 이름으로 한 곳에 정의하고 참조한다. 고정값은 `const` 또는 `static readonly`, 기획·밸런스 조정값은 기존 데이터 에셋이나 `[SerializeField]` 설정을 사용한다.
- 같은 의미의 값을 여러 클래스에 각각 선언하거나 복사하지 않는다. 값을 바꿀 때 수정할 곳이 하나가 되도록 소유 위치를 정한다. 예: 처치 보상량 `50f`를 지급 코드에 직접 쓰지 않고 이름 붙인 설정값을 참조한다.
