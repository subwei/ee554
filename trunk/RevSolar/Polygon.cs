using System;
using System.Collections;
using System.IO;

namespace test {
    public class Polygon {
        private ArrayList vertices;     // vertices that make up the polygon
        private ArrayList distances;    // stores the distances between vertices and a different Polygon
        private Segment segment;        // segment that an intersecting line makes with this polygon
        private Vector normal;          // contains the normal vector of the polygon
        private double d;
        private double minX;
        private double minY;
        private double minZ;
        private double maxX;
        private double maxY;
        private double maxZ;
        private Vertex barycenter;      // barycenter of polygon = average of polygon's vertices
        private int category;               // category of polygon
        public const int UNKNOWN = -1;
        public const int INSIDE = 0;   // used to categorize polygon outside of other object
        public const int OUTSIDE = 1;    // used to categorize polygon inside of other object
        public const int SAME = 2;      // used to categorize polygon on the boundary of other object with normal vector in same direction
        public const int OPPOSITE = 3;  // used to categorize polygon on the boundary of other object with normal vector in opposite direction
        public const double LIMIT = 1E-6;

        public Polygon()
            : this(new ArrayList()) {
        }

        public Polygon(ArrayList list) {
            vertices = new ArrayList(list);
            reset();
            distances = new ArrayList();
        }

        public void reset() {
            segment = new Segment();
            distances = new ArrayList();
            calcExtent();
            calcNormal();
            if (vertices.Count > 0) {
                calcBarycenter();
            }
            category = Polygon.UNKNOWN;
        }

        // copy constructor
        public Polygon(Polygon polygon, ArrayList newVertices) {

            vertices = new ArrayList();
            distances = new ArrayList();

            foreach (Vertex oldVertex in polygon.vertices) {
                Vertex v = Vertex.findVertex(oldVertex, newVertices);
                if (v != null) {
                    vertices.Add(v);
                }
                else {
                    Console.WriteLine("Polygon::Polygon => ERROR: Can't create new polygon, vertice not found in newVertices list");
                    throw new System.InvalidOperationException("Can't create new polygon, vertice not found in newVertices list");
                }
            }
            foreach (double num in polygon.distances) {
                distances.Add(num);
            }

            segment = new Segment(polygon.segment);
            normal = new Vector(polygon.normal);
            d = polygon.d;
            minX = polygon.minX;
            minY = polygon.minY;
            minZ = polygon.minZ;
            maxX = polygon.maxX;
            maxY = polygon.maxY;
            maxZ = polygon.maxZ;
            barycenter = new Vertex(polygon.barycenter);
            category = polygon.getCategory();
        }

        public void classifyByVertex() {
            foreach (Vertex vertex in vertices) {
                if (vertex.getState() != Vertex.ON_BOUNDARY) {
                    category = vertex.getState();
                    break;
                }
            }
        }

        /* 
         * Sets the internal classificationb based on the
         * Polygon Classification Routine
         * NOTE:  The dot product and distance qualifiers are the opposite
         * of the paper b/c we use CCW polygons instead of CW
         */
        public void classify(Polyhedron objectB) {

            /*
            printInfo();
            if (((Vertex)vertices[0]).Equals(new Vertex(2, 2, 6))) {
                int blah = 0;
            }*/

            Polygon closestPolygon = null;    // stores the closest polygon to polyA
            Vector ray = normal;
            double dot = 0;             // dot product between ray direction and normal vector from PolyB
            double distance = 0;        // distance from polygon to vertex
            double closestDistance = 0;
            bool castSuccess = true;

            do {
                closestDistance = Double.MaxValue;
                castSuccess = true;
                foreach (Polygon polygonB in objectB.getPolygons()) {

                    /////
                    //DEBUG
                    /////
                    polygonB.printInfo();


                    Vertex intersectPoint = polygonB.calcIntersection(ray, barycenter);
                    if (intersectPoint != null) {
                        dot = ray.Dot(polygonB.getNormal());
                        distance = polygonB.calcDistance(barycenter);

                        // ray is inside polygon so perturb the ray
                        if (Math.Abs(dot) < Vertex.ZERO_LIMIT && Math.Abs(distance) < Vertex.ZERO_LIMIT) {
                            // cast unsuccessful, leave loop and perturb
                            castSuccess = false;
                            ray = perturb(ray);
                            break;
                        }
                        // barycenter lies in plane of polygon
                        else if (Math.Abs(dot) > Vertex.ZERO_LIMIT && Math.Abs(distance) < Vertex.ZERO_LIMIT) {
                            // if barycenter is inside the polygon.  Note:  SAME means the barycenter lies on the bo
                            if (polygonB.encompassesVertex(barycenter) == Polygon.INSIDE || polygonB.encompassesVertex(barycenter) == Polygon.SAME) {
                                closestPolygon = polygonB;
                                closestDistance = 0;
                                break;
                            }
                        }
                        // check if the ray intersects
                        // deviates from the paper Math.Abs(dot)> Vertex.ZERO_LIMIT
                        else if (dot > Vertex.ZERO_LIMIT && distance < -Vertex.ZERO_LIMIT) {

                            if (Math.Abs(distance) < Math.Abs(closestDistance) && polygonB.isExtentOverlap(intersectPoint)) {

                                int status = polygonB.encompassesVertex(intersectPoint);
                                if (status == Polygon.INSIDE) {
                                    closestPolygon = polygonB;
                                    closestDistance = distance;
                                }
                                else if (status == Polygon.SAME) {  // ray hits edge of polygon, perturb ray
                                    castSuccess = false;
                                    ray = perturb(ray);
                                    break;
                                }
                            }
                        }
                    }
                }// end for each polygonB in objectB
            } while (!castSuccess);

            if (closestPolygon == null) {   // no intersection
                category = Polygon.OUTSIDE;
            }
            else {  // does intersect
                dot = ray.Dot(closestPolygon.getNormal());
                distance = closestPolygon.calcDistance(barycenter);

                if (Math.Abs(distance) < Vertex.ZERO_LIMIT) {
                    if (dot > Vertex.ZERO_LIMIT) {
                        category = Polygon.SAME;
                    }
                    else if (dot < -Vertex.ZERO_LIMIT) {
                        category = Polygon.OPPOSITE;
                    }
                }
                else if (dot > Vertex.ZERO_LIMIT) {
                    category = Polygon.INSIDE;
                }
                else if (dot < -Vertex.ZERO_LIMIT) {
                    category = Polygon.OUTSIDE;
                }
            }
        }

