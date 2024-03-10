using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 컴포넌트
[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip clip;
}

public class GameManager : MonoBehaviour
{
    // 점수 & 스테이지 관리
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public int coinCount;
    public int starCount;
    public int recordIndex;
    public int storeCoin;
    public int storeStar;
    public GameObject[] Stages;

    // 서브메뉴 체크
    public bool isSubMenu = false; // 현재상태가 서브메뉴인지 체크
    bool isSubMenuOption = false; // 현재상태가 서브메뉴의 옵션메뉴인지 체크
    public bool isMain = false; // 현재상태가 메인메뉴인지 체크
    public bool hasPortion = true; // 현재 포션을 가지고있는 상태인지 체크
    public bool hasInvincibility = true; // 현재 무적아이템을 가지고있는 상태인지 체크

    // 메인메뉴
    public GameObject mainBackground;
    public GameObject redRunnerText;
    public GameObject startBtn;
    public GameObject mainToMainOptionBtn;
    public GameObject exitBtn;
    public GameObject characterimage;
    public GameObject enemyCharacterimage;
    public GameObject maceImage;
    public GameObject infoBtn;
    public GameObject rankBtn;
    public GameObject tutorialBtn;
    public GameObject storeBtn;
    public GameObject infoPanel;
    public GameObject infoToMainBtn;
    public GameObject tutorialPanel;
    public GameObject tutorialToMainBtn;
    public GameObject water1;
    public GameObject water2;
    public GameObject water3;
    public GameObject water4;
    public GameObject water5;
    public GameObject water6;
    public GameObject water7;
    public GameObject water8;
    public GameObject water9;
    public GameObject water10;
    public GameObject notImplementPanel;
    public GameObject rankPanel;
    public GameObject[] rankPanelText;
    public GameObject rankPanelToMainBtn;
    public GameObject storePanel;
    public GameObject adsToStoreBtn;
    public GameObject shareToRankBtn;
    public GameObject storeCoinText;
    public GameObject storeStarText;

    // 옵션메뉴
    public GameObject BGMslider;
    public GameObject EffectSoundslider;
    public GameObject BGMimage;
    public GameObject EffectSoundimage;
    public GameObject soundSaveBtn;

    // 플레이 화면
    public Image[] healthImage;
    public Text pointText;
    public Text stageText;
    public Text coinText;
    public Text starText;
    public GameObject leftBtn;
    public GameObject rightBtn;
    public GameObject upBtn;
    public GameObject escBtn;
    public GameObject portionBtn;
    public GameObject invincibilityBtn;

    // 성공 & 실패 메뉴
    public GameObject mainOptionToMainBtn;
    public GameObject continueBtn;
    public GameObject subMenuToSubOptionBtn;
    public GameObject subMenuOptionToSubMenuBtn;
    public GameObject celebrateAndEncourageText;
    public GameObject pointClearAndFailText;
    public GameObject coinClearAndFailText;
    public GameObject starClearAndFailText;
    public GameObject coinClearAndFailImage;
    public GameObject starClearAndFailImage;
    public GameObject clearAndFailToMainBtn;
    public GameObject nameInputField;
    public GameObject pointRecordText;
    public GameObject pointRecordBtn;
    

    // BGM 관리
    public CharacterMove characterMove;
    AudioSource audioSource;
    [SerializeField] Sound[] bgmSounds;
    [SerializeField] AudioSource bgmPlayer;

    void Awake()
    {
        // 메인에서 0번째 음악 재생
        bgmPlayer.clip = bgmSounds[0].clip;
        bgmPlayer.Play();

        // 메인에서 시간 멈추기
        Pause();

        // 메인에서 ESC 동작 막기
        isSubMenuOption = true;

        // 메인메뉴 체크
        isMain = true;

        // 컴포넌트 초기화
        audioSource = GetComponent<AudioSource>();

        // 화면 꺼짐 방지
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 물 꺼주기
        VisibleWater(false);

        // 사운드 로드
        SoundLoad();

        // 랭크 로드
        RankLoad();

        // 상점 로드
        StoreLoad();

        // 기록테스트용
        //PlayerPrefs.DeleteAll();
    }

