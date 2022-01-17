// Copyright 2022 Jason Thorsness
namespace Jt.Scratch.Tests.Unit.Svg
{
    using System.Numerics;
    using System.Text;
    using Jt.Scratch.Svg;
    using Xunit;

    /// <summary>.</summary>
    public class PathFromOutlineTests
    {
        /// <summary>.</summary>
        [Fact]
        public void Basic()
        {
            string input = "#\n";
            string path = this.GetPath(input, 0, 1);
            Assert.Equal("M0,0.5C0,0.125 0.125,0 0.5,0 0.875,0 1,0.125 1,0.5 1,0.875 0.875,1 0.5,1 0.125,1 0,0.875 0,0.5Z", path);

            input = "#\n";
            path = this.GetPath(input, 0, 2);
            Assert.Equal("M0,1C0,-2 0.25,-2.25 1,0 4,0 4.25,0.25 2,1 2,4 1.75,4.25 1,2 -2,2 -2.25,1.75 0,1Z", path);

            input = "#\n";
            path = this.GetPath(input, 0.5f, 1);
            Assert.Equal(
                "M0.25,0.5C0.25,0.3125 0.3125,0.25 0.5,0.25 0.6875,0.25 0.75,0.3125 0.75,0.5 0.75,0.6875 0.6875,0.75 0.5,0.75 0.3125,0.75 0.25,0.6875 0.25,0.5Z",
                path);

            input = "#\n";
            path = this.GetPath(input, -0.5f, 1);
            Assert.Equal(
                "M-0.25,0.5C-0.25,-0.0625 -0.0625,-0.25 0.5,-0.25 1.0625,-0.25 1.25,-0.0625 1.25,0.5 1.25,1.0625 1.0625,1.25 0.5,1.25 -0.0625,1.25 -0.25,1.0625 -0.25,0.5Z",
                path);

            input =
                "##\n" +
                "##\n";
            path = this.GetPath(input, 0, 1);
            Assert.Equal("M0,0.5C0,0.125 0.125,0 0.5,0L1.5,0C1.875,0 2,0.125 2,0.5L2,1.5C2,1.875 1.875,2 1.5,2L0.5,2C0.125,2 0,1.875 0,1.5Z", path);

            input =
                ".#\n" +
                "##\n";
            path = this.GetPath(input, 0, 1);
            Assert.Equal(
                "M1,0.5C1,0.125 1.125,0 1.5,0 1.875,0 2,0.125 2,0.5L2,1.5C2,1.875 1.875,2 1.5,2L0.5,2C0.125,2 0,1.875 0,1.5 0,1.125 0.125,1 0.5,1 0.875,1 1,0.875 1,0.5Z",
                path);

            input =
                "#.\n" +
                "##\n";
            path = this.GetPath(input, 0, 1);
            Assert.Equal(
                "M0,0.5C0,0.125 0.125,0 0.5,0 0.875,0 1,0.125 1,0.5 1,0.875 1.125,1 1.5,1 1.875,1 2,1.125 2,1.5 2,1.875 1.875,2 1.5,2L0.5,2C0.125,2 0,1.875 0,1.5Z",
                path);

            input =
                "##\n" +
                "#.\n";
            path = this.GetPath(input, 0, 1);
            Assert.Equal(
                "M0,0.5C0,0.125 0.125,0 0.5,0L1.5,0C1.875,0 2,0.125 2,0.5 2,0.875 1.875,1 1.5,1 1.125,1 1,1.125 1,1.5 1,1.875 0.875,2 0.5,2 0.125,2 0,1.875 0,1.5Z",
                path);

            input =
                "##\n" +
                ".#\n";
            path = this.GetPath(input, 0, 1);
            Assert.Equal(
                "M0,0.5C0,0.125 0.125,0 0.5,0L1.5,0C1.875,0 2,0.125 2,0.5L2,1.5C2,1.875 1.875,2 1.5,2 1.125,2 1,1.875 1,1.5 1,1.125 0.875,1 0.5,1 0.125,1 0,0.875 0,0.5Z",
                path);
        }

