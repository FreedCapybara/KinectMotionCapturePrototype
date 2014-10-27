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

		private AliceCodeGenerator aliceGenerator;

		public SkeletonRecorder()
		{
			aliceGenerator = new AliceCodeGenerator();
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
			if (frames.Count == 0)
			{
				return;
			}

			StringBuilder sb = new StringBuilder();

			var rootPosition = frames.First().Joints.First(joint => joint.JointType == JointType.HipCenter).Position;

			foreach (var skeleton in frames)
			{
				sb.Append(aliceGenerator.GetMovementCode(skeleton, rootPosition));
				sb.Append(aliceGenerator.GetJointsCode(skeleton));
				sb.Append("box.delay(0.0166);\n"); // delay 1/60s each frame
			}

			using (StreamWriter outfile = new StreamWriter(filename, false))
			{
				outfile.Write(sb.ToString());
			}
		}
	}
}
