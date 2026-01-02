using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    [Header("이동할 목적지 좌표")]
    public Vector2 targetPosition; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 물체가 Player 태그를 가지고 있는지 확인
        if (collision.CompareTag("Player"))
        {
            // 1. 플레이어 위치를 목적지 좌표로 순간이동
            collision.transform.position = new Vector3(targetPosition.x, targetPosition.y, collision.transform.position.z);
            
            // 2. 카메라를 즉시 해당 위치로 이동시킴 (새로 추가된 부분)
            // 메인 카메라를 찾아 그 안에 붙어있는 CameraFollow 스크립트를 가져옵니다.
            CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
            
            if (camFollow != null)
            {
                // 앞서 CameraFollow.cs에 추가했던 TeleportCamera 함수를 호출합니다.
                camFollow.TeleportCamera();
            }
            
            Debug.Log($"텔레포트 및 카메라 고정 완료: {targetPosition}");
        }
    }
}