        public Vertex calcIntersection(Vector ray, Vertex barycenter) {

            double numerator = (d - (normal.x * barycenter.GetX() + normal.y * barycenter.GetY() + normal.z * barycenter.GetZ()));
            double denominator = (normal.x * ray.x + normal.y * ray.y + normal.z * ray.z);
            double t = 0;
            double Rx = 0;
            double Ry = 0;
            double Rz = 0;

            //if line is parallel to the plane
            if (Math.Abs(denominator) < Vertex.ZERO_LIMIT) {
                //if line is in the plane
                if (Math.Abs(numerator) < Vertex.ZERO_LIMIT) {
                    return barycenter;
                }
                else {
                    return null;
                }
            }
            else {
                t = numerator / denominator;
                Rx = barycenter.GetX() + t * ray.x;
                Ry = barycenter.GetY() + t * ray.y;
                Rz = barycenter.GetZ() + t * ray.z;
                Vertex R = new Vertex(Rx, Ry, Rz);
                return R;
            }
        }

        /* This method combines two techniques to solve
         * the point in polygon problem
         * 1) Project polygon onto a plane by taking the smallest resolution dimension and zeroing it
         * 2) Assume the polygon is convex
         * 3) If point is inside polygon, it will be on the same side of all the lines in the polygon
         * For CW oriented polygons, points inside will be on the right
         * Point in question is (x,y)  Points making up the polygon edge are (x0,y0) (x1,y1)
         * R = (y - y0) (x1 - x0) - (x - x0) (y1 - y0)
         * if R < 0, vertex is to the right of the segment
         * else if R > 0, vertex is to the left of the segment
         * else vertex is on the line segment
         * Returns INSIDE if vertex is in the polygon
         *         OUTSIDE if vertex is outside the polygon
         *         SAME if vertex is on the line
         */
        public int encompassesVertex(Vertex vertex) {

            ArrayList modVertices = new ArrayList();
            Vertex modVertex = new Vertex();
            int numRight = 0;   // number of edges to the right of the vertex
            int numLeft = 0;    // number of edges to the left of the vertex
            double arbiter = 0; // this determines if the edge is on the right, left, or on the edge
            int smallest = 0;   // 0 = x dimension should be eliminated, 1 = y, 2 = z

            /* These store the smallest and largest x, y, and z values
             * they will be used to determine which dimension to eliminate
             */
            double smallX = ((Vertex)vertices[0]).GetX();
            double smallY = ((Vertex)vertices[0]).GetY();
            double smallZ = ((Vertex)vertices[0]).GetZ();
            double largeX = ((Vertex)vertices[0]).GetX();
            double largeY = ((Vertex)vertices[0]).GetY();
            double largeZ = ((Vertex)vertices[0]).GetZ();

            foreach (Vertex v in vertices) {
                if (v.GetX() < smallX) {
                    smallX = v.GetX();
                }
                else if (v.GetX() > largeX) {
                    largeX = v.GetX();
                }

                if (v.GetY() < smallY) {
                    smallY = v.GetY();
                }
                else if (v.GetY() > largeY) {
                    largeY = v.GetY();
                }

                if (v.GetZ() < smallZ) {
                    smallZ = v.GetZ();
                }
                else if (v.GetZ() > largeZ) {
                    largeZ = v.GetZ();
                }
            }

            double diffX = Math.Abs(largeX - smallX);
            double diffY = Math.Abs(largeY - smallY);
            double diffZ = Math.Abs(largeZ - smallZ);
            double smallestValue = diffX;


            if (diffX < diffY && diffX < diffZ) {
                smallest = 0;
            }
            else if (diffY < diffX && diffY < diffZ) {
                smallest = 1;
            }
            else if (diffZ < diffX && diffZ < diffY) {
                smallest = 2;
            }
            else {
                smallest = 0;
            }

            if (smallest == 0) {
                foreach (Vertex v in vertices) {
                    modVertices.Add(new Vertex(v.GetY(), v.GetZ(), 0));
                    modVertex = new Vertex(vertex.GetY(), vertex.GetZ(), 0);
                }

            }
            else if (smallest == 1) {
                foreach (Vertex v in vertices) {
                    modVertices.Add(new Vertex(v.GetX(), v.GetZ(), 0));
                    modVertex = new Vertex(vertex.GetX(), vertex.GetZ(), 0);
                }
            }
            else if (smallest == 2) {
                foreach (Vertex v in vertices) {
                    modVertices.Add(new Vertex(v.GetX(), v.GetY(), 0));
                    modVertex = new Vertex(vertex.GetX(), vertex.GetY(), 0);
                }
            }

            Vertex v0;
            Vertex v1;

            for (int x = 0; x < modVertices.Count; x++) {
                v0 = (Vertex)modVertices[x];
                if (x == vertices.Count - 1) {
                    v1 = (Vertex)modVertices[0];
                }
                else {
                    v1 = (Vertex)modVertices[x + 1];
                }

                arbiter = (modVertex.GetY() - v0.GetY()) * (v1.GetX() - v0.GetX()) -
                            (modVertex.GetX() - v0.GetX()) * (v1.GetY() - v0.GetY());
                // point is to the right of the segment
                if (arbiter < -Vertex.ZERO_LIMIT) {
                    numRight++;
                }
                // point is to the left of the segment
                else if (arbiter > Vertex.ZERO_LIMIT) {
                    numLeft++;
                }
                else if (BuildingVRMLNode.checkbetween(v0, v1, modVertex)) {  // point is on edge
                    return Polygon.SAME;
                }
            }


            // For vertices oriented CCW, the point will be to the left of all the edges.  they will be the right if vertices oriented CW
            if (numLeft == modVertices.Count) {    // all edges are to the left of the point
                return Polygon.INSIDE;
            }
            else if (numRight == modVertices.Count) {
                return Polygon.INSIDE;
            }
            else {
                return Polygon.OUTSIDE;
            }

            /* Need to take into account both CCW and CW polygons
             * b/c dimension elimination won't preserve CW/CCW with respect to the normal vector
             * If the point is inside some polygon, CW or CCW then return INSIDE
             *
            if (numLeft == modVertices.Count || numRight == modVertices.Count) {
                return Polygon.INSIDE;
            }
            else {
                return Polygon.OUTSIDE;
            }*/
        }