    void Update()
    {
        // UI 업데이트
        pointText.text = (totalPoint + stagePoint).ToString() + " Point";
        coinText.text = " X " + coinCount.ToString();
        starText.text = " X " + starCount.ToString();

        if(isSubMenuOption==false) // 서브메뉴옵션에서 ESC 동작 막기
        {
            if (Input.GetButtonDown("Cancel")) // ESC를 누르면
            {
                if (isSubMenu == false) // 서브메뉴가 띄워져있는 상태가 아니면
                {
                    SubMenuActive(); // 서브메뉴 띄우기
                }
                else // 서브메뉴가 띄워져있는 상태이면
                {
                    Continue(); // 플레이로
                }
            }
        }
        if(hasPortion==true)
        {
            if (Input.GetKeyDown(KeyCode.V)) // V를 누르면 포션사용
            {
                // HP가 풀이면 리턴
                if (health == 3)
                {
                    return;
                }

                // 포션 비활성화
                portionBtn.SetActive(false);

                // health 증가
                health++;

                // healthImage 업데이트
                healthImage[health - 1].color = new Color(1, 1, 1, 1);

                // 포션 체크
                hasPortion = false;

                // 사운드
                characterMove.audioSource.clip = characterMove.audioPortion;
                characterMove.audioSource.Play();
            }
        }

        if(hasInvincibility == true)
        {
            if (Input.GetKeyDown(KeyCode.B)) // B를 누르면 무적아이템 사용
            {
                // 이미 무적 상태이면 리턴
                if (characterMove.gameObject.layer == 11)
                {
                    return;
                }

                // 무적아이템 비활성화
                invincibilityBtn.SetActive(false);

                // 레이어를 11.CharacterDamaged로 변경
                characterMove.gameObject.layer = 11;

                // 잠시 투명
                characterMove.spriteRenderer.color = new Color(1, 1, 1, 0.4f);

                // 무적아이템 5초뒤에 해제
                Invoke("OffInvincibility", 5);

                // 무적아이템 체크
                hasInvincibility = false;

                // 사운드
                characterMove.audioSource.clip = characterMove.audioInvincibility;
                characterMove.audioSource.Play();
            }
        }
    }

    public void NextStage() // 다음 스테이지로 설정하는 함수
    {
        // 포인트 누적
        totalPoint += stagePoint;

        // 현재 스테이지 포인트 0으로
        stagePoint = 0;

        // HP 리셋
        ResetHP();
        
        // 상자 체크
        characterMove.isOpen = false;
        
        if (stageIndex < Stages.Length - 1) // 스테이지가 남아있다면
        {
            // 현재 스테이지 비활성화
            Stages[stageIndex].SetActive(false);

            // 스테이지 인덱스 증가
            stageIndex++;

            // 다음 스테이지 활성화
            Stages[stageIndex].SetActive(true);

            // 캐릭터를 원래 위치로
            CharacterReposition();

            // 스테이지 UI 업데이트
            stageText.text = "STAGE " + (stageIndex + 1);

            // Start() -> 0번째 음악 재생
            // StartGame() -> 1번째 음악 재생
            // 여기까지 stageIndex는 0이며 다음스테이지로 갈때 stageIndex는 1이되고 인덱스 2 음악을 재생
            bgmPlayer.clip = bgmSounds[stageIndex + 1].clip;
            bgmPlayer.Play();
        }
        else // 마지막 스테이지면
        {
            // 성공 로직
            ClearAndFailLogic("축하해요!");

            // 점수기록텍스트 이름입력 점수기록버튼 활성화
            pointRecordText.SetActive(true);
            pointRecordBtn.SetActive(true);
            nameInputField.SetActive(true);
        }
    }

