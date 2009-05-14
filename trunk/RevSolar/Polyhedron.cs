using System;
using System.Collections;
using System.IO;

namespace test {
    public class Polyhedron {
        ArrayList vertices;
        ArrayList polygons;
        private double minX;
        private double minY;
        private double minZ;
        private double maxX;
        private double maxY;
        private double maxZ;

        public Polyhedron()
            : this(new ArrayList(), new ArrayList(), new ArrayList()) {
        }

        // copy constructor
        public Polyhedron(Polyhedron polyhedral) {
            vertices = new ArrayList();
            polygons = new ArrayList();
            foreach (Vertex v in polyhedral.vertices) {
                vertices.Add(new Vertex(v));
            }
            foreach (Polygon p in polyhedral.polygons) {
                polygons.Add(new Polygon(p, vertices));
            }
            minX = polyhedral.minX;
            minY = polyhedral.minY;
            minZ = polyhedral.minZ;
            maxX = polyhedral.maxX;
            maxY = polyhedral.maxY;
            maxZ = polyhedral.maxZ;
        }

        public Polyhedron(ArrayList vertices, ArrayList tempPolygons) {
            this.vertices = new ArrayList(vertices);
            polygons = tempPolygons;
            calcExtent();
        }

        public Polyhedron(ArrayList vertices, ArrayList faces, ArrayList normals) {
            this.vertices = new ArrayList(vertices);
            polygons = new ArrayList();
            ArrayList tempVertices = new ArrayList();
            // convert faces and normals into polygons
            for (int x = 0; x < faces.Count; x++) {
                ArrayList face = (ArrayList)faces[x];
                tempVertices = new ArrayList();
                // cycles through the faces arraylist replacing vetices indexes into actual vertices
                foreach (int y in face) {
                    tempVertices.Add((Vertex)vertices[y]);
                }
                Polygon polygon = new Polygon(tempVertices);
                polygons.Add(polygon);
            }
            calcExtent();
        }

        // divide polygons in this polyhedron if it overlaps with objectB
        public void subDivide(Polyhedron objectB) {

            Vertex intersectPoint = new Vertex();   // intersectPoint and direction make up the line of intersection b/w 2 polygons
            Vector direction = new Vector();
            Segment segmentA = new Segment();       // intersection segment from this polygon
            Segment segmentB = new Segment();       // intersection segment from shadow polygon
            Segment segmentIntersect = new Segment();    // intersection between segment 1 and 2.  There are 10 cases.
            ArrayList newPolygons = new ArrayList();
            ArrayList tempPolygons = new ArrayList();

            // Search for overlapping extents between building polygons and shadow object
            if (isExtentOverlap(objectB)) {
                for (int x = 0; x < polygons.Count; x++) {
                    Polygon polygonA = (Polygon)polygons[x];

                    /******DEBUG******/
                    if (false) {
                        polygonA.printInfo();
                        Console.WriteLine(polygonA.isExtentOverlap(objectB));
                    }

                    if (polygonA.isExtentOverlap(objectB)) {
                        foreach (Polygon polygonB in objectB.polygons){
                            /******DEBUG******/
                            if (false) {
                                polygonA.printInfo();
                                polygonB.printInfo();
                                Console.WriteLine(polygonA.isExtentOverlap(polygonB));
                                Console.WriteLine();
                            }
                            
                            if (polygonA.getVertex(polygonA.getVertices().Count - 1).Equals(new Vertex(2, 2, 2)) && polygonB.getVertex(0).Equals(new Vertex(4, 3, 1))) {
                                int blah = 0;
                            }

                            if (polygonA.isExtentOverlap(polygonB)) {
                                // Test to see both polygon distances reveal that they overlap each other
                                if (polygonA.doesOverlap(polygonB)) {
                                    if (polygonB.doesOverlap(polygonA)) {

                                        calcAdjacentVertices(true); // need to call this before segment calculations b/c they make use of the adjacentVertices to figure out middle segment
                                        objectB.calcAdjacentVertices(false);
                                        calcLineIntersection(polygonA, polygonB, ref intersectPoint, ref direction);


                                        if (polygonA.getVertex(0).Equals(new Vertex(4, 4, 4)) && polygonA.getVertex(polygonA.getVertices().Count - 1).Equals(new Vertex(2, 4, 4))) {
                                            int blah = 0;
                                        }

                                        // calculate segment for polygonA and polygonB
                                        segmentA = polygonA.calcSegment(intersectPoint, direction, vertices);
                                        // need to use the shadow's vertices to calculate segment 2
                                        segmentB = polygonB.calcSegment(intersectPoint, direction, objectB.getVertices());

                                        if (segmentA.overlaps(segmentB)) {

                                            segmentIntersect = segmentA.calcSegmentIntersection(segmentB, vertices, polygonA.getVertices());

                                            /* subdivide polygons into non-intersecting parts
                                             */
                                            tempPolygons = polygonA.split(vertices, segmentIntersect, segmentA, intersectPoint, direction);

                                            if (tempPolygons != null) {
                                                polygons.Remove(polygonA);
                                                polygons.AddRange(tempPolygons);
                                                /******DEBUG******/
                                                if (false) {
                                                    polygonA.printInfo();
                                                    polygonB.printInfo();
                                                    Console.WriteLine("Added Polygons");
                                                    foreach (Polygon tempPolygon in tempPolygons) {
                                                        tempPolygon.printInfo();
                                                    }
                                                    Console.WriteLine();
                                                }

                                                // if polygonA doesn't match up anymore, then it was deleted and new polygons were added
                                                if (polygonA != (Polygon)polygons[x]) {
                                                    x--;
                                                    break;
                                                }
                                            }
                                        }
                                    }// end if polygonB.doesOverlap(polygonA)
                                }// end if polygonA.doesOverlap(polygonB)
                            }
                        }// end for all shadow polygons
                    }// end if polygon overlaps with shadow polyhedron
                }// end for all building polygons
            }// end if buildings overlap
        }