        // returns true if vertex is on the edge of the polygon
        public bool containsEdgeVertex(Vertex vertex) {
            Vertex v1;
            Vertex v2;
            for (int x = 0; x < vertices.Count; x++) {
                v1 = (Vertex)vertices[x];
                if (x == vertices.Count - 1) {
                    v2 = (Vertex)vertices[0];
                }
                else {
                    v2 = (Vertex)vertices[x + 1];
                }
                if (vertex.isBetween(v1, v2)) {
                    return true;
                }
            }
            return false;
        }

        // returns a direction vector slightly shifted.
        public Vector perturb(Vector v) {
            Random rand = new Random();
            return new Vector(v.x + Vertex.ZERO_LIMIT * (rand.NextDouble() + 1), v.y + Vertex.ZERO_LIMIT * (rand.NextDouble() + 1), v.z + Vertex.ZERO_LIMIT * (rand.NextDouble() + 1));
        }

        public void calcNormal() {

            if (vertices.Count > 2) {

                Vector v1 = new Vector(((Vertex)vertices[1]).GetX() - ((Vertex)vertices[0]).GetX(),
                                        ((Vertex)vertices[1]).GetY() - ((Vertex)vertices[0]).GetY(),
                                        ((Vertex)vertices[1]).GetZ() - ((Vertex)vertices[0]).GetZ());

                Vector v2 = new Vector(((Vertex)vertices[2]).GetX() - ((Vertex)vertices[1]).GetX(),
                                        ((Vertex)vertices[2]).GetY() - ((Vertex)vertices[1]).GetY(),
                                        ((Vertex)vertices[2]).GetZ() - ((Vertex)vertices[1]).GetZ());

                normal = v1.Cross(v2);
                normal.Normalize();
                d = calcD(normal, (Vertex)vertices[0]);
            }
        }

        public void calcBarycenter() {

            double averageX = 0;
            double averageY = 0;
            double averageZ = 0;

            for (int x = 0; x < vertices.Count; x++) {
                Vertex v = (Vertex)vertices[x];
                averageX += v.GetX();
                averageY += v.GetY();
                averageZ += v.GetZ();
            }

            barycenter = new Vertex(averageX / vertices.Count, averageY / vertices.Count, averageZ / vertices.Count);
        }

        public Vertex getBarycenter() {
            return barycenter;
        }

        public void addVertice(Vertex vertex) {
            vertices.Add(vertex);
            calcExtent();
        }

        public Vertex getVertex(int x) {
            return (Vertex)vertices[x];
        }

        public ArrayList getVertices() {
            return vertices;
        }

        public void setVertex(Vertex vertex, int index) {
            vertices[index] = vertex;
        }

