using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;

    // [추가] 조이스틱 변수 (인스펙터에서 연결 필요!)
    public Joystick joystick;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. PC 키보드 입력
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. 모바일 조이스틱 입력 더하기 (연결됐을 때만)
        if (joystick != null)
        {
            h += joystick.Horizontal;
            v += joystick.Vertical;
        }

        // 3. 벡터 합성 및 크기 제한 (Normalize 대신 ClampMagnitude 사용)
        // 이렇게 해야 조이스틱을 살짝 밀었을 때 천천히 걷습니다.
        movement = new Vector2(h, v);
        movement = Vector2.ClampMagnitude(movement, 1f);

        // 4. 애니메이션 (입력이 있을 때만 방향 갱신)
        if (movement.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}