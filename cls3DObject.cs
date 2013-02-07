using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DeModulate
{
    class cls3DObject
    {
        #region fields
        //Type Declarations
        private clsAABRender boundingBoxViewer;
        private BoundingBox boundingBox; 
        public clsDrops PowerUp;
        private Model[] fbxModel;
        private Matrix worldMatrix;

        private float Scale, Speed;
        private Vector3 Position, Rotation, Size, Velocity, ObjCollision, CollidePercent;
        private Vector2 screenCoords_position, screenCoords_size;

        private int Health;
        private byte Frame;
        private bool Visible;

        
        //Constructor        
        public cls3DObject(byte _ModelCount, Vector3 _Size, bool _Visible, GraphicsDevice _graphicsDevice, Matrix _worldMatrix)
        { 
           PowerUp = new clsDrops(-1,null,Vector2.Zero,Vector2.Zero,Vector2.Zero,0);
           boundingBoxViewer = new clsAABRender(_graphicsDevice);
           worldMatrix = _worldMatrix;
           fbxModel = new Model[_ModelCount];
           Scale = 1;
           Rotation = new Vector3(-90, 90, 0);
           Visible = _Visible;
           Size = _Size;
           Frame = 0;
           Health = 1;
           screenCoords_size = new Vector2();
           screenCoords_position = new Vector2();
        }
        #endregion

        #region collisions
        //Collision Detection
        //Objects within objects, hitting object walls.
        public void InternalBounce(cls3DObject otherModel)
        {
                    if (Position.X + Size.X > otherModel.Position.X + otherModel.Size.X || Position.X > otherModel.Position.X + otherModel.Size.X)
                        if (Velocity.X > 0)
                            Velocity.X = -Velocity.X;
                    if (Position.X < otherModel.Position.X || Position.X + Size.X<otherModel.Position.X)
                        if (Velocity.X <= 0)
                            Velocity.X = -Velocity.X;

                    if (Position.Y + Size.Y > otherModel.Position.Y + otherModel.Size.Y || Position.Y > otherModel.Position.Y + otherModel.Size.Y)
                        if (Velocity.Y > 0)
                            Velocity.Y = -Velocity.Y;
                    if (Position.Y < otherModel.Position.Y || Position.Y + Size.Y < otherModel.Position.Y)
                        if (Velocity.Y < 0)
                            Velocity.Y = -Velocity.Y;

                    if (Position.Z + Size.Z > otherModel.Position.Z + otherModel.Size.Z || Position.Z > otherModel.Position.Z + otherModel.Size.Z)
                        if (Velocity.Z > 0)
                            Velocity.Z = -Velocity.Z;
                    if (Position.Z < otherModel.Position.Z || Position.Z + Size.Z < otherModel.Position.Z)
                         if (Velocity.Z < 0)
                             Velocity.Z = -Velocity.Z;
                    Move();
        }

        //Objects colliding with objects that have a flat surface
        public void ExtFlatSurfaceBounce(cls3DObject otherModel)
        {
            if (Position.X < otherModel.Position.X + otherModel.Size.X && Position.X + Size.X > otherModel.Position.X + otherModel.Size.X)
                if(Velocity.X<0)  
                    Velocity.X = -Velocity.X;

            if (Position.X + Size.X > otherModel.Position.X && Position.X < otherModel.Position.X)
                if (Velocity.X >= 0)
                    Velocity.X = -Velocity.X;

            if (Position.Y < otherModel.Position.Y + otherModel.Size.Y && Position.Y + Size.Y > otherModel.Position.Y + otherModel.Size.Y)
                if (Velocity.Y < 0)
                    Velocity.Y = -Velocity.Y;

            if (Position.Y + Size.Y > otherModel.Position.Y && Position.Y < otherModel.Position.Y)
                if (Velocity.Y >= 0)
                    Velocity.Y = -Velocity.Y;

            if (Position.Z < otherModel.Position.Z + otherModel.Size.Z && Position.Z + Size.Z > otherModel.Position.Z + otherModel.Size.Z)
                if (Velocity.Z < 0)
                    Velocity.Z = -Velocity.Z;

            if (Position.Z + Size.Z > otherModel.Position.Z && Position.Z < otherModel.Position.Z)
                    if (Velocity.Z >= 0)
                        Velocity.Z = -Velocity.Z;
            Move();
        }

        //Objects colliding with objects that have a curved surface
        public void ExtCurvedSurfaceBounce(cls3DObject otherModel)
        {
            ObjCollision = -(otherModel.Position - Position);
            CollidePercent.X = (float)Math.Floor((ObjCollision.X / otherModel.Size.X) * 100);

            if (Position.X < otherModel.Position.X + otherModel.Size.X && Position.X + Size.X > otherModel.Position.X + otherModel.Size.X)
                if (Velocity.X < 0)
                    Velocity.X = -Velocity.X;

            if (Position.X + Size.X > otherModel.Position.X && Position.X < otherModel.Position.X)
                if (Velocity.X >= 0)
                    Velocity.X = -Velocity.X;

            if (Position.Y < otherModel.Position.Y + otherModel.Size.Y && Position.Y + Size.Y > otherModel.Position.Y + otherModel.Size.Y)
                if (Velocity.Y < 0)
                    Velocity.Y = -Velocity.Y;

            if (Position.Y + Size.Y > otherModel.Position.Y && Position.Y < otherModel.Position.Y)
                if (Velocity.Y >= 0)
                    Velocity.Y = -Velocity.Y;

            if (Position.Z < otherModel.Position.Z + otherModel.Size.Z && Position.Z + Size.Z > otherModel.Position.Z + otherModel.Size.Z)
            {
                if (CollidePercent.X >= 0 && CollidePercent.X < 10)
                { Velocity.Z = Speed * 0.2f; Velocity.X = Speed * -0.8f; }

                else if (CollidePercent.X >= 10 && CollidePercent.X < 20)
                { Velocity.Z = Speed * 0.4f; Velocity.X = Speed * -0.6f; }

                else if (CollidePercent.X >= 20 && CollidePercent.X < 30)
                { Velocity.Z = Speed * 0.6f; Velocity.X = Speed * -0.4f; }

                else if (CollidePercent.X >= 30 && CollidePercent.X < 40)
                { Velocity.Z = Speed * 0.8f; Velocity.X = Speed * -0.2f; }

                else if (CollidePercent.X >= 40 && CollidePercent.X < 50)
                { Velocity.Z = Speed * 1f; Velocity.X = Speed * 0f; }

                else if (CollidePercent.X >= 50 && CollidePercent.X < 60)
                { Velocity.Z = Speed * 1f; Velocity.X = Speed * 0f; }

                else if (CollidePercent.X >= 60 && CollidePercent.X < 70)
                { Velocity.Z = Speed * 0.8f; Velocity.X = Speed * 0.2f; }

                else if (CollidePercent.X >= 70 && CollidePercent.X < 80)
                { Velocity.Z = Speed * 0.6f; Velocity.X = Speed * 0.4f; }

                else if (CollidePercent.X >= 80 && CollidePercent.X < 90)
                { Velocity.Z = Speed * 0.4f; Velocity.X = Speed * 0.6f; }

                else if (CollidePercent.X >= 90)
                { Velocity.Z = Speed * 0.2f; Velocity.X = Speed * 0.8f; }
            }

            else if (Position.Z + Size.Z > otherModel.Position.Z && Position.Z < otherModel.Position.Z)
            {
                if (CollidePercent.X <= 10)
                { Velocity.Z = Speed * -0.2f; Velocity.X = Speed * -0.8f; }

                else if (CollidePercent.X >= 10 && CollidePercent.X < 20)
                { Velocity.Z = Speed * -0.4f; Velocity.X = Speed * -0.6f; }

                else if (CollidePercent.X >= 20 && CollidePercent.X < 30)
                { Velocity.Z = Speed * -0.6f; Velocity.X = Speed * -0.4f; }

                else if (CollidePercent.X >= 30 && CollidePercent.X < 40)
                { Velocity.Z = Speed * -0.8f; Velocity.X = Speed * -0.2f; }

                else if (CollidePercent.X >= 40 && CollidePercent.X < 50)
                { Velocity.Z = Speed * -1f; Velocity.X = Speed * 0f; }

                else if (CollidePercent.X >= 50 && CollidePercent.X < 60)
                { Velocity.Z = Speed * -1f; Velocity.X = Speed * 0f; }

                else if (CollidePercent.X >= 60 && CollidePercent.X < 70)
                { Velocity.Z = Speed * -0.8f; Velocity.X = Speed * 0.2f; }

                else if (CollidePercent.X >= 70 && CollidePercent.X < 80)
                { Velocity.Z = Speed * -0.6f; Velocity.X = Speed * 0.4f; }

                else if (CollidePercent.X >= 80 && CollidePercent.X < 90)
                { Velocity.Z = Speed * -0.4f; Velocity.X = Speed * 0.6f; }

                else if (CollidePercent.X >= 90)
                { Velocity.Z = Speed * -0.2f; Velocity.X = Speed * 0.8f; }
            }
            Move();
        }

        //Determine if a collision between two objects has been made
        public bool ExternalCollision(cls3DObject otherModel)
        {
            if (otherModel.isVisible())
            {
                BoundingBox tester = boundingBox;
                tester.Max += Velocity;
                tester.Min += Velocity;

                if (tester.Intersects(otherModel.boundingBox))
                    return true;
            }
            return false;
        }
        //Determine if a collision between two objects has been made c
        public bool ExternalCollision(cls3DObject otherModel, int scalar)
        {
            if (otherModel.isVisible())
            {
                BoundingBox tester = boundingBox;
                tester.Max += Velocity * scalar;
                tester.Min += Velocity * scalar;

                if (tester.Intersects(otherModel.boundingBox))
                    return true;
            }
            return false;
        }
        //Determine if an internal object has hit the wall of an eclosing object
        public bool InternalCollision(cls3DObject otherModel)
        {
            if (otherModel.isVisible())
            {
                BoundingBox tester = boundingBox;
                tester.Max += Velocity;
                tester.Min += Velocity;

                if (tester.Max.X > otherModel.boundingBox.Max.X
                    || tester.Max.Y > otherModel.boundingBox.Max.Y
                    || tester.Max.Z > otherModel.boundingBox.Max.Z
                    || tester.Min.X<otherModel.boundingBox.Min.X
                    || tester.Min.Y<otherModel.boundingBox.Min.Y
                    || tester.Min.Z<otherModel.boundingBox.Min.Z)

                    return true;
            }
            return false;
        }
        //Determine if an internal object has hit the wall of an eclosing object         //Determine if an internal object has hit the wall of an eclosing object
        public bool InternalCollision(cls3DObject otherModel, int scalar)
        {
            if (otherModel.isVisible())
            {
                BoundingBox tester = boundingBox;
                tester.Max += Velocity*scalar;
                tester.Min += Velocity*scalar;

                if (tester.Max.X > otherModel.boundingBox.Max.X
                    || tester.Max.Y > otherModel.boundingBox.Max.Y
                    || tester.Max.Z > otherModel.boundingBox.Max.Z
                    || tester.Min.X < otherModel.boundingBox.Min.X
                    || tester.Min.Y < otherModel.boundingBox.Min.Y
                    || tester.Min.Z < otherModel.boundingBox.Min.Z)
                    return true;
            }
            return false;
        }

        //Render a new bounding box
        public void BoundingRender(clsCamera Cam)
        {
            boundingBoxViewer.Render(boundingBox, Cam.viewMatrix, Cam.projectionMatrix, Color.Red);
        }
        #endregion

        #region initializers
        //Add a model to specific frame number
        public void AddModel(ref Model _model, byte _framenumber)
        {
            fbxModel[_framenumber] = _model;
        }

        //Instantiate a power up object
        public void AddPowerUP(int _ID, Texture2D _texture, clsCamera Cam, Vector2 _position, Vector2 _size, GraphicsDevice GD, byte _framesinAnim)
        {
            PowerUp = new clsDrops(_ID, _texture, _position, _size, new Vector2(0, 1), _framesinAnim);
        }

        //Reinstantiate a blank powe up object
        public void RemovePowerUp()
        {
            PowerUp = new clsDrops(-1, null, Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);
        }
        #endregion

        #region 3Dto2D coords
        //Convert current 3D position into screen coordinates
        public Vector2 TransformPosition(clsCamera Cam, GraphicsDevice gd, Vector3 _in)
        {
            Vector4 oTransformedPosition = Vector4.Transform(_in, Cam.viewMatrix * Cam.projectionMatrix);
            if (oTransformedPosition.W != 0)
            {
                oTransformedPosition.X /= oTransformedPosition.W;
                oTransformedPosition.Y /= oTransformedPosition.W;
                oTransformedPosition.Z /= oTransformedPosition.W;
            }
            return new Vector2(
              oTransformedPosition.X * gd.PresentationParameters.BackBufferWidth / 2 + gd.PresentationParameters.BackBufferWidth / 2,
              -oTransformedPosition.Y * gd.PresentationParameters.BackBufferHeight / 2 + gd.PresentationParameters.BackBufferHeight / 2);
        }
        #endregion

        #region getsets
        //Move the object
        public void Move()
        {
            Position += Velocity;
        }

        //Set the current frame
        public void SetFrame(byte _frame)
        {
            Frame = _frame;
        }

        //Set a new speed value
        public void SetSpeed(float _speed)
        {
            Speed = _speed;
        }

        //Set a new size value
        public void SetSize(Vector3 _size)
        {
            Size = _size;
        }

        //Set a new health value      
        public void SetHealth(int _health)
        {
            Health = _health;
        }

        //Set a new scale factor
        public void SetScale(float _Scale)
        {
            Scale = _Scale;
        }

        //Set a new rotation
        public void SetRotation(Vector3 _Rotation)
        {
            Rotation = _Rotation;
        }

        //Set a new position
        public void SetPosition(Vector3 _Position)
        {
            Position = _Position;
        }
        
        //Set a new velocity
        public void SetVelocity(Vector3 _velocity)
        {
            Velocity = _velocity;
        }

        //Get current screen coordinates of object
        public Vector2 GetScreenPos()
        {
            return screenCoords_position;
        }

        //Get current screen size values of object
        public Vector2 GetScreenSize()
        {
            return screenCoords_size;
        }

        //Get current size
        public Vector3 GetSize()
        {
            return Size;
        }

        //Get current speed
        public float GetSpeed()
        {
            return Speed;
        }

        //Get current frame
        public int GetFrame()
        {
            return Frame;
        }

        //Get current position
        public Vector3 GetPosition()
        {
            return Position;
        }

        //Get current velocity
        public Vector3 GetVelocity()
        {
            return Velocity;
        }

        //Get current health
        public int GetHealth()
        {
            return Health;
        }

        //Get current scale factor
        public float GetScale()
        {
            return Scale;
        }

        //Get current rotation
        public Vector3 GetRotation()
        {
            return Rotation;
        }

        //Hide the object
        public void Hide()
        {
            Visible = false;
        }

        //Show the object
        public void Show()
        {
            Visible = true;
        }

        //Get current visibility
        public bool isVisible()
        {
            return Visible;
        }
        #endregion

        #region draw
        //Draw Object, Set Objects Powerup to the objects current position.
        public void Draw(clsCamera Cam,GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont font)
        {
            if(Visible)
            {
                boundingBox.Min = Position; boundingBox.Max = Position + Size;

                foreach (ModelMesh mesh in fbxModel[Frame].Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = worldMatrix * 
                                Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X)) *
                                Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)) *
                                Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Z)) *
                                Matrix.CreateTranslation(Position);
                        effect.View = Cam.viewMatrix;
                        effect.Projection = Cam.projectionMatrix;
                    }
                    mesh.Draw();
                }
                screenCoords_position = TransformPosition(Cam, gd, Position);
                screenCoords_size = TransformPosition(Cam, gd, Position + Size);
                screenCoords_size -= screenCoords_position;
                if (!PowerUp.isVisible())
                {
                    PowerUp.SetPosition(screenCoords_position + screenCoords_size / 2);
                }
               }
            }

        //Draw the object in a certain position
        public void Draw(clsCamera Cam, Vector3 _position)
        {
            Position = _position;
            if (Visible)
            {
                foreach (ModelMesh mesh in fbxModel[Frame].Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = worldMatrix * Matrix.CreateScale(Scale) *
                                Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X)) *
                                Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)) *
                                Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Z)) *
                                Matrix.CreateTranslation(Position);
                        effect.View = Cam.viewMatrix;
                        effect.Projection = Cam.projectionMatrix;
                    }
                    mesh.Draw();
                }
            }
        }
        #endregion
    }
}
