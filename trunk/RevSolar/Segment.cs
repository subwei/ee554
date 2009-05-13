using System;
using System.Collections;
using System.IO;

namespace test {
    public class Segment {

        private double startDistance;
        private double endDistance;
        private int start;          // start descriptor
        private int middle;         // middle descriptor
        private int end;            // end descriptor
        private int startPoint;
        private int endPoint;

        private Vertex startVertex;      // start Vertex
        private Vertex endVertex;      // end Vertex

        //private bool isSwapped; // true if the segment has been swapped initially

        public const int VERTEX = 1;
        public const int EDGE = 2;
        public const int FACE = 3;

        public Segment() {
            startDistance = 0;
            endDistance = 0;
            start = VERTEX;
            middle = VERTEX;
            end = VERTEX;
            startPoint = 0;
            endPoint = 0;
            //isSwapped = false;
            startVertex = new Vertex();
            endVertex = new Vertex();
        }

        public Segment(Segment segment) {
            startDistance = segment.startDistance;
            endDistance = segment.endDistance;
            start = segment.start;
            middle = segment.middle;
            end = segment.end;
            startPoint = segment.startPoint;
            endPoint = segment.endPoint;
            startVertex = segment.startVertex;
            endVertex = segment.endVertex;
            //isSwapped = segment.isSwapped;
        }

        // returns segment that overlaps with this segment and segmentB.  vertices should be polyhedral vertices and not polygon
        public Segment calcSegmentIntersection(Segment segmentB) {

            /* Don't have to check if the segment intersects here because
             * calling function should have verified segments overlap
             */
            Segment segmentIntersection = new Segment(this);

            //set starting point to the furthest startingPoint		
            if (segmentB.getStartDistance() > startDistance) {
                segmentIntersection.setStartDistance(segmentB.getStartDistance());
                segmentIntersection.setStartDescriptor(segmentIntersection.getMiddleDescriptor());
                //segmentIntersection.setStartPoint(segmentB.getStartPoint());
                segmentIntersection.setStartVertex(segmentB.getStartVertex());
            }
            //set ending point to the closest endingPoint
            if (segmentB.getEndDistance() < endDistance) {
                segmentIntersection.setEndDistance(segmentB.getEndDistance());
                segmentIntersection.setEndDescriptor(segmentIntersection.getMiddleDescriptor());
                //segmentIntersection.setEndPoint(segmentB.getEndPoint());
                segmentIntersection.setEndVertex(segmentB.getEndVertex());
            }

            // if the startingPoint and endingPoint are the same, the segment is VVV
            if (segmentIntersection.getStartVertex().Equals(segmentIntersection.getEndVertex())) {
                segmentIntersection.setStartDescriptor(Segment.VERTEX);
                segmentIntersection.setMiddleDescriptor(Segment.VERTEX);
                segmentIntersection.setEndDescriptor(Segment.VERTEX);
            }

            return segmentIntersection;
        }
        
        /* use distances from start and end of each segment to see if they overlap
         * return true if segments overlap
         * ASSUMES STARTPOINT FOR A SEGMENT IS SHORTER DISTANCE THAN ENDPOINT
         */
        public bool overlaps(Segment segmentB) {
            if (segmentB.getStartDistance() <= endDistance && segmentB.getStartDistance() >= startDistance) {
                return true;
            }
            else {
                return false;
            }

            /*
            // Need to have some way of comparing startDistance and endDistance to see which one is larger
            if (segmentB.getStartDistance() < segmentB.getEndDistance()) {
                if (startDistance >= segmentB.getStartDistance() && startDistance <= segmentB.getEndDistance() ||
                    endDistance >= segmentB.getStartDistance() && endDistance <= segmentB.getEndDistance()) {
                    isOverlap = true;
                }
            }
            else {
                if (startDistance >= segmentB.getStartDistance() && startDistance <= segmentB.getEndDistance() ||
                    endDistance >= segmentB.getEndDistance() && endDistance <= segmentB.getStartDistance()) {
                    isOverlap = true;
                }
            }
            return isOverlap;
             */
        }

