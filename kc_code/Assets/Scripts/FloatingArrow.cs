using UnityEngine;

public class FloatingArrow : MonoBehaviour
{
    [Header("--- 움직임 설정 ---")]
    [Tooltip("화살표가 위아래로 움직이는 속도입니다. 숫자가 작을수록 천천히 움직입니다.")]
    [Range(0.1f, 5.0f)] // 인스펙터에서 슬라이더로 조절 가능하게 함
    public float moveSpeed = 3.0f;

    [Tooltip("중심점을 기준으로 위아래로 움직이는 최대 거리(반경)입니다.")]
    [Range(0.1f, 2.0f)]
    public float moveDistance = 0.2f;

    // 화살표의 원래 시작 위치를 저장할 변수
    private Vector3 startPosition;

    void Start()
    {
        // 게임이 시작될 때, 화살표를 배치해둔 현재 위치를 기준점으로 삼습니다.
        startPosition = transform.position;
    }

    void Update()
    {
        // Mathf.Sin(Time.time)은 시간이 지남에 따라 -1과 1 사이를 부드럽게 오가는 값을 만듭니다.
        // 여기에 moveSpeed를 곱해 속도를 조절하고,
        // moveDistance를 곱해 움직이는 범위를 결정합니다.
        float yOffset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        // 원래 시작 위치의 Y값에 계산된 오프셋을 더해서 새로운 위치를 적용합니다.
        // (X와 Z축은 원래 위치를 유지합니다.)
        transform.position = new Vector3(startPosition.x, startPosition.y + yOffset, startPosition.z);
    }
}