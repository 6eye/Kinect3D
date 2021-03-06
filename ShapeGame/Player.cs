//------------------------------------------------------------------------------
// <copyright file="Player.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace ShapeGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Microsoft.Kinect;
    using System.Windows.Media.Media3D;

    public class Player
    {

        JointCollection joints;

        // Keeping track of all bone segments of interest as well as head, hands and feet
        //private readonly Dictionary<Bone, BoneData> segments = new Dictionary<Bone, BoneData>();
        private readonly int id;
        private Rect playerBounds;
        private System.Windows.Point playerCenter;
        private double playerScale;
        private Cube head, leftArm, rightArm, rightLeg, leftLeg, rightCalf, leftCalf, rightForearm, leftForearm, torso, chest;
        private Cube leftHand, rightHand, leftFoot, rightFoot;

        public Player(int skeletonSlot, Viewport3D myViewport3D)
        {
            this.id = skeletonSlot;
            this.head = new Cube(myViewport3D, (float)0.5);
            this.leftArm = new Cube(myViewport3D, (float)0.2);
            this.rightArm = new Cube(myViewport3D, (float)0.2);
            this.rightLeg = new Cube(myViewport3D, (float)0.2);
            this.leftLeg = new Cube(myViewport3D, (float)0.2);
            this.torso = new Cube(myViewport3D, (float)0.6);
            this.chest = new Cube(myViewport3D, (float)0.45);
            this.leftForearm = new Cube(myViewport3D, (float)0.2);
            this.rightForearm = new Cube(myViewport3D, (float)0.2);
            this.leftCalf = new Cube(myViewport3D, (float)0.2);
            this.rightCalf = new Cube(myViewport3D, (float)0.2);

            this.leftHand = new Cube(myViewport3D, (float)0.7);
            this.rightHand = new Cube(myViewport3D, (float)0.7);
            this.leftFoot = new Cube(myViewport3D, (float)0.7);
            this.rightFoot = new Cube(myViewport3D, (float)0.7);

            this.LastUpdated = DateTime.Now;
            //setupMesh(myViewport3D);
        }

        public bool IsAlive { get; set; }

        public DateTime LastUpdated { get; set; }

        public int GetId()
        {
            return this.id;
        }

        public void SetBounds(Rect r)
        {
            this.playerBounds = r;
            this.playerCenter.X = (this.playerBounds.Left + this.playerBounds.Right) / 2;
            this.playerCenter.Y = (this.playerBounds.Top + this.playerBounds.Bottom) / 2;
            this.playerScale = Math.Min(this.playerBounds.Width, this.playerBounds.Height / 2);
        }

        public void UpdateAllJoints(Microsoft.Kinect.JointCollection joints)
        {
            this.joints = joints;
        }

        public void Draw()
        {
            if (!this.IsAlive)
            {
                return;
            }
        
            // Draw all bones first, then circles (head and hands).
            DateTime cur = DateTime.Now;

            Point3DCollection myPositionCollection = new Point3DCollection();

            Console.WriteLine("{0} {1} {2}",
                    (float)joints[JointType.Head].Position.X, (float)joints[JointType.Head].Position.Y, (float)joints[JointType.Head].Position.Z
            );

            this.head.update(joints[JointType.Head].Position, joints[JointType.ShoulderCenter].Position);

            this.leftArm.update(joints[JointType.ShoulderLeft].Position, joints[JointType.ElbowLeft].Position);
            this.rightArm.update(joints[JointType.ShoulderRight].Position, joints[JointType.ElbowRight].Position);
            this.leftForearm.update(joints[JointType.ElbowLeft].Position, joints[JointType.WristLeft].Position);
            this.rightForearm.update(joints[JointType.ElbowRight].Position, joints[JointType.WristRight].Position);
            this.leftHand.update(joints[JointType.WristLeft].Position, joints[JointType.HandLeft].Position);
            this.rightHand.update(joints[JointType.WristRight].Position, joints[JointType.HandRight].Position);

            this.leftLeg.update(joints[JointType.HipLeft].Position, joints[JointType.KneeLeft].Position);
            this.rightLeg.update(joints[JointType.HipRight].Position, joints[JointType.KneeRight].Position);
            this.leftCalf.update(joints[JointType.KneeLeft].Position, joints[JointType.AnkleLeft].Position);
            this.rightCalf.update(joints[JointType.KneeRight].Position, joints[JointType.AnkleRight].Position);
            this.leftFoot.update(joints[JointType.AnkleLeft].Position, joints[JointType.FootLeft].Position);
            this.rightFoot.update(joints[JointType.AnkleRight].Position, joints[JointType.FootRight].Position);

            this.chest.update(joints[JointType.ShoulderCenter].Position, joints[JointType.Spine].Position);
            this.torso.update(joints[JointType.Spine].Position, joints[JointType.HipCenter].Position);

            /*  point order
             *  { bottom left }
             *  { top left }
             *  { top right }
             *  { bottom right }
             */

            /*float[,] P = {
                {-(float)joints[JointType.Head].Position.X , (float)joints[JointType.Head].Position.Y, (float)joints[JointType.Head].Position.Z},
                {-(float)joints[JointType.Head].Position.X , (float)joints[JointType.Head].Position.Y + 0.05f, (float)joints[JointType.Head].Position.Z},
                {-(float)joints[JointType.Head].Position.X + 0.05f, (float)joints[JointType.Head].Position.Y + 0.05f, (float)joints[JointType.Head].Position.Z},
                {-(float)joints[JointType.Head].Position.X + 0.05f, (float)joints[JointType.Head].Position.Y, (float)joints[JointType.Head].Position.Z}, 
                
                {-(float)joints[JointType.ShoulderLeft].Position.X , (float)joints[JointType.ShoulderLeft].Position.Y, (float)joints[JointType.ShoulderLeft].Position.Z},
                {-(float)joints[JointType.ShoulderLeft].Position.X , (float)joints[JointType.ShoulderLeft].Position.Y + 0.05f, (float)joints[JointType.ShoulderLeft].Position.Z},
                {-(float)joints[JointType.ShoulderLeft].Position.X + 0.05f, (float)joints[JointType.ShoulderLeft].Position.Y + 0.05f, (float)joints[JointType.ShoulderLeft].Position.Z},
                {-(float)joints[JointType.ShoulderLeft].Position.X + 0.05f, (float)joints[JointType.ShoulderLeft].Position.Y, (float)joints[JointType.ShoulderLeft].Position.Z}, 

                {-(float)joints[JointType.ShoulderRight].Position.X , (float)joints[JointType.ShoulderRight].Position.Y, (float)joints[JointType.ShoulderRight].Position.Z},
                {-(float)joints[JointType.ShoulderRight].Position.X , (float)joints[JointType.ShoulderRight].Position.Y + 0.05f, (float)joints[JointType.ShoulderRight].Position.Z},
                {-(float)joints[JointType.ShoulderRight].Position.X + 0.05f, (float)joints[JointType.ShoulderRight].Position.Y + 0.05f, (float)joints[JointType.ShoulderRight].Position.Z},
                {-(float)joints[JointType.ShoulderRight].Position.X + 0.05f, (float)joints[JointType.ShoulderRight].Position.Y, (float)joints[JointType.ShoulderRight].Position.Z}, 

                {-(float)joints[JointType.ElbowRight].Position.X , (float)joints[JointType.ElbowRight].Position.Y, (float)joints[JointType.ElbowRight].Position.Z},
                {-(float)joints[JointType.ElbowRight].Position.X , (float)joints[JointType.ElbowRight].Position.Y + 0.05f, (float)joints[JointType.ElbowRight].Position.Z},
                {-(float)joints[JointType.ElbowRight].Position.X + 0.05f, (float)joints[JointType.ElbowRight].Position.Y + 0.05f, (float)joints[JointType.ElbowRight].Position.Z},
                {-(float)joints[JointType.ElbowRight].Position.X + 0.05f, (float)joints[JointType.ElbowRight].Position.Y, (float)joints[JointType.ElbowRight].Position.Z}, 

                {-(float)joints[JointType.ElbowLeft].Position.X , (float)joints[JointType.ElbowLeft].Position.Y, (float)joints[JointType.ElbowLeft].Position.Z},
                {-(float)joints[JointType.ElbowLeft].Position.X , (float)joints[JointType.ElbowLeft].Position.Y + 0.05f, (float)joints[JointType.ElbowLeft].Position.Z},
                {-(float)joints[JointType.ElbowLeft].Position.X + 0.05f, (float)joints[JointType.ElbowLeft].Position.Y + 0.05f, (float)joints[JointType.ElbowLeft].Position.Z},
                {-(float)joints[JointType.ElbowLeft].Position.X + 0.05f, (float)joints[JointType.ElbowLeft].Position.Y, (float)joints[JointType.ElbowLeft].Position.Z}, 

            };
           

            for (int i = 0; i < P.Length/3; i++)
            {
                myPositionCollection.Add(new Point3D(
                    P[i, 0],
                    P[i, 1],
                    P[i, 2]
                ));
            }

            this.mesh.Positions = myPositionCollection;*/
                    //myTranslation.OffsetX = seg.X1 / 100 - 3;

            // Remove unused players after 1/2 second.
            if (DateTime.Now.Subtract(this.LastUpdated).TotalMilliseconds > 500)
            {
                this.IsAlive = false;
            }
        }
    }
}
