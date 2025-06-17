using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Uniterm
{

    public class MyDrawing
    {

        #region Fields

        public static Pen pen
        {
            get
            {
                return new Pen(Brushes.SteelBlue, (int)Math.Log(fontsize, 3));
            }
        }

        private static Brush br = Brushes.White;

        public static FontFamily fontFamily = new FontFamily("Arial");

        public static /*double*/ Int32 fontsize = 12;

        public static string sA, sB, sOp;

        public static string pA, pB;

        public static char oper = ' ';


        public static int FS = 12;

        public DrawingContext dc;
        #endregion

        #region Initalizers

        public MyDrawing(DrawingContext drawingContext)
        {
            dc = drawingContext;
        }

        #endregion

        #region Public Methods

        public void Redraw()
        {
            if (oper != ' ')
            {
                DrawSwitched(new Point(20, fontsize + 30));
            }
            else
            {
                int baseX = 30;
                int seqY = fontsize + 30;
                int elimY = fontsize * 3 + 30;
                int offsetX = baseX;

                if (!string.IsNullOrEmpty(sA) &&
                    !string.IsNullOrEmpty(sOp) &&
                    !string.IsNullOrEmpty(sB))
                {
                    DrawSek(new Point(offsetX, seqY));

                    string seqText = sA
                                   + Environment.NewLine
                                   + sOp
                                   + Environment.NewLine
                                   + sB;
                    int seqWidth = GetTextLength(seqText);

                    offsetX += seqWidth + 40;
                }

                if (!string.IsNullOrEmpty(pA) &&
                    !string.IsNullOrEmpty(pB))
                {
                    DrawParallel(new Point(offsetX, elimY));
                }
            }
        }

        public static void ClearAll()
        {
            sA = sB = sOp = "";
            pA = pB = "";
            oper = ' ';
        }

        public void DrawSek(Point pt)
        {
            if (string.IsNullOrEmpty(sA) || string.IsNullOrEmpty(sOp) || string.IsNullOrEmpty(sB))
                return;

            string text = sA
                        + Environment.NewLine
                        + sOp
                        + Environment.NewLine
                        + sB;

            int textHeight = GetTextHeight(text);

            DrawText(new Point(pt.X + 20, pt.Y), text);

            DrawBezier(pt, textHeight);
        }

        public void DrawParallel(Point pt)
        {
            if (string.IsNullOrEmpty(pA) ||
                string.IsNullOrEmpty(pB))
                return;

            string text = pA + " ; " + pB;
            int width = GetTextLength(text);

            double textOffsetY = fontsize + 4;
            DrawText(new Point(pt.X, pt.Y + textOffsetY), text);

            DrawHoriz(pt, width);
        }


        public void DrawSwitched(Point pt)
        {
            if (string.IsNullOrEmpty(sA) ||
                string.IsNullOrEmpty(sOp) ||
                string.IsNullOrEmpty(sB) ||
                string.IsNullOrEmpty(pA) ||
                string.IsNullOrEmpty(pB))
            {
                return;
            }

            double marginX = 20;
            double textOffY = fontsize + 4;
            string elimText = pA + " ; " + pB;
            int elimWidth = GetTextLength(elimText);

            if (oper == 'A')
            {
                Point bracketPt = new Point(pt.X + marginX, pt.Y);
                DrawHoriz(bracketPt, elimWidth);
                DrawText(new Point(bracketPt.X, bracketPt.Y + textOffY), elimText);

                string rest = sOp + Environment.NewLine + sB;
                double yRest = bracketPt.Y + textOffY + GetTextHeight(elimText);
                DrawText(new Point(bracketPt.X, yRest), rest);

                double bottomY = yRest + GetTextHeight(rest);
                int totalH = (int)(bottomY - pt.Y);
                DrawBezier(pt, totalH + 5);
            }
            else
            {
                string first = sA + Environment.NewLine + sOp;
                DrawText(new Point(pt.X + marginX, pt.Y), first);
                double firstH = GetTextHeight(first);

                Point bracketPt = new Point(pt.X + marginX, pt.Y + firstH + 10);
                DrawHoriz(bracketPt, elimWidth);
                DrawText(new Point(bracketPt.X, bracketPt.Y + textOffY), elimText);

                double bottomY = bracketPt.Y + textOffY + GetTextHeight(elimText);
                int totalH = (int)(bottomY - pt.Y);
                DrawBezier(pt, totalH + 5);
            }
        }

        #endregion

        #region Private Methods

        private void DrawBezier(Point p0, int length)
        {
            Point start = p0;
            double b = Math.Sqrt(length) / 2 + 2;

            Point p1 = new Point(p0.X - b, p0.Y + length * 0.25);
            Point p2 = new Point(p0.X - b, p0.Y + length * 0.75);
            Point p3 = new Point(p0.X, p0.Y + length);

            foreach (Point pt in GetBezierPoints(p0, p1, p2, p3))
            {
                dc.DrawLine(pen, start, pt);
                start = pt;
            }
        }

        private void DrawHoriz(Point pt, int length)
        {
            dc.DrawLine(pen,
                pt,
                new Point(pt.X + length, pt.Y));

            double b = Math.Sqrt(length) / 2 + 2;

            dc.DrawLine(pen,
                new Point(pt.X, pt.Y),
                new Point(pt.X, pt.Y + b));

            dc.DrawLine(pen,
                new Point(pt.X + length, pt.Y),
                new Point(pt.X + length, pt.Y + b));
        }


        private void DrawText(Point point, string text)
        {
            dc.DrawText(GetFormattedText(text), point);
        }

        private int GetTextHeight(string text)
        {
            return (int)GetFormattedText(text).Height;
        }

        private int GetTextLength(string text)
        {
            return (int)GetFormattedText(text).Width;
        }

        private IEnumerable<Point> GetBezierPoints(Point A, Point B, Point C, Point D)
        {
            List<Point> points = new List<Point>();

            for (double t = 0.0d; t <= 1.0; t += 1.0 / 500)
            {
                double tbs = Math.Pow(t, 2);
                double tbc = Math.Pow(t, 3);
                double tas = Math.Pow((1 - t), 2);
                double tac = Math.Pow((1 - t), 3);

                points.Add(new Point
                {
                    Y = +tac * A.Y
                        + 3 * t * tas * B.Y
                        + 3 * tbs * (1 - t) * C.Y
                        + tbc * D.Y,
                   X = +tac * A.X
                        + 3 * t * tas * B.X
                        + 3 * tbs * (1 - t) * C.X
                        + tbc * D.X
                });
            }

            return points;
        }

        private FormattedText GetFormattedText(string text)
        {
            FontStyle style = FontStyles.Normal;

            style = FontStyles.Normal;
            Typeface typeface = new Typeface(fontFamily, style, FontWeights.Light, FontStretches.Medium);

            FormattedText formattedText = new FormattedText(text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface, fontsize, Brushes.Black);

            formattedText.TextAlignment = TextAlignment.Left;

            return formattedText;
        }

        #endregion
    }
}