        public void calcExtent() {
            if (vertices.Count > 0) {
                Vertex i = (Vertex)vertices[0];
                minX = i.GetX();
                minY = i.GetY();
                minZ = i.GetZ();
                maxX = i.GetX();
                maxY = i.GetY();
                maxZ = i.GetZ();
            }
            foreach (Vertex vertex in vertices) {
                if (vertex.GetX() < minX) minX = vertex.GetX();
                if (vertex.GetY() < minY) minY = vertex.GetY();
                if (vertex.GetZ() < minZ) minZ = vertex.GetZ();
                if (vertex.GetX() > maxX) maxX = vertex.GetX();
                if (vertex.GetY() > maxY) maxY = vertex.GetY();
                if (vertex.GetZ() > maxZ) maxZ = vertex.GetZ();
            }
        }

        // return true if extent overlaps with obj
        // perhaps use float for these comparisons?
        public bool isExtentOverlap(Object obj) {

            calcExtent();
            bool overlap = false;
            if (obj is Polyhedron) {
                Polyhedron polyhedral = (Polyhedron)obj;

                if (maxX < polyhedral.getMinX() || minX > polyhedral.getMaxX() ||
                    maxY < polyhedral.getMinY() || minY > polyhedral.getMaxY() ||
                    maxZ < polyhedral.getMinZ() || minZ > polyhedral.getMaxZ()) {
                    return false;
                }
                else {
                    return true;
                }

            }
            else if (obj is Polygon) {
                Polygon polygon = (Polygon)obj;
                if (maxX < polygon.getMinX() || minX > polygon.getMaxX() ||
                    maxY < polygon.getMinY() || minY > polygon.getMaxY() ||
                    maxZ < polygon.getMinZ() || minZ > polygon.getMaxZ()) {
                    return false;
                }
                else {
                    return true;
                }
            }
            else if (obj is Vertex) {
                Vertex v = (Vertex)obj;
                if (v.GetX() < minX || v.GetX() > maxX ||
                    v.GetY() < minY || v.GetY() > maxY ||
                    v.GetZ() < minZ || v.GetZ() > maxZ) {
                    return false;
                }
                else {
                    return true;
                }
            }
            return overlap;
        }

        // Calculate distances for vertices from this polygon from polygonB using the point plane distance formula
        public void calcDistances(Polygon polygon) {
            // need to erase distances from the previous calculations
            distances.Clear();
            foreach (Vertex vertex in vertices) {
                distances.Add(polygon.calcDistance(vertex));
            }
        }

        /* Calculate distance from this polygon to Vertex v using this polgon's normal vector
         */
        public double calcDistance(Vertex v) {
            // The d value calculated for this polygon is using equation Ax+By+Cz=d;
            // The following equation uses Ax+By+Cz+d=0, so need to subtract d instead of add.
            double magnitude = Math.Sqrt(normal.x * normal.x + normal.y * normal.y + normal.z * normal.z);
            double distance = (normal.x * v.GetX() + normal.y * v.GetY() + normal.z * v.GetZ() - d) / magnitude;
            //double distance = (normal.x * v.GetX() + normal.y * v.GetY() + normal.z * v.GetZ() + d) / magnitude;
            return distance;
        }

        // returns true if distances are not mixed.  This indicates the polygons may overlap
        public bool doesOverlap(Polygon polygonB) {
            int positiveCount = 0;
            int negativeCount = 0;
            int zeroCount = 0;
            calcDistances(polygonB);

            bool newest = false;

            foreach (double distance in distances) {
                if (Math.Abs(distance) <= Vertex.ZERO_LIMIT) {
                    zeroCount++;
                }
                else if (distance < -Vertex.ZERO_LIMIT) {
                    negativeCount++;
                }
                else {
                    positiveCount++;
                }
            }

            if (distances.Count == 0) {
                return false;
            }
            else {
                // if all distances are 0 (coplanar) or all positive or all negative, then return false( doesn't overlap)
                return (!(zeroCount == distances.Count || negativeCount == distances.Count || positiveCount == distances.Count));
            }
        }

        // converts all vertices to new vertices with respect to the masterVertices list
        // returns true if all vertices are converted, false otherwise
        public bool convertVertices(ArrayList newVertices) {
            ArrayList tempVertices = new ArrayList();
            foreach (Vertex vertex in vertices) {
                Vertex foundVertex = Vertex.findVertex(vertex, newVertices);
                if (foundVertex != null) {
                    tempVertices.Add(foundVertex);
                }
                else {
                    Console.WriteLine("Polygon::convertVertices => ERROR: Can't convert vertices b/c at least one vertice does not exist in the list");
                    throw new System.InvalidOperationException("Polygon::convertVertices => ERROR: Can't convert vertices b/c at least one vertice does not exist in the list");
                }
            }

            if (tempVertices.Count != vertices.Count) {
                return false;
            }
            else {
                vertices = tempVertices;
                return true;
            }
        }

