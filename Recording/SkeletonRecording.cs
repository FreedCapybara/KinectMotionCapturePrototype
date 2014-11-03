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

		private AliceFileBuilder fileBuilder;

		public SkeletonRecorder()
		{
			fileBuilder = new AliceFileBuilder();
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

			fileBuilder.Prepare();

			foreach (var skeleton in frames)
			{
				fileBuilder.ApplyFrame(skeleton);
			}

			fileBuilder.Finish();
		}
	}
}
