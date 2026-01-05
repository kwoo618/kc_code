using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("--- 오디오 소스 연결 ---")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("--- 배경음악 (BGM) ---")]
    public AudioClip titleBgm;
    public AudioClip mainBgm;
    public AudioClip bankBgm;
    public AudioClip academyBgm;
    public AudioClip homeBgm;
    public AudioClip officeBgm;   // [추가] 회사 내부 배경음

    [Header("--- 효과음 (SFX) ---")]
    public AudioClip clickSfx;
    public AudioClip nextMonthSfx;
    public AudioClip eatSfx;
    public AudioClip crashSfx;
    public AudioClip doorSfx;
    public AudioClip alertSfx;

    [Header("--- 결과 효과음 ---")]
    public AudioClip successBgm;
    public AudioClip failMoneyBgm;
    public AudioClip failStressBgm;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // --- 장소별 편의 함수 ---
    public void PlayMainBGM() => PlayBGM(mainBgm);
    public void PlayBankBGM() => PlayBGM(bankBgm);
    public void PlayAcademyBGM() => PlayBGM(academyBgm);
    public void PlayHomeBGM() => PlayBGM(homeBgm);
    public void PlayOfficeBGM() => PlayBGM(officeBgm); // [추가] 회사 배경음 재생 함수
}