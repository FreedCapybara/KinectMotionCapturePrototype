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
				sb.Append(GetCode(skeleton, "RightShoulder", JointType.ElbowRight, refPosition));
				// right elbow -> hand
				sb.Append(GetCode(skeleton, "RightElbow", JointType.HandRight, refPosition));
				// left shoulder -> left elbow
				sb.Append(GetCode(skeleton, "LeftShoulder", JointType.ElbowLeft, refPosition));
				// left elbow -> hand
				sb.Append(GetCode(skeleton, "LeftElbow", JointType.HandLeft, refPosition));
				// right hip -> right knee
				sb.Append(GetCode(skeleton, "RightHip", JointType.KneeRight, refPosition));
				// right knee -> right foot
				sb.Append(GetCode(skeleton, "RightKnee", JointType.FootRight, refPosition));
				// left hip -> left knee
				sb.Append(GetCode(skeleton, "LeftHip", JointType.KneeLeft, refPosition));
				// left knee -> left foot
				sb.Append(GetCode(skeleton, "LeftKnee", JointType.FootLeft, refPosition));
				// delay
				sb.Append("box.delay(0.0166);\n");
            }

            using (StreamWriter outfile = new StreamWriter(filename, false))
            {
                outfile.Write(sb.ToString());
            }
        }

		private string GetCode(Skeleton skeleton, string aliceJointFrom, JointType kinectJointTo, SkeletonPoint refPosition, double scale = 5)
		{
			var position = skeleton.Joints.First(joint => joint.JointType == kinectJointTo).Position;
			return string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\nbiped.get{3}().pointAt(box, PointAt.duration(0));\n",
				(position.X - refPosition.X) * scale,
				(position.Y - refPosition.Y) * scale,
				(position.Z - refPosition.Z) * scale,
				aliceJointFrom
			);
		}
    }
}
