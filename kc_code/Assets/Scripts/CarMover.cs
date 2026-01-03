using UnityEngine;
using System.Collections;

public class CarMover : MonoBehaviour
{
    [Header("--- 이동 설정 ---")]
    public Transform spawnPoint; // 시작 위치
    public Transform endPoint;   // 목적지
    public float speed = 5f;     // 이동 속도

    [Header("--- 애니메이션 (움직이는 느낌) ---")]
    public bool isHorizontalCar = false; 
    public float bobSpeed = 10f;         
    public float bobAmount = 0.05f;      

    [Header("--- 리스폰 설정 (시간) ---")]
    public float minRespawnTime = 5f;       // 도착 후 재등장까지 최소 시간
    public float maxRespawnTime = 8f;       // 도착 후 재등장까지 최대 시간

    [Header("--- 플레이어 충돌 설정 ---")]
    public int damageCooldown = 2; 
    private float _canDamageTime = 0f;

    private bool isActive = true;          
    private SpriteRenderer spriteRenderer;  

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SpawnCar();
    }

    void Update()
    {
        if (!isActive) return;

        // 1. 목적지를 향해 이동
        transform.position = Vector3.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);

        // 2. 엔진 진동/움직이는 느낌
        if (isHorizontalCar)
        {
            float newY = spawnPoint.position.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        // 3. 목적지에 도착했는지 확인 (가장 중요한 부분)
        if (Vector3.Distance(transform.position, endPoint.position) < 0.1f)
        {
            // 부딪히길 기다리지 않고, 목적지 도착 시 바로 리스폰 코루틴 실행
            StartCoroutine(RespawnTimer());
        }
    }

    void SpawnCar()
    {
        if (spawnPoint == null) return;
        
        transform.position = spawnPoint.position;
        isActive = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }

    IEnumerator RespawnTimer()
    {
        // 도착하는 순간 호출됨
        isActive = false; // 이동 멈춤
        if (spriteRenderer != null) spriteRenderer.enabled = false; // 차 숨기기

        // 설정한 랜덤 시간만큼 대기
        float waitTime = Random.Range(minRespawnTime, maxRespawnTime);
        yield return new WaitForSeconds(waitTime);

        // 대기 후 시작점에서 다시 생성
        SpawnCar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 부딪혔을 때 데미지 주는 로직은 유지
        if (other.CompareTag("Player") && Time.time >= _canDamageTime)
        {
            _canDamageTime = Time.time + damageCooldown;
            if (GameManager.instance != null)
                GameManager.instance.OnCarAccident();
        }
    }
}