// Copyright 2022 Jason Thorsness
namespace Jt.Scratch.Svg
{
    using System.Numerics;

    /// <summary>.</summary>
    public static class VerticesFromGridOutline
    {
        /// <summary>.</summary>
        public static ReadOnlyMemory<Vector2> Apply(ReadOnlySpan<Side> sides, int startIndex, int stride, float fullGrid, float shrinkage, bool hole)
        {
            float halfGrid = (float)(fullGrid / 2.0);
            Memory<Vector2> resultsMemory = new Vector2[sides.Length * 2];
            Span<Vector2> resultsSpan = resultsMemory.Span;

            shrinkage = hole ? -shrinkage : shrinkage;

            int index = hole ? resultsMemory.Length - 1 : 0;
            int step = hole ? -1 : 1;

            int gridIndex = startIndex;

            for (int i = 0; i < sides.Length; ++i)
            {
                Side fromSide = sides[i];
                Side toSide = sides[(i + 1) % sides.Length];

                int fromGridIndex = gridIndex;
                Vector2 fromDelta = GetEdgeDelta(fromSide, halfGrid);
                Vector2 from = GetPoint(fromGridIndex % stride, fromGridIndex / stride, fromDelta, fullGrid, shrinkage);

                resultsSpan[index] = from;
                index += step;

                gridIndex += (fromSide, toSide) switch
                {
                    (Side.North, Side.East) => 0,
                    (Side.East, Side.South) => 0,
                    (Side.South, Side.West) => 0,
                    (Side.West, Side.North) => 0,

                    (Side.North, Side.North) => 1,
                    (Side.East, Side.East) => stride,
                    (Side.South, Side.South) => -1,
                    (Side.West, Side.West) => -stride,

                    (Side.North, Side.West) => -stride + 1,
                    (Side.East, Side.North) => stride + 1,
                    (Side.South, Side.East) => stride - 1,
                    (Side.West, Side.South) => -stride - 1,

                    _ => throw new ArgumentException(),
                };

                if (toSide == fromSide)
                {
                    continue;
                }

                int toGridIndex = gridIndex;
                Vector2 toDelta = GetEdgeDelta(toSide, halfGrid);
                Vector2 to = GetPoint(toGridIndex % stride, toGridIndex / stride, toDelta, fullGrid, shrinkage);

                Vector2 fromToCornerFull = fromSide switch
                {
                    Side.North => new Vector2(fullGrid, 0f),
                    Side.East => new Vector2(0f, fullGrid),
                    Side.South => new Vector2(-fullGrid, 0f),
                    Side.West => new Vector2(0f, -fullGrid),
                    _ => throw new ArgumentException(),
                };

                Vector2 fromToCorner = Vector2.Multiply(Vector2.Dot(to - from, fromToCornerFull), fromToCornerFull);

                resultsSpan[index] = from + fromToCorner;
                index += step;
            }

            return hole ? resultsMemory[^(resultsMemory.Length - 1 - index)..] : resultsMemory[..index];
        }

        private static Vector2 GetPoint(int gx, int gy, Vector2 delta, float fullGrid, float shrinkage)
        {
            float cx = gx * fullGrid;
            float cy = gy * fullGrid;
            return new Vector2(cx, cy) + Vector2.Multiply(1.0f - shrinkage, delta) + new Vector2(fullGrid / 2, fullGrid / 2);
        }

        private static Vector2 GetEdgeDelta(Side side, float halfGrid)
        {
            float dx = side switch
            {
                Side.North => 0f,
                Side.East => halfGrid,
                Side.South => 0f,
                Side.West => -halfGrid,
                _ => throw new ArgumentException(),
            };

            float dy = side switch
            {
                Side.North => -halfGrid,
                Side.East => 0f,
                Side.South => halfGrid,
                Side.West => 0f,
                _ => throw new ArgumentException(),
            };

            return new Vector2(dx, dy);
        }
    }
}
