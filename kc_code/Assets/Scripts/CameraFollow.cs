using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // 따라갈 대상 (플레이어)
    public float smoothSpeed = 0.125f; // 따라가는 속도 (0~1 사이, 작을수록 부드러움)
    public Vector3 offset = new Vector3(0, 0, -10); // 카메라 거리 (Z값 -10 유지 필수)

    // 맵 밖으로 카메라가 못 나가게 하려면 아래 값 설정 (필요 없으면 무시)
    public bool useLimit = false;
    public Vector2 minLimit;
    public Vector2 maxLimit;

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // 부드러운 이동 (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 맵 제한 적용 (체크했을 경우만)
        if (useLimit)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minLimit.x, maxLimit.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minLimit.y, maxLimit.y);
        }

        transform.position = smoothedPosition;
    }
}