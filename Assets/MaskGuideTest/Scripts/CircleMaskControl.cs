using UnityEngine;
using UnityEngine.UI;

public class CircleMaskControl : MaskControl
{
    /// <summary>
    /// 镂空区域半径
    /// </summary>
    private float _radius;

    /// <summary>
    /// 当前高亮区域的半径
    /// </summary>
    private float _currentRadius;

    /// <summary>
    /// 收缩速度
    /// </summary>
    private float _shrinkVelocity = 0f;

    void Update()
    {
        if (Target == null) return;

        //从当前半径到目标半径差值显示收缩动画
        float value = Mathf.SmoothDamp(_currentRadius, _radius, ref _shrinkVelocity, _shrinkTime);
        if (!Mathf.Approximately(value, _currentRadius))
        {
            _currentRadius = value;
            _material.SetFloat("_Slider", _currentRadius);
        }
    }

    public override void SetCurTarget()
    {
        if (Target != null) return;

        if (targets.Length > curIdx) Target = targets[curIdx];
        curIdx++;

        //获取高亮区域的四个顶点的世界坐标
        Target.GetWorldCorners(_corners);

        GetComponent<Image>().enabled = true;
        //计算最终高亮显示区域的半径 设置系数比根号2 = 1.414 稍微大点
        _radius = 1.45f * Vector2.Distance(WorldToCanvasPos(Canvas, _corners[0]), WorldToCanvasPos(Canvas, _corners[1])) / 2f;

        //计算高亮显示区域的圆心
        float x = _corners[0].x + ((_corners[3].x - _corners[0].x) / 2f);
        float y = _corners[0].y + ((_corners[1].y - _corners[0].y) / 2f);

        Vector3 centerWorld = new Vector3(x, y, 0);

        Vector2 center = WorldToCanvasPos(Canvas, centerWorld);

        //设置遮罩材料中的圆心变量
        Vector4 centerMat = new Vector4(center.x, center.y, 0, 0);

        _material.SetVector("_Center", centerMat);

        //计算当前高亮显示区域的半径
        RectTransform canRectTransform = Canvas.transform as RectTransform;
        if (canRectTransform != null)
        {
            //获取画布区域的四个顶点
            canRectTransform.GetWorldCorners(_corners);

            //将画布顶点距离高亮区域中心最远的距离作为当前高亮区域半径的初始值
            foreach (Vector3 corner in _corners)
            {
                _currentRadius = Mathf.Max(Vector3.Distance(WorldToCanvasPos(Canvas, corner), center), _currentRadius);
            }
        }

        _material.SetFloat("_Slider", _currentRadius);
    }

    protected override void OnDestroy()
    {
        _material.SetFloat("_Slider", 0);
    }
}