        public double getStartDistance() {
            return startDistance;
        }

        public double getEndDistance() {
            return endDistance;
        }

        public int getStartDescriptor() {
            return start;
        }

        public int getMiddleDescriptor() {
            return middle;
        }

        public int getEndDescriptor() {
            return end;
        }

        public int getStartPoint() {
            return startPoint;
        }

        public int getEndPoint() {
            return endPoint;
        }

        public void setStartDistance(double startDistance){
            this.startDistance = startDistance;
        }

        public void setEndDistance(double endDistance) {
            this.endDistance = endDistance;
        }

        public void setStartDescriptor(int start) {
            this.start = start;
        }

        public void setMiddleDescriptor(int middle) {
            this.middle = middle;
        }

        public void setEndDescriptor(int end) {
            this.end = end;
        }

        public void setStartPoint(int startPoint) {
            this.startPoint = startPoint;
        }

        public void setEndPoint(int endPoint) {
            this.endPoint = endPoint;
        }

        public void setStartVertex(Vertex v) {
            startVertex = v;
        }

        public void setEndVertex(Vertex v) {
            endVertex = v;
        }

        public Vertex getStartVertex() {
            return startVertex;
        }

        public Vertex getEndVertex() {
            return endVertex;
        }

        /*
        public bool wasSwapped() {
            return isSwapped;
        }*/

        public void swap() {
            int temp;
            double tempDistance;
            //isSwapped = !isSwapped;

            temp = start;
            start = end;
            end = temp;

            temp = startPoint;
            startPoint = endPoint;
            endPoint = temp;

            tempDistance = startDistance;
            startDistance = endDistance;
            endDistance = tempDistance;

            Vertex v = startVertex;
            startVertex = endVertex;
            endVertex = v;
        }

        public void printInfo() {
            Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}", startDistance, endDistance, start, middle, end, startPoint, endPoint); 
        }

    }
}





