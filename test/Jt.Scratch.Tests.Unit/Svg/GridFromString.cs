// Copyright 2022 Jason Thorsness
namespace Jt.Scratch.Tests.Unit.Svg
{
    /// <summary>.</summary>
    public static class GridFromString
    {
        /// <summary>.</summary>
        public static bool TryParse(string input, char zero, char nonZero, out ReadOnlyMemory<byte> output, out int cols)
        {
            ReadOnlySpan<char> characters = input.AsSpan();

            // pass 1 - determine buffer size and ensure all lines are the same length.
            int rows = 0;
            cols = 0;

            State state = State.Head;

            foreach (ReadOnlySpan<char> line in characters.EnumerateLines())
            {
                if (line.Length == 0)
                {
                    if (state == State.Body)
                    {
                        state = State.Tail;
                    }

                    continue;
                }

                if (state == State.Head)
                {
                    state = State.Body;
                }
                else if (state == State.Tail)
                {
                    output = default;
                    cols = default;
                    return false;
                }

                if (cols != 0 &&
                    line.Length != 0 &&
                    line.Length != cols)
                {
                    output = default;
                    cols = default;
                    return false;
                }

                rows++;
                cols = line.Length;
            }

            // pass 2 - read content into buffer
            byte[] gridBuffer = new byte[rows * cols];
            Span<byte> grid = gridBuffer;

            foreach (ReadOnlySpan<char> line in characters.EnumerateLines())
            {
                if (line.Length == 0)
                {
                    continue;
                }

                for (int i = 0; i < line.Length; ++i)
                {
                    if (line[i] == zero)
                    {
                        grid[i] = 0;
                    }
                    else if (line[i] == nonZero)
                    {
                        grid[i] = 1;
                    }
                    else
                    {
                        output = default;
                        cols = default;
                        return false;
                    }
                }

                grid = grid[cols..];
            }

            output = gridBuffer;
            return true;
        }

        private enum State
        {
            Head,
            Body,
            Tail,
        }
    }
}
