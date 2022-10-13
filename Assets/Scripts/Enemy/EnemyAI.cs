using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    // 적 캐릭터의 상태를 표현하기 위한 열거형 변수 정의
    // enum 쓰면 유니티에서 메뉴형으로 보인다
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    // Animator 컴포넌트를 저장할 변수
    private Animator animator;

    // 상태를 저장할 변수
    public State state = State.PATROL;

    // 주인공의 위치를 저장할 변수
    private Transform platerTr;

    //적 캐릭터의 위치를 저장할 변수
    private Transform enemyTr;

    // 공격 사정거리
    public float attackDist = 5.0f;

    // 추적 사정거리
    public float traceDist = 10.0f;

    // 사망 여부를 판달하는 변수
    public bool isDie = false;

    // 코루틴에서 사용할 지연시간 변수
    private WaitForSeconds ws;

    // 이동을 제어하는 MoveAgent 클래스를 저장할 변수
    private MoveAgent moveAgent;

    // 총알 발사를 제어하는 EnemyFire 클래스를 저장할 변수
    private EnemyFire enemyFire;

    // 애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    // Animator에서 Parameters 추가하면 반드시 작성해야 한다
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
	
	void Update ()
    {
        // Speed 파라미터에 이동 속도를 전달
        animator.SetFloat(hashSpeed, moveAgent.speed);
	}

    void Awake()
    {
        // 주인공 게임오브젝트 추출
        var player = GameObject.FindGameObjectWithTag("PLAYER");

        // 주인공의 Transform 컴포넌트 추출
        if(player != null)
        {
            platerTr = player.GetComponent<Transform>();
        }

        // 적 캐릭터의 Transform 컴포넌트 추출
        enemyTr = GetComponent<Transform>();

        // 이동을 제어하는 MoveAgent 클래스를 추출
        moveAgent = GetComponent<MoveAgent>();

        // 총알 발사를 제어하는 EnemyFire 클래스를 추출
        enemyFire = GetComponent<EnemyFire>();

        // 코루틴의 지연시간 생성
        ws = new WaitForSeconds(0.3f);

        // Animator 컴포넌트 추출
        animator = GetComponent<Animator>();

        // Ctcle Offset 값을 불규칙하게 변경
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));

        // Speed 값을 불규칙하게 변경
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 1.2f));
    }

    void OnEnable()     // Awake 함수와 Start 함수 사이에 실행, 책 319p참조
    {
        // CheckState 코루틴 함수 실행
        StartCoroutine(CheckState());

        // Action 코루틴 함수 실행
        StartCoroutine(Action());

        Damage.OnPlayerDie += this.OnPlayerDie;         // +는 더하는 의미가 아니라 이벤트 실행 의미
    }

    private void OnDisable()
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;         // -는 빼라는 의미가 아니라 이벤트 실행 의미
    }

    IEnumerator CheckState()
    {
        while(!isDie)   // while(isDie == false)
        {
            // 상태가 사망이면 코루틴 함수를 종료시킴
            // yield break는 while을 빠져나가는 것이 아니라 IEnumerator CheckState()함수 자체를 나가는 것이다.
            if (state == State.DIE)
            {
                yield break;
            }

            // 주인공과 적 캐릭터 간의 거리를 계산
            float dist = Vector3.Distance(platerTr.position, enemyTr.position);

            // 공격 사정거리 이내인 경우
            if (dist <= attackDist)
            {
                state = State.ATTACK;
            }
            // 추적 사정거리 이내인 경우
            else if (dist <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }

            // 0.3초 동안 대기하는 동안 제어권을 양보
            // 루프가 한번 끝나면 0.3초 동안 코루틴 함수 실행 안한다
            yield return ws;    // = yidle return new WaitForSeconds(0.3f);
        }
    }

    // 상태에 따라 적 캐릭터의 행동을 처리하는 코루틴 함수
    IEnumerator Action()
    {
        // 적 캐릭터가 사망할 때까지 무한 루프
        while (!isDie)
        {
            yield return ws;

            switch (state)
            {
                case State.PATROL:
                    // 총알 발사 정지
                    enemyFire.isFire = false;

                    // 순찰 모드를 활성화
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;

                case State.TRACE:
                    // 총알 발사 정지
                    enemyFire.isFire = false;

                    // 주인공의 위치를 넘겨 적 모드로 변경
                    moveAgent.traceTarget = platerTr.position;
                    animator.SetBool(hashMove, true);
                    break;

                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);

                    // 총알 발사 시작
                    if(enemyFire.isFire == false)
                    {
                        enemyFire.isFire = true;
                    }
                    break;

                case State.DIE:
                    isDie = true;              
                    enemyFire.isFire = false;

                    // 순찰 및 추적을 정지
                    moveAgent.Stop();
                    // 사망 애니메이션의 종류를 지정
                    animator.SetInteger(hashDieIdx, Random.Range(0, 3));
                    // 사망 애니메이션 실행
                    animator.SetTrigger(hashDie);
                    // capsule Collider 컴포넌트를 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;

                    moveAgent.Stop();
                    break;
            }
        }
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;

        // 모든 코루틴 함수를 종료시킴
        StopAllCoroutines();

        animator.SetTrigger(hashPlayerDie);
    }
}