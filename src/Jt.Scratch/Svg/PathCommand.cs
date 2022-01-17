// Copyright 2022 Jason Thorsness
namespace Jt.Scratch.Svg
{
    /// <summary>.</summary>
    public enum PathCommand : byte
    {
        /// <summary>.</summary>
        ClosePath,

        /// <summary>.</summary>
        MoveTo,

        /// <summary>.</summary>
        LineTo,

        /// <summary>.</summary>
        CurveTo,

        /// <summary>.</summary>
        SmoothCurveTo,
    }
}
