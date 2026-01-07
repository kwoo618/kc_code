using UnityEngine;

public class CarMover : MonoBehaviour
{
    [Header("이동 설정")]
    public Transform startPoint; // 출발 지점
    public Transform endPoint;   // 도착 지점
    public float speed = 5f;     // 이동 속도

    [Header("애니메이션 설정")]
    public bool isHorizontalCar = false; // 가로로 움직이는 차인가? (체크하면 위아래로 둥실거림)
    public float bobSpeed = 10f;  // 둥실거리는 속도
    public float bobAmount = 0.05f; // 둥실거리는 높이

    [Header("충돌 설정")]
    public int damageCooldown = 2; // 연속 충돌 방지 시간 (초)

    private float _canDamageTime = 0f;

    void Start()
    {
        // 시작 위치로 초기화
        transform.position = startPoint.position;
    }

    void Update()
    {
        // 1. 목적지를 향해 이동
        transform.position = Vector3.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);

        // 2. 가로 방향 차라면 위아래로 둥실거리는 효과 추가
        if (isHorizontalCar)
        {
            // 현재 위치에 사인파(Sin Wave)를 더해 위아래 움직임 구현
            transform.position += Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmount * Time.deltaTime;
        }

        // 3. 도착 지점과 거의 가까워지면(거리 0.1 이하) 출발지로 순간이동 (루프)
        if (Vector3.Distance(transform.position, endPoint.position) < 0.1f)
        {
            transform.position = startPoint.position;
        }
    }

    // 플레이어와 충돌 처리
    void OnTriggerEnter2D(Collider2D other)
    {
        // 쿨타임이 지났고, 충돌한 대상이 플레이어라면
        if (other.CompareTag("Player") && Time.time >= _canDamageTime)
        {
            _canDamageTime = Time.time + damageCooldown; // 쿨타임 갱신
            GameManager.instance.OnCarAccident(); // 돈 깎기
        }
    }
}