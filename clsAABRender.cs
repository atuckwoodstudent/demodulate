using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DeModulate
{
    class clsAABRender
    {
        #region fields
        //Object Declarations
        static VertexPositionColor[] verts = new VertexPositionColor[8];
        public GraphicsDevice graphicsDevice;
        static int[] indices = new int[]
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
            0, 4,
            1, 5,
            2, 6,
            3, 7,
            4, 5,
            5, 6,
            6, 7,
            7, 4,
        };
        static BasicEffect effect;
        static VertexDeclaration vertDecl;

        //Constructor
        public clsAABRender(GraphicsDevice _graphicsDevice)
        {
            graphicsDevice = _graphicsDevice;
        }
        #endregion

        #region render
        //Render 3D bounding box
        public void Render(
            BoundingBox box,
            Matrix view,
            Matrix projection,
            Color color)
        {
            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice, null);
                effect.VertexColorEnabled = true;
                effect.LightingEnabled = false;
                vertDecl = new VertexDeclaration(graphicsDevice, VertexPositionColor.VertexElements);
            }

            Vector3[] corners = box.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position = corners[i];
                verts[i].Color = color;
            }

            graphicsDevice.VertexDeclaration = vertDecl;

            effect.View = view;
            effect.Projection = projection;

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    verts,
                    0,
                    8,
                    indices,
                    0,
                    indices.Length / 2);

                pass.End();
            }
            effect.End();
        }
        #endregion
    }
}