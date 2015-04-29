using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
	/// <summary>
	/// Builds a list of Kinect Skeletons and provides utilities to export them.
	/// </summary>
	class SkeletonRecorder
	{
		private List<Skeleton> frames = new List<Skeleton>();
		private IEnumerator<Skeleton> frameEnumerator;
		private int currentFrame;

		private AliceFileBuilder fileBuilder;

		public int FrameCount
		{
			get { return frames.Count; }
		}

		public SkeletonRecorder()
		{
			fileBuilder = new AliceFileBuilder(framesPerSegment: 200, maxSegments: 1);
		}

		/// <summary>
		/// Resets the animation.
		/// </summary>
		public void Clear()
		{
			frames.Clear();
			frameEnumerator = null;
		}

		/// <summary>
		/// Adds a skeleton to the list of frames.
		/// </summary>
		/// <param name="skeleton">The skeleton that represents a frame of animation</param>
		public void Capture(Skeleton skeleton)
		{
			frames.Add(skeleton);
		}

		/// <summary>
		/// Resets the frame enumeration from a given index.
		/// </summary>
		/// <param name="startIndex">The index to start the animation from</param>
		private void MoveToStart(int startIndex)
		{
			CheckEnumerator();

			frameEnumerator.Reset();
			currentFrame = startIndex;
			// move the enumerator to the start position
			for (var i = 0; i < startIndex; i++)
			{
				if (!frameEnumerator.MoveNext())
					break;
			}
		}

		/// <summary>
		/// Returns the skeleton in the next frame of the animation.
		/// (Used during animation playback)
		/// </summary>
		/// <param name="startIndex">The index of the frame where the animation should start playback</param>
		/// <param name="endIndex">The index of the frame where the antimation should end playback</param>
		/// <returns>A skeleton representing one frame in the animation sequence</returns>
		public Skeleton NextFrame(int startIndex = 0, int? endIndex = null)
		{
			CheckEnumerator();
			currentFrame++;

			if (!frameEnumerator.MoveNext() || currentFrame >= (endIndex ?? frames.Count - 1))
			{
				MoveToStart(startIndex);
				frameEnumerator.MoveNext();
			}

			return frameEnumerator.Current;
		}

		/// <summary>
		/// Configure the output path for animation exports.
		/// </summary>
		/// <param name="fullPath">The full desination path of the output file</param>
		public void ConfigureOutput(string fullPath)
		{
			var animationName = Path.GetFileNameWithoutExtension(fullPath);
			if (!string.IsNullOrWhiteSpace(animationName))
			{
				fileBuilder.AnimationClassName = animationName;
			}

			fileBuilder.OutputDirectory = Path.GetDirectoryName(fullPath);
		}

		/// <summary>
		/// Applies the recorded frames and generates the animation.
		/// </summary>
		/// <param name="startIndex">Index of the frame at which to start the animation</param>
		/// <param name="endIndex">Index of the frame at which to end the animation</param>
		public void Export(int startIndex = 0, int? endIndex = null)
		{
			if (frames.Count == 0)
			{
				return;
			}

			fileBuilder.Prepare();

			var exportFrames = frames.GetRange(startIndex, (endIndex ?? frames.Count - 1) - startIndex);
			foreach (var skeleton in exportFrames)
			{
				fileBuilder.ApplyFrame(skeleton);
			}

			//fileBuilder.Finish();
		}

		/// <summary>
		/// Ensures the enumerator is not null.
		/// </summary>
		private void CheckEnumerator()
		{
			if (frameEnumerator == null)
			{
				frameEnumerator = frames.GetEnumerator();
			}
		}
	}
}
