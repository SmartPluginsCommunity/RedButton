using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Solid;
using tsm = Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.Extensions
{
    public static class GeometryPlaneExtension
    {
        /// <summary>
        /// Преобразование Plane в геометрический
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static GeometricPlane GetGeometricPlane(this tsm.Plane plane)
        {
            return new GeometricPlane(plane.Origin, plane.AxisX, plane.AxisY);
        }
        /// <summary>
        /// Получение словаря плоскостей из солида, точность солида следует получать
        /// при получении экземпляра из объекта
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        /// TODO требуется упрощение
        public static Dictionary<tsm.Plane, Face> GetGeometricPlanes(this tsm.Solid solid)
        {
            FaceEnumerator faceEnum = solid.GetFaceEnumerator();
            Dictionary<tsm.Plane, Face> planes = new Dictionary<tsm.Plane, Face>();
            while (faceEnum.MoveNext())
            {
                List<Point> planeVertexes = new List<Point>();

                Face face = faceEnum.Current as Face;
                LoopEnumerator loops = face.GetLoopEnumerator();
                while (loops.MoveNext())
                {
                    Loop loop = loops.Current as Loop;
                    VertexEnumerator vertexes = loop.GetVertexEnumerator();

                    while (vertexes.MoveNext())
                    {
                        Point vertex = vertexes.Current as Point;
                        if (!planeVertexes.Contains(vertex))
                        {
                            //Three points form a plane and they cannot be aligned.
                            if (planeVertexes.Count != 3 ||
                                (planeVertexes.Count == 3 && !ArePointAligned(planeVertexes[0], planeVertexes[1], vertex)))
                                planeVertexes.Add(vertex);

                            if (planeVertexes.Count == 3)
                            {
                                Vector vector1 = new Vector(planeVertexes[1].X - planeVertexes[0].X, planeVertexes[1].Y - planeVertexes[0].Y, planeVertexes[1].Z - planeVertexes[0].Z);
                                Vector vector2 = new Vector(planeVertexes[2].X - planeVertexes[0].X, planeVertexes[2].Y - planeVertexes[0].Y, planeVertexes[2].Z - planeVertexes[0].Z);

                                tsm.Plane plane = new tsm.Plane();
                                plane.Origin = planeVertexes[0];
                                plane.AxisX = vector1;
                                plane.AxisY = VectorNew(vector1, 90, Vector.Cross(vector1, vector2));
                                planes.Add(plane, face);
                                break;
                            }
                        }
                    }
                    break;
                }
            }
            return planes;
        }
        private static Vector VectorNew(Vector vector, double angle, Vector rotateAxis)
        {
            Matrix matrix = MatrixFactory.Rotate(angle, rotateAxis);
            return VectorNew(vector, matrix);
        }
        private static Vector VectorNew(Vector vector, Matrix matrix)
        {
            Point p = new Point(vector.X, vector.Y, vector.Z);
            Point point = matrix.Transform(p);
            return new Vector(point.X, point.Y, point.Z);
        }
        /// <summary>
        /// Проверка соноправленности векторов
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        /// TODO требуется перенести в папку с алгоритмами\математикой
        internal static bool ArePointAligned(Point point1, Point point2, Point point3)
        {
            Vector vector1 = new Vector(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);
            Vector vector2 = new Vector(point3.X - point1.X, point3.Y - point1.Y, point3.Z - point1.Z);
            return Tekla.Structures.Geometry3d.Parallel.VectorToVector(vector1, vector2);
        }
    }
}