    public void RankRecord() // 점수기록버튼을 누를때 사용하는 함수
    {
        // 인덱스가 꽉 찼으면 리턴
        if(recordIndex == 20)
        {
            return;
        }

        // 점수기록 UI 비활성화
        nameInputField.SetActive(false);
        pointRecordBtn.SetActive(false);
        pointRecordText.SetActive(false);

        // 랭크판넬텍스트에 이름 & 점수 기록
        rankPanelText[recordIndex].GetComponentInChildren<Text>().text = nameInputField.GetComponent<InputField>().text + " " + totalPoint.ToString() + " Point";

        // 기록인덱스 증가
        recordIndex++;

        // 랭크 세이브
        RankSave();

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void HealthDown() // HP 감소 함수
    {
        if (health > 1) // HP가 2개까지는
        {
            // HP 감소
            health--;

            // HP UI 업데이트
            healthImage[health].color = new Color(1, 0, 0, 0.4f);
        }
        else // HP가 한개 남았을때
        {
            if(stageIndex == 0)
            {
                totalPoint = stagePoint;
            }

            // 캐릭터가 죽음
            characterMove.OnDie();

            // HP UI 업데이트
            healthImage[0].color = new Color(1, 0, 0, 0.4f);

            // 실패 로직
            ClearAndFailLogic("아쉬워요!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision) // 캐릭터가 땅에 떨어졌을때 사용하는 함수
    {
        if (collision.gameObject.tag == "Character") // 캐릭터가 땅에 떨어졌으면
        {
            // HP가 2개까지는 캐릭터를 원래 위치로
            if (health > 1)
            {
                CharacterReposition();

                // 사운드
                characterMove.PlaySound("DAMAGED");
            }

            // HP가 한개 남았을때는 HP감소 함수만 호출
            HealthDown();
        }
    }

    public void CharacterReposition() // 캐릭터를 원래 위치로 설정하는 함수
    {
        // 캐릭터를 원래 위치로 설정
        characterMove.transform.position = new Vector3(0, 3, 0);

        // 낙하속도를 0으로 만드는 함수 호출
        characterMove.VelocityZero();
    }

    public void ClearAndFailToMain() // 성공 & 실패에서 메인메뉴로 가는 함수
    {
        // Scene 복구
        SceneManager.LoadScene(0);

        // 메인상태 체크
        isMain = true;

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void ResetHP() // HP 리셋 함수
    {
        health = 3;
        healthImage[1].color = new Color(1, 1, 1, 1);
        healthImage[2].color = new Color(1, 1, 1, 1);
    }

    public void StartGame() // 게임 시작 함수
    {
        // 메인배경 & 메인메뉴 비활성화
        mainBackground.SetActive(false);
        VisibleMain(false);

        // 1번째 음악 재생
        bgmPlayer.clip = bgmSounds[1].clip;
        bgmPlayer.Play();

        // 시간 재생
        UnPause();

        // ESC 활성화
        isSubMenuOption = false;

        // 모바일 UI 활성화
        // VisibleMobileUI(true);

        // 물 보이기
        VisibleWater(true);

        // 메인상태 체크
        isMain = false;

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void MainToMainOption() // 메인에서 메인옵션으로가는 함수
    {
        // 메인메뉴 비활성화
        VisibleMain(false);

        // 옵션 활성화
        VisibleOption(true);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void ExitGame() // 게임 종료 함수
    {
        // 사운드
        characterMove.PlaySound("BUTTON");

        // 종료
        Application.Quit();
    }

    public void MainOptionToMain() // 메인옵션에서 메인화면으로 가는 함수
    {
        // 옵션 비활성화
        VisibleOption(false);

        // 메인메뉴 활성화
        VisibleMain(true);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }


    public void Information() // 게임 정보 함수
    {
        // 메인메뉴 비활성화
        VisibleMain(false);

        // 게임 정보 메뉴 활성화
        infoPanel.SetActive(true);

        // 게임 정보에서 메인메뉴로가는 버튼 활성화
        infoToMainBtn.SetActive(true);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void InfoToMain() // 정보에서 메인으로 가는 함수
    {
        // 메인메뉴 활성화
        VisibleMain(true);

        // 게임 정보 메뉴 비활성화
        infoPanel.SetActive(false);

        // 게임 정보에서 메인메뉴로가는 버튼 비할성화
        infoToMainBtn.SetActive(false);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void Tutorial() // 튜토리얼
    {
        // 메인메뉴 비활성화
        VisibleMain(false);

        // 튜토리얼 활성화
        tutorialPanel.SetActive(true);

        // 튜토리얼에서 메인메뉴로 가는 버튼 활성화
        tutorialToMainBtn.SetActive(true);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void TutorialToMain() // 튜토리얼에서 메인메뉴로 가는 함수
    {
        // 메인메뉴 활성화
        VisibleMain(true);

        // 튜토리얼 비활성화
        tutorialPanel.SetActive(false);

        // 튜토리얼에서 메인메뉴로 가는 버튼 비활성화
        tutorialToMainBtn.SetActive(false);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void Rank() // 랭크시스템으로 가는 함수
    {
        // 메인메뉴 비활성화
        VisibleMain(false);

        // 랭크판넬 활성화
        rankPanel.SetActive(true);

        // 랭크판넬에서 메인으로가는버튼 활성화
        rankPanelToMainBtn.SetActive(true);

        //사운드
        characterMove.PlaySound("BUTTON");
    }

    public void RankToMain() // 랭크시스템에서 메인으로가는 함수
    {
        // 메인메뉴 활성화
        VisibleMain(true);

        // 랭크판넬 비활성화
        rankPanel.SetActive(false);

        // 랭크판넬에서 메인으로가는버튼 비활성화
        rankPanelToMainBtn.SetActive(false);

        //사운드
        characterMove.PlaySound("BUTTON");
    }

    public void RankToShare() // 랭크시스템에서 공유로가는 함수
    {
        // 랭크패널 비활성화
        rankPanel.SetActive(false);

        // 랭크에서 메인으로가는 버튼 비활성화
        rankPanelToMainBtn.SetActive(false);

        // 미구현함수 호출
        NotImplement("share");

        // 공유에서 랭크로 가는 버튼 활성화
        shareToRankBtn.SetActive(true);

        //사운드
        characterMove.PlaySound("BUTTON");

    }

    public void ShareToRank() // 공유에서 랭크시스템으로 가는 함수
    {
        // 미구현패널 비활성화
        notImplementPanel.SetActive(false);

        // 공유에서 랭크로 가는 버튼 비활성화
        shareToRankBtn.SetActive(false);

        // 랭크패널 활성화
        rankPanel.SetActive(true);

        // 랭크에서 메인으로가는 버튼 활성화
        rankPanelToMainBtn.SetActive(true);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void Store() // 상정시스템으로 가는 함수
    {
        // 메인화면 비활성화
        VisibleMain(false);

        // 상점 UI 활성화
        storePanel.SetActive(true);

        //사운드
        characterMove.PlaySound("BUTTON");
    }

    public void StoreToMain() // 상점시스템에서 메인으로가는 함수
    {
        // 메인화면 활성화
        VisibleMain(true);

        // 상점 UI 비활성화
        storePanel.SetActive(false);

        //사운드
        characterMove.PlaySound("BUTTON");
    }

    public void StoreToAds() // 상점시스템에서 광고로가는 함수
    {
        // 미구현함수 호출
        NotImplement("ads");

        // 상점 UI 비활성화
        storePanel.SetActive(false);

        // AdsToStoreButton 활성화
        adsToStoreBtn.SetActive(true);

        //사운드
        characterMove.PlaySound("BUTTON");
    }

    public void AdsToStore() // 광고에서 상점시스템으로 가는 함수
    {
        // 미구현패널 비활성화
        notImplementPanel.SetActive(false);

        // AdsToStoreButton 비활성화
        adsToStoreBtn.SetActive(false);

        // 상점 UI 활성화
        storePanel.SetActive(true);

        //사운드
        characterMove.PlaySound("BUTTON");
    }

    public void NotImplement(string funcName) // 미구현 함수
    {
        // 미구현 정보 활성화
        notImplementPanel.SetActive(true);
        Text comingSoonText = notImplementPanel.GetComponentInChildren<Text>();
        comingSoonText.text = funcName + " function is not implemented!";

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void NotImplementToMain() // 미구현 정보에서 메인으로 가는 함수
    {
        // 미구현 정보 비활성화
        notImplementPanel.SetActive(false);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void SubMenuActive() // 서브 메뉴 함수
    {
        // 시간 멈추기
        Pause();

        // 메인배경 활성화
        mainBackground.SetActive(true);

        // 서브메뉴 UI 활성화
        SubMenuUI(true);

        // 서브메뉴 상태로 체크
        isSubMenu = true;

        // 모바일 UI 비활성화
        // VisibleMobileUI(false);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void SubMenuToSubOption() // 서브메뉴에서 서브메뉴옵션으로가는 함수
    {
        // 서브메뉴 UI 비활성화
        SubMenuUI(false);

        // 서브메뉴옵션 UI 활성화
        SubMenuOptionUI(true);

        // 서브메뉴옵션에서 서브메뉴로가는 버튼 활성화
        subMenuOptionToSubMenuBtn.SetActive(true);

        // 서브메뉴옵션 상태로 체크
        isSubMenuOption = true;

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void SubMenuOptionToSubMenu() // 서브메뉴옵션에서 서브메뉴로 가는 함수
    {
        // 서브메뉴 UI 활성화
        SubMenuUI(true);

        // 서브메뉴옵션에서 서브메뉴로가는 버튼 비활성화
        subMenuOptionToSubMenuBtn.SetActive(false);

        // 서브메뉴옵션UI 비활성화
        SubMenuOptionUI(false);

        // 서브메뉴옵션 상태 풀기
        isSubMenuOption = false;

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void Continue() // 서브메뉴에서 플레이화면으로 가는 함수
    {
        // 시간 풀기
        UnPause();

        // 메인배경 비활성화
        mainBackground.SetActive(false);

        // 서브메뉴 UI 비활성화
        SubMenuUI(false);

        // 서브메뉴에서 옵션으로가는버튼 비활성화
        subMenuToSubOptionBtn.SetActive(false);

        // 서브메뉴상태가 아님
        isSubMenu = false;

        // 모바일 UI 활성화
        // VisibleMobileUI(true);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void VisibleMain(bool b) // 메인메뉴 함수
    {
        // 메인메뉴
        redRunnerText.SetActive(b);
        startBtn.SetActive(b);
        mainToMainOptionBtn.SetActive(b);
        exitBtn.SetActive(b);
        characterimage.SetActive(b);
        enemyCharacterimage.SetActive(b);
        maceImage.SetActive(b);
        infoBtn.SetActive(b);
        storeBtn.SetActive(b);
        tutorialBtn.SetActive(b);
        rankBtn.SetActive(b);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void VisibleOption(bool b) // 옵션메뉴 함수
    {
        // 옵션 메뉴 보이게하기
        BGMslider.SetActive(b);
        EffectSoundslider.SetActive(b);
        mainOptionToMainBtn.SetActive(b);
        BGMimage.SetActive(b);
        EffectSoundimage.SetActive(b);
        soundSaveBtn.SetActive(b);

        // 사운드
        characterMove.PlaySound("BUTTON");
    }

    public void SubMenuUI(bool b) // 서브메뉴 UI 함수
    {
        continueBtn.SetActive(b);
        subMenuToSubOptionBtn.SetActive(b);
        exitBtn.SetActive(b);
        characterimage.SetActive(b);
        enemyCharacterimage.SetActive(b);
    }

    public void SubMenuOptionUI(bool b) // 서브메뉴옵션 UI 함수
    {
        BGMslider.SetActive(b);
        EffectSoundslider.SetActive(b);
        BGMimage.SetActive(b);
        EffectSoundimage.SetActive(b);
        soundSaveBtn.SetActive(b);
    }

    public void ClearAndFailLogic(string message) // 성공 & 실패 함수
    {
        // 시간 멈추기
        Pause();

        // 메인배경 활성화
        mainBackground.SetActive(true);

        // 축하메세지 활성화
        celebrateAndEncourageText.SetActive(true);
        Text celebrateText = celebrateAndEncourageText.GetComponentInChildren<Text>();
        celebrateText.text = message;

        // 점수 활성화
        pointClearAndFailText.SetActive(true);
        Text pointClearText = pointClearAndFailText.GetComponentInChildren<Text>();
        pointClearText.text = totalPoint.ToString() + " Point ";

        // 코인 갯수 활성화
        coinClearAndFailText.SetActive(true);
        Text coinClearText = coinClearAndFailText.GetComponentInChildren<Text>();
        coinClearText.text = " X " + coinCount.ToString();

        // 별 갯수 활성화
        starClearAndFailText.SetActive(true);
        Text starClearText = starClearAndFailText.GetComponentInChildren<Text>();
        starClearText.text = " X " + starCount.ToString();

        // 성공 & 실패에서 메인으로가는 버튼 활성화
        clearAndFailToMainBtn.SetActive(true);

        // EXIT 버튼 활성화
        exitBtn.SetActive(true);

        // ESC 동작 막기
        isSubMenuOption = true;

        // 코인이미지 & 별이미지 활성화
        coinClearAndFailImage.SetActive(true);
        starClearAndFailImage.SetActive(true);

        // 모바일UI 비활성화
        // VisibleMobileUI(false);

        // 배경음악 Off
        bgmPlayer.Stop();

        // 상점에서 사용 할 코인 & 별에 현재 코인 & 별 갯수 더하기
        storeCoin += coinCount;
        storeStar += starCount;

        // 상점에서 사용 할 코인 & 별 상점에 텍스트 표시
        storeCoinText.GetComponentInChildren<Text>().text = " X " + storeCoin;
        storeStarText.GetComponentInChildren<Text>().text = " X " + storeStar;

        // 상점 세이브
        StoreSave();
    }

    // public void VisibleMobileUI(bool b) // 모바일 UI 함수
    // {
    //     leftBtn.SetActive(b);
    //     rightBtn.SetActive(b);
    //     upBtn.SetActive(b);
    //     escBtn.SetActive(b);
    // }

    public void Pause() // 시간을 멈추는 함수
    {
        Time.timeScale = 0;
    }

    public void UnPause() // 시간을 재생하는 함수
    {
        Time.timeScale = 1;
    }

    public void SetBGMVolume(float volume) // 배경음 조절 함수
    {
        audioSource.volume = volume;
    }

    public void VisibleWater(bool b) // 물 오브젝트 활성화
    {
        water1.SetActive(b);
        water2.SetActive(b);
        water3.SetActive(b);
        water4.SetActive(b);
        water5.SetActive(b);
        water6.SetActive(b);
        water7.SetActive(b);
        water8.SetActive(b);
        water9.SetActive(b);
        water10.SetActive(b);
    }

    public void SoundSave() // 사운드 세이브
    {
        // 슬라이더 참조형으로 받고
        Slider bgmSliderValue = BGMslider.GetComponentInChildren<Slider>();
        Slider effectSoundSliderValue = EffectSoundslider.GetComponentInChildren<Slider>();

        // 현재 볼륨을 프리팹에 저장
        PlayerPrefs.SetFloat("BGMVolume", audioSource.volume);
        PlayerPrefs.SetFloat("EffectVolume", characterMove.audioSource.volume);

        // 현재 슬라이더값을 프리팹에 저장
        PlayerPrefs.SetFloat("BGMSliderValue", bgmSliderValue.value);
        PlayerPrefs.SetFloat("EffectSliderValue", effectSoundSliderValue.value);

        // 프리팹 저장
        PlayerPrefs.Save();

        //사운드
        characterMove.PlaySound("BUTTON");
    }

    public void SoundLoad() // 사운드 로드
    {
        // 저장된 값이 없으면 리턴
        if(!PlayerPrefs.HasKey("BGMVolume"))
        {
            return;
        }

        // 변수에 사운드 저장
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        float effectVolume = PlayerPrefs.GetFloat("EffectVolume");

        // 변수에 슬라이더값 저장
        float sliderValueBgm = PlayerPrefs.GetFloat("BGMSliderValue");
        float sliderValueEffectSound = PlayerPrefs.GetFloat("EffectSliderValue");
        
        // 저장된 사운드 슬라이더값 로드
        audioSource.volume = bgmVolume;
        characterMove.audioSource.volume = effectVolume;
        BGMslider.GetComponentInChildren<Slider>().value = sliderValueBgm;
        EffectSoundslider.GetComponentInChildren<Slider>().value = sliderValueEffectSound;
    }

    public void RankSave() // 랭크 세이브
    {
        // 토큰으로 구분해서 모든텍스트를 문자열에 저장
        string rankPanelTextArr = "";
        for (int i = 1; recordIndex - i >= 0; i++)
        {
            rankPanelTextArr = rankPanelTextArr + rankPanelText[recordIndex - i].GetComponentInChildren<Text>().text;
            if(recordIndex - i >= 0)
            {
                rankPanelTextArr = rankPanelTextArr + ",";
            }
        }

        // 기록 인덱스 저장
        PlayerPrefs.SetInt("RecordIndex", recordIndex);

        // 랭크판넬텍스트 저장
        PlayerPrefs.SetString("RankPanelText", rankPanelTextArr);

        // 프리팹 저장
        PlayerPrefs.Save();
    }

    public void RankLoad() // 랭크 로드
    {
        // 저장된값이 없으면 리턴
        if (!PlayerPrefs.HasKey("RecordIndex"))
        {
            return;
        }

        // 토큰으로 분기해서 변수에 저장
        int recordindex = PlayerPrefs.GetInt("RecordIndex");
        string[] rankPanelTextArr = PlayerPrefs.GetString("RankPanelText").Split(',');

        // 저장된 값 로드
        recordIndex = recordindex;
        for (int i = 1; recordIndex - i >= 0; i++)
        {
            rankPanelText[recordIndex - i].GetComponentInChildren<Text>().text = rankPanelTextArr[i - 1];
        }
    }

    public void StoreSave() // 상점 세이브
    {
        // 상점에서 사용 할 코인 & 별의 갯수를 저장하고 텍스트를 저장
        PlayerPrefs.SetInt("StoreCoin", storeCoin);
        PlayerPrefs.SetInt("StoreStar", storeStar);
        PlayerPrefs.SetString("StoreCoinText", storeCoinText.GetComponentInChildren<Text>().text);
        PlayerPrefs.SetString("StoreStarText", storeStarText.GetComponentInChildren<Text>().text);

        // 프리팹 저장
        PlayerPrefs.Save();
    }

    public void StoreLoad() // 상점 로드
    {
        // 저장된값이 없으면 리턴
        if (!PlayerPrefs.HasKey("StoreCoin"))
        {
            return;
        }
        
        // 변수에 저장
        int storecoin = PlayerPrefs.GetInt("StoreCoin");
        int storestar = PlayerPrefs.GetInt("StoreStar");
        string storecointext = PlayerPrefs.GetString("StoreCoinText");
        string storestartext = PlayerPrefs.GetString("StoreStarText");

        // 저장된 값 로드
        storeCoin = storecoin;
        storeStar = storestar;
        storeCoinText.GetComponentInChildren<Text>().text = storecointext;
        storeStarText.GetComponentInChildren<Text>().text = storestartext;
    }

    public void Portion() // 포션 함수
    {
        // HP가 풀이면 리턴
        if(health == 3)
        {
            return;
        }

        // 포션 비활성화
        portionBtn.SetActive(false);

        // health 증가
        health++;

        // healthImage 업데이트
        healthImage[health - 1].color = new Color(1, 1, 1, 1);

        // 포션 체크
        hasPortion = false;

        // 사운드
        characterMove.audioSource.clip = characterMove.audioPortion;
        characterMove.audioSource.Play();
    }

    public void OnInvincibility() // 무적아이템 함수
    {
        // 이미 무적 상태이면 리턴
        if(characterMove.gameObject.layer == 11)
        {
            return;
        }

        // 무적아이템 비활성화
        invincibilityBtn.SetActive(false);

        // 레이어를 11.CharacterDamaged로 변경
        characterMove.gameObject.layer = 11;

        // 잠시 투명
        characterMove.spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 무적아이템 5초뒤에 해제
        Invoke("OffInvincibility", 5);

        // 무적아이템 체크
        hasInvincibility = false;

        // 사운드
        characterMove.audioSource.clip = characterMove.audioInvincibility;
        characterMove.audioSource.Play();
    }

    public void OffInvincibility() // 무적아이템 해제 함수
    {
        // 레이어를 10.Character로 변경
        characterMove.gameObject.layer = 10;

        // 투명 해제
        characterMove.spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}

// 메인메뉴 (메인배경 : mainBackground)
// 시작하기 옵션 종료하기 캐릭터 적캐릭터를 보이게하고 배경음악 효과음 슬라이더와 메인메뉴로가는 버튼을 안보이게하기
// 시작하기를 누르면 메인화면 시작하기 옵션 종료하기 캐릭터 적캐릭터 안보이게하기
// 옵션을 누르면 시작하기 옵션 종료하기 캐릭터 적캐릭터 안보이게하고 배경음악 효과음 슬라이더와 메인화면으로가는 버튼 보이게하기
// 메인화면으로가는 버튼을 누르면 배경음악 효과음 슬라이더와 메인화면으로가는 버튼을 안보이게하고 시작하기 옵션 종료하기 캐릭터 적캐릭터를 보이게하기
// 종료하기를 누르면 게임 종료

// 서브메인
// ESC를 누르면 시간을 멈추고 메인화면 계속하기(Continue버튼) 서브메뉴에서옵션으로가는버튼(GoOption버튼) 끝내기(Exit버튼) 캐릭터 적캐릭터를 보이게하기
// GoOption버튼을 누르면 계속하기 Continue GoOption Exit 캐릭터 적캐릭터를 안보이게하고 배경음악 효과음 슬라이더와 옵션에서 서브메뉴로가는버튼(GoSubMenu)를 보이게하기
// GoSubMenu버튼을 누르면 Continue GoOption Exit 캐릭터 적캐릭터를 보이게하고 배경음악 효과음 슬라이더와 GoSubMenu를 안보이게하기
// Continue버튼을 누르면 시간을 재생하고 메인화면 Continue GoOption Exit 캐릭터 적캐릭터를 안보이게하기
// 서브메뉴옵션에서는 ESC를 동작 못하게하기

// 성공 & 실패(총점수 코인갯수 별갯수 다시시작하기 끝내기)
// 시간을 멈추고 메인화면을 띄우고 totalPoint텍스트 coinCount텍스트 starCount텍스트 메인메뉴로가는버튼 EXIT버튼을 보여주고 ESC동작 막기

// 모바일 버튼
// leftBtn rightBtn upBtn escBtn

// 정보
// Info 버튼을 누르면 메인메뉴 비활성화 InfoPanel과 InfoToMainButton이 활성화
// InfoToMainButton을 누르면 InfoToMainButton과 InfoPanel이 비활성화 메인메뉴가 활성화

// 미구현
// 미구현된 아이콘을 누르면 메인메뉴 비활성화 NotImplementPanel과 NotImplementToMainButton이 활성화
// NotImplementToMainButton을 누르면 NotImplementToMainButton과 NotImplementPanel이 비활성화 메인메뉴가 활성화

// 튜토리얼
// TutorialBtn을 누르면 메인메뉴 비활성화 TutorialPanel과 TutorialToMainBtn이 활성화
// TutorialPanel 하위에는 모바일버튼 이미지와 텍스트로 설명
// TutorialToMainBtn을 누르면 TutorailPanel과 TutorialToMainBtn이 비활성화 메인메뉴가 활성화

// 대화
// DadCharacter에 콜라이더를 넣고 범위를 넓히고 tag를 DadCharacter로
// 캐릭터가 OnCollisionEnter2D로 tag가 DadCharacter와 충돌하면 처음대화와 DialogueCharacterImage와 커서가 활성화
// isTalk변수를 만들어서 true로 해서 isTalk가 true일때 캐릭터가 이동하지못하도록 막기
// 커서버튼을 누르게되면 캐릭터이미지와 대화내용을 바꿔주고
// 마지막 대화가 끝나면 DadCharacter가 사라지고
// isTalk를 false로해서 움직이기

// 사운드 저장 
// 배경음이 조절될때 슬라이더값이 저장되고
// 효과음이 조절될때 슬라이더값이 저장되고
// 메인으로 갈때 저장된 슬라이더값 로드

// 점수기록 저장
// 성공할때 점수기록텍스트와 이름인풋필드와 랭크등록버튼 활성화
// 랭크등록버튼을 누르면  점수기록텍스트와 이름인풋필드와 랭크등록버튼이 비활성화
// 메인메뉴에서 랭크버튼을 누르면 랭크판넬 활성화
// 랭크판넬에서메인으로가는버튼을 누르면 랭크판넬 비활성화
// 랭크시스템판넬에 이름과 점수가 기록된다
// 그러면 이름텍스트와 토탈포인트를 랭크판넬의 텍스트에 넣고
// 랭크판넬 텍스트는 배열로 선언해서 인덱스 이용해서 접근
// 저장되어야할게 기록인덱스하고 랭크판넬에 기록된 텍스트

// 상점시스템

// 코인 & 별 저장
// storeCoin & storeStar 생성하고 성공 & 실패시 coinCount & starCount를 더해서저장
// storeCoinText에 " X " + storeCoin 를 쓰고 storeStarText에 " X " + storeStar을 쓰고
// 저장기능으로 storeCoin & storeStar을 저장하고 storeCoinText & storeStarText를 저장하고
// start에서 storeCoin & storeStar 을 로드하고 storeCoinText & storeStarText를 로드하고

// 아이템 사용
// Portion을 사용하면 Portion비활성화 health++ healthImage[health] 원래색으로
// Invincibility를 사용하면 Invincibility비활성화
// OnInvincibility()에서 캐릭터 5초동안 투명 및 캐릭터 레이어 CharacterDamaged로 변경 Invoke("OffInvincibility", 5)
// OoffInvincibility()에서 캐릭터 원래색으로 캐릭터 레이어 Character로 변경

// 캐릭터 스킨 상점
// 공유 시스템
// 광고 시스템
