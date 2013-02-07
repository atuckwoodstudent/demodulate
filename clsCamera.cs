using Microsoft.Xna.Framework;

namespace DeModulate
{
    class clsCamera
    {
        #region fields
        //Type Declarations
        private Vector3 position;
        public Matrix viewMatrix, projectionMatrix;

        //Constructor 
        public clsCamera()
        {
            MainCam();
        }
        #endregion

        #region camtypes
        //Method to process main camera angle
        public void MainCam()
        {
            position = new Vector3(0, 25, 45);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(40.0f), 800f/600f, 1f, 10000f);
        }

        //Method to process topdown camera angle
        public void TopDown()
        {
            position = new Vector3(0,62,4);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), 800f / 600f, 1f, 100f);
        }
        #endregion

        #region update
        //Update camera matrix
        public void Update()
        {
            viewMatrix = Matrix.CreateLookAt(position, new Vector3(position.X, 0, 3), Vector3.Up);
        }
        #endregion
    }
}
