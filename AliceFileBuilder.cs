using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
	class AliceFileBuilder
	{
		public int FramesPerSegment { get; set; }
		public int MaxSegments { get; set; }
		public string OutputFileName { get; set; }

		private AliceCodeGenerator aliceGenerator;

		private StringBuilder sb = new StringBuilder();
		private int currentFrame;
		private int currentSegment;

		private string segmentTemplate;
		private string animationTemplate;

		public AliceFileBuilder(int framesPerSegment = 750, int maxSegments = 50, string outputFileName = "generated.java")
		{
			aliceGenerator = new AliceCodeGenerator();
			FramesPerSegment = framesPerSegment;
			MaxSegments = maxSegments;
			OutputFileName = outputFileName;
		}

		public void Prepare()
		{
			sb.Clear();
			currentFrame = 0;
			currentSegment = 0;
		}

		public void ApplyFrame(Skeleton skeleton)
		{
			sb.Append(aliceGenerator.GetMovementCode(skeleton));
			sb.Append(aliceGenerator.GetJointsCode(skeleton));
			sb.Append("box.delay(0.0166);\n"); // delay 1/60s each frame

			currentFrame++;
			if (currentFrame > FramesPerSegment)
			{
				WriteSB();
				currentFrame = 0;
				currentSegment++;
			}
		}

		/// <summary>
		/// Helper method that writes the StringBuilder's contents to a file.
		/// </summary>
		private void WriteSB()
		{
			using (StreamWriter outfile = new StreamWriter(OutputFileName, false))
			{
				outfile.Write(sb.ToString());
			}
		}
	}
}
