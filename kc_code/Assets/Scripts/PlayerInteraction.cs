using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // --- [퀴즈 트리거 우선 체크] ---
        // 퀴즈는 시간 정지(Time.timeScale == 0) 상태에서도 실행될 수 있도록 위로 올립니다.
        if (other.CompareTag("Quiz"))
        {
            Debug.Log("Quiz Trigger Detected!"); // 트리거 작동 확인용 로그
            GameManager.instance.ShowQuizConfirm();
            PlayAlertSound();
            return; // 퀴즈 실행 시 여기서 함수 종료 (다른 로직과 섞이지 않게 함)
        }

        // [주의] 퀴즈 이외의 다른 로직은 시간이 멈춰있을 때 실행되지 않도록 차단
        if (Time.timeScale == 0) return;

        // --- [1. BGM 체크 및 문 소리 재생] ---
        if (other.CompareTag("Home_BGM"))
        {
            SoundManager.instance.PlayHomeBGM();
            PlayDoorSound(); // 여기서 문 소리 추가!
        }
        else if (other.CompareTag("Bank_BGM"))
        {
            SoundManager.instance.PlayBankBGM();
            PlayDoorSound();
        }
        else if (other.CompareTag("Academy_BGM"))
        {
            SoundManager.instance.PlayAcademyBGM();
            PlayDoorSound();
        }
        else if (other.CompareTag("Quiz_BGM"))
        {
            SoundManager.instance.PlayOfficeBGM();
            PlayDoorSound();
        }

        // --- [2. UI 및 효과음 체크] ---
        if (other.CompareTag("Door"))
        {
            PlayDoorSound();
        }
        else if (other.CompareTag("Bank"))
        {
            GameManager.instance.bankPanel.SetActive(true);
            PlayAlertSound();
        }
        else if (other.CompareTag("Academy"))
        {
            GameManager.instance.academyPanel.SetActive(true);
            PlayAlertSound();
        }
        else if (other.CompareTag("Store"))
        {
            GameManager.instance.storePanel.SetActive(true);
            PlayAlertSound();
        }
        else if (other.CompareTag("NextTurn"))
        {
            PlayAlertSound();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 시간이 멈춘 상태라면 Exit 로직 무시 (패널 닫힘 방지)
        if (Time.timeScale == 0) return;

        if (other.CompareTag("Home_BGM") || other.CompareTag("Bank_BGM") || other.CompareTag("Academy_BGM") || other.CompareTag("Quiz_BGM"))
        {
            SoundManager.instance.PlayMainBGM();
            PlayDoorSound();
        }

        if (other.CompareTag("Bank") || other.CompareTag("Academy") || other.CompareTag("Store"))
        {
            GameManager.instance.CloseAllPanels();
        }
    }

    void PlayDoorSound()
    {
        if (SoundManager.instance != null && SoundManager.instance.doorSfx != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.doorSfx);
        }
    }

    void PlayAlertSound()
    {
        if (SoundManager.instance != null && SoundManager.instance.alertSfx != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.alertSfx);
        }
    }
}