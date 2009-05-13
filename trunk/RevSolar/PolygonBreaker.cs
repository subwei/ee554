using System;
using System.Collections;
using System.IO;

namespace test {
    public class PolygonBreaker {

        private Vertex newVertex1;
        private Vertex newVertex2;
        private ArrayList newPolygons;
        private ArrayList vert1;  // vertices for polygon1
        private ArrayList vert2;  // vertices for polygon2
        private ArrayList vert3;  // vertices for polygon3
        private ArrayList vert4;  // vertices for polygon4  
        private ArrayList vert5;  // vertices for polygon5
        private ArrayList vert6;  // vertices for polygon6
        private ArrayList vertices;

        public PolygonBreaker(ArrayList tempVertices) {
            newVertex1 = null;
            newVertex2 = null;
            newPolygons = new ArrayList();
            vert1 = new ArrayList();
            vert2 = new ArrayList();
            vert3 = new ArrayList();
            vert4 = new ArrayList();
            vert5 = new ArrayList();
            vert6 = new ArrayList();
            vertices = tempVertices;
        }

        public ArrayList splitVEE(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            /* This fixes the situation where if the segment is swapped, the edge vertex will not be the vertice
             * before the actual edge.  There should be only 2 cases.  start and end point are the same, or
             * start and end point are at 0 and count -1.  If segmentIntersection is something other than this case,
             * make the endpoint = startpoint.  (in failing case, the endpoint was startpoint + 1 b/c of the swap)
             */
            if (!((mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) == mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) ||
                  (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) == 0 && mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) == vertices.Count - 1))) {
                segmentIntersection.setEndPoint(segmentIntersection.getStartPoint());
            }

            // Need to calculate where new vertices appear.  Then add to the vertices list of the building
            //newVertex1 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getEndDistance(), segmentIntersection.getEndPoint());
            newVertex1 = segmentIntersection.getEndVertex();

            // if vertice doesn't already exist, then add it to the list of vertices and mark as boundary vertex.
            if (Vertex.findVertex(newVertex1, vertices) == null) {
                objectVertices.Add(newVertex1);
                newVertex1.setState(Vertex.ON_BOUNDARY);
            }
            ((Vertex)objectVertices[segmentIntersection.getStartPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons
            // Reglar case where endpoint and startpoint are the same
            if (segmentIntersection.getStartPoint() == segmentIntersection.getEndPoint()) {
                for (int x = 0; x < vertices.Count; x++) {
                    if (x == mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                        vert1.Add(newVertex1);
                        if (x - 1 < 0) {
                            vert2.Add((Vertex)vertices[vertices.Count - 1]);
                        }
                        else {
                            vert2.Add((Vertex)vertices[x - 1]);
                        }
                        vert2.Add((Vertex)vertices[x]);
                        vert2.Add(newVertex1);
                    }
                    else {
                        vert1.Add((Vertex)vertices[x]);
                    }
                }
            }
            else {
                // odd case where endpoint is at the last index
                for (int x = 0; x < vertices.Count; x++) {
                    if (x == mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                        vert1.Add(newVertex1);
                        vert2.Add(newVertex1);
                        vert2.Add((Vertex)vertices[x - 1]);
                        vert2.Add((Vertex)vertices[x]);
                    }
                    else {
                        vert1.Add((Vertex)vertices[x]);
                    }
                }
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEEV(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            // takes care of anomaly case where the intersecting segment is between the last vertice and first vertice
            if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) == 0 &&
                mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) == vertices.Count - 1) {
                segmentIntersection.setStartPoint(segmentIntersection.getEndPoint());
            }

            // Need to calculate where new vertices appear.  Then add to the vertices list of the building
            //newVertex1 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getStartDistance(), segmentIntersection.getStartPoint());
            newVertex1 = segmentIntersection.getStartVertex();

            // if vertice doesn't already exist, then add it to the list of vertices and mark as boundary vertex.
            if (Vertex.findVertex(newVertex1, vertices) == null) {
                objectVertices.Add(newVertex1);
                newVertex1.setState(Vertex.ON_BOUNDARY);
            }
            ((Vertex)objectVertices[segmentIntersection.getEndPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons
            // Need different case if endpoint and startpoint are the same
            if (segmentIntersection.getStartPoint() == segmentIntersection.getEndPoint()) {
                for (int x = 0; x < vertices.Count; x++) {
                    if (x < mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) || x > mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                        vert1.Add((Vertex)vertices[x]);
                    }
                    if (x == mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                        vert1.Add(newVertex1);
                        if (x - 1 < 0) {
                            vert2.Add((Vertex)vertices[vertices.Count - 1]);
                        }
                        else {
                            vert2.Add((Vertex)vertices[x - 1]);
                        }
                        vert2.Add((Vertex)vertices[x]);
                        vert2.Add(newVertex1);
                    }
                }
            }
            else {
                for (int x = 0; x < vertices.Count; x++) {
                    if (x == mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                        vert1.Add(newVertex1);
                        vert2.Add(newVertex1);
                        vert2.Add((Vertex)vertices[x]);
                        if (x < vertices.Count - 1) {
                            vert2.Add((Vertex)vertices[x + 1]);
                        }
                        else {
                            vert2.Add((Vertex)vertices[0]);
                        }
                    }
                    else {
                        vert1.Add((Vertex)vertices[x]);
                    }
                }
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitVFV(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            // Mark end vertices as boundary
            ((Vertex)objectVertices[segmentIntersection.getStartPoint()]).setState(Vertex.ON_BOUNDARY);
            ((Vertex)objectVertices[segmentIntersection.getEndPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons

            for (int x = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()); x <= mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()); x++) {
                vert1.Add((Vertex)vertices[x]);
            }

            for (int x = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()); x != mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1; x++) {
                if (x == vertices.Count) {
                    x = 0;
                }
                vert2.Add((Vertex)vertices[x]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitVFE(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            // Need to calculate where new vertices appear.  Then add to the vertices list of the building
            //newVertex1 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getEndDistance(), segmentIntersection.getEndPoint());
            newVertex1 = segmentIntersection.getEndVertex();

            // if vertice doesn't exist, then add it to the list of vertices and mark as boundary vertex
            if (Vertex.findVertex(newVertex1, vertices) == null) {
                objectVertices.Add(newVertex1);
                newVertex1.setState(Vertex.ON_BOUNDARY);
            }
            ((Vertex)objectVertices[segmentIntersection.getStartPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons

            for (int x = 0; x < vertices.Count; x++) {
                // conditions to create Poly1
                if (x >= mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) &&
                    x < mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                    vert1.Add((Vertex)vertices[x]);
                }

                if (x == mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                    vert1.Add((Vertex)vertices[x]);
                    vert1.Add(newVertex1);
                    vert2.Add(newVertex1);
                }

                // conditions to create Poly2
                if (x <= mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) &&
                    x > mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                    vert2.Add((Vertex)vertices[x]);
                }
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEFV(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            // Need to calculate where new vertices appear.  Then add to the vertices list of the building
            //newVertex1 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getStartDistance(), segmentIntersection.getStartPoint());
            newVertex1 = segmentIntersection.getStartVertex();

            // if vertice doesn't exist, then add it to the list of vertices and mark as boundary vertex
            if (Vertex.findVertex(newVertex1, vertices) == null) {
                objectVertices.Add(newVertex1);
                newVertex1.setState(Vertex.ON_BOUNDARY);
            }
            ((Vertex)objectVertices[segmentIntersection.getEndPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons

            for (int x = 0; x < vertices.Count; x++) {
                // conditions to create Poly1
                if (x > mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) &&
                    x <= mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                    vert1.Add((Vertex)vertices[x]);
                }

                if (x == mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint())) {
                    //vert1.Add((Vertex)vertices[x]);
                    vert1.Add(newVertex1);
                    vert2.Add((Vertex)vertices[x]);
                    vert2.Add(newVertex1);
                }

                // conditions to create Poly2
                if (x < mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) ||
                    x >= mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint())) {
                    vert2.Add((Vertex)vertices[x]);
                }
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitVFF(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            // Need to calculate where new vertice appears.  Then add to the vertices list of the building
            //newVertex1 = calcVertexInPlane(intersectPoint, direction, segmentIntersection.getEndDistance(), segmentIntersection.getEndPoint());
            newVertex1 = segmentIntersection.getEndVertex();

            //add it to the list of vertices and mark as boundary vertex
            objectVertices.Add(newVertex1);
            newVertex1.setState(Vertex.ON_BOUNDARY);
            ((Vertex)objectVertices[segmentIntersection.getStartPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons

            int startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
            int endValue = 0;

            if (segment.getEndDescriptor() == Segment.EDGE) {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());
            }
            else {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) - 1;
            }

            // Create first polygon.  BottomLeft polygon
            for (int x = startValue; x != endValue; x++) {
                if (x == vertices.Count) {
                    if (endValue == 0) {
                        break;
                    }
                    x = 0;
                }
                vert1.Add((Vertex)vertices[x]);
            }
            vert1.Add((Vertex)vertices[endValue]);
            vert1.Add(newVertex1);

            // create 2nd polygon.  Topleft polygon
            if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 == vertices.Count) {
                startValue = 0;
            }
            else {
                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
            }
            endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
            vert2.Add(newVertex1);

            for (int x = startValue; x != endValue; x++) {
                if (x == vertices.Count) {
                    if (endValue == 0) {
                        break;
                    }
                    x = 0;
                }
                vert2.Add((Vertex)vertices[x]);
            }
            vert2.Add((Vertex)vertices[endValue]);

            // create 3rd polygon.  rightmost polygon if right is edge. topRight polygon if right is vertex.
            startValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());
            if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 == vertices.Count) {
                endValue = 0;
            }
            else {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
            }

            vert3.Add(newVertex1);
            vert3.Add((Vertex)vertices[startValue]);
            vert3.Add((Vertex)vertices[endValue]);

            // create 4th polygon if the rightmost intersection is a vertex.  4th polygon is bottomRight polygon
            if (segment.getEndDescriptor() == Segment.EDGE) {

                if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) - 1 < 0) {
                    startValue = vertices.Count - 1;
                }
                else {
                    startValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) - 1;
                }
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());

                vert4.Add(newVertex1);
                vert4.Add((Vertex)vertices[startValue]);
                vert4.Add((Vertex)vertices[endValue]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitFFV(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            // Need to calculate where new vertice appears.  Then add to the vertices list of the building
            //newVertex1 = calcVertexInPlane(intersectPoint, direction, segmentIntersection.getStartDistance(), segmentIntersection.getStartPoint());
            newVertex1 = segmentIntersection.getStartVertex();

            //add it to the list of vertices and mark as boundary vertex
            objectVertices.Add(newVertex1);
            newVertex1.setState(Vertex.ON_BOUNDARY);
            ((Vertex)objectVertices[segmentIntersection.getEndPoint()]).setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons

            int startValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());
            int endValue = 0;

            if (segment.getStartDescriptor() == Segment.EDGE) {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
            }
            else {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1;
            }

            // Create first polygon.  BottomLeft polygon
            for (int x = startValue; x != endValue; x++) {
                if (x == vertices.Count) {
                    if (endValue == 0) {
                        break;
                    }
                    x = 0;
                }
                vert1.Add((Vertex)vertices[x]);
            }
            vert1.Add((Vertex)vertices[endValue]);
            vert1.Add(newVertex1);

            // create 2nd polygon.  Topleft polygon
            if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1 == vertices.Count) {
                startValue = 0;
            }
            else {
                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
            }
            endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());
            vert2.Add(newVertex1);

            for (int x = startValue; x != endValue; x++) {
                if (x == vertices.Count) {
                    if (endValue == 0) {
                        break;
                    }
                    x = 0;
                }
                vert2.Add((Vertex)vertices[x]);
            }
            vert2.Add((Vertex)vertices[endValue]);

            // create 3rd polygon.  rightmost polygon if right is edge. topRight polygon if right is vertex.
            startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
            if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1 == vertices.Count) {
                endValue = 0;
            }
            else {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
            }

            vert3.Add(newVertex1);
            vert3.Add((Vertex)vertices[startValue]);
            vert3.Add((Vertex)vertices[endValue]);

            // create 4th polygon if the rightmost intersection is a vertex.  4th polygon is bottomRight polygon
            if (segment.getEndDescriptor() == Segment.EDGE) {

                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    startValue = vertices.Count - 1;
                }
                else {
                    startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1;
                }
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());

                vert4.Add(newVertex1);
                vert4.Add((Vertex)vertices[startValue]);
                vert4.Add((Vertex)vertices[endValue]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEEE(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            if (segmentIntersection.getStartPoint() != segmentIntersection.getEndPoint()){
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) == 0 && mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) == vertices.Count - 1) {
                    segmentIntersection.swap();
                    segmentIntersection.setEndPoint(segmentIntersection.getStartPoint());
                }
                else {
                    segmentIntersection.swap();
                    segmentIntersection.setEndPoint(segmentIntersection.getStartPoint());
                }
            }

            // Need to calculate where new vertices appear.  Then add to the vertices list of the building
            // For EEE, the newVertex1 should be point that is lower in index than newVertex2
            //newVertex1 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getStartDistance(), segmentIntersection.getStartPoint());
            //newVertex2 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getEndDistance(), segmentIntersection.getEndPoint());
            newVertex1 = segmentIntersection.getStartVertex();
            newVertex2 = segmentIntersection.getEndVertex();

            // if vertice doesn't exist, then add it to the list of vertices and mark as boundary vertex
            if (Vertex.findVertex(newVertex1, vertices) == null) {
                objectVertices.Add(newVertex1);
                newVertex1.setState(Vertex.ON_BOUNDARY);
            }
            if (Vertex.findVertex(newVertex2, vertices) == null) {
                objectVertices.Add(newVertex2);
                newVertex2.setState(Vertex.ON_BOUNDARY);
            }

            // Subdivide original polygon into multiple polygons
            int startValue = 0;
            int endValue = 0;

            if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1 >= vertices.Count) {
                startValue = 0;
            }
            else {
                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
            }

            if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                endValue = vertices.Count - 1;
            }
            else {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1;
            }

            // create Poly1 (rightmost polygon)
            vert1.Add(newVertex2);
            for (int x = startValue; x != endValue; x++) {
                if (x == vertices.Count) {
                    if (endValue == 0) {
                        break;
                    }
                    x = 0;
                }
                vert1.Add((Vertex)vertices[x]);
            }
            vert1.Add((Vertex)vertices[endValue]);

            // create Poly2 (leftmost polygon)
            vert2.Add((Vertex)vertices[mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint())]);
            vert2.Add(newVertex1);
            vert2.Add((Vertex)vertices[endValue]);

            // create Poly3 (middle polygon created if the two new points are not equal)
            if (!newVertex1.Equals(newVertex2)) {
                vert3.Add(newVertex1);
                vert3.Add(newVertex2);
                vert3.Add((Vertex)vertices[endValue]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEFE(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            // Need to calculate where new vertices appear.  Then add to the vertices list of the building
            //newVertex1 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getStartDistance(), segmentIntersection.getStartPoint());
            //newVertex2 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getEndDistance(), segmentIntersection.getEndPoint());
            newVertex1 = segmentIntersection.getStartVertex();
            newVertex2 = segmentIntersection.getEndVertex();

            // if vertice doesn't exist, then add it to the list of vertices and mark as boundary vertex
            if (Vertex.findVertex(newVertex1, vertices) == null) {
                objectVertices.Add(newVertex1);
                newVertex1.setState(Vertex.ON_BOUNDARY);
            }
            if (Vertex.findVertex(newVertex2, vertices) == null) {
                objectVertices.Add(newVertex2);
                newVertex2.setState(Vertex.ON_BOUNDARY);
            }

            // Subdivide original polygon into multiple polygons
            int startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
            int endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());

            // create Poly1
            for (int x = startValue; x <= endValue; x++) {
                vert1.Add((Vertex)vertices[x]);
            }
            vert1.Add(newVertex2);
            vert1.Add(newVertex1);

            // create Poly2
            if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 >= vertices.Count) {
                startValue = 0;
            }
            else {
                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
            }

            endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
            for (int x = startValue; x != endValue; x++) {
                if (x == vertices.Count) {
                    if (endValue == 0) {
                        break;
                    }
                    x = 0;
                }
                vert2.Add((Vertex)vertices[x]);
            }

            vert2.Add(newVertex1);
            vert2.Add(newVertex2);

            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitEFF(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            // Need to calculate where the new vertices appear.  Then add to the vertices list of the building
            //newVertex1 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getStartDistance(), segmentIntersection.getStartPoint());
            //newVertex2 = calcVertexInPlane(intersectPoint, direction, segmentIntersection.getEndDistance(), segmentIntersection.getEndPoint());
            newVertex1 = segmentIntersection.getStartVertex();
            newVertex2 = segmentIntersection.getEndVertex();

            // if vertice doesn't exist, then add it to the list of vertices and mark as boundary vertex
            if (Vertex.findVertex(newVertex1, vertices) == null) {
                objectVertices.Add(newVertex1);
                newVertex1.setState(Vertex.ON_BOUNDARY);
            }
            //add it to the list of vertices and mark as boundary vertex
            objectVertices.Add(newVertex2);
            newVertex2.setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons
            int startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
            int endValue = 0;

            if (segment.getEndDescriptor() == Segment.EDGE) {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());
            }
            else {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) - 1;
            }

            // Create first polygon.  bottomLeft polygon
            for (int x = startValue; x != endValue; x++) {
                if (x == vertices.Count) {
                    if (endValue == 0) {
                        break;
                    }
                    x = 0;
                }
                vert1.Add((Vertex)vertices[x]);
            }
            vert1.Add((Vertex)vertices[endValue]);
            vert1.Add(newVertex2);
            vert1.Add(newVertex1);

            // create 2nd polygon.  topLeft polygon
            if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 == vertices.Count) {
                startValue = 0;
            }
            else {
                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
            }
            endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;


            for (int x = startValue; x != endValue; x++) {
                if (x == vertices.Count) {
                    if (endValue == 0) {
                        break;
                    }
                    x = 0;
                }
                vert2.Add((Vertex)vertices[x]);
            }
            vert2.Add(newVertex1);
            vert2.Add(newVertex2);

            // create 3rd polygon.  rightmost polygon if right is edge. topRight polygon if right is vertex.
            startValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());
            if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 == vertices.Count) {
                endValue = 0;
            }
            else {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
            }

            vert3.Add(newVertex2);
            vert3.Add((Vertex)vertices[startValue]);
            vert3.Add((Vertex)vertices[endValue]);

            // create 4th polygon if the rightmost intersection is a vertex.  4th polygon is topright polygon
            if (segment.getEndDescriptor() == Segment.VERTEX) {
                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) - 1;
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());

                vert4.Add(newVertex2);
                vert4.Add((Vertex)vertices[startValue]);
                vert4.Add((Vertex)vertices[endValue]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitFFE(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            // Need to calculate where the new vertices appear.  Then add to the vertices list of the building
            //newVertex1 = calcVertexOnLine(objectVertices, intersectPoint, direction, segmentIntersection.getEndDistance(), segmentIntersection.getEndPoint());
            //newVertex2 = calcVertexInPlane(intersectPoint, direction, segmentIntersection.getStartDistance(), segmentIntersection.getStartPoint());
            newVertex1 = segmentIntersection.getStartVertex();
            newVertex2 = segmentIntersection.getEndVertex();

            // if vertice doesn't exist, then add it to the list of vertices and mark as boundary vertex
            if (Vertex.findVertex(newVertex2, vertices) == null) {
                objectVertices.Add(newVertex2);
                newVertex2.setState(Vertex.ON_BOUNDARY);
            }

            //add it to the list of vertices and mark as boundary vertex
            objectVertices.Add(newVertex1);
            newVertex1.setState(Vertex.ON_BOUNDARY);

            // Subdivide original polygon into multiple polygons

            int startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
            int endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());

            // Create first polygon.  bottomRight polygon
            for (int x = startValue; x <= endValue; x++) {
                vert1.Add((Vertex)vertices[x]);
            }
            vert1.Add(newVertex2);
            vert1.Add(newVertex1);

            // create 2nd polygon.  topRight polygon
            if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 >= vertices.Count) {
                startValue = 0;
            }
            else {
                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
            }

            if (segment.getStartDescriptor() == Segment.EDGE) {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
            }
            else {
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    startValue = vertices.Count;
                }
                else {
                    startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
                }
            }

            for (int x = startValue; x != endValue; x++) {
                if (x == vertices.Count) {
                    if (endValue == 0) {
                        break;
                    }
                    x = 0;
                }
                vert2.Add((Vertex)vertices[x]);
            }
            vert2.Add((Vertex)vertices[endValue]);
            vert2.Add(newVertex1);
            vert2.Add(newVertex2);

            // create 3rd polygon.  leftmost polygon if right is edge. bottomleft polygon if right is vertex.
            startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
            if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1 >= vertices.Count) {
                endValue = 0;
            }
            else {
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
            }

            vert3.Add(newVertex1);
            vert3.Add((Vertex)vertices[startValue]);
            vert3.Add((Vertex)vertices[endValue]);

            // create 4th polygon if the leftmost intersection is a vertex.  4th polygon is topleft polygon
            if (segment.getStartDescriptor() == Segment.VERTEX) {
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    startValue = vertices.Count;
                }
                else {
                    startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1;
                }
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());

                vert4.Add(newVertex1);
                vert4.Add((Vertex)vertices[startValue]);
                vert4.Add((Vertex)vertices[endValue]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        public ArrayList splitFFF(ArrayList objectVertices, Segment segmentIntersection, Segment segment, Vertex intersectPoint, Vector direction) {

            //newVertex1 = calcVertexInPlane(intersectPoint, direction, segmentIntersection.getStartPoint(), segmentIntersection.getStartPoint());
            //newVertex2 = calcVertexInPlane(intersectPoint, direction, segmentIntersection.getEndDistance(), segmentIntersection.getEndPoint());
            newVertex1 = segmentIntersection.getStartVertex();
            newVertex2 = segmentIntersection.getEndVertex();

            if (Vertex.findVertex(newVertex1, objectVertices) == null) {
                newVertex1.setState(Vertex.ON_BOUNDARY);
                objectVertices.Add(newVertex1);
            }

            if (Vertex.findVertex(newVertex2, objectVertices) == null) {
                newVertex2.setState(Vertex.ON_BOUNDARY);
                objectVertices.Add(newVertex2);
            }

            // Subdivide original polygon into multiple polygons

            int startValue = 0;
            int endValue = 0;

            // 4 cases where segment1 start/endpoints can be VV, VE, EV, or EE
            if (segment.getStartDescriptor() == Segment.VERTEX && segment.getEndDescriptor() == Segment.VERTEX) {

                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1 >= vertices.Count) {
                    endValue = 0;
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
                }

                vert1.Add(newVertex1);
                vert1.Add((Vertex)vertices[startValue]);
                vert1.Add((Vertex)vertices[endValue]);

                /////////////////////////////////

                startValue = endValue;
                if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) - 1 < 0) {
                    endValue = mapIndex(objectVertices, vertices, vertices.Count - 1);
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) - 1;
                }

                vert2.Add(newVertex2);
                vert2.Add(newVertex1);
                for (int x = startValue; x != endValue; x++) {
                    if (x == vertices.Count) {
                        if (endValue == 0) {
                            break;
                        }
                        x = 0;
                    }
                    vert2.Add((Vertex)vertices[x]);
                }
                vert2.Add((Vertex)vertices[endValue]);


                //////////////////

                startValue = endValue;
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());

                vert3.Add(newVertex2);
                vert3.Add((Vertex)vertices[startValue]);
                vert3.Add((Vertex)vertices[endValue]);

                /////////////

                startValue = endValue;
                if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 >= vertices.Count) {
                    endValue = 0;
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
                }

                vert4.Add(newVertex2);
                vert4.Add((Vertex)vertices[startValue]);
                vert4.Add((Vertex)vertices[endValue]);


                ////////////////////////

                startValue = endValue;
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint() - 1) < 0) {
                    endValue = mapIndex(objectVertices, vertices, vertices.Count - 1);
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1;
                }

                vert5.Add(newVertex1);
                vert5.Add(newVertex2);
                for (int x = startValue; x != endValue; x++) {
                    if (x == vertices.Count) {
                        if (endValue == 0) {
                            break;
                        }
                        x = 0;
                    }
                    vert5.Add((Vertex)vertices[x]);
                }
                vert5.Add((Vertex)vertices[endValue]);

                ///////////////

                startValue = endValue;
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());

                vert6.Add(newVertex1);
                vert6.Add((Vertex)vertices[startValue]);
                vert6.Add((Vertex)vertices[endValue]);
            }
            else if (segment.getStartDescriptor() == Segment.VERTEX && segment.getEndDescriptor() == Segment.EDGE) {


                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1 >= vertices.Count) {
                    endValue = 0;
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
                }

                vert1.Add(newVertex1);
                vert1.Add((Vertex)vertices[startValue]);
                vert1.Add((Vertex)vertices[endValue]);

                /////////////////////////////////

                startValue = endValue;
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());

                vert2.Add(newVertex2);
                vert2.Add(newVertex1);
                for (int x = startValue; x != endValue; x++) {
                    if (x == vertices.Count) {
                        if (endValue == 0) {
                            break;
                        }
                        x = 0;
                    }
                    vert2.Add((Vertex)vertices[x]);
                }
                vert2.Add((Vertex)vertices[endValue]);

                //////////////////

                startValue = endValue;
                if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 >= vertices.Count) {
                    endValue = 0;
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
                }

                vert3.Add(newVertex2);
                vert3.Add((Vertex)vertices[startValue]);
                vert3.Add((Vertex)vertices[endValue]);


                ////////////////////////

                startValue = endValue;
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    endValue = mapIndex(objectVertices, vertices, vertices.Count - 1);
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1;
                }

                vert4.Add(newVertex1);
                vert4.Add(newVertex2);
                for (int x = startValue; x != endValue; x++) {
                    if (x == vertices.Count) {
                        if (endValue == 0) {
                            break;
                        }
                        x = 0;
                    }
                    vert4.Add((Vertex)vertices[x]);
                }
                vert4.Add((Vertex)vertices[endValue]);

                ///////////////

                startValue = endValue;
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());

                vert5.Add(newVertex1);
                vert5.Add((Vertex)vertices[startValue]);
                vert5.Add((Vertex)vertices[endValue]);
            }
            else if (segment.getStartDescriptor() == Segment.EDGE && segment.getEndDescriptor() == Segment.VERTEX) {

                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1 >= vertices.Count) {
                    endValue = 0;
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
                }

                vert1.Add(newVertex1);
                vert1.Add((Vertex)vertices[startValue]);
                vert1.Add((Vertex)vertices[endValue]);

                /////////////////////////////////

                startValue = endValue;
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) - 1 < 0) {
                    endValue = mapIndex(objectVertices, vertices, vertices.Count - 1);
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) - 1;
                }

                vert2.Add(newVertex2);
                vert2.Add(newVertex1);
                for (int x = startValue; x != endValue; x++) {
                    if (x == vertices.Count) {
                        if (endValue == 0) {
                            break;
                        }
                        x = 0;
                    }
                    vert2.Add((Vertex)vertices[x]);
                }
                vert2.Add((Vertex)vertices[endValue]);


                //////////////////

                startValue = endValue;
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());

                vert3.Add(newVertex2);
                vert3.Add((Vertex)vertices[startValue]);
                vert3.Add((Vertex)vertices[endValue]);

                /////////////

                startValue = endValue;
                if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 >= vertices.Count) {
                    endValue = 0;
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
                }

                vert4.Add(newVertex2);
                vert4.Add((Vertex)vertices[startValue]);
                vert4.Add((Vertex)vertices[endValue]);


                ////////////////////////

                startValue = endValue;
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());

                vert5.Add(newVertex1);
                vert5.Add(newVertex2);
                for (int x = startValue; x != endValue; x++) {
                    if (x == vertices.Count) {
                        if (endValue == 0) {
                            break;
                        }
                        x = 0;
                    }
                    vert5.Add((Vertex)vertices[x]);
                }
                vert5.Add((Vertex)vertices[endValue]);
            }
            else if (segment.getStartDescriptor() == Segment.EDGE && segment.getEndDescriptor() == Segment.EDGE) {


                startValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());
                if (mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1 >= vertices.Count) {
                    endValue = 0;
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint()) + 1;
                }

                vert1.Add(newVertex1);
                vert1.Add((Vertex)vertices[startValue]);
                vert1.Add((Vertex)vertices[endValue]);

                /////////////////////////////////

                startValue = endValue;
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint());

                vert2.Add(newVertex2);
                vert2.Add(newVertex1);
                for (int x = startValue; x != endValue; x++) {
                    if (x == vertices.Count) {
                        if (endValue == 0) {
                            break;
                        }
                        x = 0;
                    }
                    vert2.Add((Vertex)vertices[x]);
                }
                vert2.Add((Vertex)vertices[endValue]);


                //////////////////

                startValue = endValue;
                if (mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1 >= vertices.Count) {
                    endValue = 0;
                }
                else {
                    endValue = mapIndex(objectVertices, vertices, segmentIntersection.getEndPoint()) + 1;
                }

                vert3.Add(newVertex2);
                vert3.Add((Vertex)vertices[startValue]);
                vert3.Add((Vertex)vertices[endValue]);

                /////////////

                startValue = endValue;
                endValue = mapIndex(objectVertices, vertices, segmentIntersection.getStartPoint());

                vert4.Add(newVertex1);
                vert4.Add(newVertex2);
                for (int x = startValue; x != endValue; x++) {
                    if (x == vertices.Count) {
                        if (endValue == 0) {
                            break;
                        }
                        x = 0;
                    }
                    vert4.Add((Vertex)vertices[x]);
                }
                vert4.Add((Vertex)vertices[endValue]);
            }
            return createNewPolygons(vert1, vert2, vert3, vert4, vert5, vert6);
        }

        // Maps the vertex index from list1 to list 2. list1[index] -> list2[index2]
        public static int mapIndex(ArrayList list1, ArrayList list2, int index) {
            return list2.IndexOf((Vertex)list1[index]);
        }

        public void reset(ArrayList tempVertices) {
            newVertex1 = null;
            newVertex2 = null;
            newPolygons.Clear();
            vert1 = new ArrayList();
            vert2 = new ArrayList();
            vert3 = new ArrayList();
            vert4 = new ArrayList();
            vert5 = new ArrayList();
            vert6 = new ArrayList();
            vertices = tempVertices;
        }

        public ArrayList makeList(int startIndex, int endIndex, ArrayList vertices) {
            ArrayList tempVertices = new ArrayList();
            for (int x = startIndex; x != endIndex; x++) {
                if (x == vertices.Count) {
                    if (endIndex == 0) {
                        break;
                    }
                    x = 0;
                }
                tempVertices.Add((Vertex)vertices[x]);
            }
            tempVertices.Add((Vertex)vertices[endIndex]);
            return tempVertices;
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
            vertices = tempVertices;
        }
    }
}
