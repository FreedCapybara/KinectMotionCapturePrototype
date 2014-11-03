﻿using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
	class AliceFileBuilder
	{
		private readonly string package = "edu/calvin/cs/alicekinect";

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

		public AliceFileBuilder(int framesPerSegment = 80, int maxSegments = 50, string animationClassName = "KinectAnimation", string outputDirectory = ".")
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

			try
			{
				Directory.Delete("src", true);
				Directory.Delete("build", true);
			}
			catch (Exception)
			{
			}

			Directory.CreateDirectory(OutputDirectory);
			Directory.CreateDirectory(string.Format("src/{0}", package));
			Directory.CreateDirectory("build");
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
			var outputFileName = string.Format("src/{0}/{1}.java", package, AnimationClassName);
			using (StreamWriter outfile = new StreamWriter(outputFileName, false))
			{
				outfile.Write(string.Format(animationTemplate, currentSegment + 1));
			}
			// copy the interface to the output directory
			var outputFile = string.Format("src/{0}/IAnimator.java", package);
			File.Copy("AliceTemplates/IAnimator.java", outputFile, true);
			// compile the sources and create a jar file
			RunBuildScript();
		}

		/// <summary>
		/// Helper method that writes the StringBuilder's contents to an AnimationSegment file and clears its contents.
		/// </summary>
		private void WriteSB()
		{
			var fileName = string.Format("src/{0}/AnimationSegment{1}.java", package, currentSegment);
			using (StreamWriter outfile = new StreamWriter(fileName, false))
			{
				outfile.Write(string.Format(segmentTemplate, currentSegment, sb.ToString()));
			}
			sb.Clear();
		}

		//http://stackoverflow.com/questions/1469764/run-command-prompt-commands
		private void RunBuildScript()
		{
			Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.FileName = "powershell.exe";
			startInfo.Arguments = string.Format("./AliceBuild/build.ps1 {0}", AnimationClassName);
			process.StartInfo = startInfo;
			process.Start();
		}
	}
}