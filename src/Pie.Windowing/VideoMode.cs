using System.Drawing;

namespace Pie.Windowing;

/// <summary>
/// Contains information about a monitor's current video mode.
/// </summary>
public struct VideoMode
{
    /// <summary>
    /// The size/resolution, in pixels, of this <see cref="VideoMode"/>.
    /// </summary>
    public readonly Size Size;

    /// <summary>
    /// The refresh rate, in hz, of this <see cref="VideoMode"/>.
    /// </summary>
    public readonly int RefreshRate;

    /// <summary>
    /// The accurate aspect ratio of this <see cref="VideoMode"/>. This value is, as the name implies, completely accurate,
    /// however may make certain resolutions (such as 1366x768) appear to have the incorrect aspect ratio to the end user.
    /// </summary>
    public readonly Size AccurateAspectRatio;

    /// <summary>
    /// The aspect ratio of this <see cref="VideoMode"/>. This value is a best guess, working within a certain tolerance.
    /// While not as accurate, its results may be more like what an end user may expect. For example, it labels
    /// 1366x768 as 16:9, even though the actual ratio is very slightly different.
    /// </summary>
    public readonly Size AspectRatio;

    /// <summary>
    /// Create a new <see cref="VideoMode"/> from the given size and optional refresh rate.
    /// </summary>
    /// <param name="size">The size/resolution, in pixels, of this video mode.</param>
    /// <param name="refreshRate">The refresh rate, if any.</param>
    /// <remarks><see cref="AspectRatio"/> and <see cref="AccurateAspectRatio"/> will automatically be calculated.</remarks>
    public VideoMode(Size size, int refreshRate = -1)
    {
        Size = size;
        RefreshRate = refreshRate;

        AccurateAspectRatio = size / GreatestCommonFactor(size.Width, size.Height);
        AspectRatio = CalculateAspectRatio(size);
    }

    /// <inheritdoc /> 
    public override string ToString()
    {
        return Size.Width + "x" + Size.Height + "@" + RefreshRate + "Hz";
    }

    // Before you ask, yes I did copy this from Silk.NET videomode because I wrote it :)
    // No I didn't just copy and paste, what made you think that? Absolutely not!
    private static Size CalculateAspectRatio(Size res)
    {
        // Calculate the width-height ratio.
        float ratio = res.Width / (float) res.Height;

        // Count up until the lowest value as the aspect ratio cannot be higher than the lowest value.
        int lowestValue = res.Width < res.Height ? res.Width : res.Height;
        for (int i = 1; i < lowestValue; i++)
        {
            // Multiply both together and calculate a good enough value, a bias of 0.1 seems to work well.
            float multiplied = ratio * i;
            if (multiplied - (int) multiplied < 0.1f)
                return new Size((int) multiplied, i);
        }

        return res;
    }

    // I also copied this from my initial PR cause it works and I can't be bothered to find where abouts in Cubic the
    // original code is located... Also because it doesn't use recursion
    private static int GreatestCommonFactor(int value1, int value2)
    {
        while (true)
        {
            if (value2 > value1)
            {
                int val = value2 - value1;
                if (val <= 0)
                    return value1;
                if (val > value1)
                {
                    value2 = val;
                    continue;
                }

                int value3 = value1;
                value1 = val;
                value2 = value3;
            }
            else
            {
                int val = value1 - value2;
                if (val <= 0)
                    return value2;
                if (val > value2)
                {
                    value1 = value2;
                    value2 = val;
                    continue;
                }

                value1 = val;
            }
        }
    }
}