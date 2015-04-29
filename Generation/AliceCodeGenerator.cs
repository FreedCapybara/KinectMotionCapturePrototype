using edu.calvin.cs.alicekinect;
using Microsoft.Kinect;
using org.lgna.story;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
	/// <summary>
	/// Generates code for Alice animation methods from Kinect skeletons,
	/// adjusting for Alice bone lengths to produce more accurate animations.
	/// 
	/// Alice Bipeds are animated with invisible scene objects that are placed relative to an Alice Biped according to Kinect data.
	/// The algorithm is something like the following:
	/// 
	///		For every skeletonJoint in a sequence of joints:
	///			(1) position an object at the coordinates of the joint from the Kinect data
	///			(2) point the corresponding Alice Biped joint at the object
	///			(3) -optional- rotate the joint as necessary to make it look more normal
	///	
	/// </summary>
	class AliceCodeGenerator
	{
		/// <summary>
		/// Helper class for holding data to calculate skeleton rotations.
		/// </summary>
		// See boneData[] below.
		private class JointData
		{
			public string AliceJointFrom { get; set; }
			public JointType KinectJointFrom { get; set; }
			public JointType KinectJointTo { get; set; }
			public float JointLength { get; set; }

			public JointData(string aliceJointFrom, JointType kinectJointFrom, JointType kinectJointTo, float jointLength)
			{
				this.AliceJointFrom = aliceJointFrom;
				this.KinectJointFrom = kinectJointFrom;
				this.KinectJointTo = kinectJointTo;
				this.JointLength = jointLength;
			}
		}

		/// <summary>
		/// String of Alice joint names to ignore when calculating bone rotations.
		/// These are mostly joints that cause the Biped to become too distorted in a recording.
		/// </summary>
		private string ignore = "Neck Pelvis LeftAnkle RightAnkle";
		private string rotationIgnore = "LeftShoulder RightShoulder";

		/// <summary>
		/// The data in this array specifies
		/// (1) the order in which the joints are processed, and
		/// (2) the length of each bone for improved animation accuracy when translating the movements to Alice,
		/// </summary>
		private JointData[] boneData = new JointData[16] {
			new JointData("SpineBase", JointType.HipCenter, JointType.ShoulderCenter, 0.4375f),
			new JointData("Neck", JointType.ShoulderCenter, JointType.Head, 0.0625f), 
			new JointData("Neck", JointType.ShoulderCenter, JointType.ShoulderLeft, 0.125f),
			new JointData("Neck", JointType.ShoulderCenter, JointType.ShoulderRight, 0.125f),
			new JointData("LeftShoulder", JointType.ShoulderLeft, JointType.ElbowLeft, 0.25f),
			new JointData("RightShoulder", JointType.ShoulderRight, JointType.ElbowRight, 0.25f),
			new JointData("LeftElbow", JointType.ElbowLeft, JointType.HandLeft, 0.125f),
			new JointData("RightElbow", JointType.ElbowRight, JointType.HandRight, 0.125f),
			new JointData("Pelvis", JointType.HipCenter, JointType.HipLeft, 0.0875f),
			new JointData("Pelvis", JointType.HipCenter, JointType.HipRight, 0.0875f),
			new JointData("LeftHip", JointType.HipLeft, JointType.KneeLeft, 0.5f),
			new JointData("RightHip", JointType.HipRight, JointType.KneeRight, 0.5f),
			new JointData("LeftKnee", JointType.KneeLeft, JointType.FootLeft, 0.4375f),
			new JointData("RightKnee", JointType.KneeRight, JointType.FootRight, 0.4375f),
			new JointData("LeftAnkle", JointType.AnkleLeft, JointType.FootLeft, 0.125f),
			new JointData("RightAnkle", JointType.AnkleRight, JointType.FootRight, 0.125f),
		};

		/// <summary>
		/// Stores the final positions of joints so the absolute positions of joints can be computed without recomputing parent joints.
		/// </summary>
		private static Dictionary<JointType, SkeletonPoint> finalPositions = new Dictionary<JointType, SkeletonPoint>();
		/// <summary>
		/// The original position of a joint in the Kinect skeleton that will play the role of root joint,
		/// from which translations of the entire Alice Biped are calculated in #GetMovementCode().
		/// </summary>
		private SkeletonPoint originalRootPosition;
		/// <summary>
		/// Flag indicating whether the original root position has been set already (SkeletonPoint is a struct, so this can't be accomplished with a null check).
		/// </summary>
		private bool originalRootPositionSet;

		public AliceCodeGenerator()
		{
			// seed the dictionary
			for (var i = JointType.HipCenter; i < JointType.FootRight; i++)
			{
				finalPositions.Add(i, new SkeletonPoint());
			}
			// set the final position of the root joint
			finalPositions[JointType.HipCenter] = new SkeletonPoint() { Y = 1.0f };
		}

		public void Init()
		{
			originalRootPositionSet = false;
		}

		/// <summary>
		/// Generates Alice code for moving the entire Biped (allows the Biped to bounce a little during a walking animation, for example).
		/// </summary>
		/// <param name="skeleton">The Kinect skeleton to calculate movements from</param>
		/// <returns>A string of Alice code that executes a Biped's movements</returns>
		public void GetMovementCode(Skeleton skeleton, AliceKinect aliceKinect)
		{
			// get the pelvis
			var pelvis = skeleton.Joints[JointType.HipCenter].Position;
			// set the pelvis as the original root if it hasn't been set yet (should happen on the first frame after init() only)
			if (!originalRootPositionSet)
			{
				originalRootPosition = pelvis;
				originalRootPositionSet = true;
			}
			// get the position difference between the original root position and the current position of the root
			var moveDifference = pelvis.Subtract(originalRootPosition).Normalize().Multiply(0.25f);

			// this moving algorithm attempts to use absolute positioning rather than relative positioning (adjusting based on the previous position)
			// in order to stop Alice characters from drifting away after the animation has run a few iterations
			// 1. move a box that does not have the biped as a vehicle to the desired position
			aliceKinect.addSetRootPositionRelativeToVehicle(0, moveDifference.Y, 0);
			// 2. move the person to the box
			aliceKinect.addMoveToRoot();
		}

		/// <summary>
		/// Generates Alice code for rotating a Biped's joints using another object present in the scene.
		/// The object is placed at each of the skeleton's joints, and then in Alice the parent joint is is pointed at the object to create the desired rotation.
		/// </summary>
		/// <param name="skeleton">The Kinect skeleton to calculate joint rotations from</param>
		/// <returns>A string of Alice code that rotates the joints into their final positions according to the provided Kinect skeleton</returns>
		public void GetJointsCode(Skeleton skeleton, AliceKinect aliceKinect)
		{
			const double twoPi = Math.PI;
			// compute the final positions for each joint and add the Alice code to the stringbuilder
			SkeletonPoint fromJoint;
			SkeletonPoint toJoint;
			SkeletonPoint adjustedDifference;

			for (int i = 0; i < boneData.Length; i++)
			{
				fromJoint = skeleton.Joints[boneData[i].KinectJointFrom].Position;
				toJoint = skeleton.Joints[boneData[i].KinectJointTo].Position;
				// adjust the position difference between the two joints so that it matches the length of the Alice joint
				// to do this,
				//		(1) get the difference [subtract],
				//		(2) get a unit vector [normalize], and
				//		(3) multiply by the length of the bone
				adjustedDifference = toJoint
										.Subtract(fromJoint)
										.Normalize()
										.Multiply(boneData[i].JointLength);
				// the adjusted difference needs to be added to the final position of the 'from' joint,
				// which is stored in a dictionary and has been computed already if the values in boneData are ordered correctly
				// ~ it's kind of a dynamic programming approach in that it avoids repeated calculation of the previous joints
				var finalPosition = finalPositions[boneData[i].KinectJointFrom].Add(adjustedDifference);
				finalPositions[boneData[i].KinectJointTo] = finalPosition;

				var rotation = skeleton.BoneOrientations[boneData[i].KinectJointFrom].HierarchicalRotation.Quaternion;
				var rollAmount = rotation.Y / twoPi;

				// add the Alice code
				// (ignore rotations from the neck and pelvis for now, which screw stuff up)
				if (!ignore.Contains(boneData[i].AliceJointFrom))
				{
					// position the box and point the joint at it
					aliceKinect.addSetBoxPositionRelativeToVehicle(finalPosition.X, finalPosition.Y, finalPosition.Z);
					aliceKinect.addPointAt(boneData[i].AliceJointFrom);
					// roll the bone to match the Kinect data
					aliceKinect.addRoll(boneData[i].AliceJointFrom, rollAmount, finalPosition.X > 0 ? RollDirection.LEFT : RollDirection.RIGHT);
					// if the final joint position was behind its parent joint, the parent needs to be rolled 180 degrees (0.5 rotations)
					// otherwise the joint gets turned around and everything looks weird (twisted spines and knees and such)
					if (finalPosition.Z > 0 && !rotationIgnore.Contains(boneData[i].AliceJointFrom))
					{
						aliceKinect.addRoll(boneData[i].AliceJointFrom, 0.5, RollDirection.LEFT);
					}
				}
			}
		}

	}
}