        public Segment calcSegment(Vertex P, Vector direction, ArrayList masterVertices) {

            ArrayList indexSignChange = new ArrayList();
            ArrayList pointsOnLine = new ArrayList();
            Segment segment = new Segment();
            ArrayList newVertexDistances = new ArrayList();
            ArrayList newVertices = new ArrayList();
            double ratio = 0;
            int index = 0;
            Vector tempDirection = new Vector(0, 0, 0);
            Vertex vertex = new Vertex();
            Vertex nextVertex = new Vertex();
            Vertex intersection = new Vertex();

            int startPoint = 0;
            int endPoint = 0;
            double startDistance = 0;
            double endDistance = 0;
            int startDescriptor = Segment.VERTEX;
            int endDescriptor = Segment.VERTEX;
            Vertex startVertex = new Vertex();
            Vertex endVertex = new Vertex();

            /* if a point is on the line. ie. distance = 0, then it MUST be an endpoint.
             * This is a Vertex start or endpoint.
             */
            for (int x = 0; x < distances.Count; x++) {
                if (Math.Abs((double)distances[x]) < Vertex.ZERO_LIMIT) {
                    pointsOnLine.Add(x);
                }
            }

            /* if the distances between two points change sign,then the line intersects at an edge
             * This is an Edge start or endpoint.
             */
            for (int x = 0; x < distances.Count; x++) {
                if (x == distances.Count - 1) {
                    if ((double)distances[x] < -Vertex.ZERO_LIMIT && (double)distances[0] > Vertex.ZERO_LIMIT) {
                        indexSignChange.Add(x);
                    }
                    else if ((double)distances[x] > Vertex.ZERO_LIMIT && (double)distances[0] < -Vertex.ZERO_LIMIT) {
                        indexSignChange.Add(x);
                    }
                }
                else {
                    if ((double)distances[x] < -Vertex.ZERO_LIMIT && (double)distances[x + 1] > Vertex.ZERO_LIMIT) {
                        indexSignChange.Add(x);
                    }
                    else if ((double)distances[x] > Vertex.ZERO_LIMIT && (double)distances[x + 1] < -Vertex.ZERO_LIMIT) {
                        indexSignChange.Add(x);
                    }
                }
            }

            /* Calculate point of intersection using ratio of distances.
             * Caution: Using the distances for a polygon centered around x=0 might result in error?
             */
            for (int x = 0; x < indexSignChange.Count; x++) {
                index = (int)indexSignChange[x];
                vertex = (Vertex)vertices[index];
                if (index == distances.Count - 1) {
                    nextVertex = (Vertex)vertices[0];
                }
                else {
                    nextVertex = (Vertex)vertices[index + 1];
                }
                if (index == distances.Count - 1) {
                    ratio = Math.Abs((double)distances[index]) / (Math.Abs((double)distances[index]) + Math.Abs((double)distances[0]));
                }
                else {
                    ratio = Math.Abs((double)distances[index]) / (Math.Abs((double)distances[index]) + Math.Abs((double)distances[index + 1]));
                }
                tempDirection.x = (nextVertex.GetX() - vertex.GetX()) * ratio;
                tempDirection.y = (nextVertex.GetY() - vertex.GetY()) * ratio;
                tempDirection.z = (nextVertex.GetZ() - vertex.GetZ()) * ratio;
                intersection = new Vertex(tempDirection.x + vertex.GetX(), tempDirection.y + vertex.GetY(), tempDirection.z + vertex.GetZ());

                /*
                tempDirection = new Vector(nextVertex.GetX() - vertex.GetX(), nextVertex.GetY() - vertex.GetY(), nextVertex.GetZ() - vertex.GetZ());
                // Need to project intersection onto the line to reduce error propagation
                Vertex projectedPoint = intersection.projectOntoLine(vertex, tempDirection);
                if (!projectedPoint.Equals(intersection)) {
                    int blah = 0;
                }
                newVertices.Add(projectedPoint);
                newVertexDistances.Add(projectedPoint.calcSignDistance(P, direction));
                 */

                newVertices.Add(intersection);
                newVertexDistances.Add(intersection.calcSignDistance(P, direction));
            }

            /* Only these cases can appear if operating on a convex polygon
             * 2 points on the line, 1 point on the line and one edge, 2 edges
             */
            if (pointsOnLine.Count == 0) {
                startDescriptor = Segment.EDGE;
                startDistance = (double)newVertexDistances[0];
                startPoint = masterVertices.IndexOf((Vertex)vertices[((int)indexSignChange[0])]);
                startVertex = (Vertex)newVertices[0];
                endDescriptor = Segment.EDGE;
                endDistance = (double)newVertexDistances[1];
                endPoint = masterVertices.IndexOf((Vertex)vertices[((int)indexSignChange[1])]);
                endVertex = (Vertex)newVertices[1];
            }
            else if (pointsOnLine.Count == 1) {

                double dist = ((Vertex)vertices[((int)pointsOnLine[0])]).calcSignDistance(P, direction);

                // if no new vertices, then it means VVV
                if (newVertexDistances.Count == 0) {
                    startDescriptor = Segment.VERTEX;
                    startDistance = dist;
                    startPoint = masterVertices.IndexOf((Vertex)vertices[((int)pointsOnLine[0])]);
                    startVertex = (Vertex)masterVertices[startPoint];
                    endDescriptor = startDescriptor;
                    endDistance = startDistance;
                    endPoint = startPoint;
                    endVertex = startVertex;
                }
                else {

                    if ((int)pointsOnLine[0] <= (int)indexSignChange[0]) {
                        startDescriptor = Segment.VERTEX;
                        startDistance = dist;
                        startPoint = masterVertices.IndexOf((Vertex)vertices[((int)pointsOnLine[0])]);
                        startVertex = (Vertex)masterVertices[startPoint];
                        endDescriptor = Segment.EDGE;
                        endDistance = (double)newVertexDistances[0];
                        endPoint = masterVertices.IndexOf((Vertex)vertices[((int)indexSignChange[0])]);
                        endVertex = (Vertex)newVertices[0];
                    }
                    else {
                        startDescriptor = Segment.EDGE;
                        startDistance = (double)newVertexDistances[0];
                        startPoint = masterVertices.IndexOf((Vertex)vertices[((int)indexSignChange[0])]);
                        startVertex = (Vertex)newVertices[0];
                        endDescriptor = Segment.VERTEX;
                        endDistance = dist;
                        endPoint = masterVertices.IndexOf((Vertex)vertices[((int)pointsOnLine[0])]);
                        endVertex = (Vertex)masterVertices[endPoint];
                    }
                }
            }
            else if (pointsOnLine.Count == 2) {
                double dist0 = ((Vertex)vertices[((int)pointsOnLine[0])]).calcSignDistance(P, direction);
                double dist1 = ((Vertex)vertices[((int)pointsOnLine[1])]).calcSignDistance(P, direction);

                startDescriptor = Segment.VERTEX;
                startDistance = dist0;
                startPoint = masterVertices.IndexOf((Vertex)vertices[((int)pointsOnLine[0])]);
                startVertex = (Vertex)masterVertices[startPoint];
                endDescriptor = Segment.VERTEX;
                endDistance = dist1;
                endPoint = masterVertices.IndexOf((Vertex)vertices[((int)pointsOnLine[1])]);
                endVertex = (Vertex)masterVertices[endPoint];

            }
            else {
                Console.WriteLine("Polygon::CalcSegment => ERROR: More than 2 points lie on intersection line.");
                throw new System.InvalidOperationException("More than 2 points lie on intersection line.");
            }

            segment.setStartDescriptor(startDescriptor);
            segment.setStartDistance(startDistance);
            segment.setStartPoint(startPoint);
            segment.setStartVertex(startVertex);
            segment.setEndDescriptor(endDescriptor);
            segment.setEndDistance(endDistance);
            segment.setEndPoint(endPoint);
            segment.setEndVertex(endVertex);

            // Want to have start point be closer to the intersectionPoint than the endDistance for right now
            // Makes it easier for the segmentIntersection.  will switch back to startpoint being smaller index after segmentIntersection
            if (segment.getEndDistance() < segment.getStartDistance()) {
                segment.swap();
            }

            /* set middle descriptor based on start and end points
             * if start and end points are adjacent vertices in the same polygon, then middle segment is an edge
             * else if the start and end points are the same vertex, middle segment is a vertex
             * else the middle segment is a face
             */

            if (isAdjacent(startVertex, endVertex) && segment.getStartDescriptor() == Segment.VERTEX && segment.getEndDescriptor() == Segment.VERTEX) segment.setMiddleDescriptor(Segment.EDGE);
            else if (startPoint == endPoint) segment.setMiddleDescriptor(Segment.VERTEX);
            else segment.setMiddleDescriptor(Segment.FACE);

            return segment;
        }


