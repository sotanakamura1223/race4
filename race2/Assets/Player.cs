using UnityEngine;

public class Player : MonoBehaviour
{
    public enum InputMode { AxisNames, FixedKeys }
    public InputMode inputMode = InputMode.FixedKeys;

    [Tooltip("1 or 2")]
    public int playerId = 1;

    // AxisNames 用の名前（Input Managerに追加する場合）
    public string horizontalAxisNameP1 = "Horizontal";
    public string verticalAxisNameP1 = "Vertical";
    public string horizontalAxisNameP2 = "Horizontal2";
    public string verticalAxisNameP2 = "Vertical2";

    public float moveSpeed = 3f;
    public float dashForce = 12f;
    public float dashDuration = 0.25f;

    Rigidbody rb;
    bool isDashing = false;
    float dashTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // ダッシュトリガー（Updateで検出）
        if (!isDashing)
        {
            if (IsDashKeyDown())
            {
                Vector3 moveDir = GetMoveDirection();
                if (moveDir.sqrMagnitude > 0.01f)
                {
                    StartDash(moveDir);
                }
                else
                {
                    StartDash(transform.forward); // 動いてなければ前方に突進
                }
            }
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
    }

    void FixedUpdate()
    {
        // 普通移動（ダッシュ中は移動入力を無視）
        if (!isDashing)
        {
            Vector3 moveDir = GetMoveDirection();
            if (moveDir.sqrMagnitude > 0.001f)
            {
                // カメラ基準にする場合はここで変換（必要なら）
                Vector3 newPos = rb.position + moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPos);

                // 向きを変える
                Quaternion target = Quaternion.LookRotation(moveDir);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, target, 12f * Time.fixedDeltaTime));
            }
        }
        else
        {
            // ダッシュ中は速度を保つ（rb.velocity は StartDash で設定済み）
            // 必要に応じて摩擦やブレーキ処理を追加
        }
    }

    bool IsDashKeyDown()
    {
        if (inputMode == InputMode.AxisNames)
        {
            // ジャンプ軸など別途定義しているなら使う（ここではキー扱い）
            // 便宜的にUnityの"Jump"や"Jump2"があるなら使うが、無ければGetKeyDown fallback
            if (playerId == 1) return Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump");
            return Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Jump2");
        }
        else
        {
            if (playerId == 1) return Input.GetKeyDown(KeyCode.Space);
            return Input.GetKeyDown(KeyCode.K);
        }
    }

    Vector3 GetMoveDirection()
    {
        if (inputMode == InputMode.AxisNames)
        {
            string hx = playerId == 1 ? horizontalAxisNameP1 : horizontalAxisNameP2;
            string vx = playerId == 1 ? verticalAxisNameP1 : verticalAxisNameP2;
            float h = Input.GetAxis(hx);
            float v = Input.GetAxis(vx);
            Vector3 raw = new Vector3(h, 0f, v);
            if (raw.magnitude > 1f) raw.Normalize();
            // カメラ基準にしたいなら Camera.main.transform.TransformDirection(raw) して y=0 にする
            if (Camera.main != null)
            {
                Vector3 camF = Camera.main.transform.forward; camF.y = 0f; camF.Normalize();
                Vector3 camR = Camera.main.transform.right; camR.y = 0f; camR.Normalize();
                return (camR * raw.x + camF * raw.z);
            }
            return raw;
        }
        else
        {
            // 固定キー: P1 = WASD, P2 = 矢印
            float h = 0f, v = 0f;
            if (playerId == 1)
            {
                if (Input.GetKey(KeyCode.A)) h = -1f;
                if (Input.GetKey(KeyCode.D)) h = 1f;
                if (Input.GetKey(KeyCode.W)) v = 1f;
                if (Input.GetKey(KeyCode.S)) v = -1f;
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftArrow)) h = -1f;
                if (Input.GetKey(KeyCode.RightArrow)) h = 1f;
                if (Input.GetKey(KeyCode.UpArrow)) v = 1f;
                if (Input.GetKey(KeyCode.DownArrow)) v = -1f;
            }

            Vector3 raw = new Vector3(h, 0f, v);
            if (raw.magnitude > 1f) raw.Normalize();
            if (Camera.main != null)
            {
                Vector3 camF = Camera.main.transform.forward; camF.y = 0f; camF.Normalize();
                Vector3 camR = Camera.main.transform.right; camR.y = 0f; camR.Normalize();
                return (camR * raw.x + camF * raw.z);
            }
            return raw;
        }
    }

    void StartDash(Vector3 dir)
    {
        isDashing = true;
        dashTimer = dashDuration;
        rb.linearVelocity = dir.normalized * dashForce;
    }
}
