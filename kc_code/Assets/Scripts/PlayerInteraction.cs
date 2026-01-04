using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // [수정] 타이틀 화면(정지 상태)에서는 트리거가 BGM을 바꾸지 못하게 차단
        if (Time.timeScale == 0) return;

        // --- [1. BGM 체크] ---
        if (other.CompareTag("Home_BGM")) SoundManager.instance.PlayHomeBGM();
        else if (other.CompareTag("Bank_BGM")) SoundManager.instance.PlayBankBGM();
        else if (other.CompareTag("Academy_BGM")) SoundManager.instance.PlayAcademyBGM();

        // --- [2. UI 및 효과음 체크] ---
        if (other.CompareTag("Door"))
        {
            Debug.Log("Door 소리 재생");
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
        if (Time.timeScale == 0) return;

        if (other.CompareTag("Home_BGM") || other.CompareTag("Bank_BGM") || other.CompareTag("Academy_BGM"))
        {
            SoundManager.instance.PlayMainBGM();
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