        private Vertex calcVertexInPlane(Vertex intersectPoint, Vector direction, double distance, int startIndex) {

            // look to see if other methods use direction that is not normalized.
            direction.Normalize();

            // Find point on the line
            Vertex v = new Vertex(distance * direction.x + intersectPoint.GetX(),
                                        distance * direction.y + intersectPoint.GetY(),
                                        distance * direction.z + intersectPoint.GetZ());

            // calculate projection onto plane
            double x = v.GetX() - direction.x * (direction.x * v.GetX() + direction.y * v.GetY() + direction.z * v.GetZ() + d) / (
                                direction.x * direction.x + direction.y * direction.y + direction.z * direction.z);
            double y = v.GetY() - direction.y * (direction.x * v.GetX() + direction.y * v.GetY() + direction.z * v.GetZ() + d) / (
                                direction.x * direction.x + direction.y * direction.y + direction.z * direction.z);
            double z = v.GetZ() - direction.z * (direction.x * v.GetX() + direction.y * v.GetY() + direction.z * v.GetZ() + d) / (
                                direction.x * direction.x + direction.y * direction.y + direction.z * direction.z);

            return (new Vertex(x, y, z));
        }


        private Vertex calcVertexOnLine(ArrayList objectVertices, Vertex intersectPoint, Vector direction, double distance, int startIndex) {

            Vertex v1 = (Vertex)objectVertices[startIndex];
            Vertex v2;
            int index = vertices.IndexOf(v1);

            // look to see if other methods use direction that is not normalized.
            direction.Normalize();

            // Find point on the line and then calculate projection onto segment edge
            Vertex point = new Vertex(distance * direction.x + intersectPoint.GetX(),
                                        distance * direction.y + intersectPoint.GetY(),
                                        distance * direction.z + intersectPoint.GetZ());

            // Get unit vector that represents the edge of intersection in the polygon

            // if the next consecutive vertex is out of bounds, loop back to 0
            if (startIndex == vertices.Count - 1) {
                v2 = (Vertex)vertices[0];
            }
            else {
                v2 = (Vertex)vertices[index + 1];
            }
            Vector edgeVector = new Vector(v2.GetX() - v1.GetX(), v2.GetY() - v1.GetY(), v2.GetZ() - v1.GetZ());
            edgeVector.Normalize();


            // calculate vector from the new point on line to the intersectionPoint
            Vector vector = new Vector(point.GetX() - v1.GetX(),
                                        point.GetY() - v1.GetY(),
                                        point.GetZ() - v1.GetZ());

            double scalar = vector.Dot(edgeVector);

            // Calculates the projection of vector onto direction
            Vector projection = new Vector(scalar * edgeVector.x, scalar * edgeVector.y, scalar * edgeVector.z);

            // The new point is calculated
            Vertex newPoint = new Vertex(v1.GetX() + projection.x,
                                            v1.GetY() + projection.y,
                                            v1.GetZ() + projection.z);
            return newPoint;
        }

