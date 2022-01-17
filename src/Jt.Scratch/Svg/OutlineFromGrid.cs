// Copyright 2022 Jason Thorsness
namespace Jt.Scratch.Svg
{
    /// <summary>.</summary>
    public static class OutlineFromGrid
    {
        /// <summary>.</summary>
        public static int DiscoverRegions(ReadOnlySpan<byte> input, int cols, Span<int> output, Stack<int> stack)
        {
            if (input.Length % cols != 0 ||
                input.Length != output.Length)
            {
                throw new ArgumentException();
            }

            int numberOfRegions = 1;

            const int unvisitedOutput = 0;
            const int impliedBorder = 0;

            // Fill implied border from edges
            {
                for (int i = 0; i < cols; ++i)
                {
                    // North
                    if (input[i] == impliedBorder &&
                        output[i] == unvisitedOutput)
                    {
                        FloodFillRegion(input, cols, i, output, numberOfRegions, stack);
                    }

                    int southIndex = input.Length - cols + i;

                    // South
                    if (input[southIndex] == impliedBorder &&
                        output[southIndex] == unvisitedOutput)
                    {
                        FloodFillRegion(input, cols, southIndex, output, numberOfRegions, stack);
                    }
                }

                for (int i = cols; i < input.Length - cols - 1; i += cols)
                {
                    // West
                    if (input[i] == impliedBorder &&
                        output[i] == unvisitedOutput)
                    {
                        FloodFillRegion(input, cols, i, output, numberOfRegions, stack);
                    }

                    int eastIndex = i + (cols - 1);

                    // East
                    if (input[eastIndex] == impliedBorder &&
                        output[eastIndex] == unvisitedOutput)
                    {
                        FloodFillRegion(input, cols, eastIndex, output, numberOfRegions, stack);
                    }
                }
            }

            // Fill actual regions
            {
                numberOfRegions++;

                int startIndex = 0;
                int delta = output[startIndex..].IndexOf(unvisitedOutput);

                while (delta >= 0)
                {
                    startIndex += delta;
                    FloodFillRegion(input, cols, startIndex, output, numberOfRegions, stack);
                    numberOfRegions++;
                    delta = output[startIndex..].IndexOf(unvisitedOutput);
                }
            }

            return numberOfRegions - 2;
        }

        /// <summary>.</summary>
        public static int GetRequiredOutlineBufferSize(int inputLength)
        {
            return (inputLength * 3) + 1;
        }

        /// <summary>.</summary>
        public static ReadOnlySpan<Side> GetOutline(
            ReadOnlySpan<byte> input,
            int cols,
            ReadOnlySpan<int> regions,
            Span<Side> outlineBuffer,
            int regionIndex,
            out int startIndex)
        {
            if (outlineBuffer.Length != GetRequiredOutlineBufferSize(input.Length))
            {
                throw new ArgumentException();
            }

            int region = regionIndex + 2;

            startIndex = regions.IndexOf(region);
            const Side startSide = Side.West;

            bool hole = input[startIndex] == 0;

            int index = startIndex;
            Side side = startSide;

            int length = 0;

            do
            {
                outlineBuffer[length++] = side;

                int indexCol = index % cols;

                bool frontRightFilled = side switch
                {
                    Side.North => indexCol < cols - 1 && regions[index + 1] == region,
                    Side.East => index < regions.Length - cols && regions[index + cols] == region,
                    Side.South => indexCol > 0 && regions[index - 1] == region,
                    Side.West => index >= cols && regions[index - cols] == region,
                    _ => throw new ArgumentException(),
                };

                bool frontLeftFilled = side switch
                {
                    Side.North => index >= cols && indexCol < cols - 1 && regions[index - cols + 1] == region,
                    Side.East => index < regions.Length - cols && indexCol < cols - 1 && regions[index + cols + 1] == region,
                    Side.South => index < regions.Length - cols && indexCol > 0 && regions[index + cols - 1] == region,
                    Side.West => index >= cols && indexCol > 0 && regions[index - cols - 1] == region,
                    _ => throw new ArgumentException(),
                };

                (index, side) = (side, frontLeftFilled, frontRightFilled) switch
                {
                    (Side.North, true, true) => (index - cols + 1, Side.West),
                    (Side.North, true, false) => hole ? (index, Side.East) : (index - cols + 1, Side.West),
                    (Side.North, false, true) => (index + 1, Side.North),
                    (Side.North, false, false) => (index, Side.East),
                    (Side.East, true, true) => (index + cols + 1, Side.North),
                    (Side.East, true, false) => hole ? (index, Side.South) : (index + cols + 1, Side.North),
                    (Side.East, false, true) => (index + cols, Side.East),
                    (Side.East, false, false) => (index, Side.South),
                    (Side.South, true, true) => (index + cols - 1, Side.East),
                    (Side.South, true, false) => hole ? (index, Side.West) : (index + cols - 1, Side.East),
                    (Side.South, false, true) => (index - 1, Side.South),
                    (Side.South, false, false) => (index, Side.West),
                    (Side.West, true, true) => (index - cols - 1, Side.South),
                    (Side.West, true, false) => hole ? (index, Side.North) : (index - cols - 1, Side.South),
                    (Side.West, false, true) => (index - cols, Side.West),
                    (Side.West, false, false) => (index, Side.North),
                    _ => throw new ArgumentException(),
                };
            }
            while (index != startIndex || side != startSide);

            return outlineBuffer[..length];
        }

