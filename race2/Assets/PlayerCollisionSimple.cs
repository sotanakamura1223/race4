using UnityEngine;

public class PlayerCollisionSimple : MonoBehaviour
{
    [Tooltip("このオブジェクトが何Pか (1 or 2)")]
    public int playerId = 1;

    [Header("Ground 判定 (タグ or レイヤー)")]
    [Tooltip("地面にしたいオブジェクトの Tag (例: Ground)。空文字で無効")]
    public string groundTag = "Ground";

    [Tooltip("地面レイヤー（LayerMask）。0なら無効")]
    public LayerMask groundLayer = 0;

    [Header("落下判定")]
    [Tooltip("プレイヤーがこの Y 以下になったら敗北扱い（通常はステージ下に落ちたとき用）。無効にするなら非常に小さい値にする")]
    public float fallY = -5f;

    // 内部フラグ：すでに敗北を処理したか（多重呼び出し防止）
    bool hasLost = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        // 落下での敗北判定（簡易）
        if (!hasLost && transform.position.y <= fallY)
        {
            LoseBecauseGround();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasLost) return;

        

        // 地面にぶつかったか判定（Collision）
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
        // タグ判定
        if (!string.IsNullOrEmpty(groundTag) && other.CompareTag(groundTag))
        {
            Debug.Log($"Player{playerId} touched Ground(tag:{groundTag}) -> Player{playerId} LOSE");
            LoseBecauseGround();
            return;
        }

        // レイヤー判定
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