        /* Returns a list of polygons that remain from the subdivision
         * or null list if no extra polygons are created
         */
        public ArrayList split(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            PolygonBreaker polygonBreaker = new PolygonBreaker(objectVertices, vertices, segmentIntersection, segment, intersectPoint, direction);
            ArrayList newPolygons = new ArrayList();

            if (segmentIntersection.getEndVertex().Equals(new Vertex(3, 2, 2)) || segmentIntersection.getStartVertex().Equals(new Vertex(3, 2, 2))) {
                int blah = 0;
            }

            /* There are ten cases for the intersecting segment
             */
            if (segmentIntersection.getStartDescriptor() == Segment.VERTEX &&
                segmentIntersection.getMiddleDescriptor() == Segment.VERTEX &&
                segmentIntersection.getEndDescriptor() == Segment.VERTEX) {
                segmentIntersection.getStartVertex().setState(Vertex.ON_BOUNDARY);
                newPolygons = null;
                Console.WriteLine("VVV");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.VERTEX &&
                    segmentIntersection.getMiddleDescriptor() == Segment.EDGE &&
                    segmentIntersection.getEndDescriptor() == Segment.VERTEX) {
                segmentIntersection.getStartVertex().setState(Vertex.ON_BOUNDARY);
                segmentIntersection.getEndVertex().setState(Vertex.ON_BOUNDARY);
                newPolygons = null;
                Console.WriteLine("VEV");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.VERTEX &&
                    segmentIntersection.getMiddleDescriptor() == Segment.EDGE &&
                    segmentIntersection.getEndDescriptor() == Segment.EDGE) {

                newPolygons = polygonBreaker.splitVEE();
                Console.WriteLine("VEE");

            }
            else if (segmentIntersection.getStartDescriptor() == Segment.EDGE &&
               segmentIntersection.getMiddleDescriptor() == Segment.EDGE &&
               segmentIntersection.getEndDescriptor() == Segment.VERTEX) {

                newPolygons = polygonBreaker.splitEEV();
                Console.WriteLine("EEV");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.VERTEX &&
                    segmentIntersection.getMiddleDescriptor() == Segment.FACE &&
                    segmentIntersection.getEndDescriptor() == Segment.VERTEX) {

                newPolygons = polygonBreaker.splitVFV();
                Console.WriteLine("VFV");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.VERTEX &&
                     segmentIntersection.getMiddleDescriptor() == Segment.FACE &&
                     segmentIntersection.getEndDescriptor() == Segment.EDGE) {

                newPolygons = polygonBreaker.splitVFE();
                Console.WriteLine("VFE");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.EDGE &&
                segmentIntersection.getMiddleDescriptor() == Segment.FACE &&
                segmentIntersection.getEndDescriptor() == Segment.VERTEX) {

                newPolygons = polygonBreaker.splitEFV();
                Console.WriteLine("EFV");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.VERTEX &&
               segmentIntersection.getMiddleDescriptor() == Segment.FACE &&
               segmentIntersection.getEndDescriptor() == Segment.FACE) {

                newPolygons = polygonBreaker.splitVFF();
                Console.WriteLine("VFF");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.FACE &&
                segmentIntersection.getMiddleDescriptor() == Segment.FACE &&
                segmentIntersection.getEndDescriptor() == Segment.VERTEX) {

                newPolygons = polygonBreaker.splitFFV();
                Console.WriteLine("FFV");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.EDGE &&
               segmentIntersection.getMiddleDescriptor() == Segment.EDGE &&
               segmentIntersection.getEndDescriptor() == Segment.EDGE) {

                newPolygons = polygonBreaker.splitEEE();
                Console.WriteLine("EEE");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.EDGE &&
                    segmentIntersection.getMiddleDescriptor() == Segment.FACE &&
                    segmentIntersection.getEndDescriptor() == Segment.EDGE) {

                newPolygons = polygonBreaker.splitEFE();
                Console.WriteLine("EFE");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.EDGE &&
               segmentIntersection.getMiddleDescriptor() == Segment.FACE &&
               segmentIntersection.getEndDescriptor() == Segment.FACE) {

                newPolygons = polygonBreaker.splitEFF();
                Console.WriteLine("EFF");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.FACE &&
          segmentIntersection.getMiddleDescriptor() == Segment.FACE &&
          segmentIntersection.getEndDescriptor() == Segment.EDGE) {

                newPolygons = polygonBreaker.splitFFE();
                Console.WriteLine("FFE");
            }
            else if (segmentIntersection.getStartDescriptor() == Segment.FACE &&
          segmentIntersection.getMiddleDescriptor() == Segment.FACE &&
          segmentIntersection.getEndDescriptor() == Segment.FACE) {

                newPolygons = polygonBreaker.splitFFF();
                Console.WriteLine("FFF");
            }
            else {
                Console.WriteLine("No splitting happened");
            }


            if (newPolygons != null) {
                foreach (Polygon p in newPolygons) {
                    if (p.getVertices().Count > 3) {

                        if (p.getVertex(0).Equals(new Vertex(1.5, 1.25, 1)) && p.getVertex(2).Equals(new Vertex(5, 5, 3))) {
                            int blah = 0;
                        }
                    }
                }
            }

            return newPolygons;
        }

