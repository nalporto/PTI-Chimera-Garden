using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class SkewImage : BaseMeshEffect
{
    [SerializeField] private float skewX = 0.3f;
    [SerializeField] private float skewY = 0f;

    public float SkewX
    {
        get { return skewX; }
        set
        {
            skewX = value;
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public float SkewY
    {
        get { return skewY; }
        set
        {
            skewY = value;
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        UIVertex vertex = new UIVertex();
        int count = vh.currentVertCount;

        for (int i = 0; i < count; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);

            RectTransform rectTransform = transform as RectTransform;
            Vector2 rectSize = rectTransform.rect.size;
            
            float normalizedY = (vertex.position.y + rectSize.y * rectTransform.pivot.y) / rectSize.y;
            float normalizedX = (vertex.position.x + rectSize.x * rectTransform.pivot.x) / rectSize.x;

            vertex.position.x += normalizedY * skewX * rectSize.y;
            vertex.position.y += normalizedX * skewY * rectSize.x;

            vh.SetUIVertex(vertex, i);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (graphic != null)
            graphic.SetVerticesDirty();
    }
#endif
}
