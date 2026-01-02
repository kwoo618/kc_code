using UnityEngine;

public class CarMover : MonoBehaviour
{
    [Header("�̵� ����")]
    public Transform startPoint; // ��� ����
    public Transform endPoint;   // ���� ����
    public float speed = 5f;     // �̵� �ӵ�

    [Header("�ִϸ��̼� ����")]
    public bool isHorizontalCar = false; // ���η� �����̴� ���ΰ�? (üũ�ϸ� ���Ʒ��� �սǰŸ�)
    public float bobSpeed = 10f;  // �սǰŸ��� �ӵ�
    public float bobAmount = 0.05f; // �սǰŸ��� ����

    [Header("�浹 ����")]
    public int damageCooldown = 2; // ���� �浹 ���� �ð� (��)

    private float _canDamageTime = 0f;

    void Start()
    {
        transform.position = startPoint.position;
    }

    void Update()
    {
        // 1. �ϴ� ������ ������ X�ุ �̵��Ѵٰ� ���� (�⺻ �̵�)
        transform.position = Vector3.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);

        // 2. ���� ���� ����� -> Y��(����)�� ������ ������(Sin)�� �����
        if (isHorizontalCar)
        {
            // ���� ����(startPoint.y)�� �������� ���Ʒ��� ��鸲
            float newY = startPoint.position.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;

            // ���� X, Z�� �����ϰ� Y�� ��ü
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        // 3. ���� üũ �� ����
        if (Vector3.Distance(transform.position, endPoint.position) < 0.1f)
        {
            transform.position = startPoint.position;
        }
    }

    // �÷��̾�� �浹 ó��
    void OnTriggerEnter2D(Collider2D other)
    {
        // ��Ÿ���� ������, �浹�� ����� �÷��̾���
        if (other.CompareTag("Player") && Time.time >= _canDamageTime)
        {
            _canDamageTime = Time.time + damageCooldown; // ��Ÿ�� ����
            GameManager.instance.OnCarAccident(); // �� ���
        }
    }


}