        // returns true if one of the vertices in the polygon is unknown
        public bool isUnknown() {
            foreach (Vertex v in vertices) {
                if (v.getState() == Vertex.UNKNOWN) {
                    return true;
                }
            }
            return false;
        }

        // returns true if v1 and v2 are adjacent on this polygon
        public bool isAdjacent(Vertex startVertex, Vertex endVertex) {
            for (int x = 0; x < vertices.Count; x++) {
                Vertex v = (Vertex)vertices[x];
                if (v.Equals(startVertex)) {
                    if (x == 0) {
                        if (((Vertex)vertices[x + 1]).Equals(endVertex) || ((Vertex)vertices[vertices.Count - 1]).Equals(endVertex)) {
                            return true;
                        }
                    }
                    else if (x == vertices.Count - 1) {
                        if (((Vertex)vertices[0]).Equals(endVertex) || ((Vertex)vertices[x - 1]).Equals(endVertex)) {
                            return true;
                        }
                    }
                    else {
                        if (((Vertex)vertices[x + 1]).Equals(endVertex) || ((Vertex)vertices[x - 1]).Equals(endVertex)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // calls vertex marking routine on all unknown vertices
        public void markVertices() {
            foreach (Vertex vertex in vertices) {
                if (vertex.getState() == Vertex.UNKNOWN) {
                    // might be a problem with converting the SAME and OPPOSITE qualifiers into vertex categories
                    if (category == Polygon.SAME || category == Polygon.OPPOSITE) {
                        vertex.mark(Vertex.ON_BOUNDARY);
                    }
                    else {
                        vertex.mark(category);
                    }
                }
            }
        }

        public ArrayList convertToFace(ArrayList buildingVertices) {
            ArrayList face = new ArrayList();
            foreach (Vertex v in vertices) {
                face.Add(buildingVertices.IndexOf(v));
            }
            return face;
        }

        public void reverse() {
            vertices.Reverse();
            calcNormal();
        }

        private double calcD(Vector vector, Vertex vertex) {
            return vector.x * vertex.GetX() + vector.y * vertex.GetY() + vector.z * vertex.GetZ();
        }

        public Vector getNormal() {
            return normal;
        }

        public int getSize() {
            return vertices.Count;
        }

        public double getD() {
            return d;
        }

        public double getMinX() {
            return minX;
        }

        public double getMinY() {
            return minY;
        }

        public double getMinZ() {
            return minZ;
        }

        public double getMaxX() {
            return maxX;
        }

        public double getMaxY() {
            return maxY;
        }

        public double getMaxZ() {
            return maxZ;
        }

        public int getCategory() {
            return category;
        }

        public void setCategory(int x) {
            category = x;
        }

        public void printInfo() {
            Console.Write("Polygon = ");
            if (category == Polygon.INSIDE) {
                Console.WriteLine("INSIDE");
            }
            else if (category == Polygon.OPPOSITE) {
                Console.WriteLine("OPPOSITE");
            }
            else if (category == Polygon.OUTSIDE) {
                Console.WriteLine("OUTSIDE");
            }
            else if (category == Polygon.SAME) {
                Console.WriteLine("SAME");
            }
            else if (category == Polygon.UNKNOWN) {
                Console.WriteLine("UNKNOWN");
            }
            else {
                Console.WriteLine("INVALID CATEGORY");
            }
            foreach (Vertex v in vertices) {
                v.printInfo();
            }
        }

        public BuildingVRMLNode convertToBuilding() {
            BuildingVRMLNode newBuilding = new BuildingVRMLNode(vertices);
            ArrayList face = new ArrayList();
            for (int x = 0; x < vertices.Count; x++) {
                face.Add(x);
            }
            newBuilding.addFace(face);
            return newBuilding;
        }
    }
}