        private void calcLineIntersection(Polygon polygonA, Polygon polygonB, ref Vertex intersectPoint, ref Vector direction) {

            Vector normalA = polygonA.getNormal();
            Vector normalB = polygonB.getNormal();
            double d1 = -polygonA.getD();
            double d2 = -polygonB.getD();
            double x = 0;
            double y = 0;
            double z = 0;

            // Calculate the direction of the intersection line
            direction = normalA.Cross(normalB);
            direction.Normalize();
            /* Calculates a point on the intersection line
             * A1x + B1y + C1z = d1, A2x + B2y = C2z = d2
             * set x = 0, solve equation.
             * Need to put in case to handle the divide by 0 error
             */

            //getting a line point, zero is set to a coordinate whose direction 
            //component isn't zero (line intersecting its origin plan)
            if (Math.Abs(direction.x) > 0) {
                x = 0;
                y = (d2 * normalA.z - d1 * normalB.z) / direction.x;
                z = (d1 * normalB.y - d2 * normalA.y) / direction.x;
            }
            else if (Math.Abs(direction.y) > 0) {
                x = (d1 * normalB.z - d2 * normalA.z) / direction.y;
                y = 0;
                z = (d2 * normalA.x - d1 * normalB.x) / direction.y;
            }
            else {
                x = (d2 * normalA.y - d1 * normalB.y) / direction.z;
                y = (d1 * normalB.x - d2 * normalA.x) / direction.z;
                z = 0;
            }

            intersectPoint = new Vertex(x, y, z);

            if (Double.IsNaN(direction.x) || Double.IsNaN(direction.y) || Double.IsNaN(direction.z) ||
                Double.IsNaN(x) || Double.IsNaN(y) || Double.IsNaN(z)) {
                Console.WriteLine("Polyhedron::calcLineIntersection => ERROR:  Invalid calculation has occured");
                throw new System.InvalidOperationException("Invalid calculation has occured while calculating line intersection");
            }
        }

        public void calcAdjacentVertices(bool erase) {

            int index = 0;

            foreach (Vertex vertex in vertices){
                // Need to clear the adjacency list every time b/c this changes dynamically
                if (erase) {
                    vertex.clearAdjacentList();
                }
                foreach (Polygon polygon in polygons){
                    index = polygon.getVertices().IndexOf(vertex);
                    if (index != -1) {
                        Vertex vertexBefore = null;
                        Vertex vertexAfter = null;
                        if (index == 0) {
                            vertexBefore = (Vertex)polygon.getVertex(polygon.getSize() - 1);
                            vertexAfter = (Vertex)polygon.getVertex(index + 1);
                        }
                        else if (index == polygon.getSize() - 1) {
                            vertexBefore = (Vertex)polygon.getVertex(index - 1);
                            vertexAfter = (Vertex)polygon.getVertex(0);
                        }
                        else {
                            vertexBefore = (Vertex)polygon.getVertex(index - 1);
                            vertexAfter = (Vertex)polygon.getVertex(index + 1);
                        }

                        // Check to see if the vertices aren't already in the adjacent vertices list
                        if (!vertex.adjacentExists(vertexBefore) && vertexBefore != null) {
                            vertex.addAdjacentVertex(vertexBefore);
                        }
                        if (!vertex.adjacentExists(vertexAfter) && vertexAfter != null) {
                            vertex.addAdjacentVertex(vertexAfter);
                        }
                    }
                }
            }
        }

        public void calcExtent() {
            if (polygons.Count > 0) {
                Polygon i = (Polygon)polygons[0];
                i.calcExtent();
                minX = i.getMinX();
                minY = i.getMinY();
                minZ = i.getMinZ();
                maxX = i.getMaxX();
                maxY = i.getMaxY();
                maxZ = i.getMaxZ();
            }
            foreach (Polygon polygon in polygons){
                polygon.calcExtent();
                if (polygon.getMinX() < minX) minX = polygon.getMinX();
                if (polygon.getMinY() < minY) minY = polygon.getMinY();
                if (polygon.getMinZ() < minZ) minZ = polygon.getMinZ();
                if (polygon.getMaxX() > maxX) maxX = polygon.getMaxX();
                if (polygon.getMaxY() > maxY) maxY = polygon.getMaxY();
                if (polygon.getMaxZ() > maxZ) maxZ = polygon.getMaxZ();
            }
        }


