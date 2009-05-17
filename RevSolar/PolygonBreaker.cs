using System;
using System.Collections;
using System.IO;

namespace test {
    public class PolygonBreaker {

        private ArrayList newPolygons;  // new polygons created during the split
        private ArrayList vert1;  // polyVertices for polygon1
        private ArrayList vert2;  // polyVertices for polygon2
        private ArrayList vert3;  // polyVertices for polygon3
        private ArrayList vert4;  // polyVertices for polygon4  
        private ArrayList vert5;  // polyVertices for polygon5
        private ArrayList vert6;  // polyVertices for polygon6
        private ArrayList polyVertices;
        private ArrayList objectVertices;
        private Segment segmentIntersection;
        private Segment segment;
        private Vertex intersectPoint;
        private Vector direction;
        private Vertex newStartVertex;      // starting vertex of the segmentIntersection
        private Vertex newEndVertex;        // ending vertex of the segmentIntersection
        private Vertex foundStartVertex;    // reference to starting vertex if found in the object Vertices
        private Vertex foundEndVertex;      // reference to ending vertex if found in the object Vertices

        public PolygonBreaker(ArrayList objectVertices, ArrayList polyVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {
            newPolygons = new ArrayList();
            vert1 = new ArrayList();
            vert2 = new ArrayList();
            vert3 = new ArrayList();
            vert4 = new ArrayList();
            vert5 = new ArrayList();
            vert6 = new ArrayList();
            this.objectVertices = objectVertices;
            this.polyVertices = polyVertices;
            this.segmentIntersection = segmentIntersection;
            this.segment = segment;
            this.intersectPoint = intersectPoint;
            this.direction = direction;
            newStartVertex = segmentIntersection.getStartVertex();
            newEndVertex = segmentIntersection.getEndVertex();
            findVertices();
        }

        private void findVertices() {
            if (segmentIntersection.getEndVertex().Equals(new Vertex(3, 2, 2)) || segmentIntersection.getStartVertex().Equals(new Vertex(3, 2, 2))) {
                int blah = 0;
            }
            foundStartVertex = Vertex.findVertex(newStartVertex, objectVertices);
            foundEndVertex = Vertex.findVertex(newEndVertex, objectVertices);
        }

        public ArrayList splitVEE() {

            // if vertice doesn't already exist, then add it to the list of polyVertices and mark as boundary vertex.
            if (foundEndVertex == null) {
                objectVertices.Add(newEndVertex);
                newEndVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newEndVertex = foundEndVertex;
            }
            ((Vertex)objectVertices[segmentIntersection.getStartPoint()]).setState(Vertex.ON_BOUNDARY);

            // start index should always be equal to end index
            for (int x = 0; x < polyVertices.Count; x++) {
                if (x == mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint())) {
                    vert1.Add(newEndVertex);
                    if (x - 1 < 0) {
                        vert2 = createTriangle((Vertex)polyVertices[polyVertices.Count - 1], (Vertex)polyVertices[x], newEndVertex);
                    }
                    else {
                        vert2 = createTriangle((Vertex)polyVertices[x - 1], (Vertex)polyVertices[x], newEndVertex);
                    }
                }
                else {
                    vert1.Add((Vertex)polyVertices[x]);
                }
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEEV() {

            if (foundStartVertex == null) {
                objectVertices.Add(newStartVertex);
                newStartVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newStartVertex = foundStartVertex;
            }
            ((Vertex)objectVertices[segmentIntersection.getEndPoint()]).setState(Vertex.ON_BOUNDARY);

            // start index should always be one less than end index
            for (int x = 0; x < polyVertices.Count; x++) {
                if (x == mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint())) {
                    vert1.Add(newStartVertex);
                    if (x < polyVertices.Count - 1) {
                        vert2 = createTriangle(newStartVertex, (Vertex)polyVertices[x], (Vertex)polyVertices[x + 1]);
                    }
                    else {
                        vert2 = createTriangle(newStartVertex, (Vertex)polyVertices[x], (Vertex)polyVertices[0]);
                    }
                }
                else {
                    vert1.Add((Vertex)polyVertices[x]);
                }
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEEE() {

            // if vertice doesn't exist, then add it to the list of polyVertices and mark as boundary vertex
            if (foundStartVertex == null) {
                objectVertices.Add(newStartVertex);
                newStartVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newStartVertex = foundStartVertex;
            }

            if (foundEndVertex == null) {
                objectVertices.Add(newEndVertex);
                newEndVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newEndVertex = foundEndVertex;
            }

            // Subdivide original polygon into multiple polygons
            int startIndex = 0;
            int endIndex = 0;

            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1 >= polyVertices.Count) {
                startIndex = 0;
            }
            else {
                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
            }

            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                endIndex = polyVertices.Count - 1;
            }
            else {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1;
            }

            // create Poly1 (rightmost polygon)
            vert1.Add(newEndVertex);
            vert1.AddRange(makeList(startIndex, endIndex, polyVertices));

            // create Poly2 (leftmost polygon)
            vert2 = createTriangle((Vertex)polyVertices[mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint())], newStartVertex, (Vertex)polyVertices[endIndex]);

            // create Poly3 (middle polygon created if the two new points are not equal)
            if (!newStartVertex.Equals(newEndVertex)) {
                vert3 = createTriangle(newStartVertex, newEndVertex, (Vertex)polyVertices[endIndex]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitVFV() {

            // Mark end polyVertices as boundary
            ((Vertex)objectVertices[segmentIntersection.getStartPoint()]).setState(Vertex.ON_BOUNDARY);
            ((Vertex)objectVertices[segmentIntersection.getEndPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons
            for (int x = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()); x <= mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()); x++) {
                vert1.Add((Vertex)polyVertices[x]);
            }

            for (int x = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()); x != mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1; x++) {
                if (x == polyVertices.Count) {
                    x = 0;
                }
                vert2.Add((Vertex)polyVertices[x]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitVFE() {

            // if vertice doesn't exist, then add it to the list of polyVertices and mark as boundary vertex
            if (foundEndVertex == null) {
                objectVertices.Add(newEndVertex);
                newEndVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newEndVertex = foundEndVertex;
            }

            ((Vertex)objectVertices[segmentIntersection.getStartPoint()]).setState(Vertex.ON_BOUNDARY);

            int startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
            int endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
            vert1.AddRange(makeList(startIndex, endIndex, polyVertices));
            vert1.Add(newEndVertex);

            if (endIndex + 1 >= polyVertices.Count) {
                startIndex = 0;
            }
            else {
                startIndex = endIndex + 1;
            }
            endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());

            vert2.Add(newEndVertex);
            vert2.AddRange(makeList(startIndex, endIndex, polyVertices));
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEFV() {

            // if vertice doesn't exist, then add it to the list of polyVertices and mark as boundary vertex
            if (foundStartVertex == null) {
                objectVertices.Add(newStartVertex);
                newStartVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newStartVertex = foundStartVertex;
            }
            ((Vertex)objectVertices[segmentIntersection.getEndPoint()]).setState(Vertex.ON_BOUNDARY);

            int startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
            int endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
            vert1.Add(newStartVertex);
            vert1.AddRange(makeList(startIndex, endIndex, polyVertices));

            startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
            endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
            vert2.Add(newStartVertex);
            vert2.AddRange(makeList(startIndex, endIndex, polyVertices));
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitVFF() {

            //add it to the list of polyVertices and mark as boundary vertex
            objectVertices.Add(newEndVertex);
            newEndVertex.setState(Vertex.ON_BOUNDARY);
            ((Vertex)objectVertices[segmentIntersection.getStartPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons

            int startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
            int endIndex = 0;

            if (segment.getEndDescriptor() == Segment.EDGE) {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
            }
            else {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) - 1;
            }

            // Create first polygon.  BottomLeft polygon
            vert1.AddRange(makeList(startIndex, endIndex, polyVertices));
            vert1.Add(newEndVertex);

            // create 2nd polygon.  Topleft polygon
            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 == polyVertices.Count) {
                startIndex = 0;
            }
            else {
                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
            }
            endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
            vert2.Add(newEndVertex);
            vert2.AddRange(makeList(startIndex, endIndex, polyVertices));

            // create 3rd polygon.  rightmost polygon if right is edge. topRight polygon if right is vertex.
            startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 == polyVertices.Count) {
                endIndex = 0;
            }
            else {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
            }

            vert3 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);

            // create 4th polygon if the rightmost intersection is a vertex.  4th polygon is bottomRight polygon
            if (segment.getEndDescriptor() == Segment.VERTEX) {
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) - 1 < 0) {
                    startIndex = polyVertices.Count - 1;
                }
                else {
                    startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) - 1;
                }
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
                vert4 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitFFV() {

            //add it to the list of polyVertices and mark as boundary vertex
            objectVertices.Add(newStartVertex);
            newStartVertex.setState(Vertex.ON_BOUNDARY);
            ((Vertex)objectVertices[segmentIntersection.getEndPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons
            int startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
            int endIndex = 0;

            if (segment.getStartDescriptor() == Segment.EDGE) {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
            }
            else {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1;
            }

            vert1.AddRange(makeList(startIndex, endIndex, polyVertices));
            vert1.Add(newStartVertex);

            // create 2nd polygon.  Topleft polygon
            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1 == polyVertices.Count) {
                startIndex = 0;
            }
            else {
                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
            }
            endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
            vert2.Add(newStartVertex);
            vert2.AddRange(makeList(startIndex, endIndex, polyVertices));

            // create 3rd polygon.  rightmost polygon if right is edge. topRight polygon if right is vertex.
            startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1 == polyVertices.Count) {
                endIndex = 0;
            }
            else {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
            }
            vert3 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);

            // create 4th polygon if the rightmost intersection is a vertex.  4th polygon is bottomRight polygon
            if (segment.getEndDescriptor() == Segment.VERTEX) {

                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    startIndex = polyVertices.Count - 1;
                }
                else {
                    startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1;
                }
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                vert4 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEFE() {

            // if vertice doesn't exist, then add it to the list of polyVertices and mark as boundary vertex
            if (foundStartVertex == null) {
                objectVertices.Add(newStartVertex);
                newStartVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newStartVertex = foundStartVertex;
            }

            if (foundEndVertex == null) {
                objectVertices.Add(newEndVertex);
                newEndVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newEndVertex = foundEndVertex;
            }

            // Subdivide original polygon into multiple polygons
            int startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
            int endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());

            // create Poly1
            vert1.AddRange(makeList(startIndex, endIndex, polyVertices));
            vert1.Add(newEndVertex);
            vert1.Add(newStartVertex);

            // create Poly2
            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 >= polyVertices.Count) {
                startIndex = 0;
            }
            else {
                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
            }
            endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
            
            vert2.AddRange(makeList(startIndex, endIndex, polyVertices));
            vert2.Add(newStartVertex);
            vert2.Add(newEndVertex);

            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEFF() {

            // if vertice doesn't exist, then add it to the list of polyVertices and mark as boundary vertex
            if (foundStartVertex == null) {
                objectVertices.Add(newStartVertex);
                newStartVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newStartVertex = foundStartVertex;
            }

            //add it to the list of polyVertices and mark as boundary vertex
            objectVertices.Add(newEndVertex);
            newEndVertex.setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons
            int startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
            int endIndex = 0;

            if (segment.getEndDescriptor() == Segment.EDGE) {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
            }
            else {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) - 1;
            }

            vert1.AddRange(makeList(startIndex, endIndex, polyVertices));
            vert1.Add(newEndVertex);
            vert1.Add(newStartVertex);

            // create 2nd polygon.  topLeft polygon
            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 == polyVertices.Count) {
                startIndex = 0;
            }
            else {
                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
            }
            endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());

