using UnityEngine;

public class ZoomableImage : MonoBehaviour
{
    private Vector3 _StartTouchPosition;

    public float ZoomSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this._Scroll();
        this._Zoom();
    }

    private void _Scroll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _StartTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            var dragDelta = Input.mousePosition - _StartTouchPosition;

            var scale = this.transform.localScale;
            var pos = this.transform.localPosition + dragDelta;

            //  画像が元の表示範囲内に収まるよう調整
            var imRect = this.GetComponent<RectTransform>();
            pos = CalcAdjustedImageLocalPosition(pos, scale, imRect.rect.width, imRect.rect.height);

            this.transform.localPosition = pos;

            _StartTouchPosition = Input.mousePosition;
        }
    }

    private void _Zoom()
    {
        float val = Input.mouseScrollDelta.y;

        if (val == 0)
        {
            return;
        }

        Vector2 pastPos = this.transform.localPosition;
        var pastScale = this.transform.localScale;

        // ズームするスケールを計算
        var scale = this.transform.localScale * (1 + val * ZoomSpeed);

        // カーソルを中心にズームするようにオフセット座標を計算
        var offsetPos = new Vector2(_CursorPotision().x * (scale.x - pastScale.x), _CursorPotision().y * (scale.y - pastScale.y));
        var pos = pastPos - offsetPos;

        //  縮小時の画像が元の表示範囲内に収まるよう調整
        var imRect = this.GetComponent<RectTransform>();
        pos = CalcAdjustedImageLocalPosition(pos, scale, imRect.rect.width, imRect.rect.height);

        // 倍率1以下にはせず、リセット
        if (scale.x < 1)
        {
            scale = Vector3.one;
            pos = Vector2.zero;
        }

        this.transform.localScale = scale;
        this.transform.localPosition = pos;
    }

    /// <summary>
    /// 元の表示範囲内に収まるように調整したローカル座標を返す
    /// </summary>
    public static Vector3 CalcAdjustedImageLocalPosition(Vector3 inPos, Vector3 scale, float width, float height)
    {
        var posXMax = width * 0.5f * (scale.x - 1);
        var posYMax = height * 0.5f * (scale.y - 1);

        var outPos = Vector3.zero;
        outPos.x = inPos.x > 0 ? Mathf.Min(inPos.x, posXMax) : Mathf.Max(inPos.x, -1 * posXMax);
        outPos.y = inPos.y > 0 ? Mathf.Min(inPos.y, posYMax) : Mathf.Max(inPos.y, -1 * posYMax);

        return outPos;
    }

    // このコンポーネントがアタッチされた UI Image 上のマウスカーソルの座標を取得
    private Vector2 _CursorPotision()
    {
        var canvas = this.GetComponentInParent<Canvas>();
        var imRect = this.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(imRect, Input.mousePosition, canvas.worldCamera, out var pos);

        return pos;
    }
}

