using UnityEngine;

public class BedTrigger : MonoBehaviour
{
    [Header("연결할 잠자기 확인 패널")]
    public GameObject nextPanel;

    // 1. 플레이어가 침대 영역에 '들어오자마자' 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어인지 확인
        if (collision.CompareTag("Player"))
        {
            // 패널이 연결되어 있다면
            if (nextPanel != null)
            {
                // 안전하게 다른 창들은 닫아주고 (GameManager가 있다면)
                if (GameManager.instance != null)
                    GameManager.instance.CloseAllPanels();

                // 바로 잠자기 창을 켭니다!
                nextPanel.SetActive(true);
            }
        }
    }

    // 2. 플레이어가 침대 밖으로 '나가면' 실행
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 멀어지면 창 닫기
            if (nextPanel != null)
            {
                nextPanel.SetActive(false);
            }
        }
    }
}