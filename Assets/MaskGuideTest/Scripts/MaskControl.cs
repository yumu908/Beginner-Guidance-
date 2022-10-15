using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskControl : MonoBehaviour, ICanvasRaycastFilter
{
    public Canvas Canvas;
    /// <summary>
    /// 所有目标
    /// </summary>
    public RectTransform[] targets;

    /// <summary>
    /// 
    /// </summary>
    protected int curIdx;

    /// <summary>
    /// 要高亮显示的目标
    /// </summary>
    public RectTransform Target { get; protected set; }

    /// <summary>
    /// 区域范围缓存
    /// </summary>
    protected Vector3[] _corners = new Vector3[4];

    /// <summary>
    /// 镂空区域中心
    /// </summary>
    protected Vector4 _center;

    /// <summary>
    /// 遮罩材质
    /// </summary>
    [SerializeField] protected Material _material;

    /// <summary>
    /// 高亮区域缩放的动画时间
    /// </summary>
    protected float _shrinkTime = 0.2f;

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (Target == null) return true;

        //在目标范围内做事件渗透
        return !RectTransformUtility.RectangleContainsScreenPoint(Target, sp, eventCamera);
    }

    public Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world, null, out position);

        return position;
    }

    // Start is called before the first frame update
    void Awake()
    {
        curIdx = 0;
        _material = GetComponent<Image>().material;
    }

    public virtual void SetCurTarget()
    {

    }

    public void CurTargetDone()
    {
        //GetComponent<Image>().enabled = false;
        Target = null;
    }

    protected virtual void OnDestroy()
    {

    }
}
