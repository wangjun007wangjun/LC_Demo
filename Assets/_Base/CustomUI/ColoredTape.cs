using UnityEngine;
using UnityEngine.UI;

namespace BaseFramework.UI
{
    public class ColoredTape : MaskableGraphic
    {
        public enum DrawDirection
        {
            Vertical,
            Horizontal
        }

        [Header("Colored tape type setting")]
        public DrawDirection tapeDirection = DrawDirection.Vertical;

        [Header("Colored tape colors setting")]
        public Color[] colors;
        [HideInInspector]
        public Vector2 rectSize;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            rectSize = GetPixelAdjustedRect().size;
            vh.Clear();
            
            //if(colors.IsNullOrEmpty()) return;
            
            if (tapeDirection == DrawDirection.Vertical)
                DrawVerticalColoredTape(vh);
            else
                DrawHorizontalColoredTape(vh);
        }
        
        private void DrawVerticalColoredTape(VertexHelper vh)
        {
            int colorNumber = colors.Length;
            float offset = rectSize.y / ( colorNumber -1);
            Vector2 topLeftPos = new Vector2(-rectSize.x / 2.0f , rectSize.y / 2.0f);
            Vector2 topRightPos = new Vector2(rectSize.x / 2.0f , rectSize.y / 2.0f);
            Vector2 bottomLeftPos = topLeftPos - new Vector2(0 , offset);
            Vector2 bottomRightPos = topRightPos - new Vector2(0 , offset);
            for ( int i = 0 ; i < colorNumber - 1; i++ )
            {
                Color startColor = colors[i];
                Color endColor = colors[i + 1];
                UIVertex first = GetUIVertex(topLeftPos, startColor);
                UIVertex second = GetUIVertex(topRightPos, startColor);
                UIVertex third = GetUIVertex(bottomRightPos,endColor);
                UIVertex four = GetUIVertex(bottomLeftPos, endColor);
                vh.AddUIVertexQuad(new UIVertex[] { first , second , third , four });
                topLeftPos = bottomLeftPos;
                topRightPos = bottomRightPos;
                bottomLeftPos = topLeftPos - new Vector2(0 , offset);
                bottomRightPos = topRightPos - new Vector2(0 , offset);
            }
        }
        
        private void DrawHorizontalColoredTape(VertexHelper vh)
        {
            int colorNumber = colors.Length;
            float offset = rectSize.x / ( colorNumber - 1 );
            Vector2 topLeftPos = new Vector2(-rectSize.x / 2.0f , rectSize.y / 2.0f);
            Vector2 bottomLeftPos = topLeftPos - new Vector2(0 , rectSize.y);
            Vector2 topRightPos = topLeftPos + new Vector2(offset, 0);
            Vector2 bottomRightPos = bottomLeftPos + new Vector2(offset, 0);
            for ( int i = 0 ; i < colorNumber - 1 ; i++ )
            {
                Color startColor = colors[i];
                Color endColor = colors[i + 1];
                UIVertex first = GetUIVertex(topLeftPos , startColor);
                UIVertex second = GetUIVertex(topRightPos , endColor);
                UIVertex third = GetUIVertex(bottomRightPos , endColor);
                UIVertex four = GetUIVertex(bottomLeftPos , startColor);
                vh.AddUIVertexQuad(new UIVertex[] { first , second , third , four });
                topLeftPos = topRightPos;
                bottomLeftPos = bottomRightPos;
                topRightPos = topLeftPos + new Vector2(offset,0);
                bottomRightPos = bottomLeftPos + new Vector2(offset , 0);
            }
        }     
                       
        private UIVertex GetUIVertex( Vector2 point , Color color0 )
        {
            UIVertex vertex = new UIVertex
            {
                position = point ,
                color = color0 ,
            };
            return vertex;
        }        
    }
}