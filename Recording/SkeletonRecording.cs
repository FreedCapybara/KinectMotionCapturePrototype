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
		private int currentFrame;

		private AliceFileBuilder fileBuilder;

		public int FrameCount
		{
			get { return frames.Count; }
		}

		public SkeletonRecorder()
		{
			fileBuilder = new AliceFileBuilder();
		}

		public void Clear()
		{
			frames.Clear();
			frameEnumerator = null;
		}

		public void Capture(Skeleton skeleton)
		{
			frames.Add(skeleton);
		}

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

		public void Export(string filename)
		{
			if (frames.Count == 0)
			{
				return;
			}

			fileBuilder.Prepare();

			foreach (var skeleton in frames)
			{
				fileBuilder.ApplyFrame(skeleton);
			}

			fileBuilder.Finish();
		}

		private void CheckEnumerator()
		{
			if (frameEnumerator == null)
			{
				frameEnumerator = frames.GetEnumerator();
			}
		}
	}
}
