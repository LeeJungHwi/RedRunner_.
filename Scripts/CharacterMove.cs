using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMove : MonoBehaviour
{
    // 컴포넌트
    public float maxSpeed; 
    public float jumpPower; 
    int cnt = 2;
    public GameManager gameManager;
    bool isRun = false;
    bool isJump = false;
    Rigidbody2D rigid; 
    public SpriteRenderer spriteRenderer; 
    CapsuleCollider2D capsuleCollider;
    Animator anim;
    public AudioSource audioSource; // AudioClip 재생역할
    public AudioClip audioJump; // 재생 할 음원파일
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioCoin;
    public AudioClip audioStar;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public AudioClip audioWater;
    public AudioClip audioRun;
    public AudioClip audioBtnClick;
    public AudioClip audioCursor;
    public AudioClip audioBox;
    public AudioClip audioFieldItem;
    public AudioClip audioPortion;
    public AudioClip audioInvincibility;
    public GameObject[] dialoguePanel;
    public GameObject DadCharacter;
    public int dialogueIndex;
    bool isTalk = false;
    public bool isOpen = false;
    bool isStage2Coin = false;
    bool isStage3Coin = false;
    bool isStage4Coin = false;
    bool isStage5Coin = false;
    bool isStage6Coin = false;
    bool isStage7Coin = false;
    public GameObject[] stage2Coin;
    public GameObject[] stage3Coin;
    public GameObject[] stage4Coin;
    public GameObject[] stage5Coin;
    public GameObject[] stage6Coin;
    public GameObject[] stage7Coin;


    // Mobile Key
    int left_Value;
    int right_Value;
    bool up_Down;
    bool left_Down;
    bool right_Down;
    bool left_Up;
    bool right_Up;

    void Awake() // 초기화
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();               
        spriteRenderer = GetComponent<SpriteRenderer>();    
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() // 이동 & 점프
    {
        // 대화상태, 메인메뉴, 서브메뉴 상태가 아닐때
        if(isTalk == false && gameManager.isMain == false && gameManager.isSubMenu == false)
        {
            // 2단점프
            if ((Input.GetButtonDown("Jump") || up_Down) && cnt > 0) // 점프키를 누르면서 점프횟수가 0보다 크다면
            {
                // 점프할때마다 Y축 속도를 0으로 초기화
                rigid.velocity = new Vector2(rigid.velocity.x, 0);

                // 위쪽으로 힘을 준다
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

                // 에니메이션
                anim.SetBool("isJump", true);

                // 점프 카운팅 감소
                cnt--;

                // 사운드
                PlaySound("JUMP");

                // 점프상태
                isJump = true;
            }

            // 점프 할때 이동속도 제한
            if (Input.GetButtonUp("Horizontal") || right_Up || left_Up) // GetButtonUp() -> 버튼을 눌렀다가 땠을경우 True 반환
            {
                rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y); // normalized -> 단위벡터
            }

            // 방향 전환
            if (Input.GetButton("Horizontal") || left_Down || right_Down) // GetButton() -> 버튼을 누를때 항상 체크
                spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") + left_Value + right_Value == -1;

            // 에니메이션
            if (Mathf.Abs(rigid.velocity.x) < 0.3) // 행동을 멈추면
                anim.SetBool("isRun", false);
            else
                anim.SetBool("isRun", true);

            // 달릴때 사운드
            RunSound();

            up_Down = false;
            left_Down = false;
            right_Down = false;

            left_Up = false;
            right_Up = false;
        }
    }

    void FixedUpdate() // 이동 & 점프
    {
        // 대화상태가 아닐때
        if(isTalk == false)
        {
            // 이동속도
            float h = Input.GetAxisRaw("Horizontal") + left_Value + right_Value;
            rigid.AddForce(Vector2.right * h * 2, ForceMode2D.Impulse);

            // 최대 이동 속도 제한
            if (rigid.velocity.x > maxSpeed) // Right Max Speed
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            else if (rigid.velocity.x < maxSpeed * (-1)) // Left Max Speed
                rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

            // 착지 상태
            if (rigid.velocity.y < 0) // y축의 속도가 내려가는 상태이면
            {
                // 빛을 쏜다
                Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

                // 빛을 맞은 객체 검색
                RaycastHit2D rayHitDown = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
                RaycastHit2D rayHitForward = Physics2D.Raycast(rigid.position, Vector3.forward, 1, LayerMask.GetMask("Platform"));

                if (rayHitDown.collider != null || rayHitForward.collider != null) // 빛을 맞은 객체의 정보가 있다면
                {
                    if (rayHitDown.distance < 0.5f || rayHitForward.distance < 0.5f) // 빛을 닿은 거리가 0.5보다 작으면
                    {
                        anim.SetBool("isJump", false); // 점프하는 상태가 아니다
                        cnt = 2; // 착지 상태 이므로, 다시 2단 점프 할 수 있게 점프 카운팅 2로 초기화
                        isJump = false; // 착지상태
                    }
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) // 공격 & 피격
    {
        if (collision.gameObject.tag == "Enemy") // Enemy와 충돌하면
        {
            OnDamaged(collision.transform.position); // 캐릭터가 데미지를 입는다
        }
        else if(collision.gameObject.tag == "EnemyCharacter") // EnemyCharacter와 충돌하면
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) // y축의 속도가 내려가는 상태이면서 적캐릭터의 위치보다 높다면
            {
                OnAttack(collision.transform); // 캐릭터가 공격한다
            }
            else
                OnDamaged(collision.transform.position); // 캐릭터가 데미지를 입는다
        }
        else if (collision.gameObject.tag == "Water") // Water와 충돌했을때 
        {
            // HP가 2개까지는 캐릭터를 원래 위치로
            if (gameManager.health > 1)
            {
                gameManager.CharacterReposition();
            }
            // HP가 한개 남았을때는 HP감소 함수만 호출
            gameManager.HealthDown();

            //사운드
            PlaySound("WATER");
        }

    }

    void OnTriggerEnter2D(Collider2D collision) // 코인 & 피니쉬
    {
        if (collision.gameObject.tag == "Item") // Item과 충돌하면
        {
            // 코인의 종류에 따라서 점수 증가
            bool isCoin = collision.gameObject.name.Contains("Coin"); 
            bool isStar = collision.gameObject.name.Contains("Star");

            // 아이템 종류에 따라 아이템 활성화
            bool isPortion = collision.gameObject.name.Contains("Portion");
            bool isInvincibility = collision.gameObject.name.Contains("Invincibility");

            if (isCoin)
            {
                // 점수 증가
                gameManager.stagePoint += 50;

                // 갯수 증가
                gameManager.coinCount++;

                // 아이템 제거
                collision.gameObject.SetActive(false);

                // 사운드
                PlaySound("COIN");
            }
            else if (isStar)
            {
                // 점수 증가
                gameManager.stagePoint += 500;

                // 갯수 증가
                gameManager.starCount++;

                // 아이템 제거
                collision.gameObject.SetActive(false);

                // 사운드
                PlaySound("STAR");
            }
            else if(isPortion) // 포션이 없을때만
            {
                if(gameManager.hasPortion == false)
                {
                    // 포션 추가
                    gameManager.portionBtn.SetActive(true);

                    // 아이템 제거
                    collision.gameObject.SetActive(false);

                    // 포션 체크
                    gameManager.hasPortion = true;

                    // 사운드
                    audioSource.clip = audioFieldItem;
                    audioSource.Play();
                }
            }
            else if(isInvincibility)
            {
                if(gameManager.hasInvincibility == false) // 무적아이템이 없을때만
                {
                    // 무적아이템 추가
                    gameManager.invincibilityBtn.SetActive(true);

                    // 아이템 제거
                    collision.gameObject.SetActive(false);;

                    // 포션 체크
                    gameManager.hasInvincibility = true;

                    // 사운드
                    audioSource.clip = audioFieldItem;
                    audioSource.Play();
                }
            }
        }
        else if (collision.gameObject.tag == "Finish") // Finish와 충돌하면
        {
            // 다음 스테이지
            gameManager.NextStage();

            // 사운드
            PlaySound("FINISH");
        }
        else if (collision.gameObject.tag == "DadCharacter") // DadCharacter와 충돌하면
        {
            // 모바일 UI 비활성화
            // gameManager.VisibleMobileUI(false);

            // 대화 활성화
            dialoguePanel[dialogueIndex].SetActive(true);

            // 대화 상태
            isTalk = true;

            // 애니메이션
            anim.SetBool("isRun", false);
            anim.SetBool("isJump", false);
        }
        else if(collision.gameObject.tag == "Box") // Box와 충돌하면
        {
            if(isOpen == false) // 상자가 열린상태가 아니면
            {
                audioSource.PlayOneShot(audioBox); // 사운드 재생
            }
            isOpen = true; // 상자가 열린상태
        }

        if(collision.gameObject.name == "Stage2Box") // 스테이지 2 박스이면
        {
            if(isStage2Coin == false) // 스테이지 2 코인을 먹은적이 없으면
            {
                for (int i = 0; i < 3; i++) // 스테이지 2 코인 활성화
                {
                    stage2Coin[i].SetActive(true);
                    isStage2Coin = true;
                }
            }
        }
        else if(collision.gameObject.name == "Stage3Box") // 스테이지 3 박스이면
        {
            if (isStage3Coin == false) // 스테이지 3 코인을 먹은적이 없으면
            {
                for (int i = 0; i < 5; i++) // 스테이지 3 코인 활성화
                {
                    stage3Coin[i].SetActive(true);
                    isStage3Coin = true;
                }
            }
        }
        else if (collision.gameObject.name == "Stage4Box") // 스테이지 4 박스이면
        {
            if (isStage4Coin == false) // 스테이지 4 코인을 먹은적이 없으면
            {
                for (int i = 0; i < 3; i++) // 스테이지 4 코인 활성화
                {
                    stage4Coin[i].SetActive(true);
                    isStage4Coin = true;
                }
            }
        }
        else if (collision.gameObject.name == "Stage5Box") // 스테이지 5 박스이면
        {
            if (isStage5Coin == false) // 스테이지 5 코인을 먹은적이 없으면
            {
                for (int i = 0; i < 5; i++) // 스테이지 5 코인 활성화
                {
                    stage5Coin[i].SetActive(true);
                    isStage5Coin = true;
                }
            }
        }
        else if (collision.gameObject.name == "Stage6Box") // 스테이지 6 박스이면
        {
            if (isStage6Coin == false) // 스테이지 6 코인을 먹은적이 없으면
            {
                for (int i = 0; i < 2; i++) // 스테이지 6 코인 활성화
                {
                    stage6Coin[i].SetActive(true);
                    isStage6Coin = true;
                }
            }
        }
        else if (collision.gameObject.name == "Stage7Box") // 스테이지 7 박스이면
        {
            if (isStage7Coin == false) // 스테이지 7 코인을 먹은적이 없으면
            {
                for (int i = 0; i < 10; i++) // 스테이지 7 코인 활성화
                {
                    stage7Coin[i].SetActive(true);
                    isStage7Coin = true;
                }
            }
        }
    }

    public void Cursor() // 대화창 커서를 클릭했을때
    {
        // 마지막 대화에서 커서를 클릭하면
        if(dialogueIndex == 3)
        {
            // 대화창 비활성화
            dialoguePanel[dialogueIndex].SetActive(false);

            // 모바일 UI 활성화 
            // gameManager.VisibleMobileUI(true);

            // DadCharacter 비활성화
            DadCharacter.SetActive(false);

            // 대화 상태 풀기
            isTalk = false;
        }
        else
        {
            // 이전 대화창 비활성화
            dialoguePanel[dialogueIndex].SetActive(false);

            // 대화 인덱스 증가
            dialogueIndex++;

            // 다음 대화창 활성화
            dialoguePanel[dialogueIndex].SetActive(true);
        }
        // 사운드
        PlaySound("CURSOR");
    }

    void OnAttack(Transform enemy) // 공격 함수
    {
        // 점수
        gameManager.stagePoint += 200;

        // 반발력
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // 적캐릭터는 공격을 당한다
        EnemyCharacterMove enemyCharacterMove = enemy.GetComponent<EnemyCharacterMove>();
        enemyCharacterMove.OnDamaged();

        // 사운드
        PlaySound("ATTACK");
    }

    void OnDamaged(Vector2 targetPos) // 피격 함수
    {
        // 레이어를 11.CharacterDamaged로 변경
        gameObject.layer = 11;

        // 잠시 투명
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 캐릭터 위치 - Enemy 위치 > 0 일때는 캐릭터가 Enemy보다 오른쪽에 있으므로, 1(오른쪽) 으로 튕겨나간다
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        // 에니메이션
        anim.SetTrigger("doDamaged");

        // 무적시간
        Invoke("OffDamaged", 3);

        // HP 감소
        gameManager.HealthDown();

        // 사운드
        PlaySound("DAMAGED");
    }

    void OffDamaged() // 무적 해제 함수
    {
        // 레이어를 10.Character로 변경
        gameObject.layer = 10;

        // 투명 해제
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie() // 죽는 함수
    {
        // 투명
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 방향전환 Y축
        spriteRenderer.flipY = true;

        // 캡슐 콜라이더 비활성화
        capsuleCollider.enabled = false;

        // 죽을때 점프 효과
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // 애니메이션 스탑
        anim.SetBool("isRun", false);
        anim.SetBool("isJump", false);

        // 메인메뉴 체크
        gameManager.isMain = true;

        // 사운드
        PlaySound("DIE");
    }

    public void VelocityZero() // 낙하속도를 0으로 설정하는 함수
    {
        rigid.velocity = Vector2.zero;
    }

    public void PlaySound(string action) // 사운드 함수
    {
        // 해당하는 음원파일 지정
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "COIN":
                audioSource.clip = audioCoin;
                break;
            case "STAR":
                audioSource.clip = audioStar;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
            case "WATER":
                audioSource.clip = audioWater;
                break;
            case "BUTTON":
                audioSource.clip = audioBtnClick;
                break;
            case "CURSOR":
                audioSource.clip = audioCursor;
                break;
        }
        // 클립 재생
        audioSource.Play();
    }

    void RunSound() // 발소리 사운드 함수
    {
        // 서있는 상태가 아니면서 점프상태가 아닌경우 달리는상태
        if(rigid.velocity.x != 0 && !isJump)
        {
            isRun = true;
        }
        else
            isRun = false;

        // 달리는 경우 반복적으로 사운드를 재생하고 아니면 소리를0으로
        if(isRun)
        {
            if(!audioSource.isPlaying)
                audioSource.PlayOneShot(audioRun, 1f);
        }
        else
            audioSource.PlayOneShot(audioRun, 0);
    }

    public void SetEffectSoundVolume(float volume) // 효과음 조절 함수
    {
        // 효과음 조절
        audioSource.volume = volume;
    }

    public void ButtonDown(string type)
    {
        switch (type)
        {
            case "U":
                up_Down = true;
                break;
            case "L":
                left_Value = -1;
                left_Down = true;
                break;
            case "R":
                right_Value = 1;
                right_Down = true;
                break;
            case "C":
                gameManager.SubMenuActive();
                break;
        }
    }

    public void ButtonUp(string type)
    {
        switch (type)
        {
            case "L":
                left_Value = 0;
                left_Up = true;
                break;
            case "R":
                right_Value = 0;
                right_Up = true;
                break;
        }
    }
}

// 달리는 상태에 사운드를 넣을때 잡음이 발생하는문제
// 달리는상태를 체크하는 변수를 만들고 서있는 경우가 아니면 true 그외는 false로 지정
// 달리는상태일 동안 반복해서 사운드를 재생
// 달리는상태가 아닐경우에는 사운드의 소리를 0으로

// 캐릭터가 점프상태인 경우에도 달리는 음원파일이 재생되는문제
// 점프상태를 체크하는 변수를 만들고 점프하면 true 착지하면 false로 지정
// 착지상태 일때는 달리는 음원파일을 재생해주고 점프상태 일때는 달리는 음원파일의 볼륨을 0으로

// BigMace가 떨어진 상태인데 플레이어가 가까이가면 여전히 충돌해서 음원파일이 재생되는문제
// 떨어졌는지 체크하는 변수 생성
// false인경우에만 음원파일을 재생
// 변수를 true로 체크

// 경사면에서 착지상태가 적용되지않는 문제
// 캐릭터기준 전방으로도 객체를 검사

// 모바일 버튼