        private static void FloodFillRegion(ReadOnlySpan<byte> input, int cols, int startIndex, Span<int> output, int current, Stack<int> stack)
        {
            byte toFill = input[startIndex];
            bool allowDiagonal = toFill != 0;
            stack.Push(startIndex);

            while (stack.TryPop(out int fillFromIndex))
            {
                output[fillFromIndex] = current;

                int fillFromCol = fillFromIndex % cols;
                int fillLength = 1;

                for (; fillFromCol + fillLength < cols; ++fillLength)
                {
                    int index = fillFromIndex + fillLength;

                    if (output[index] != 0 ||
                        input[index] != toFill)
                    {
                        break;
                    }
                }

                // West end
                if (fillFromCol > 0)
                {
                    if (allowDiagonal)
                    {
                        int nw = fillFromIndex - cols - 1;
                        int sw = fillFromIndex + cols - 1;

                        if (nw >= 0 &&
                            output[nw] == 0 &&
                            input[nw] == toFill)
                        {
                            stack.Push(nw);
                        }

                        if (sw < input.Length &&
                            output[sw] == 0 &&
                            input[sw] == toFill)
                        {
                            stack.Push(sw);
                        }
                    }

                    int w = fillFromIndex - 1;

                    if (output[w] == 0 &&
                        input[w] == toFill)
                    {
                        stack.Push(w);
                    }
                }

                int fillToIndex = fillFromIndex + fillLength - 1;

                // East end
                if (fillFromCol + fillLength < cols &&
                    allowDiagonal)
                {
                    int ne = fillToIndex - cols + 1;
                    int se = fillToIndex + cols + 1;

                    if (ne >= 0 &&
                        output[ne] == 0 &&
                        input[ne] == toFill)
                    {
                        stack.Push(ne);
                    }

                    if (se < input.Length &&
                        output[se] == 0 &&
                        input[se] == toFill)
                    {
                        stack.Push(se);
                    }
                }

                // Middle
                for (int i = fillFromIndex; i < fillFromIndex + fillLength; ++i)
                {
                    output[i] = current;

                    int n = i - cols;
                    int s = i + cols;

                    if (n >= 0 &&
                        output[n] == 0 &&
                        input[n] == toFill)
                    {
                        stack.Push(n);
                    }

                    if (s < input.Length &&
                        output[s] == 0 &&
                        input[s] == toFill)
                    {
                        stack.Push(s);
                    }
                }
            }
        }
    }
}
