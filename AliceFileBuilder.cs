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
		public string AnimationClassName { get; set; }
		public string OutputDirectory { get; set; }

		private AliceCodeGenerator aliceGenerator;

		private StringBuilder sb = new StringBuilder();
		private int currentFrame;
		private int currentSegment;

		private string segmentTemplate; // {0} segment number, {1} data
		private string animationTemplate; // {0} number of segments

		public AliceFileBuilder(int framesPerSegment = 80, int maxSegments = 50, string animationClassName = "KinectAnimation", string outputDirectory = "generated")
		{
			aliceGenerator = new AliceCodeGenerator();
			// note: it's approx. 10 lines of code per frame
			FramesPerSegment = framesPerSegment;
			MaxSegments = maxSegments;
			AnimationClassName = animationClassName;
			OutputDirectory = outputDirectory;
		}

		public void Prepare()
		{
			// clear the string builder and initialize frame and segment counts
			sb.Clear();
			currentFrame = 0;
			currentSegment = 0;

			// load the templates if they haven't already been loaded
			if (animationTemplate == null)
			{
				using (StreamReader reader = new StreamReader("AliceTemplates/KinectAnimation.java"))
				{
					animationTemplate = reader.ReadToEnd();
				}
			}

			if (segmentTemplate == null)
			{
				using (StreamReader reader = new StreamReader("AliceTemplates/AnimationSegment.java"))
				{
					segmentTemplate = reader.ReadToEnd();
				}
			}

			Directory.CreateDirectory(OutputDirectory);
		}

		public void ApplyFrame(Skeleton skeleton)
		{
			if (currentSegment > MaxSegments)
			{
				return;
			}

			sb.Append(aliceGenerator.GetMovementCode(skeleton));
			sb.Append(aliceGenerator.GetJointsCode(skeleton));
			sb.Append("box.delay(0.0166);\n"); // delay 1/60s each frame

			currentFrame++;
			// write an animation segment if we reach the maximum number of frames for a segment
			if (currentFrame > FramesPerSegment)
			{
				WriteSB();
				currentFrame = 0;
				currentSegment++;
			}
		}

		public void Finish()
		{
			// write any remaining contents of the stringbuilder
			WriteSB();
			// write the total number of segments (currentSegment + 1) to the KinectAnimation template
			var outputFileName = string.Format("{0}/{1}.java", OutputDirectory, AnimationClassName);
			using (StreamWriter outfile = new StreamWriter(outputFileName, false))
			{
				outfile.Write(string.Format(animationTemplate, currentSegment + 1));
			}
			// copy the interface to the output directory
			var outputFile = string.Format("{0}/IAnimator.java", OutputDirectory);
			File.Copy("AliceTemplates/IAnimator.java", outputFile, true);
			// run java to create a jar file
		}

		/// <summary>
		/// Helper method that writes the StringBuilder's contents to an AnimationSegment file and clears its contents.
		/// </summary>
		private void WriteSB()
		{
			var fileName = string.Format("{0}/AnimationSegment{1}.java", OutputDirectory, currentSegment);
			using (StreamWriter outfile = new StreamWriter(fileName, false))
			{
				outfile.Write(string.Format(segmentTemplate, currentSegment, sb.ToString()));
			}
			sb.Clear();
		}
	}
}
