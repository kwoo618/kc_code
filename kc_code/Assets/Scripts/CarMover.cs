//using UnityEngine;

//public class CarMover : MonoBehaviour
//{
//    [Header("이동 설정")]
//    public Transform startPoint; // 출발 지점
//    public Transform endPoint;   // 도착 지점
//    public float speed = 5f;     // 이동 속도

//    [Header("애니메이션 설정")]
//    public bool isHorizontalCar = false; // 가로로 움직이는 차인가? (체크하면 위아래로 둥실거림)
//    public float bobSpeed = 10f;  // 둥실거리는 속도
//    public float bobAmount = 0.05f; // 둥실거리는 높이

//    [Header("충돌 설정")]
//    public int damageCooldown = 2; // 연속 충돌 방지 시간 (초)

//    private float _canDamageTime = 0f;

//    void Start()
//    {
//        // 시작 위치로 초기화
//        transform.position = startPoint.position;
//    }

//    void Update()
//    {
//        // 1. 일단 목적지 쪽으로 X축만 이동한다고 가정 (기본 이동)
//        transform.position = Vector3.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);

//        // 2. 가로 방향 차라면 -> Y축(높이)을 강제로 사인파(Sin)로 덮어씌움
//        if (isHorizontalCar)
//        {
//            // 원래 높이(startPoint.y)를 기준으로 위아래로 흔들림
//            float newY = startPoint.position.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;

//            // 현재 X, Z는 유지하고 Y만 교체
//            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
//        }

//        // 3. 도착 체크 및 리셋
//        if (Vector3.Distance(transform.position, endPoint.position) < 0.1f)
//        {
//            transform.position = startPoint.position;
//        }
//    }

//    // 플레이어와 충돌 처리
//    void OnTriggerEnter2D(Collider2D other)
//    {
//        // 쿨타임이 지났고, 충돌한 대상이 플레이어라면
//        if (other.CompareTag("Player") && Time.time >= _canDamageTime)
//        {
//            _canDamageTime = Time.time + damageCooldown; // 쿨타임 갱신
//            GameManager.instance.OnCarAccident(); // 돈 깎기
//        }
//    }


//}