// Copyright 2022 Jason Thorsness
namespace Jt.Scratch.Svg
{
    using System.Numerics;
    using System.Text;

    /// <summary>.</summary>
    public static class PathFromVertices
    {
        private const float RoundedCornerThreshold = 0.98078528f;

        /// <summary>.</summary>
        public static void Apply(ReadOnlySpan<Vector2> vertices, StringBuilder stringBuilder)
        {
            PathBuilder pathBuilder = new(vertices[0].X, vertices[0].Y);

            for (int i = 0; i < vertices.Length; ++i)
            {
                Vector2 from = vertices[i];
                Vector2 to = vertices[(i + 1) % vertices.Length];
                Vector2 next = vertices[(i + 2) % vertices.Length];

                float dot = Vector2.Dot(Vector2.Normalize(to - from), Vector2.Normalize(next - to));

                if (dot > RoundedCornerThreshold)
                {
                    // Line continues
                    continue;
                }

                if (pathBuilder.Current != from)
                {
                    // Fill in all skipped line segments with single line
                    pathBuilder.LineTo(from.X, from.Y);
                }

                Vector2 bezierOne = from + Vector2.Multiply(0.75f, to - from);
                Vector2 bezierTwo = next - Vector2.Multiply(0.75f, next - to);
                Vector2 bezierEnd = next;

                pathBuilder.CurveTo(bezierOne.X, bezierOne.Y, bezierTwo.X, bezierTwo.Y, bezierEnd.X, bezierEnd.Y);

                // Move past skipped vertex
                i++;
            }

            pathBuilder.ClosePath();
            pathBuilder.Serialize(stringBuilder);
        }
    }
}