        /// <summary>.</summary>
        [Fact]
        public void Complex()
        {
            string input =
                "############\n" +
                "#....##....#\n" +
                "#.##.##.##.#\n" +
                "##...##...##\n" +
                "#.###..#.#.#\n" +
                "#.#.#..###.#\n" +
                "##...##...##\n" +
                "#.##.##.##.#\n" +
                "#....##....#\n" +
                "############\n";

            string path = this.GetPath(input, 0, 1);
            Assert.Equal(
                "M0,0.5C0,0.125 0.125,0 0.5,0L11.5,0C11.875,0 12,0.125 12,0.5L12,9.5C12,9.875 11.875,10 11.5,10L0.5,10C0.125,10 0,9.875 0,9.5ZM1,2.5C1,2.875 1.125,3 1.5,3 1.875,3 2,2.875 2,2.5 2,2.125 2.125,2 2.5,2L3.5,2C3.875,2 4,2.125 4,2.5 4,2.875 3.875,3 3.5,3L2.5,3C2.125,3 2,3.125 2,3.5 2,3.875 2.125,4 2.5,4L4.5,4C4.875,4 5,3.875 5,3.5L5,1.5C5,1.125 4.875,1 4.5,1L1.5,1C1.125,1 1,1.125 1,1.5ZM7,2.5L7,3.5C7,3.875 7.125,4 7.5,4 7.875,4 8,4.125 8,4.5 8,4.875 8.125,5 8.5,5 8.875,5 9,4.875 9,4.5 9,4.125 9.125,4 9.5,4 9.875,4 10,3.875 10,3.5 10,3.125 9.875,3 9.5,3L8.5,3C8.125,3 8,2.875 8,2.5 8,2.125 8.125,2 8.5,2L9.5,2C9.875,2 10,2.125 10,2.5 10,2.875 10.125,3 10.5,3 10.875,3 11,2.875 11,2.5L11,1.5C11,1.125 10.875,1 10.5,1L7.5,1C7.125,1 7,1.125 7,1.5ZM1,5.5C1,5.875 1.125,6 1.5,6 1.875,6 2,5.875 2,5.5L2,4.5C2,4.125 1.875,4 1.5,4 1.125,4 1,4.125 1,4.5ZM5,5.5C5,5.875 5.125,6 5.5,6L6.5,6C6.875,6 7,5.875 7,5.5L7,4.5C7,4.125 6.875,4 6.5,4L5.5,4C5.125,4 5,4.125 5,4.5ZM10,5.5C10,5.875 10.125,6 10.5,6 10.875,6 11,5.875 11,5.5L11,4.5C11,4.125 10.875,4 10.5,4 10.125,4 10,4.125 10,4.5ZM3,6L2.5,6C2.125,6 2,6.125 2,6.5 2,6.875 2.125,7 2.5,7L3.5,7C3.875,7 4,7.125 4,7.5 4,7.875 3.875,8 3.5,8L2.5,8C2.125,8 2,7.875 2,7.5 2,7.125 1.875,7 1.5,7 1.125,7 1,7.125 1,7.5L1,8.5C1,8.875 1.125,9 1.5,9L4.5,9C4.875,9 5,8.875 5,8.5L5,6.5C5,6.125 4.875,6 4.5,6 4.125,6 4,5.875 4,5.5 4,5.125 3.875,5 3.5,5 3.125,5 3,5.125 3,5.5 3,5.875 2.875,6 2.5,6ZM7,7.5L7,8.5C7,8.875 7.125,9 7.5,9L10.5,9C10.875,9 11,8.875 11,8.5L11,7.5C11,7.125 10.875,7 10.5,7 10.125,7 10,7.125 10,7.5 10,7.875 9.875,8 9.5,8L8.5,8C8.125,8 8,7.875 8,7.5 8,7.125 8.125,7 8.5,7L9.5,7C9.875,7 10,6.875 10,6.5 10,6.125 9.875,6 9.5,6L7.5,6C7.125,6 7,6.125 7,6.5Z",
                path);
        }

        /// <summary>.</summary>
        [Fact]
        public void Big()
        {
            const float shrinkage = 0f;
            const float fullGrid = 1.0f;

            const int cols = 256;
            const int rows = 256;
            Span<byte> input = new byte[rows * cols];

            Random random = new();

            for (int i = 0; i < input.Length; ++i)
            {
                input[i] = random.NextDouble() > 0.9 ? (byte)1 : (byte)0;
            }

            StringBuilder stringBuilder = new();
            Span<int> regions = new int[input.Length];
            Span<Side> sides = new Side[OutlineFromGrid.GetRequiredOutlineBufferSize(input.Length)];
            Stack<int> stack = new();

            regions.Fill(0);
            int regionCount = OutlineFromGrid.DiscoverRegions(input, cols, regions, stack);
            for (int i = 0; i < regionCount; ++i)
            {
                ReadOnlySpan<Side> outline = OutlineFromGrid.GetOutline(input, cols, regions, sides, i, out int startIndex);
                ReadOnlyMemory<Vector2> vertices = VerticesFromGridOutline.Apply(outline, startIndex, cols, fullGrid, shrinkage, input[startIndex] == 0);
                PathFromVertices.Apply(vertices.Span, stringBuilder);
            }

            string result2 = stringBuilder.ToString();
        }

        private string GetPath(string inputString, float shrinkage, float fullGrid)
        {
            Assert.True(GridFromString.TryParse(inputString, '.', '#', out ReadOnlyMemory<byte> input, out int cols));

            StringBuilder stringBuilder = new();
            Span<int> regions = new int[input.Length];
            Span<Side> sides = new Side[OutlineFromGrid.GetRequiredOutlineBufferSize(input.Length)];
            Stack<int> stack = new();

            regions.Fill(0);
            int regionCount = OutlineFromGrid.DiscoverRegions(input.Span, cols, regions, stack);
            for (int i = 0; i < regionCount; ++i)
            {
                ReadOnlySpan<Side> outline = OutlineFromGrid.GetOutline(input.Span, cols, regions, sides, i, out int startIndex);
                ReadOnlyMemory<Vector2> vertices = VerticesFromGridOutline.Apply(outline, startIndex, cols, fullGrid, shrinkage, input.Span[startIndex] == 0);
                PathFromVertices.Apply(vertices.Span, stringBuilder);
            }

            return stringBuilder.ToString();
        }
    }
}
