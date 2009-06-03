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
        private int startPoint;     // index of the startPoint in relation to the object vertices
        private int endPoint;       // index of the endPoint in relation to the object vertices
        private Vertex startVertex;      // start Vertex
        private Vertex endVertex;      // end Vertex
        private bool swapped;     // true if the segment has been swapped
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
            swapped = false;
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
            swapped = segment.swapped;
        }

        // returns segment that overlaps with this segment and segmentB.  vertices should be polyhedral vertices and not polygon
        public Segment calcSegmentIntersection(Segment segmentB, ArrayList objectVertices, ArrayList polyaVertices) {

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

            /* Correct the segmentIntersection for proper polygon splitting
             * Need to have edge points be at the smaller index of the edge points
             */
            if (segmentIntersection.isSwapped()) {
                segmentIntersection.swap();
                swap();
            }

            /* Need to correct for initial poly-line intersection of VEV -> EEV, VEE, EEE
             * This is a corner case where start index is 0 and end index is vertices.count - 1
             */
            if (PolygonBreaker.mapIndex(objectVertices, polyaVertices, segmentIntersection.getStartPoint()) == 0 &&
                PolygonBreaker.mapIndex(objectVertices, polyaVertices, segmentIntersection.getEndPoint()) == polyaVertices.Count - 1){

                // VEV -> VEE, EEE
                if (start == VERTEX && segmentIntersection.getStartDescriptor() == EDGE) {
                    segmentIntersection.setStartPoint(segmentIntersection.getEndPoint());
                    segmentIntersection.swap();
                }
                // VEV -> VEE -> EEV
                else if (end == VERTEX && segmentIntersection.getEndDescriptor() == EDGE) {
                    segmentIntersection.swap();
                }
            }
            else if (end == VERTEX && segmentIntersection.getEndDescriptor() == EDGE){
                int tempIndex = PolygonBreaker.mapIndex(objectVertices, polyaVertices, segmentIntersection.getEndPoint()) - 1;
                int newIndex = objectVertices.IndexOf((Vertex)polyaVertices[tempIndex]);
                segmentIntersection.setEndPoint(newIndex);
            }

            return segmentIntersection;
        }
        
        /* use distances from start and end of each segment to see if they overlap
         * return true if segments overlap
         * ASSUMES STARTPOINT FOR A SEGMENT IS SHORTER DISTANCE THAN ENDPOINT
         */
        public bool overlaps(Segment segmentB) {
            if (segmentB.getEndDistance() < startDistance + Vertex.ZERO_LIMIT || segmentB.getStartDistance() > endDistance + Vertex.ZERO_LIMIT) {
                return false;
            }
            else {
                return true;
            }
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

        
        public bool isSwapped() {
            return swapped;
        }

        public void swap() {
            int temp;
            double tempDistance;
            swapped = !swapped;

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