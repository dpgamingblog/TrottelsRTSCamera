using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrottelsRTSSystem
{

    /// <summary>
    /// Terrain Height Sensible RTS Camera with focus on a center point, to which it is parented
    /// todo : make a curve editor window for the vales to make finetuning easier !
    /// 
    /// Made in 2020 by Tom Trottel 
    /// </summary>
    public class TopDownRTSCameraController : MonoBehaviour
    {
        #region Public Fields

        public bool debugMode = false;

        public float heightAdjustmentMultiplicator = 2f;

        public float minimalHeight = 4f;

        public float rotationAngleXforMinimalHeight = 14f;

        public float maximalHeight = 14f;

        public float rotationAngleXforMaximalHeight = 55f;

        #endregion

        #region Private Fields

        bool thereIsNoTerrainUnderTheCamera = false;

        float lastTerrainCollisionHeight;

        #endregion

        #region Unity Methods
        #endregion

        #region Class Methods

        public void AdjustHeight(float mouseWheelValue)
        {

            transform.position += new Vector3(0, mouseWheelValue * heightAdjustmentMultiplicator, 0);


            float terrainCorrectedHeight = GetHeightRelativeToTerrain().y;

            // make sure that there is actually terrain under the camera
            // if not, use the last valid terrain height ray value !

            if (this.thereIsNoTerrainUnderTheCamera)
                terrainCorrectedHeight = this.lastTerrainCollisionHeight;
            else
                this.lastTerrainCollisionHeight = terrainCorrectedHeight;


            // make sure the camera isnt going to low or to high, relativ to the terrain under it !

            if (transform.position.y < minimalHeight + terrainCorrectedHeight)
            {
                Vector3 correctedHeight = transform.position;
                correctedHeight.y = minimalHeight + terrainCorrectedHeight + 0.3f;
                transform.position = correctedHeight;
            }

            if (transform.position.y > maximalHeight + terrainCorrectedHeight)
            {
                Vector3 correctedHeight = transform.position;
                correctedHeight.y = maximalHeight + terrainCorrectedHeight;
                transform.position = correctedHeight;
            }

            this.AdjustVerticalAngle(terrainCorrectedHeight);
        }

        public void AdjustVerticalAngle(float terrainCorrectedHeight)
        {

            // first convert the position to 0 to maximalheight - minimalheight
            // then get the percentage that the current height is of that
            // then use that percentage to get the x rotation for that height from the 

            float transformedHeightToZero = maximalHeight - minimalHeight;

            float currentHeight = (this.transform.position.y - terrainCorrectedHeight) - minimalHeight;

            float heightPercentage = currentHeight / transformedHeightToZero;

            float transformedAngleToZero = rotationAngleXforMaximalHeight - rotationAngleXforMinimalHeight;

            float currentXRotationAngle = (transformedAngleToZero * heightPercentage) + rotationAngleXforMinimalHeight;

            Quaternion currentRotation = this.transform.rotation;

            Vector3 currentRotationInEuler = currentRotation.eulerAngles;

            currentRotationInEuler.x = currentXRotationAngle;

            currentRotation = Quaternion.Euler(currentRotationInEuler);

            this.transform.rotation = currentRotation;


        }

        public Vector3 GetHeightRelativeToTerrain()
        {

            // only terrain !
            int layerMask = 1 << 8;

            Ray rayFromCameraDown = new Ray(transform.position, Vector3.down);

            RaycastHit hit;

            if (Physics.Raycast(rayFromCameraDown, out hit, 200f, layerMask))
            {
                this.thereIsNoTerrainUnderTheCamera = false;

                return hit.point;
            }

            // set this if there is no terrain under the camera
            // that happens when the mouse wheel is turned fast and the camera 
            // goes under the terrain before the adjustment is done !!!

            this.thereIsNoTerrainUnderTheCamera = true;

            return Vector3.zero;
        }



        protected void dbg(string message, bool error = false)
        {
            if (debugMode & !error)
                Debug.Log("[ " + this.GetType().Name + " (" + Time.time + ")] " + message);

            if (error)
                Debug.LogError("[" + this.GetType().Name + " (" + Time.time + ")] " + message);
        }


        #endregion
    }




}