/* OLD calcSegmentIntersection code
            float startB = (float)segmentB.getStartDistance();
            float endB = (float)segmentB.getEndDistance();

            Vertex seg1StartPoint = startVertex;
            Vertex seg1EndPoint = endVertex;
            Vertex seg2StartPoint = segmentB.getStartVertex();
            Vertex seg2EndPoint = segmentB.getEndVertex();
            bool isOpposite = false;  // true if the startPoints for both segments aren't on the same side. i.e. can have B1 E2 E1 B2
            bool isSwap = false;    // if the startpoint is 0  and endpoint is the last vertex, then the ends need to be swapped after startpoint is set to endpoint

            Vertex seg1StartPoint = new Vertex();
            Vertex seg1EndPoint = new Vertex();
            Vertex seg2StartPoint = new Vertex();
            Vertex seg2EndPoint = new Vertex();

            * Need to accurately capture where the startpoints and endpoints are with respect to each segment
             * Vertex points can be used directly, but must calculate EDGE/FACE points with the distance given.
             * Need to perform checks when calculating the intersection point b/c the direction vector and distances
             * might not give the correct result.
             * Also need to use verticesB for segmentB
             *
            if (start == Segment.VERTEX) {
                seg1StartPoint = (Vertex)verticesA[startPoint];
            }
            else{
                seg1StartPoint = new Vertex(intersectPoint.GetX() + startDistance * direction.x,
                                            intersectPoint.GetY() + startDistance * direction.y,
                                            intersectPoint.GetZ() + startDistance * direction.z);

                // check to see the created point is in the polygon
                if (polygonA.encompassesVertex(seg1StartPoint) == Polygon.OUTSIDE){
                    direction = new Vector(0 - direction.x, 0 - direction.y, 0 - direction.z);
                    seg1StartPoint = new Vertex(intersectPoint.GetX() + startDistance * direction.x,
                                                intersectPoint.GetY() + startDistance * direction.y,
                                                intersectPoint.GetZ() + startDistance * direction.z);
                }

            }
            if (end == Segment.VERTEX) {
                seg1EndPoint = (Vertex)verticesA[endPoint];
            }
            else {
                seg1EndPoint = new Vertex(intersectPoint.GetX() + endDistance * direction.x,
                                            intersectPoint.GetY() + endDistance * direction.y,
                                            intersectPoint.GetZ() + endDistance * direction.z);

                if (polygonA.encompassesVertex(seg1EndPoint) == Polygon.OUTSIDE) {
                    direction = new Vector(0 - direction.x, 0 - direction.y, 0 - direction.z);
                    seg1EndPoint = new Vertex(intersectPoint.GetX() + endDistance * direction.x,
                                                intersectPoint.GetY() + endDistance * direction.y,
                                                intersectPoint.GetZ() + endDistance * direction.z);
                }
            }
            if (segmentB.getStartDescriptor() == Segment.VERTEX) {
                seg2StartPoint = (Vertex)verticesB[segmentB.getStartPoint()];
            }
            else {
                seg2StartPoint = new Vertex(intersectPoint.GetX() + segmentB.getStartDistance() * direction.x,
                                            intersectPoint.GetY() + segmentB.getStartDistance() * direction.y,
                                            intersectPoint.GetZ() + segmentB.getStartDistance() * direction.z);

                if (polygonB.encompassesVertex(seg2StartPoint) == Polygon.OUTSIDE) {
                    direction = new Vector(0 - direction.x, 0 - direction.y, 0 - direction.z);
                    seg2StartPoint = new Vertex(intersectPoint.GetX() + segmentB.getStartDistance() * direction.x,
                                                intersectPoint.GetY() + segmentB.getStartDistance() * direction.y,
                                                intersectPoint.GetZ() + segmentB.getStartDistance() * direction.z);
                }
            }
            if (segmentB.getEndDescriptor() == Segment.VERTEX) {
                seg2EndPoint = (Vertex)verticesB[segmentB.getEndPoint()];
            }
            else {
                seg2EndPoint = new Vertex(intersectPoint.GetX() + segmentB.getEndDistance() * direction.x,
                                            intersectPoint.GetY() + segmentB.getEndDistance() * direction.y,
                                            intersectPoint.GetZ() + segmentB.getEndDistance() * direction.z);

                if (polygonB.encompassesVertex(seg2EndPoint) == Polygon.OUTSIDE) {
                    direction = new Vector(0 - direction.x, 0 - direction.y, 0 - direction.z);
                    seg2EndPoint = new Vertex(intersectPoint.GetX() + segmentB.getEndDistance() * direction.x,
                                                intersectPoint.GetY() + segmentB.getEndDistance() * direction.y,
                                                intersectPoint.GetZ() + segmentB.getEndDistance() * direction.z);
                }
            }
             *

            // Use the checkBetween function to figure out where segmentB start/endpoints lie in relation to segment 1
            
            bool seg2StartInBetween = seg2StartPoint.isBetween(seg1StartPoint, seg1EndPoint);
            bool seg2EndInBetween = seg2EndPoint.isBetween(seg1StartPoint, seg1EndPoint);
            bool seg1StartInBetween = seg1StartPoint.isBetween(seg2StartPoint, seg2EndPoint);
            bool seg1EndInBetween = seg1EndPoint.isBetween(seg2StartPoint, seg2EndPoint);

            // if the segmentB end point is close to this segment start point, then the ends have reversed
            if (seg1StartPoint.calcDistance(seg2EndPoint) < seg1StartPoint.calcDistance(seg2StartPoint)) {
                isOpposite = true;
            }

            /* When calculating the segment intersection, don't replace the
             * startPoint and endPoint of segment 1, just modify the descriptors and
             * start/endpoint distances.  This is b/c the start/endpoint for segmentB
             * is an index relative to the shadow vertices, not the building vertices.
             *
            if (seg2StartInBetween && seg2EndInBetween) {   // segmentB is inside segment1

                // segmentB start point is closer to segment1 start point
                if (seg1StartPoint.calcDistance(seg2StartPoint) < seg1StartPoint.calcDistance(seg2EndPoint)) {

                    segmentIntersection.setEndDistance(endB);
                    segmentIntersection.setEndDescriptor(segmentIntersection.getMiddleDescriptor());
                    segmentIntersection.setEndVertex(segmentB.getEndVertex());
                    /* Need to change the endPoint if the middle descriptor is an edge b/c need to have
                     * 1st index of the edge.  Only use on endpoints b/c program assumes startpoint comes before endpoint.
                     * Also, since this middle descriptor only applies to VEV segments, can just set endpoint to startpoint
                     *
                    if (segmentIntersection.getMiddleDescriptor() == Segment.EDGE) {
                        /* This holds true normally, unless startpoint = 0 index and endPoint = count-1 index.
                         * Then the start index needs to be set to the end index
                         *
                        if (segmentIntersection.getStartPoint() == 0 && segmentIntersection.getEndPoint() == polygonA.getVertices().Count - 1) {
                            segmentIntersection.setStartPoint(segmentIntersection.getEndPoint());
                            isSwap = true;
                        }
                        else {
                            segmentIntersection.setEndPoint(segmentIntersection.getStartPoint());
                        }
                    }
                    segmentIntersection.setStartDistance(startB);
                    segmentIntersection.setStartDescriptor(segmentIntersection.getMiddleDescriptor());
                    segmentIntersection.setStartVertex(segmentB.getStartVertex());
                    
                }
                // segmentB end point is closer to segment1 start point
                else {
                    segmentIntersection.setEndDistance(startB);
                    segmentIntersection.setEndDescriptor(segmentIntersection.getMiddleDescriptor());
                    segmentIntersection.setEndVertex(segmentB.getStartVertex());
                    /* Need to change the endPoint if the middle descriptor is an edge b/c need to have
                     * 1st index of the edge.  Only use on endpoints b/c program assumes startpoint comes before endpoint.
                     * Also, since this middle descriptor only applies to VEV segments, can just set endpoint to startpoint
                     *
                    if (segmentIntersection.getMiddleDescriptor() == Segment.EDGE) {
                        /* This holds true normally, unless startpoint = 0 index and endPoint = count-1 index.
                         * Then the start index needs to be set to the end index
                         *
                        if (segmentIntersection.getStartPoint() == 0 && segmentIntersection.getEndPoint() == polygonA.getVertices().Count - 1) {
                            segmentIntersection.setStartPoint(segmentIntersection.getEndPoint());
                            isSwap = true;
                        }
                        else {
                            segmentIntersection.setEndPoint(segmentIntersection.getStartPoint());
                        }
                    }
                    segmentIntersection.setStartDistance(endB);
                    segmentIntersection.setStartDescriptor(segmentIntersection.getMiddleDescriptor());
                    segmentIntersection.setStartVertex(segmentB.getEndVertex());
                }
            }
            // segmentB start point is inside segment1 but not end point
            else if (seg2StartInBetween) {
                if (seg1StartInBetween) {
                    segmentIntersection.setEndDistance(startB);
                    segmentIntersection.setEndDescriptor(segmentIntersection.getMiddleDescriptor());
                    segmentIntersection.setEndVertex(segmentB.getStartVertex());
                    /* Need to change the endPoint if the middle descriptor is an edge b/c need to have
                     * 1st index of the edge.  Only use on endpoints b/c program assumes startpoint comes before endpoint.
                     * Also, since this middle descriptor only applies to VEV segments, can just set endpoint to startpoint
                     *
                    if (segmentIntersection.getMiddleDescriptor() == Segment.EDGE) {
                        /* This holds true normally, unless startpoint = 0 index and endPoint = count-1 index.
                         * Then the start index needs to be set to the end index
                         *
                        if (segmentIntersection.getStartPoint() == 0 && segmentIntersection.getEndPoint() == polygonA.getVertices().Count - 1) {
                            segmentIntersection.setStartPoint(segmentIntersection.getEndPoint());
                            isSwap = true;
                        }
                        else {
                            segmentIntersection.setEndPoint(segmentIntersection.getStartPoint());
                        }
                    }
                }
                else if (seg1EndInBetween) {

                    segmentIntersection.setStartDescriptor(segmentIntersection.getMiddleDescriptor());
                    if (segmentIntersection.getMiddleDescriptor() == Segment.EDGE &&
                        segmentIntersection.getStartPoint() == 0 &&
                        segmentIntersection.getEndPoint() == polygonA.getVertices().Count - 1) {
                        segmentIntersection.setStartPoint(segmentIntersection.getEndPoint());
                        isSwap = true;
                    }
                    segmentIntersection.setStartDistance(startB);
                    segmentIntersection.setStartVertex(segmentB.getStartVertex());
                }
                else {  // This case should not happen.  One of segment1 points should be in between segment 2 or the segments don't overlap.
                    Console.WriteLine("Error in BuildingVRMLNode::calcSegmentIntersection");
                }
            }
            // segmentB end point is inside segment1 but not start point
            else if (seg2EndInBetween) {
                if (seg1StartInBetween) {
                    segmentIntersection.setEndDistance(endB);
                    segmentIntersection.setEndDescriptor(segmentIntersection.getMiddleDescriptor());
                    segmentIntersection.setEndVertex(segmentB.getEndVertex());
                    /* Need to change the endPoint if the middle descriptor is an edge b/c need to have
                     * 1st index of the edge.  Only use on endpoints b/c program assumes startpoint comes before endpoint.
                     * Also, since this middle descriptor only applies to VEV segments, can just set endpoint to startpoint
                     *
                    if (segmentIntersection.getMiddleDescriptor() == Segment.EDGE) {
                        /* This holds true normally, unless startpoint = 0 index and endPoint = count-1 index.
                         * Then the start index needs to be set to the end index
                         *
                        if (segmentIntersection.getStartPoint() == 0 && segmentIntersection.getEndPoint() == polygonA.getVertices().Count - 1) {
                            segmentIntersection.setStartPoint(segmentIntersection.getEndPoint());
                            isSwap = true;
                        }
                        else {
                            segmentIntersection.setEndPoint(segmentIntersection.getStartPoint());
                        }
                    }
                }
                else if (seg1EndInBetween) {

                    segmentIntersection.setStartDescriptor(segmentIntersection.getMiddleDescriptor());
                    if (segmentIntersection.getMiddleDescriptor() == Segment.EDGE &&
                        segmentIntersection.getStartPoint() == 0 &&
                        segmentIntersection.getEndPoint() == polygonA.getVertices().Count - 1) {
                        isSwap = true;
                        segmentIntersection.setStartPoint(segmentIntersection.getEndPoint());
                    }
                    segmentIntersection.setStartDistance(endB);
                    segmentIntersection.setStartVertex(segmentB.getEndVertex());
                }
                else {  // This case should not happen.  One of segment1 points should be in between segment 2 or the segments don't overlap.
                    Console.WriteLine("Error in BuildingVRMLNode::calcSegmentIntersection");
                }
            }
            else {  // no intersection between segments?  Should not happen b/c this function never gets called if segments don't overlap
                Console.WriteLine("Error in BuildingVRMLNode::calcSegmentIntersection");
            }


            if (isSwap) {
                segmentIntersection.swap();
            }*/


