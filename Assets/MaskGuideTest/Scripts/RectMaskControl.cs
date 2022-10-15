using UnityEngine;
using UnityEngine.UI;

public class RectMaskControl : MaskControl
{
    /// <summary>
    /// 最终的偏移值X
    /// </summary>
    private float _targetOffsetX = 0f;

    /// <summary>
    /// 最终的偏移值Y
    /// </summary>
    private float _targetOffsetY = 0f;

    /// <summary>
    /// 当前的偏移值X
    /// </summary>
    private float _currentOffsetX = 0f;

    /// <summary>
    /// 当前的偏移值Y
    /// </summary>
    private float _currentOffsetY = 0f;

    /// <summary>
    /// 收缩速度
    /// </summary>
    private float _shrinkVelocityX = 0f;
    private float _shrinkVelocityY = 0f;

    void Update()
    {
        if (Target == null) return;

        //从当前半径到目标半径差值显示收缩动画
        float valueX = Mathf.SmoothDamp(_currentOffsetX, _targetOffsetX, ref _shrinkVelocityX, _shrinkTime);
        float valueY = Mathf.SmoothDamp(_currentOffsetY, _targetOffsetY, ref _shrinkVelocityY, _shrinkTime);
        if (!Mathf.Approximately(valueX, _currentOffsetX))
        {
            _currentOffsetX = valueX;
            _material.SetFloat("_SliderX", _currentOffsetX);
        }

        if (!Mathf.Approximately(valueY, _currentOffsetY))
        {
            _currentOffsetY = valueY;
            _material.SetFloat("_SliderY", _currentOffsetY);
        }
    }

    public override void SetCurTarget()
    {
        if (Target != null) return ;

        GetComponent<Image>().enabled = true;

        if (targets.Length > curIdx) Target = targets[curIdx];
        curIdx++;
        //获取高亮区域四个顶点的世界坐标
        Target.GetWorldCorners(_corners);
        //计算高亮显示区域咋画布中的范围
        _targetOffsetX = Vector2.Distance(WorldToCanvasPos(Canvas, _corners[0]), WorldToCanvasPos(Canvas, _corners[3])) / 2f;
        _targetOffsetY = Vector2.Distance(WorldToCanvasPos(Canvas, _corners[0]), WorldToCanvasPos(Canvas, _corners[1])) / 2f;
        //计算高亮显示区域的中心
        float x = _corners[0].x + ((_corners[3].x - _corners[0].x) / 2f);
        float y = _corners[0].y + ((_corners[1].y - _corners[0].y) / 2f);
        Vector3 centerWorld = new Vector3(x, y, 0);
        Vector2 center = WorldToCanvasPos(Canvas, centerWorld);
        //设置遮罩材料中中心变量
        Vector4 centerMat = new Vector4(center.x, center.y, 0, 0);
        _material.SetVector("_Center", centerMat);
        //计算当前偏移的初始值
        RectTransform canvasRectTransform = (Canvas.transform as RectTransform);
        if (canvasRectTransform != null)
        {
            //获取画布区域的四个顶点
            canvasRectTransform.GetWorldCorners(_corners);
            //求偏移初始值
            for (int i = 0; i < _corners.Length; i++)
            {
                if (i % 2 == 0)
                    _currentOffsetX = Mathf.Max(Vector3.Distance(WorldToCanvasPos(Canvas, _corners[i]), center), _currentOffsetX);
                else
                    _currentOffsetY = Mathf.Max(Vector3.Distance(WorldToCanvasPos(Canvas, _corners[i]), center), _currentOffsetY);
            }
        }
        //设置遮罩材质中当前偏移的变量
        _material.SetFloat("_SliderX", _currentOffsetX);
        _material.SetFloat("_SliderY", _currentOffsetY);
        Canvas.sortingOrder = 5;
    }
   
    protected override void OnDestroy()
    {
        _material.SetFloat("_SliderX", 0);
        _material.SetFloat("_SliderY", 0);
    }
}