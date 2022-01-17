// Copyright 2022 Jason Thorsness
namespace Jt.Scratch.Tests.Unit.Svg
{
    using System.Text;
    using Jt.Scratch.Svg;
    using Xunit;

    /// <summary>.</summary>
    public class PathBuilderTests
    {
        /// <summary>.</summary>
        [Fact]
        public void LineTo()
        {
            PathBuilder pathBuilder = new(0f, 0f);
            pathBuilder.LineTo(10f, 0f);
            pathBuilder.LineTo(10f, 10f);
            pathBuilder.LineTo(0f, 10f);
            pathBuilder.ClosePath();
            StringBuilder stringBuilder = new();
            pathBuilder.Serialize(stringBuilder);
            string result = stringBuilder.ToString();
            Assert.Equal("M0,0L10,0 10,10 0,10Z", result);
        }

        /// <summary>.</summary>
        [Fact]
        public void CurveTo()
        {
            PathBuilder pathBuilder = new(0f, 5f);
            pathBuilder.CurveTo(0f, 0f, 0f, 0f, 5f, 0f);
            pathBuilder.CurveTo(10f, 0f, 10f, 0f, 10f, 5f);
            pathBuilder.CurveTo(10f, 10f, 10f, 10f, 5f, 10f);
            pathBuilder.CurveTo(0f, 10f, 0f, 10f, 0f, 5f);
            pathBuilder.ClosePath();
            StringBuilder stringBuilder = new();
            pathBuilder.Serialize(stringBuilder);
            string result = stringBuilder.ToString();
            Assert.Equal("M0,5C0,0 0,0 5,0 10,0 10,0 10,5 10,10 10,10 5,10 0,10 0,10 0,5Z", result);
        }
    }
}
