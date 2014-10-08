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

            foreach (var skeleton in frames)
            {
				var rightElbowPosition = skeleton.Joints.First(joint => joint.JointType == JointType.ElbowRight).Position;
				var rightHandPosition = skeleton.Joints.First(joint => joint.JointType == JointType.HandRight).Position;
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n", rightElbowPosition.X * 10, rightElbowPosition.Y * 10, rightElbowPosition.Z * 0));
				sb.Append("biped.getRightShoulder().pointAt(box, PointAt.duration(0));\n");
				sb.Append(string.Format("box.setPositionRelativeToVehicle(new Position({0}, {1}, {2}), Move.duration(0));\n", rightHandPosition.X * 10, rightHandPosition.Y * 10, rightHandPosition.Z * 0));
				sb.Append("biped.getRightElbow().pointAt(box, PointAt.duration(0));\n");
				sb.Append("box.delay(0.0166);\n");
            }

            using (StreamWriter outfile = new StreamWriter(filename, false))
            {
                outfile.Write(sb.ToString());
            }
        }
    }
}
