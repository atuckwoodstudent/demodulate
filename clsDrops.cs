#region Using Statements
using Microsoft.Xna.Framework.Graphics;  
using Microsoft.Xna.Framework;
using System;
#endregion
namespace DeModulate
{
    class clsDrops
    {
        #region fields        
        private Texture2D Texture;
        private Vector2 Position, Size, Velocity, Frame;
        private Rectangle sourceRect;
        private byte framesinAnim;
        private bool Visible;
        private TimeSpan tmrTimer;
        private int ID;

        //Constructor 
        public clsDrops (int _ID, Texture2D _texture, Vector2 _position, Vector2 _size, Vector2 _velocity, byte _framesinAnim)
        {
            ID = _ID;
            framesinAnim = _framesinAnim;
            Texture = _texture;
            Position = _position;
            Size = _size;
            Frame = Vector2.Zero;
            Velocity = _velocity;
            tmrTimer = TimeSpan.Zero;
        }
#endregion

        #region getsets
        //Move the object
        public void Move()
        {
           Position += Velocity;
        }

        //Get the object identifier
        public int GetID()
        {
            return ID;
        }

        //Set the object position
        public void SetPosition(Vector2 _position)
        {
            Position = _position;
        }

        //Set the object velocity
        public void SetVelocity(Vector2 _velocity)
        {
            Velocity = _velocity;
        }

        //Set the object size
        public void SetSize(Vector2 _size)
        {
            Size = _size;
        }

        //Set the object animation frame
        public void SetFrame(Vector2 _frame)
        {
            Frame = _frame;
        }

        //Get the current object position
        public Vector2 GetPosition()
        {
            return Position;
        }

        //Get the current object velocity
        public Vector2 GetVelocity()
        {
            return Velocity;
        }

        //Get the current object size
        public Vector2 GetSize()
        {
            return Size;
        }

        //Get the current object animation frame
        public Vector2 GetFrame()
        {
            return Frame;
        }

        //Show the object
        public void Show()
        {
            Visible = true;
        }

        //Hide the object
        public void Hide()
        {
            Visible = false;
        }

        //Return current visibility
        public bool isVisible()
        {
            return Visible;
        }

#endregion

        #region collisions
        //Returns whether a 2D spaced object intersects with another 2D spaced object
        public bool Collides(Vector2 _Position, Vector2 _Size)
        {
            if(Visible)
            if (Position.X + Size.X + Velocity.X > _Position.X &&
                  Position.Y + Size.Y + Velocity.Y > _Position.Y &&
                  Position.X + Velocity.X < _Position.X + _Size.X &&
                Position.Y + Velocity.Y < _Position.Y + _Size.Y)
                return true;
                return false;
        }
        #endregion

        #region animations
        public void Animate(GameTime gameTime)
        {
            tmrTimer += gameTime.ElapsedGameTime;
            if (tmrTimer.Milliseconds >= 500)
            {
                if (Frame.X == framesinAnim)
                    Frame.X = 0;
                else
                    Frame.X++;
                tmrTimer = TimeSpan.Zero;
            }
        }
        #endregion

        #region draw
        //Draws the object based on the current bounding box rectangle
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(Texture!=null)
            if (Visible)
            {
                sourceRect = new Rectangle((int)Frame.X * (int)this.Size.X, (int)Frame.Y * (int)this.Size.Y, (int)this.Size.X, (int)this.Size.Y);
                spriteBatch.Draw(Texture, Position, sourceRect, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            }
        }
        #endregion
    }
}
