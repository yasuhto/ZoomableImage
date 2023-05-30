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

            //  �摜�����̕\���͈͓��Ɏ��܂�悤����
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

        // �Y�[������X�P�[�����v�Z
        var scale = this.transform.localScale * (1 + val * ZoomSpeed);

        // �J�[�\���𒆐S�ɃY�[������悤�ɃI�t�Z�b�g���W���v�Z
        var offsetPos = new Vector2(_CursorPotision().x * (scale.x - pastScale.x), _CursorPotision().y * (scale.y - pastScale.y));
        var pos = pastPos - offsetPos;

        //  �k�����̉摜�����̕\���͈͓��Ɏ��܂�悤����
        var imRect = this.GetComponent<RectTransform>();
        pos = CalcAdjustedImageLocalPosition(pos, scale, imRect.rect.width, imRect.rect.height);

        // �{��1�ȉ��ɂ͂����A���Z�b�g
        if (scale.x < 1)
        {
            scale = Vector3.one;
            pos = Vector2.zero;
        }

        this.transform.localScale = scale;
        this.transform.localPosition = pos;
    }

    /// <summary>
    /// ���̕\���͈͓��Ɏ��܂�悤�ɒ����������[�J�����W��Ԃ�
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

    // ���̃R���|�[�l���g���A�^�b�`���ꂽ UI Image ��̃}�E�X�J�[�\���̍��W���擾
    private Vector2 _CursorPotision()
    {
        var canvas = this.GetComponentInParent<Canvas>();
        var imRect = this.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(imRect, Input.mousePosition, canvas.worldCamera, out var pos);

        return pos;
    }
}

