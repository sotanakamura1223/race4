using UnityEngine;

public class PlayerCollisionSimple : MonoBehaviour
{
    [Tooltip("���̃I�u�W�F�N�g����P�� (1 or 2)")]
    public int playerId = 1;

    [Header("Ground ���� (�^�O or ���C���[)")]
    [Tooltip("�n�ʂɂ������I�u�W�F�N�g�� Tag (��: Ground)�B�󕶎��Ŗ���")]
    public string groundTag = "Ground";

    [Tooltip("�n�ʃ��C���[�iLayerMask�j�B0�Ȃ疳��")]
    public LayerMask groundLayer = 0;

    [Header("��������")]
    [Tooltip("�v���C���[������ Y �ȉ��ɂȂ�����s�k�����i�ʏ�̓X�e�[�W���ɗ������Ƃ��p�j�B�����ɂ���Ȃ���ɏ������l�ɂ���")]
    public float fallY = -5f;

    // �����t���O�F���łɔs�k�������������i���d�Ăяo���h�~�j
    bool hasLost = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        // �����ł̔s�k����i�ȈՁj
        if (!hasLost && transform.position.y <= fallY)
        {
            LoseBecauseGround();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasLost) return;

        

        // �n�ʂɂԂ�����������iCollision�j
        TryHandleGroundCollision(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasLost) return;

        TryHandlePlayerCollision(other);
        TryHandleGroundCollision(other);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hasLost) return;

        TryHandlePlayerCollision(hit.collider);
        TryHandleGroundCollision(hit.collider);
    }

    void TryHandlePlayerCollision(Collider other)
    {
        var otherPlayer = other.GetComponent<PlayerCollisionSimple>();
        if (otherPlayer != null)
        {
            Debug.Log($"Player{playerId} touched Player{otherPlayer.playerId} -> Player{playerId} LOSE");
            GameManager.Instance?.PlayerLose(playerId);
            hasLost = true;
        }
    }

    void TryHandleGroundCollision(Collider other)
    {
        // �^�O����
        if (!string.IsNullOrEmpty(groundTag) && other.CompareTag(groundTag))
        {
            Debug.Log($"Player{playerId} touched Ground(tag:{groundTag}) -> Player{playerId} LOSE");
            LoseBecauseGround();
            return;
        }

        // ���C���[����
        if (groundLayer != 0)
        {
            int otherLayerMask = 1 << other.gameObject.layer;
            if ((groundLayer.value & otherLayerMask) != 0)
            {
                Debug.Log($"Player{playerId} touched Ground(layer:{other.gameObject.layer}) -> Player{playerId} LOSE");
                LoseBecauseGround();
                return;
            }
        }
    }

    void LoseBecauseGround()
    {
        if (hasLost) return;
        hasLost = true;
        GameManager.Instance?.PlayerLose(playerId);
    }
}
