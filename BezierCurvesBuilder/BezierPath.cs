using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierCurvesBuilder
{
    class BezierPath
    {
        public List<PointF> controlPointFs=new List<PointF>();
        private int SEGMENTS_PER_CURVE = 60;

        PointF CalculateBezierPointF(float t, PointF p0, PointF p1, PointF p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            
            return new PointF(uu * p0.X + 2 * t * u * p1.X + tt * p2.X, uu * p0.Y + 2 * t * u * p1.Y + tt * p2.Y);
        }

        public void AddControlPoint(PointF point)
        {
            controlPointFs.Add(point);
        }

        public bool IsEmpty()
        {
            return !controlPointFs.Any();
        }

        public List<PointF> GetDrawingPointFs()
        {
            List<PointF> drawingPointFs = new List<PointF>();
            for (int i = 0; i < controlPointFs.Count - 2; i += 2)
            {
                PointF p0 = controlPointFs[i];
                PointF p1 = controlPointFs[i + 1];
                PointF p2 = controlPointFs[i + 2];

                if (i == 0) //Only do this for the first endPointF.
                            //When i != 0, this coincides with the end
                            //PointF of the previous segment
                {
                    drawingPointFs.Add(CalculateBezierPointF(0, p0, p1, p2));
                }

                for (int j = 1; j <= SEGMENTS_PER_CURVE; j++)
                {
                    float t = j / (float)SEGMENTS_PER_CURVE;
                    drawingPointFs.Add(CalculateBezierPointF(t, p0, p1, p2));
                }
            }
            return drawingPointFs;
        }
    }
}
