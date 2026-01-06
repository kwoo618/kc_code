using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // ���� ��� (�÷��̾�)
    public float smoothSpeed = 0.125f; // ���󰡴� �ӵ� (0~1 ����, �������� �ε巯��)
    public Vector3 offset = new Vector3(0, 0, -10); // ī�޶� �Ÿ� (Z�� -10 ���� �ʼ�)

    // �� ������ ī�޶� �� ������ �Ϸ��� �Ʒ� �� ���� (�ʿ� ������ ����)
    public bool useLimit = false;
    public Vector2 minLimit;
    public Vector2 maxLimit;

    void LateUpdate()
    {
        if (target == null) return;

        // ��ǥ ��ġ ���
        Vector3 desiredPosition = target.position + offset;

        // �ε巯�� �̵� (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // �� ���� ���� (üũ���� ��츸)
        if (useLimit)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minLimit.x, maxLimit.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minLimit.y, maxLimit.y);
        }

        transform.position = smoothedPosition;
    }
    // [추가] 텔레포트 시 카메라를 즉시 이동시키는 함수
    public void TeleportCamera()
    {
        if (target == null) return;
        
        // 보간(Lerp) 없이 즉시 목표 위치로 설정
        Vector3 targetPos = target.position + offset;
        
        if (useLimit)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minLimit.x, maxLimit.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minLimit.y, maxLimit.y);
        }
        
        transform.position = targetPos;
    }
}