            vert2.AddRange(makeList(startIndex, endIndex, polyVertices));
            vert2.Add(newStartVertex);
            vert2.Add(newEndVertex);

            // create 3rd polygon.  rightmost polygon if right is edge. topRight polygon if right is vertex.
            startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 == polyVertices.Count) {
                endIndex = 0;
            }
            else {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
            }
            vert3 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);

            // create 4th polygon if the rightmost intersection is a vertex.  4th polygon is topright polygon
            if (segment.getEndDescriptor() == Segment.VERTEX) {
                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) - 1;
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
                vert4 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitFFE() {

            // if vertice doesn't exist, then add it to the list of polyVertices and mark as boundary vertex
            if (foundEndVertex == null) {
                objectVertices.Add(newEndVertex);
                newEndVertex.setState(Vertex.ON_BOUNDARY);
            }
            else {
                newEndVertex = foundEndVertex;
            }

            //add it to the list of polyVertices and mark as boundary vertex
            objectVertices.Add(newStartVertex);
            newStartVertex.setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons

            int startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
            int endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());

            // Create first polygon.  bottomRight polygon
            vert1.AddRange(makeList(startIndex, endIndex, polyVertices));
            vert1.Add(newEndVertex);
            vert1.Add(newStartVertex);

            // create 2nd polygon.  topRight polygon
            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 >= polyVertices.Count) {
                startIndex = 0;
            }
            else {
                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
            }

            if (segment.getStartDescriptor() == Segment.EDGE) {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
            }
            else {
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    startIndex = polyVertices.Count;
                }
                else {
                    startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                }
            }

            vert2.AddRange(makeList(startIndex, endIndex, polyVertices));
            vert2.Add(newStartVertex);
            vert2.Add(newEndVertex);

            // create 3rd polygon.  leftmost polygon if right is edge. bottomleft polygon if right is vertex.
            startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
            if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1 >= polyVertices.Count) {
                endIndex = 0;
            }
            else {
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
            }
            vert3 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);

            // create 4th polygon if the leftmost intersection is a vertex.  4th polygon is topleft polygon
            if (segment.getStartDescriptor() == Segment.VERTEX) {
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    startIndex = polyVertices.Count;
                }
                else {
                    startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1;
                }
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                vert4 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitFFF() {

            if (foundStartVertex == null) {
                newStartVertex.setState(Vertex.ON_BOUNDARY);
                objectVertices.Add(newStartVertex);
            }
            else {
                newStartVertex = foundStartVertex;
            }

            if (foundEndVertex == null) {
                newEndVertex.setState(Vertex.ON_BOUNDARY);
                objectVertices.Add(newEndVertex);
            }
            else {
                newEndVertex = foundEndVertex;
            }
            // Subdivide original polygon into multiple polygons
            int startIndex = 0;
            int endIndex = 0;

            // 4 cases where segment1 start/endpoints can be VV, VE, EV, or EE
            if (segment.getStartDescriptor() == Segment.VERTEX && segment.getEndDescriptor() == Segment.VERTEX) {

                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1 >= polyVertices.Count) {
                    endIndex = 0;
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
                }
                vert1 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);

                /////////////////////////////////

                startIndex = endIndex;
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) - 1 < 0) {
                    endIndex = mapIndex(objectVertices, polyVertices, polyVertices.Count - 1);
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) - 1;
                }
                vert2.Add(newEndVertex);
                vert2.Add(newStartVertex);
                vert2.AddRange(makeList(startIndex, endIndex, polyVertices));

                //////////////////

                startIndex = endIndex;
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
                vert3 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);

                /////////////

                startIndex = endIndex;
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 >= polyVertices.Count) {
                    endIndex = 0;
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
                }
                vert4 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
                ////////////////////////

                startIndex = endIndex;
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint() - 1) < 0) {
                    endIndex = mapIndex(objectVertices, polyVertices, polyVertices.Count - 1);
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1;
                }
                vert5.Add(newStartVertex);
                vert5.Add(newEndVertex);
                vert5.AddRange(makeList(startIndex, endIndex, polyVertices));

                ///////////////
                startIndex = endIndex;
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                vert6 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
            }
            else if (segment.getStartDescriptor() == Segment.VERTEX && segment.getEndDescriptor() == Segment.EDGE) {

                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1 >= polyVertices.Count) {
                    endIndex = 0;
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
                }
                vert1 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);

                /////////////////////////////////

                startIndex = endIndex;
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
                vert2.Add(newEndVertex);
                vert2.Add(newStartVertex);
                vert2.AddRange(makeList(startIndex, endIndex, polyVertices));

                //////////////////
                startIndex = endIndex;
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 >= polyVertices.Count) {
                    endIndex = 0;
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
                }
                vert3 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
                ////////////////////////

                startIndex = endIndex;
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    endIndex = polyVertices.Count - 1;
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1;
                }
                vert4.Add(newStartVertex);
                vert4.Add(newEndVertex);
                vert4.AddRange(makeList(startIndex, endIndex, polyVertices));

                ///////////////

                startIndex = endIndex;
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                vert5 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
            }
            else if (segment.getStartDescriptor() == Segment.EDGE && segment.getEndDescriptor() == Segment.VERTEX) {

                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1 >= polyVertices.Count) {
                    endIndex = 0;
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
                }
                vert1 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
                /////////////////////////////////

                startIndex = endIndex;
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    endIndex = mapIndex(objectVertices, polyVertices, polyVertices.Count - 1);
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) - 1;
                }
                vert2.Add(newEndVertex);
                vert2.Add(newStartVertex);
                vert2.AddRange(makeList(startIndex, endIndex, polyVertices));

                //////////////////

                startIndex = endIndex;
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
                vert3 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
                /////////////

                startIndex = endIndex;
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 >= polyVertices.Count) {
                    endIndex = 0;
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
                }
                vert4 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
                ////////////////////////
                startIndex = endIndex;
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                vert5.Add(newStartVertex);
                vert5.Add(newEndVertex);
                vert5.AddRange(makeList(startIndex, endIndex, polyVertices));
            }
            else if (segment.getStartDescriptor() == Segment.EDGE && segment.getEndDescriptor() == Segment.EDGE) {

                startIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1 >= polyVertices.Count) {
                    endIndex = 0;
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint()) + 1;
                }
                vert1 = createTriangle(newStartVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);
                /////////////////////////////////

                startIndex = endIndex;
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint());
                vert2.Add(newEndVertex);
                vert2.Add(newStartVertex);
                vert2.AddRange(makeList(startIndex, endIndex, polyVertices));

                //////////////////

                startIndex = endIndex;
                if (mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1 >= polyVertices.Count) {
                    endIndex = 0;
                }
                else {
                    endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getEndPoint()) + 1;
                }
                vert3 = createTriangle(newEndVertex, (Vertex)polyVertices[startIndex], (Vertex)polyVertices[endIndex]);

                /////////////
                startIndex = endIndex;
                endIndex = mapIndex(objectVertices, polyVertices, segmentIntersection.getStartPoint());
                vert4.Add(newStartVertex);
                vert4.Add(newEndVertex);
                vert4.AddRange(makeList(startIndex, endIndex, polyVertices));
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        // Maps the vertex index from list1 to list 2. list1[index] -> list2[index2]
        public static int mapIndex(ArrayList list1, ArrayList list2, int index) {
            return list2.IndexOf((Vertex)list1[index]);
        }

        /*
        public void reset(ArrayList tempVertices) {
            newStartVertex = null;
            newEndVertex = null;
            newPolygons.Clear();
            vert1 = new ArrayList();
            vert2 = new ArrayList();
            vert3 = new ArrayList();
            vert4 = new ArrayList();
            vert5 = new ArrayList();
            vert6 = new ArrayList();
            polyVertices = tempVertices;
        }*/

        // returns a list of polyVertices starting with the startindex and ending with endindex using polyVertices list
        public ArrayList makeList(int startIndex, int endIndex, ArrayList polyVertices) {
            ArrayList tempVertices = new ArrayList();
            for (int x = startIndex; x != endIndex; x++) {
                if (x == polyVertices.Count) {
                    if (endIndex == 0) {
                        break;
                    }
                    x = 0;
                }
                tempVertices.Add((Vertex)polyVertices[x]);
            }
            tempVertices.Add((Vertex)polyVertices[endIndex]);
            return tempVertices;
        }

        // returns the arraylist comprised of v1,v2,v3
        public ArrayList createTriangle(Vertex v1, Vertex v2, Vertex v3) {
            ArrayList triangle = new ArrayList();
            triangle.Add(v1);
            triangle.Add(v2);
            triangle.Add(v3);
            return triangle;
        }

        // returns list of newPolygons
        public ArrayList createNewPolygons(ArrayList v1, ArrayList v2, ArrayList v3, ArrayList v4, ArrayList v5, ArrayList v6) {
            ArrayList verticeLists = new ArrayList();
            if (v1.Count > 2) verticeLists.Add(v1);
            if (v2.Count > 2) verticeLists.Add(v2);
            if (v3.Count > 2) verticeLists.Add(v3);
            if (v4.Count > 2) verticeLists.Add(v4);
            if (v5.Count > 2) verticeLists.Add(v5);
            if (v6.Count > 2) verticeLists.Add(v6);
            foreach (ArrayList verticeList in verticeLists) {
                newPolygons.Add(new Polygon(verticeList));
            }
            return newPolygons;
        }

        public void setVertices(ArrayList tempVertices) {
            polyVertices = tempVertices;
        }
    }
}
