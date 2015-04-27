using edu.calvin.cs.alicekinect;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
	/// <summary>
	/// Exports, compiles, and creates a jar from Java classes containing animation code for use in Alice.
	/// </summary>
	class AliceFileBuilder
	{
		private readonly string package = "edu/calvin/cs/alicekinect";

		public int FramesPerSegment { get; set; }
		public int MaxSegments { get; set; }
		public string AnimationClassName { get; set; }
		public string OutputDirectory { get; set; }

		private AliceCodeGenerator aliceGenerator;

		private AliceKinect aliceKinect = new AliceKinect(@"C:\Users\andrew\Documents\IdeaProjects\A3C-Example-Workspace\a3c-example\input\AdultPerson.a3c");
		private int currentFrame;
		private int currentSegment;

		public AliceFileBuilder(int framesPerSegment = 25, int maxSegments = 50, string animationClassName = "KinectAnimation", string outputDirectory = ".")
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
			aliceKinect.reset();
			currentFrame = 0;
			currentSegment = 0;
		}

		/// <summary>
		/// Adds one animation frame to the exportable code.
		/// When the number of frames processed surpasses the number of frames in a segment,
		/// all the code generated is written to a Java class (AnimationSegmentX.java where X is the segment number)
		/// and the next segment is started.  The goal is to make a large group of classes to avoid Java 'code too large' compiler errors.
		/// </summary>
		/// <param name="skeleton">A skeleton representing a frame of the animation</param>
		public void ApplyFrame(Skeleton skeleton)
		{
			if (currentSegment >= MaxSegments)
			{
				return;
			}

			aliceGenerator.GetMovementCode(skeleton, aliceKinect);
			aliceGenerator.GetJointsCode(skeleton, aliceKinect);
			aliceKinect.addDelay(0.0166);

			currentFrame++;
			// write an animation segment if we reach the maximum number of frames for a segment
			if (currentFrame > FramesPerSegment)
			{
				WriteSB();
				currentFrame = 0;
				currentSegment++;
			}
		}

		/// <summary>
		/// Finalizes the animation, then compiles and jars it.
		/// </summary>
		public void Finish()
		{
			// Export the skeleton data to .a3c
			WriteSB();
		}

		/// <summary>
		/// Helper method that writes the AST built by AliceKinect and clears it.
		/// </summary>
		private void WriteSB()
		{
			var fileName = string.Format("{0}/{1}{2}.a3c", OutputDirectory, AnimationClassName, currentSegment);
			aliceKinect.export(fileName);
			aliceKinect.reset();
		}

		/// <summary>
		/// Runs a Powershell script to compile and jar the generated source code.
		/// </summary>
		//http://stackoverflow.com/questions/1469764/run-command-prompt-commands
		private void RunBuildScript()
		{
			Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.FileName = "powershell.exe";
			startInfo.Arguments = string.Format("./AliceBuild/build.ps1 {0} {1}", OutputDirectory, AnimationClassName);
			process.StartInfo = startInfo;
			process.Start();
		}
	}
}
