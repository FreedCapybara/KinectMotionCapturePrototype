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
	/// Handles generation and exporting of Alice classes.
	/// 
	/// Uses the Java AliceKinect class to build and export an AST containing all the animation code for a biped.
	/// Requires an Alice Biped class from a .a3c file as input.
	/// </summary>
	class AliceFileBuilder
	{
		private readonly string package = "edu/calvin/cs/alicekinect";

		public int FramesPerSegment { get; set; }
		public int MaxSegments { get; set; }
		public string AnimationClassName { get; set; }
		public string OutputDirectory { get; set; }

		private AliceCodeGenerator aliceGenerator;

		private AliceKinect aliceKinect;
		private int currentFrame;
		private int currentSegment;

		public AliceFileBuilder(String inputAliceClassFile = @"Alice\AdultPerson.a3c", int framesPerSegment = 25, int maxSegments = 50, string animationClassName = "KinectAnimation", string outputDirectory = ".")
		{
			aliceGenerator = new AliceCodeGenerator();
			aliceKinect = new AliceKinect(inputAliceClassFile);
			// note: it's approx. 10 statements per frame
			FramesPerSegment = framesPerSegment;
			MaxSegments = maxSegments;
			AnimationClassName = animationClassName;
			OutputDirectory = outputDirectory;
		}

		/// <summary>
		/// Clears all frames and resets the frame and segment counts
		/// </summary>
		public void Prepare()
		{
			aliceKinect.reset();
			currentFrame = 0;
			currentSegment = 0;
		}

		/// <summary>
		/// Applies a Kinect Skeleton to one frame of animation in Alice.
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
				//WriteSB();
				currentFrame = 0;
				currentSegment++;
			}
		}

		/// <summary>
		/// Finalizes the animation.
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
			var fileName = string.Format("{0}/{1}{2}.a3c", OutputDirectory, AnimationClassName, currentSegment == 0 ? "" : currentSegment.ToString());
			aliceKinect.export(fileName);
			aliceKinect.reset();
		}
	}
}
