using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
	class SkeletonRecorder
	{
		class JointData
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

		private List<Skeleton> frames = new List<Skeleton>();
		private Dictionary<JointType, SkeletonPoint> finalPositions = new Dictionary<JointType, SkeletonPoint>();
		private IEnumerator<Skeleton> frameEnumerator;

		private JointData[] boneData = new JointData[16] {
			new JointData("Pelvis", JointType.HipCenter, JointType.ShoulderCenter, 0.4375f),
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

		public SkeletonRecorder()
		{
			// seed the dictionary
			for (var i = JointType.HipCenter; i < JointType.FootRight; i++)
			{
				finalPositions.Add(i, new SkeletonPoint());
			}
		}

		public void Clear()
		{
			frames.Clear();
		}

		public void Capture(Skeleton skeleton)
		{
			frames.Add(skeleton);
		}

		public Skeleton GetFrame()
		{
			if (frameEnumerator == null)
			{
				frameEnumerator = frames.GetEnumerator();
			}

			if (!frameEnumerator.MoveNext())
			{
				frameEnumerator.Reset();
				frameEnumerator.MoveNext();
			}

			return frameEnumerator.Current;
		}

		public void Export(string filename)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var skeleton in frames)
			{
				sb.Append(GetCode(skeleton));
				sb.Append("box.delay(0.0166);\n"); // delay 1/60s each frame
			}

			using (StreamWriter outfile = new StreamWriter(filename, false))
			{
				outfile.Write(sb.ToString());
			}
		}

		private string GetCode(Skeleton skeleton)
		{
			// compute the final positions for each joint and add the Alice code to the stringbuilder
			SkeletonPoint fromJoint;
			SkeletonPoint toJoint;
			SkeletonPoint adjustedDifference;
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < boneData.Length; i++)
			{
				fromJoint = skeleton.Joints.First(joint => joint.JointType == boneData[i].KinectJointFrom).Position;
				toJoint = skeleton.Joints.First(joint => joint.JointType == boneData[i].KinectJointTo).Position;
				// adjust the position difference between the two joints so that it matches the length of the Alice joint
				// to do this,
				//		(1) get the difference [subtract],
				//		(2) get a unit vector [normalize], and
				//		(3) multiply by the length of the bone
				adjustedDifference = toJoint
										.Subtract(fromJoint)
										.Normalize()
										.Multiply(boneData[i].JointLength * 5);
				// the adjusted difference needs to be added to the final position of the 'from' joint,
				// which is stored in a dictionary and has been computed already if the values in boneData are ordered correctly
				// ~ it's kind of a dynamic programming approach in that it avoids repeated calculation of the previous joints
				var finalPosition = finalPositions[boneData[i].KinectJointFrom].Add(adjustedDifference);
				finalPositions[boneData[i].KinectJointTo] = finalPosition;

				// add the Alice code to the stringbuilder
				// (ignore rotations from the neck and pelvis for now, which screw stuff up)
				if (!ignore.Contains(boneData[i].AliceJointFrom))
				{
					result.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));biped.get{3}().pointAt(box, PointAt.duration(0));\n",
						finalPosition.X, finalPosition.Y, finalPosition.Z,
						boneData[i].AliceJointFrom
					));
				}
			}
			return result.ToString();
		}

		static string ignore = "Neck Pelvis LeftAnkle RightAnkle";
	}
}
