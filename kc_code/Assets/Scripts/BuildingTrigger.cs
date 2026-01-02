using UnityEngine;

public class BuildingTrigger : MonoBehaviour
{
    [Header("연결할 패널 (직접 드래그해서 넣으세요)")]
    public GameObject targetPanel; // 예: Bank_Panel

    private bool isPlayerEnter = false;

    // 플레이어가 투명 영역에 들어왔을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // 플레이어 태그 확인
        {
            isPlayerEnter = true;
            // (선택) 여기에 "Space바를 눌러 입장하세요" 같은 안내 문구를 띄울 수 있음
        }
    }

    // 플레이어가 나갔을 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerEnter = false;
            if (targetPanel != null)
                targetPanel.SetActive(false); // 멀어지면 창 닫기
        }
    }

    private void Update()
    {
        // 영역 안에 있고 + 스페이스 바를 눌렀을 때 + 패널이 안 켜져 있다면
        if (isPlayerEnter && Input.GetKeyDown(KeyCode.Space))
        {
            if (targetPanel != null && !targetPanel.activeSelf)
            {
                // 다른 패널들이 열려있을 수 있으니 닫고 여는 것이 안전함
                GameManager.instance.CloseAllPanels();
                targetPanel.SetActive(true);
            }
        }
    }
}