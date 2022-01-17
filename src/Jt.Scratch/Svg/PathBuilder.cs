// Copyright 2022 Jason Thorsness
namespace Jt.Scratch.Svg
{
    using System.Globalization;
    using System.Numerics;
    using System.Text;

    /// <summary>.</summary>
    public class PathBuilder
    {
        private int lastPathCommandIndex;
        private int lastVector2Index;
        private Memory<PathCommand> pathCommands;
        private Memory<Vector2> points;

        /// <summary>.</summary>
        public PathBuilder(float x, float y)
        {
            this.pathCommands = new PathCommand[1];
            this.points = new Vector2[3];
            this.lastPathCommandIndex = -1;
            this.lastVector2Index = -1;
            this.AddPathCommand(PathCommand.MoveTo);
            this.AddPoints(new Vector2(x, y));
        }

        /// <summary>.</summary>
        public Vector2 Current => this.points.Span[this.lastVector2Index];

        /// <summary>.</summary>
        public void LineTo(float x2, float y2)
        {
            this.AddPathCommand(PathCommand.LineTo);
            this.AddPoints(new Vector2(x2, y2));
        }

        /// <summary>.</summary>
        public void CurveTo(float x2, float y2, float x3, float y3, float x4, float y4)
        {
            this.AddPathCommand(PathCommand.CurveTo);
            this.AddPoints(new Vector2(x2, y2), new Vector2(x3, y3), new Vector2(x4, y4));
        }

        /// <summary>.</summary>
        public void ClosePath()
        {
            this.AddPathCommand(PathCommand.ClosePath);
        }

        /// <summary>.</summary>
        public void Serialize(StringBuilder stringBuilder)
        {
            PathSerializer.Serialize(stringBuilder, this);
        }

        private void AddPathCommand(PathCommand pathCommand)
        {
            if (this.lastPathCommandIndex + 1 == this.pathCommands.Length)
            {
                int newLength = this.pathCommands.Length * 2;
                Memory<PathCommand> newPathCommands = new PathCommand[newLength];
                this.pathCommands.CopyTo(newPathCommands);
                this.pathCommands = newPathCommands;
            }

            this.pathCommands.Span[++this.lastPathCommandIndex] = pathCommand;
        }

        /// <summary>.</summary>
        private void AddPoints(Vector2 a)
        {
            this.AddPoints(1)[0] = a;
        }

        /// <summary>.</summary>
        private void AddPoints(Vector2 a, Vector2 b)
        {
            Span<Vector2> span = this.AddPoints(2);
            span[0] = a;
            span[1] = b;
        }

        /// <summary>.</summary>
        private void AddPoints(Vector2 a, Vector2 b, Vector2 c)
        {
            Span<Vector2> span = this.AddPoints(3);
            span[0] = a;
            span[1] = b;
            span[2] = c;
        }

        /// <summary>.</summary>
        private Span<Vector2> AddPoints(int length)
        {
            if (this.lastVector2Index + length >= this.points.Length)
            {
                int newLength = this.points.Length * 2;
                Memory<Vector2> newVector2s = new Vector2[newLength];
                this.points.CopyTo(newVector2s);
                this.points = newVector2s;
            }

            int from = this.lastVector2Index + 1;
            int to = from + length;
            this.lastVector2Index += length;

            return this.points.Span[from..to];
        }

        private static class PathSerializer
        {
            private static readonly ReadOnlyMemory<char> AbsoluteMemory;
            private static readonly ReadOnlyMemory<char> RelativeMemory;
            private static readonly ReadOnlyMemory<char> NumberFormat = "G9".AsMemory();

            static PathSerializer()
            {
                PathCommand[] commandTypes = Enum.GetValues<PathCommand>();
                char[] absoluteArray = new char[commandTypes.Length];
                char[] relativeArray = new char[commandTypes.Length];

                foreach (PathCommand commandType in Enum.GetValues<PathCommand>())
                {
                    absoluteArray[(int)commandType] = commandType switch
                    {
                        PathCommand.ClosePath => 'Z',
                        PathCommand.MoveTo => 'M',
                        PathCommand.LineTo => 'L',
                        PathCommand.CurveTo => 'C',
                        PathCommand.SmoothCurveTo => 'S',
                        _ => throw new ArgumentException(),
                    };

                    relativeArray[(int)commandType] = commandType switch
                    {
                        PathCommand.ClosePath => 'z',
                        PathCommand.MoveTo => 'm',
                        PathCommand.LineTo => 'l',
                        PathCommand.CurveTo => 'c',
                        PathCommand.SmoothCurveTo => 's',
                        _ => throw new ArgumentException(),
                    };
                }

                AbsoluteMemory = absoluteArray;
                RelativeMemory = relativeArray;
            }

            /// <summary>.</summary>
            public static void Serialize(StringBuilder stringBuilder, PathBuilder pathBuilder)
            {
                ReadOnlySpan<char> absolute = AbsoluteMemory.Span;

                ReadOnlySpan<PathCommand> pathCommands = pathBuilder.pathCommands.Span;
                ReadOnlySpan<Vector2> points = pathBuilder.points.Span;

                int pathCommandIndex = 0;
                int pointsIndex = 0;

                PathCommand lastPathCommand = PathCommand.ClosePath;

                while (pathCommandIndex <= pathBuilder.lastPathCommandIndex)
                {
                    PathCommand pathCommand = pathCommands[pathCommandIndex++];
                    stringBuilder.Append(pathCommand == lastPathCommand ? ' ' : absolute[(int)pathCommand]);
                    lastPathCommand = pathCommand;

                    switch (pathCommand)
                    {
                        case PathCommand.ClosePath:
                            break;

                        case PathCommand.MoveTo:
                        case PathCommand.LineTo:
                            WritePoint(stringBuilder, points[pointsIndex++]);
                            break;

                        case PathCommand.CurveTo:
                            WritePoint(stringBuilder, points[pointsIndex++]);
                            stringBuilder.Append(' ');
                            WritePoint(stringBuilder, points[pointsIndex++]);
                            stringBuilder.Append(' ');
                            WritePoint(stringBuilder, points[pointsIndex++]);
                            break;

                        case PathCommand.SmoothCurveTo:
                            WritePoint(stringBuilder, points[pointsIndex++]);
                            stringBuilder.Append(' ');
                            WritePoint(stringBuilder, points[pointsIndex++]);
                            break;

                        default:
                            throw new ArgumentException();
                    }
                }
            }

            /// <summary>.</summary>
            private static void WritePoint(StringBuilder stringBuilder, Vector2 point)
            {
                WriteNumber(stringBuilder, point.X);
                stringBuilder.Append(',');
                WriteNumber(stringBuilder, point.Y);
            }

            private static void WriteNumber(StringBuilder stringBuilder, float number)
            {
                Span<char> buffer = stackalloc char[15];

                if (!number.TryFormat(buffer, out int charsWritten, NumberFormat.Span, CultureInfo.InvariantCulture))
                {
                    throw new ArgumentException("Failed");
                }

                stringBuilder.Append(buffer[..charsWritten]);
            }
        }
    }
}
