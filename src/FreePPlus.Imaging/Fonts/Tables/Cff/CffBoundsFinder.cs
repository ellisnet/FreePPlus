// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal class CffBoundsFinder : IGlyphRenderer
{
    private readonly int nsteps;
    private Vector2 currentXY;
    private bool firstEval;
    private float maxX;
    private float maxY;
    private float minX;
    private float minY;
    private bool open;

    public CffBoundsFinder()
    {
        minX = float.MaxValue;
        maxX = float.MinValue;
        minY = float.MaxValue;
        maxY = float.MinValue;
        nsteps = 3;
        currentXY = Vector2.Zero;
        open = false;
        firstEval = true;
    }

    public void BeginFigure()
    {
        // Do nothing.
    }

    public bool BeginGlyph(FontRectangle bounds, GlyphRendererParameters parameters)
    {
        return true;
        // Do nothing.
    }

    public void BeginText(FontRectangle bounds)
    {
        // Do nothing.
    }

    public void EndFigure()
    {
        open = false;
        currentXY = Vector2.Zero;
    }

    public void EndGlyph()
    {
        if (open) EndFigure();
    }

    public void EndText()
    {
        if (open) EndFigure();
    }

    public void LineTo(Vector2 point)
    {
        currentXY = point;
        UpdateMinMax(point.X, point.Y);
        open = true;
    }

    public void MoveTo(Vector2 point)
    {
        if (open) EndFigure();

        currentXY = point;
        UpdateMinMax(point.X, point.Y);
    }

    public void CubicBezierTo(Vector2 secondControlPoint, Vector2 thirdControlPoint, Vector2 point)
    {
        var eachstep = 1F / nsteps;
        var t = eachstep; // Start

        for (var n = 1; n < nsteps; ++n)
        {
            var c = 1F - t;
            var xy = currentXY * c * c * c + secondControlPoint * 3 * t * c * c + thirdControlPoint * 3 * t * t * c +
                     point * t * t * t;
            UpdateMinMax(xy.X, xy.Y);

            t += eachstep;
        }

        currentXY = point;
        UpdateMinMax(point.X, point.Y);
        open = true;
    }

    public void QuadraticBezierTo(Vector2 secondControlPoint, Vector2 point)
    {
        var eachstep = 1F / nsteps;
        var t = eachstep; // Start

        for (var n = 1; n < nsteps; ++n)
        {
            var c = 1F - t;
            var xy = currentXY * c * c + secondControlPoint * 2 * t * c + point * t * t;
            UpdateMinMax(xy.X, xy.Y);

            t += eachstep;
        }

        currentXY = point;
        UpdateMinMax(point.X, point.Y);
        open = true;
    }

    private void UpdateMinMax(float x0, float y0)
    {
        if (firstEval)
        {
            // 4 times
            if (x0 < minX) minX = x0;

            if (x0 > maxX) maxX = x0;

            if (y0 < minY) minY = y0;

            if (y0 > maxY) maxY = y0;

            firstEval = false;
        }
        else
        {
            // 2 times
            if (x0 < minX)
                minX = x0;
            else if (x0 > maxX) maxX = x0;

            if (y0 < minY)
                minY = y0;
            else if (y0 > maxY) maxY = y0;
        }
    }

    public Bounds GetBounds()
    {
        return new Bounds(
            (short)Math.Floor(minX),
            (short)Math.Floor(minY),
            (short)Math.Ceiling(maxX),
            (short)Math.Ceiling(maxY));
    }
}