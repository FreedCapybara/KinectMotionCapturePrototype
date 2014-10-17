using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
	static class SkeletonPointExtensions
	{
		public static SkeletonPoint Add(this SkeletonPoint point1, SkeletonPoint point2)
		{
			return new SkeletonPoint()
			{
				X = point1.X + point2.X,
				Y = point1.Y + point2.Y,
				Z = point1.Z + point2.Z
			};
		}

		public static SkeletonPoint Subtract(this SkeletonPoint point1, SkeletonPoint point2)
		{
			return new SkeletonPoint()
			{
				X = point1.X - point2.X,
				Y = point1.Y - point2.Y,
				Z = point1.Z - point2.Z
			};
		}

		public static SkeletonPoint Multiply(this SkeletonPoint point, float scalar)
		{
			return new SkeletonPoint()
			{
				X = point.X * scalar,
				Y = point.Y * scalar,
				Z = point.Z * scalar
			};
		}

		public static SkeletonPoint Normalize(this SkeletonPoint point)
		{
			if (point.IsZero())
			{
				return point;
			}

			var lengthInverse = 1 / (float)Math.Sqrt((point.X * point.X) + (point.Y * point.Y) + (point.Z * point.Z));
			return new SkeletonPoint()
			{
				X = point.X * lengthInverse,
				Y = point.Y * lengthInverse,
				Z = point.Z * lengthInverse
			};
		}

		public static bool IsZero(this SkeletonPoint point)
		{
			return point.X == 0 && point.Y == 0 && point.Z == 0;
		}
	}
}
