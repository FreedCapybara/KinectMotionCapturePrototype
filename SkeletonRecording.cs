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

            // create a line for each frame in this format:
            // trackingId;jointType,jointX,jointY,jointZ;...;
            foreach (var skeleton in frames)
            {
                sb.Append(string.Format("{0};", skeleton.TrackingId));
                foreach (Joint joint in skeleton.Joints)
                {
                    sb.Append(String.Format("{0},{1},{2},{3};", joint.JointType, joint.Position.X, joint.Position.Y, joint.Position.Z));
                }
                sb.Append("\n");
            }

            using (StreamWriter outfile = new StreamWriter(filename, false))
            {
                outfile.Write(sb.ToString());
            }
        }
    }
}
