using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("--- 오디오 소스 연결 ---")]
    public AudioSource bgmSource; // 배경음악용 (Loop 체크 필수)
    public AudioSource sfxSource; // 효과음용 (Loop 체크 해제)

    [Header("--- 배경음악 (BGM) ---")]
    public AudioClip titleBgm;    // 게임 시작 전 대기 화면
    public AudioClip mainBgm;     // 외부(거리) 기본 배경음
    public AudioClip bankBgm;     // 은행 내부
    public AudioClip academyBgm;  // 학원 내부
    public AudioClip homeBgm;     // 집 내부

    [Header("--- 효과음 (SFX) ---")]
    public AudioClip clickSfx;       // 일반 버튼 클릭
    public AudioClip nextMonthSfx;   // 다음달 정산 넘어가는 소리 (종이 넘기는 소리나 띵! 소리)
    public AudioClip eatSfx;         // 편의점 냠냠
    public AudioClip crashSfx;       // 교통사고 쾅!
    public AudioClip doorSfx;        // 건물 들어가고 나갈 때

    [Header("--- 결과 효과음 ---")]
    public AudioClip successBgm;     // 게임 클리어 (환호)
    public AudioClip failMoneyBgm;   // 파산 (슬픈 음악)
    public AudioClip failStressBgm;  // 과로 (엠뷸런스 등)

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // 씬 전환시에도 끊기지 않게 하려면 아래 주석 해제 (단일 씬이라 필수는 아님)
        // DontDestroyOnLoad(gameObject);
    }

    // BGM 재생 (이미 같은 곡이 재생 중이면 다시 틀지 않음)
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // 효과음 재생 (겹쳐서 재생 가능)
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    // 모든 소리 끄기 (게임 오버 시 등)
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // --- 장소별 편의 함수 ---
    public void PlayMainBGM() => PlayBGM(mainBgm);
    public void PlayBankBGM() => PlayBGM(bankBgm);
    public void PlayAcademyBGM() => PlayBGM(academyBgm);
    public void PlayHomeBGM() => PlayBGM(homeBgm);
}