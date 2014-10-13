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
        private List<Skeleton> frames = new List<Skeleton>();
        private IEnumerator<Skeleton> frameEnumerator;

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

			JointType referenceJointType = JointType.HipCenter;
			var scaleFactor = 5;

            foreach (var skeleton in frames)
            {
				var refPosition = skeleton.Joints.First(joint => joint.JointType == referenceJointType).Position;
				// right shoulder -> right elbow
				var rightElbowPosition = skeleton.Joints.First(joint => joint.JointType == JointType.ElbowRight).Position;
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n",
					(rightElbowPosition.X - refPosition.X) * scaleFactor,
					(rightElbowPosition.Y - refPosition.Y) * scaleFactor,
					(rightElbowPosition.Z - refPosition.Z) * scaleFactor
				));
				sb.Append("biped.getRightShoulder().pointAt(box, PointAt.duration(0));\n");
				// right elbow -> hand
				var rightHandPosition = skeleton.Joints.First(joint => joint.JointType == JointType.HandRight).Position;
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n",
					(rightHandPosition.X - refPosition.X) * scaleFactor,
					(rightHandPosition.Y - refPosition.Y) * scaleFactor,
					(rightHandPosition.Z - refPosition.Z) * scaleFactor
				));
				sb.Append("biped.getRightElbow().pointAt(box, PointAt.duration(0));\n");
				// left shoulder -> left elbow
				var leftElbowPosition = skeleton.Joints.First(joint => joint.JointType == JointType.ElbowLeft).Position;
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n",
						(leftElbowPosition.X - refPosition.X) * scaleFactor,
						(leftElbowPosition.Y - refPosition.Y) * scaleFactor,
						(leftElbowPosition.Z - refPosition.Z) * scaleFactor
					));
				sb.Append("biped.getLeftShoulder().pointAt(box, PointAt.duration(0));\n");	
				// left elbow -> hand
				var leftHandPosition = skeleton.Joints.First(joint => joint.JointType == JointType.HandLeft).Position;
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n",
					(leftHandPosition.X - refPosition.X) * scaleFactor,
					(leftHandPosition.Y - refPosition.Y) * scaleFactor,
					(leftHandPosition.Z - refPosition.Z) * scaleFactor
				));
				sb.Append("biped.getLeftElbow().pointAt(box, PointAt.duration(0));\n");
				// right hip -> right knee
				var rightKneePosition = skeleton.Joints.First(joint => joint.JointType == JointType.KneeRight).Position;
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n",
					(rightKneePosition.X - refPosition.X) * scaleFactor,
					(rightKneePosition.Y - refPosition.Y) * scaleFactor,
					(rightKneePosition.Z - refPosition.Z) * scaleFactor
				));
				sb.Append("biped.getRightKnee().pointAt(box, PointAt.duration(0));\n");
				// right knee -> right foot
				var rightFootPosition = skeleton.Joints.First(joint => joint.JointType == JointType.FootRight).Position;
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n",
					(rightFootPosition.X - refPosition.X) * scaleFactor,
					(rightFootPosition.Y - refPosition.Y) * scaleFactor,
					(rightFootPosition.Z - refPosition.Z) * scaleFactor
				));
				sb.Append("biped.getRightAnkle().pointAt(box, PointAt.duration(0));\n");
				// left hip -> left knee
				var rightKneePosition = skeleton.Joints.First(joint => joint.JointType == JointType.KneeRight).Position;
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n",
					(rightKneePosition.X - refPosition.X) * scaleFactor,
					(rightKneePosition.Y - refPosition.Y) * scaleFactor,
					(rightKneePosition.Z - refPosition.Z) * scaleFactor
				));
				sb.Append("biped.getRightKnee().pointAt(box, PointAt.duration(0));\n");
				// left knee -> left foot
				var rightFootPosition = skeleton.Joints.First(joint => joint.JointType == JointType.FootRight).Position;
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n",
					(rightFootPosition.X - refPosition.X) * scaleFactor,
					(rightFootPosition.Y - refPosition.Y) * scaleFactor,
					(rightFootPosition.Z - refPosition.Z) * scaleFactor
				));
				sb.Append("biped.getRightAnkle().pointAt(box, PointAt.duration(0));\n");
				// delay
				sb.Append("box.delay(0.0166);\n");
            }

            using (StreamWriter outfile = new StreamWriter(filename, false))
            {
                outfile.Write(sb.ToString());
            }
        }
    }
}
