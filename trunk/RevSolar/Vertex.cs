using System;
using System.Collections;

namespace test
{
	/// <summary>
	/// Summary description for Vertex.
	/// </summary>
	public class Vertex
	{
		public double[] coords = null;

        private ArrayList adjacentVertices;     // stores all adjacent vertices
        private int state;                      // stores where the vertex is in relation to a buildingNode.  0 = unknown, 1 = inside boundary, 2 = outside boundary, 3 = on the boundary 

        public const int UNKNOWN = -1;
        public const int INSIDE_BOUNDARY = 0;
        public const int OUTSIDE_BOUNDARY = 1;
        public const int ON_BOUNDARY = 2;

		public Vertex():this(0,0,0)
		{
		}

        // copy constructor
        public Vertex(Vertex vertex) {
            adjacentVertices = new ArrayList();
            coords = new double[] { vertex.GetX(), vertex.GetY(), vertex.GetZ()};
            /* DO NOT COPY THE ADJACENCY LIST
            foreach (Vertex old in vertex.adjacentVertices) {
                adjacentVertices.Add(new Vertex(old));
            }
            */
            state = vertex.state;
        }

		public Vertex (double x, double y, double z) 
		{
            coords = new double[] { x, y, z };
            adjacentVertices = new ArrayList();
            state = Vertex.UNKNOWN;
		}

		public double GetX() { return coords[0]; }

		public double GetY() { return coords[1]; }
			
		public double GetZ() { return coords[2]; }

        public double[] GetCoords() { return coords; }

        public void Round()
        {
            coords[0] = Math.Round(coords[0]);
            coords[1] = Math.Round(coords[1]);
            coords[2] = Math.Round(coords[2]);
        }

        public void mark(int state) {
            setState(state);
            // recurively mark all adjacent vertices that are unknown with the state value
            foreach (Vertex vertex in adjacentVertices){
                if (vertex.getState() == Vertex.UNKNOWN) {
                    //Console.WriteLine("{0} {1} {2}", vertex.GetX(), vertex.GetY(), vertex.GetZ());
                    vertex.mark(state);
                }
            }
        }

        // return true if vertex is between v1 and v2
        // ASSUMES VERTEX IS ON A LINE BETWEEN V1 and V2
        public bool isBetween(Vertex v1, Vertex v2) {

            if ((float)v1.GetZ() > (float)v2.GetZ()) {
                if ((float)GetZ() < (float)v2.GetZ() || (float)GetZ() > (float)v1.GetZ()) {
                    return false;
                }
                else {
                    return true;
                }
            }
            else if ((float)v1.GetZ() < (float)v2.GetZ()) {
                if ((float)GetZ() < (float)v1.GetZ() || (float)GetZ() > (float)v2.GetZ()) {
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                if ((float)v1.GetX() > (float)v2.GetX()) {
                    if ((float)GetX() < (float)v2.GetX() || (float)GetX() > (float)v1.GetX()) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                else if ((float)v1.GetX() < (float)v2.GetX()) {
                    if ((float)GetX() < (float)v1.GetX() || (float)GetX() > (float)v2.GetX()) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                else {

                    if ((float)v1.GetY() > (float)v2.GetY()) {
                        if ((float)GetY() < (float)v2.GetY() || (float)GetY() > (float)v1.GetY()) {
                            return false;
                        }
                        else {
                            return true;
                        }
                    }
                    else if ((float)v1.GetY() < (float)v2.GetY()) {
                        if ((float)GetY() < (float)v1.GetY() || (float)GetY() > (float)v2.GetY()) {
                            return false;
                        }
                        else {
                            return true;
                        }
                    }
                    else {
                        return false;
                    }
                }
            }

        }

        public bool isAdjacent(Vertex vertex) {
            return adjacentVertices.Contains(vertex);
        }

        public Vertex getAdjacentVertex(int index) {
            return (Vertex)adjacentVertices[index];
        }

        public ArrayList getAdjacentVertices() {
            return adjacentVertices;
        }

        public int getAdjacentVerticesCount() {
            return adjacentVertices.Count;
        }

        public void addAdjacentVertex(Vertex vertex) {
            adjacentVertices.Add(vertex);
        }

        public int getAdjacentListSize() {
            return adjacentVertices.Count;
        }

        public void clearAdjacentList() {
            adjacentVertices.Clear();
        }

        public double calcDistance(Vertex vertex) {
            double dx = GetX() - vertex.GetX();
            double dy = GetY() - vertex.GetY();
            double dz = GetZ() - vertex.GetZ();
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public bool Equals(Vertex vertex) {
            return ((float)GetX() == (float)vertex.GetX() && (float)GetY() == (float)vertex.GetY() && (float)GetZ() == (float)vertex.GetZ());
        }

        public int getState() {
            return state;
        }

        public void setState(int x) {
            state = x;
            if (this.Equals(new Vertex(2, 2, 2)) && state == OUTSIDE_BOUNDARY) {
                int blah = 0;
            }
        }

        public void reset() {
            clearAdjacentList();
            state = UNKNOWN;
        }

        public bool adjacentExists(Vertex vertex) {
            if (adjacentVertices.IndexOf(vertex) == -1) {
                return false;
            }
            else {
                return true;
            }
        }

        public static Vertex findVertex(Vertex v, ArrayList verticeList) {
            foreach (Vertex vertex in verticeList) {
                if (vertex.Equals(v)) {
                    return vertex;
                }
            }
            return null;
        }

        public void printInfo() {
            Console.Write("{0} {1} {2} ", GetX(), GetY(), GetZ());
            if (state == Vertex.INSIDE_BOUNDARY) {
                Console.WriteLine("INSIDE");
            }
            else if (state == Vertex.ON_BOUNDARY) {
                Console.WriteLine("BOUNDARY");
            }
            else if (state == Vertex.OUTSIDE_BOUNDARY) {
                Console.WriteLine("OUTSIDE");
            }
            else if (state == Vertex.UNKNOWN) {
                Console.WriteLine("UNKNOWN");
            }
            else {
                Console.WriteLine("INVALID STATE");
            }
        }
    }
}