        public bool isExtentOverlap(Polyhedron polyhedral) {
            calcExtent();
            polyhedral.calcExtent();
            if (maxX < polyhedral.getMinX() || minX > polyhedral.getMaxX() ||
                maxY < polyhedral.getMinY() || minY > polyhedral.getMaxY() ||
                maxZ < polyhedral.getMinZ() || minZ > polyhedral.getMaxZ()) {
                return false;
            }
            else {
                return true;
            }
        }

        private void classifyPolygons() {
            foreach (Polygon polygon in polygons) {
                polygon.classifyByVertex();
            }
        }

        public void selectPolygons(bool isPrimary, Polyhedron objectB, ref ArrayList newPolygons, ref ArrayList newVertices) {
            classifyPolygons();
            // Add vertices from the primary object to the newPolygons list
            if (isPrimary) {
                foreach (Vertex vertex in vertices) {
                    // if the vertex isn't already in the list and is on the boundary or outside the boundary, add it to the list
                    if ((newVertices.IndexOf(vertex) == -1) && Vertex.findVertex(vertex, newVertices) == null && (vertex.getState() == Vertex.ON_BOUNDARY || vertex.getState() == Vertex.OUTSIDE_BOUNDARY)) {
                        newVertices.Add(vertex);
                    }
                }
            }
            else {
                foreach (Vertex vertex in vertices) {
                    if ((newVertices.IndexOf(vertex) == -1) && Vertex.findVertex(vertex, newVertices) == null && vertex.getState() == Vertex.INSIDE_BOUNDARY) {
                        newVertices.Add(vertex);
                    }
                }
            }

            // Add each applicable polygon
            foreach (Polygon polygon in polygons){
                // classify polygon if it is still unknown (all vertices are boundary vertices)
                if (polygon.getCategory() == Polygon.UNKNOWN) {
                    //polygon.printInfo();
                    polygon.classify(objectB);
                    //polygon.printInfo();
                }

                //Add polygons with category OUTSIDE and OPPOSITE for objectA
                //Add polygons with cateogry INSIDE for objectB and also convert vertices to the newVertices list
                if (isPrimary && (polygon.getCategory() == Polygon.OUTSIDE || polygon.getCategory() == Polygon.OPPOSITE)) {
                    if (polygon.convertVertices(newVertices) != true) {
                        Console.WriteLine("Polyhedron::selectPolygons => ERROR: Can't convert vertices b/c at least one vertice does not exist in the list");
                        throw new System.InvalidOperationException("Can't convert vertices b/c at least one vertice does not exist in the list");
                    }
                    newPolygons.Add(polygon);
                }
                else if (!isPrimary && (polygon.getCategory() == Polygon.INSIDE)) {
                    if (polygon.convertVertices(newVertices) != true) {
                        Console.WriteLine("Polyhedron::selectPolygons => ERROR: Can't convert vertices b/c at least one vertice does not exist in the list");
                        throw new System.InvalidOperationException("Polyhedron::selectPolygons => ERROR: Can't convert vertices b/c at least one vertice does not exist in the list");
                    }
                    polygon.reverse();
                    newPolygons.Add(polygon);
                }
            }
        }

        // Region Marking Routine
        public void markRegion(Polyhedron objectB) {
            // calculate adjacency information for all vertices
            calcAdjacentVertices(true);
            //shadow.calcAdjacentVertices();
            foreach (Polygon polyA in polygons){
                // if polygon is unknown, call polygon classification routine
                if (polyA.isUnknown()) {
                    polyA.classify(objectB);
                }
                // if any vertex is marked as UNKNOWN, call vertex marking routine
                polyA.markVertices();
            }
        }

        public BuildingVRMLNode convertToBuilding() {
            // Clear all information stored in the vertices list
            foreach (Vertex v in vertices) {
                v.clearAdjacentList();
                v.setState(Vertex.UNKNOWN);
            }
            BuildingVRMLNode building = new BuildingVRMLNode(vertices);
            foreach (Polygon polygon in polygons){
                ArrayList face = polygon.convertToFace(vertices);
                building.addFace(face);
            }
            return building;
        }

        // clear all states used in reverseEnvelope
        public void reset() {
            calcExtent();
            foreach (Polygon polygon in polygons) {
                polygon.reset();
            }
            foreach (Vertex v in vertices) {
                v.reset();
            }
        }

        public Polygon getPolygon(int x) {
            return (Polygon)polygons[x];
        }

        public int getPolygonCount() {
            return polygons.Count;
        }

        public Vertex getVertex(int x) {
            return (Vertex)vertices[x];
        }

        public int indexOf(Vertex vertex) {
            return vertices.IndexOf(vertex);
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

        public ArrayList getVertices() {
            return vertices;
        }

        public ArrayList getPolygons() {
            return polygons;
        }

        public void printInfo() {
            Console.WriteLine("Vertices");
            foreach (Vertex v in vertices) {
                v.printInfo();
            }
            Console.WriteLine("Faces");
            foreach (Polygon p in polygons) {
                p.printInfo();
            }
        }
    }

}
