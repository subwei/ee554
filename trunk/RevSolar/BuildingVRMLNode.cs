using System;
using System.Collections;
using System.IO;
//using Tao;
//using Tao.OpenGl;
//using Tao.FreeGlut;
using System.Runtime.InteropServices;

/* BuildingVRMLNode.cs represent one 3D building. */
namespace test {
    /// <summary>
    /// Summary description for BuildingNode.
    /// </summary>
    public class BuildingVRMLNode : Node {

        private ArrayList vertices = null;
        private ArrayList textureCoords = null;
        private int textureId = -1;
        private int displayList = -1;
        private bool displayListExists = false;
        private int displayList2 = -1;
        private bool displayListExists2 = false;
        public string nodeKey = null;
        private string image_URL = null;
        public bool URL_repeat = false; // indicate whether the texture of this building remains the same

        /*jenny update 10.27.06*/
        /* The following variables are for the tempo query for buildings only. */
        public int buildingId = -1;
        public DateTime start_time;
        public DateTime end_time;
        //private Font f = new Font();

        // The following info are used to compute the shadows
        private ArrayList PlaneEqs; // The plane equations represented by a, b, c, d and visibility
        private ArrayList Connectivity; // The neighboring information of faces;

        public BuildingVRMLNode() {
            vertices = new ArrayList();
            textureCoords = new ArrayList();
        }

        public BuildingVRMLNode(bool createList) {
            if (createList) {
                vertices = new ArrayList();
                textureCoords = new ArrayList();
            }
        }

        public void setConnectivity() {
            Connectivity = new ArrayList();
            ArrayList Neighbors;
            for (int i = 0; i < faces.Count; i++) {
                Neighbors = new ArrayList();
                ArrayList face = (ArrayList)faces[i];
                for (int j = 0; j < faces.Count; j++) {
                    if (i != j) {
                        ArrayList face2 = (ArrayList)faces[j];
                        int numofSameVertice = 0;
                        foreach (int verticeID in face) {
                            if (face2.Contains(verticeID)) {
                                numofSameVertice++;
                            }
                        }
                        if (numofSameVertice >= 2) {
                            Neighbors.Add(j);
                        }
                    }
                }
                Connectivity.Add(Neighbors);
            }
        }

        public void CalcPlane() {
            PlaneEqs = new ArrayList();
            for (int i = 0; i < faces.Count; i++) {
                ArrayList face = (ArrayList)faces[i];
                ArrayList Plane = new ArrayList();
                Vertex v1, v2, v3, v4;
                v1 = (Vertex)vertices[(int)face[0]];
                v2 = (Vertex)vertices[(int)face[1]];
                v3 = (Vertex)vertices[(int)face[2]];
                //v4 = (Vertex)vertices[(int)face[3]];
                v1.Round();
                v2.Round();
                v3.Round();
                //v4.Round();
                Plane.Add(Math.Round(v1.GetY() * (v2.GetZ() - v3.GetZ()) + v2.GetY() * (v3.GetZ() - v1.GetZ()) + v3.GetY() * (v1.GetZ() - v2.GetZ()), 2));
                Plane.Add(Math.Round(v1.GetZ() * (v2.GetX() - v3.GetX()) + v2.GetZ() * (v3.GetX() - v1.GetX()) + v3.GetZ() * (v1.GetX() - v2.GetX()), 2));
                Plane.Add(Math.Round(v1.GetX() * (v2.GetY() - v3.GetY()) + v2.GetX() * (v3.GetY() - v1.GetY()) + v3.GetX() * (v1.GetY() - v2.GetY()), 2));
                Plane.Add(Math.Round(-(v1.GetX() * (v2.GetY() * v3.GetZ() - v3.GetY() * v2.GetZ()) +
                      v2.GetX() * (v3.GetY() * v1.GetZ() - v1.GetY() * v3.GetZ()) +
                      v3.GetX() * (v1.GetY() * v2.GetZ() - v2.GetY() * v1.GetZ())), 2));
                PlaneEqs.Add(Plane);
            }

        }

        public void Reset() {
            displayListExists = false;
        }

        public string get_image_url() {
            return image_URL;
        }

        public void addTextureImageURL(string hostname, string URL) {
            image_URL = hostname + URL;
        }

        public void setTextureId(int textureID) {
            if (this.textureId != textureID) {
                this.textureId = textureID;
                displayList = -1;
                displayListExists = false;
            }
        }

        public BuildingVRMLNode(ArrayList vertices) {
            this.vertices = vertices;
        }

        public override void addVertex(Vertex v) {
            this.vertices.Add(v);

        }
        public override void addFace(ArrayList face) {
            this.faces.Add(face);
        }


        // returns true if the vertex R is in between v1 and v2
        public static bool checkbetween(Vertex v1, Vertex v2, Vertex R) {

            if ((float)v1.GetZ() > (float)v2.GetZ()) {
                if ((float)R.GetZ() < (float)v2.GetZ() || (float)R.GetZ() > (float)v1.GetZ()) {
                    return false;
                }
                else {
                    return true;
                }
            }
            else if ((float)v1.GetZ() < (float)v2.GetZ()) {
                if ((float)R.GetZ() < (float)v1.GetZ() || (float)R.GetZ() > (float)v2.GetZ()) {
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                if ((float)v1.GetX() > (float)v2.GetX()) {
                    if ((float)R.GetX() < (float)v2.GetX() || (float)R.GetX() > (float)v1.GetX()) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                else if ((float)v1.GetX() < (float)v2.GetX()) {
                    if ((float)R.GetX() < (float)v1.GetX() || (float)R.GetX() > (float)v2.GetX()) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                else {

                    if ((float)v1.GetY() > (float)v2.GetY()) {
                        if ((float)R.GetY() < (float)v2.GetY() || (float)R.GetY() > (float)v1.GetY()) {
                            return false;
                        }
                        else {
                            return true;
                        }
                    }
                    else if ((float)v1.GetY() < (float)v2.GetY()) {
                        if ((float)R.GetY() < (float)v1.GetY() || (float)R.GetY() > (float)v2.GetY()) {
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

        public void calculateNormals() {
            double x1, y1, z1;
            for (int i = 0; i < faces.Count; i++) {
                ArrayList face = (ArrayList)faces[i];
                Vertex v1 = (Vertex)vertices[(int)face[0]];
                Vertex v2 = (Vertex)vertices[(int)face[1]];
                x1 = v2.GetX() - v1.GetX();
                y1 = v2.GetY() - v1.GetY();
                z1 = v2.GetZ() - v1.GetZ();
                Vector vec1 = new Vector(x1, y1, z1);
                Vertex v3 = (Vertex)vertices[(int)face[2]];
                x1 = v3.GetX() - v2.GetX();
                y1 = v3.GetY() - v2.GetY();
                z1 = v3.GetZ() - v2.GetZ();
                Vector vec2 = new Vector(x1, y1, z1);
                Vector vec3 = vec1.Cross(vec2);
                vec3.Normalize();
                if (vec3.z == -1.0) vec3.z = 1.0;
                normals.Add(vec3);
            }
        }


        public void resetDisplayList() {
            displayList = -1;
            displayListExists = false;
        }

        public void resetDisplayList2() {
            displayList2 = -1;
            displayListExists2 = false;
        }




        // calculates d value for a vector and a vertex. used to figure out if the point is above or below the shadow plane
        private double calc_d(Vector v1, Vertex v2) {
            return v1.x * v2.GetX() + v1.y * v2.GetY() + v1.z * v2.GetZ();
        }

        // returns true if both vertices are equal
        private bool isEqual(Vertex a, Vertex b) {
            return ((float)a.GetX() == (float)b.GetX() && (float)a.GetY() == (float)b.GetY() && (float)a.GetZ() == (float)b.GetZ());
        }

        private bool faceIsEqual(ArrayList face1, ArrayList face2) {
            if (face1.Count != face2.Count) {
                return false;
            }
            else {
                for (int x = 0; x < face1.Count; x++) {
                    if ((int)face1[x] != (int)face2[x]) {
                        return false;
                    }
                }
                return true;
            }
        }

        // adds vertex a to new_vertices and added_vertices as appropriate.  Also updates new_face list.
        private void addVerticeToFace(double d, Vector normal, Vertex a, ref ArrayList new_vertices, ref ArrayList new_face, ref ArrayList added_vertices) {
            int index = 0;
            bool found_added = false;
            bool found_vertice = false;
            bool found_in_face = false;
            float temp = (float)calc_d(normal, a);

            if (temp <= (float)d) {
                // if point is on the shadow plane, add it to the added_vertice list so it may be used as a vertice to a new face
                if (temp == (float)d) {

                    for (int x = 0; x < added_vertices.Count; x++) {
                        Vertex b = (Vertex)added_vertices[x];
                        if (isEqual(a, b)) {
                            found_added = true;
                        }
                    }
                    if (!found_added) {
                        added_vertices.Add(a);
                    }
                }

                // search for vertice in existing new_vertices list
                for (int x = 0; x < new_vertices.Count; x++) {
                    Vertex b = (Vertex)new_vertices[x];

                    if (isEqual(a, b)) {
                        index = x;
                        found_vertice = true;
                    }
                }

                if (found_vertice) {
                    // search for vertex index in new_face
                    int found = new_face.IndexOf(index);

                    // don't add the vertex index if it already exists on the face
                    if (found == -1) {
                        new_face.Add(index);
                    }
                }
                else {
                    new_face.Add(new_vertices.Count);
                    new_vertices.Add(a);
                }
            }
        }




        public ArrayList getFaces() {
            return faces;
        }

        public ArrayList getVertices() {
            return vertices;
        }

        public ArrayList getNormals() {
            return normals;
        }


        private void calcShadowPlane(Vertex sun, Vertex shadow_p1, Vertex shadow_p2, ArrayList vertices, ref double d, ref Vector shadow_normal, ref bool above_shadow_plane, ref bool below_shadow_plane) {
            // creates two vectors from sun position to shadow line which will be used to figure out the shadow plane normal
            Vector vec1 = new Vector(sun.GetX() - shadow_p1.GetX(), sun.GetY() - shadow_p1.GetY(), sun.GetZ() - shadow_p1.GetZ());
            Vector vec2 = new Vector(sun.GetX() - shadow_p2.GetX(), sun.GetY() - shadow_p2.GetY(), sun.GetZ() - shadow_p2.GetZ());

            // take cross product of two shadow vectors
            shadow_normal = vec1.Cross(vec2);

            if (shadow_normal.z < 0) {
                shadow_normal = vec2.Cross(vec1);
            }
            // calculate d
            d = calc_d(shadow_normal, sun);

            // check to see if all vertices are above the shadow plane
            for (int x = 0; x < vertices.Count; x++) {
                Vertex b = (Vertex)vertices[x];
                float under = (float)calc_d(shadow_normal, b);

                if (under <= (float)d) {
                    above_shadow_plane = false;
                }
                else if (under >= (float)d) {
                    below_shadow_plane = false;
                }
            }

        }

        private void calcNewVertices(double d, Vector shadow_normal, ref ArrayList new_vertices, ref ArrayList added_vertices, ref ArrayList new_face_list, ref ArrayList new_face) {
            for (int i = 0; i < faces.Count; i++) {
                ArrayList face = (ArrayList)faces[i];
                for (int j = 1; j <= face.Count; j++) {

                    Vertex v1, v2;
                    if (j == face.Count) {
                        v1 = (Vertex)vertices[(int)face[j - 1]];
                        v2 = (Vertex)vertices[(int)face[0]];
                    }
                    else {
                        v1 = (Vertex)vertices[(int)face[j - 1]];
                        v2 = (Vertex)vertices[(int)face[j]];
                    }

                    // calculate slope vector from vertex v1 and v2
                    Vector D_vec = new Vector(v2.GetX() - v1.GetX(), v2.GetY() - v1.GetY(), v2.GetZ() - v1.GetZ());

                    // If line is parallel to plane, the dot product of the normal to the plane and the vertice vector will be 0
                    // if the line is parallel, skip the intersection calculation
                    double dot_prod = D_vec.x * shadow_normal.x + D_vec.y * shadow_normal.y + D_vec.z * shadow_normal.z;

                    if (dot_prod != 0) {
                        double t = (d - (shadow_normal.x * v1.GetX() + shadow_normal.y * v1.GetY() + shadow_normal.z * v1.GetZ())) / (shadow_normal.x * D_vec.x + shadow_normal.y * D_vec.y + shadow_normal.z * D_vec.z);

                        double Rx = v1.GetX() + t * D_vec.x;
                        double Ry = v1.GetY() + t * D_vec.y;
                        double Rz = v1.GetZ() + t * D_vec.z;
                        Vertex R = new Vertex(Rx, Ry, Rz);

                        // check to see if the new intersection point is on the line between the two existing points
                        if (R.isBetween(v1, v2)) {
                            addVerticeToFace(d, shadow_normal, v1, ref new_vertices, ref new_face, ref added_vertices);
                            addVerticeToFace(d, shadow_normal, R, ref new_vertices, ref new_face, ref added_vertices);
                            addVerticeToFace(d, shadow_normal, v2, ref new_vertices, ref new_face, ref added_vertices);
                        }
                        else {
                            addVerticeToFace(d, shadow_normal, v1, ref new_vertices, ref new_face, ref added_vertices);
                            addVerticeToFace(d, shadow_normal, v2, ref new_vertices, ref new_face, ref added_vertices);
                        }
                    }
                    else {
                        addVerticeToFace(d, shadow_normal, v1, ref new_vertices, ref new_face, ref added_vertices);
                        addVerticeToFace(d, shadow_normal, v2, ref new_vertices, ref new_face, ref added_vertices);
                    }
                }

                if (new_face.Count != 0) {
                    bool face_exist = false;
                    for (int k = 0; k < new_face_list.Count; k++) {
                        if (faceIsEqual(new_face, (ArrayList)new_face_list[k])) {
                            face_exist = true;
                            break;
                        }
                    }

                    if (!face_exist) {
                        new_face_list.Add(new_face);
                    }
                }
                new_face = new ArrayList();
            }

        }

        private void tagVertices(ArrayList new_face_list, ArrayList new_vertices, ArrayList added_vertices, ref int face_num, ref ArrayList face_tag) {
            // initialize all face_tags to 0
            for (int x = 0; x < added_vertices.Count; x++) {
                face_tag.Add(0);
            }


            // if there is a 0 in the face_tag list
            bool still_zero = false;
            int face_index = face_tag.IndexOf(0);
            if (face_index >= 0) {
                still_zero = true;
            }

            while (still_zero) {
                for (int x = 0; x < added_vertices.Count; x++) {
                    // if face has not been tagged
                    if ((int)face_tag[x] == 0) {

                        Vertex compare = (Vertex)added_vertices[x];

                        for (int i = 0; i < new_face_list.Count; i++) {
                            ArrayList face = (ArrayList)new_face_list[i];
                            for (int j = 0; j < face.Count; j++) {

                                // check consecutive vertices on each face to see if they are both new
                                Vertex v1 = (Vertex)new_vertices[(int)face[j]];
                                Vertex v0, v2;
                                if (j == 0) {
                                    v0 = (Vertex)new_vertices[(int)face[face.Count - 1]];
                                    v2 = (Vertex)new_vertices[(int)face[j + 1]];
                                }
                                else if (j == face.Count - 1) {
                                    v0 = (Vertex)new_vertices[(int)face[j - 1]];
                                    v2 = (Vertex)new_vertices[(int)face[0]];
                                }
                                else {
                                    v0 = (Vertex)new_vertices[(int)face[j - 1]];
                                    v2 = (Vertex)new_vertices[(int)face[j + 1]];
                                }

                                // if the untagged face is equal to a vertice from one of the new faces
                                if (isEqual(v1, compare)) {
                                    for (int k = 0; k < added_vertices.Count; k++) {
                                        Vertex b = (Vertex)added_vertices[k];

                                        // don't check a vertice with itself
                                        if (k != x) {
                                            // if the consecutive vertices are also in the added_vertice list
                                            // then those vertices should be tagged the same value

                                            if (isEqual(b, v0)) {
                                                // if the vertices that are being cycled through haven't been tagged
                                                // then tag it with the same face number as the compare vertice

                                                if ((int)face_tag[k] == 0) {
                                                    face_tag[k] = face_num;
                                                }
                                                // else if the vertices that are being cycled through have been tagged
                                                // and the compare vertice has not been tagged, tag the compare vertice w/ the same tag as the others
                                                else {
                                                    face_tag[x] = face_tag[k];
                                                }
                                            }
                                            if (isEqual(b, v2)) {
                                                if ((int)face_tag[k] == 0) {
                                                    face_tag[k] = face_num;
                                                }
                                                else {
                                                    face_tag[x] = face_tag[k];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // this was the bug
                        if ((int)face_tag[x] == 0) {
                            face_tag[x] = face_num;
                        }
                    }
                }

                face_num++;// BUG

                face_index = face_tag.IndexOf(0);
                if (face_index == -1) {
                    still_zero = false;
                }
            }

        }

        private void createNewFaces(int face_num, ArrayList new_vertices, ArrayList added_vertices, ArrayList new_face_list, ArrayList face_tag) {
            ArrayList t_face;
            ArrayList face_vertices = new ArrayList();
            ArrayList temp_face_v;

            if ((int)added_vertices.Count != 0)  // if new vertices have been added, then we need to create a new face and add to list
            {
                for (int x = 1; x <= face_num; x++) // for all assigned face numbers.
                {
                    for (int i = 0; i < added_vertices.Count; i++) {
                        // added all vertices that are tagged with
                        // the same face number to the face_vertices list                            
                        if ((int)face_tag[i] == x) {
                            Vertex v1 = (Vertex)added_vertices[i];
                            face_vertices.Add(v1);
                        }
                    }
                    Vertex compare;
                    t_face = new ArrayList();
                    temp_face_v = new ArrayList();

                    while (face_vertices.Count != 0) {
                        // start with the first face_vertice
                        compare = (Vertex)face_vertices[0];
                        temp_face_v.Add(compare);
                        face_vertices.Remove(compare);

                        for (int i = 0; i < new_face_list.Count; i++) {
                            ArrayList face = (ArrayList)new_face_list[i];
                            for (int j = 0; j < face.Count; j++) {

                                // check consecutive vertices on each face
                                Vertex v1 = (Vertex)new_vertices[(int)face[j]];
                                Vertex v0, v2;
                                if (j == 0) {
                                    v0 = (Vertex)new_vertices[(int)face[face.Count - 1]];
                                    v2 = (Vertex)new_vertices[(int)face[j + 1]];
                                }
                                else if (j == face.Count - 1) {
                                    v0 = (Vertex)new_vertices[(int)face[j - 1]];
                                    v2 = (Vertex)new_vertices[(int)face[0]];
                                }
                                else {
                                    v0 = (Vertex)new_vertices[(int)face[j - 1]];
                                    v2 = (Vertex)new_vertices[(int)face[j + 1]];
                                }

                                // if the new vertice matches one of the vertices on the new face

                                if (isEqual(v1, compare)) {
                                    for (int k = 0; k < face_vertices.Count; k++) {
                                        Vertex b = (Vertex)face_vertices[k];
                                        // don't want to do anything if the points are the same point

                                        if (!isEqual(b, compare)) {
                                            // if the consecutive vertices are equal, then add the vertice you checked
                                            // and make the compare vertice the vertice you added
                                            // basically this crawls along the edges of the building finding
                                            // the correct order for each vertice
                                            if (isEqual(b, v0)) {
                                                compare = b;
                                                temp_face_v.Add(b);
                                                face_vertices.Remove(b);
                                                // restart looking through all the faces again
                                                i = 0;
                                            }
                                            if (isEqual(b, v2)) {
                                                compare = b;
                                                temp_face_v.Add(b);
                                                face_vertices.Remove(b);
                                                // restart looking through all the faces again
                                                i = 0;
                                            }
                                        }
                                    }
                                }
                            }// end face count
                        }// end new_face_list
                    }

                    // converts the ordered vertices of a new face into the new_vertices index
                    for (int s = 0; s < temp_face_v.Count; s++) {
                        Vertex v1 = (Vertex)temp_face_v[s];
                        // search the new_vertices list for the correct index and add it to the new face

                        int index = new_vertices.IndexOf(v1);
                        if (index >= 0) {
                            t_face.Add(index);
                        }

                    }
                    if (t_face.Count != 0) {

                        bool face_exist = false;
                        for (int r = 0; r < new_face_list.Count; r++) {
                            if (faceIsEqual(t_face, (ArrayList)new_face_list[r])) {
                                face_exist = true;
                                break;
                            }
                        }

                        if (!face_exist) {
                            new_face_list.Add(t_face);
                        }

                    }
                }
            }
        }

        public BuildingVRMLNode calcVolume(Vertex sun, Vertex shadow_p1, Vertex shadow_p2) {
            bool DEBUG = false;

            bool above_shadow_plane = true;             // true if all building vertices are above the shadow plane
            bool below_shadow_plane = true;             // true if all building vertices are below the shadow plane
            Vector shadow_normal = new Vector(0, 0, 0);   // shadow normal vector
            ArrayList face_tag = new ArrayList();       // holds the face number of each vertice
            ArrayList new_face_list = new ArrayList();    // list of faces
            ArrayList new_face = new ArrayList();     // stores new faces of the building
            ArrayList new_vertices = new ArrayList();  // stores new vertices of each face of the building 
            ArrayList added_vertices = new ArrayList(); // vertices that were created by the intersection of shadow plane and building
            int face_num = 1;                           // used to tag vertices with face number
            double d = 0;                              // d is used in the shadow plane equation Ax + By + Cz = d

            if (DEBUG) {
                Console.WriteLine("\n\nInitial Vertices in Building:");
                for (int x = 0; x < vertices.Count; x++) {
                    Vertex v1 = (Vertex)vertices[x];
                    Console.WriteLine("Vertice {0}: {1} {2} {3}", x, v1.GetX(), v1.GetY(), v1.GetZ());
                }
                Console.WriteLine("\nFaces in Building:");
                for (int i = 0; i < faces.Count; i++) {
                    Console.Write("\nFace {0}: ", i);
                    ArrayList face = (ArrayList)faces[i];
                    for (int j = 0; j < face.Count; j++) {
                        Vertex v1 = (Vertex)vertices[(int)face[j]];
                        Console.Write("{0} ", face[j]);
                    }
                }
            }

            /*--------------------------------------------
                  PHASE 1:  Create Shadow Plane           
            ---------------------------------------------*/

            calcShadowPlane(sun, shadow_p1, shadow_p2, vertices, ref d, ref shadow_normal, ref above_shadow_plane, ref below_shadow_plane);

            if (DEBUG) {
                Console.WriteLine("\n\n\nPhase 1: Shadow Plane Parameters\n");
                Console.WriteLine("Shadow Normal: {0} {1} {2}", shadow_normal.x, shadow_normal.y, shadow_normal.z);
                Console.WriteLine("d value: {0}", d);
            }

            // if all points are above or below, return the same building
            if (above_shadow_plane || below_shadow_plane) {
                BuildingVRMLNode temp_building = new BuildingVRMLNode(vertices);
                for (int i = 0; i < faces.Count; i++) {
                    ArrayList face = (ArrayList)faces[i];
                    temp_building.addFace(face);
                }
                return temp_building;
            }
            else {
                /*------------------------------------------------------------------
                      PHASE 2:  Add Points of Intersection to New Vertice List     
                -------------------------------------------------------------------*/

                // Adds points of intersection and old vertices to each face if they are above the shadow plane

                calcNewVertices(d, shadow_normal, ref new_vertices, ref added_vertices, ref new_face_list, ref new_face);

                if (DEBUG) {
                    Console.WriteLine("\n\nPhase 2: New Vertices\n");
                    for (int x = 0; x < new_vertices.Count; x++) {
                        Vertex v1 = (Vertex)new_vertices[x];
                        Console.WriteLine("{0}: {1} {2} {3}", x, v1.GetX(), v1.GetY(), v1.GetZ());
                    }
                    for (int i = 0; i < new_face_list.Count; i++) {
                        Console.WriteLine("New Faces:");
                        ArrayList face = (ArrayList)new_face_list[i];
                        for (int j = 0; j < face.Count; j++) {
                            Console.Write("{0} ", face[j]);
                        }
                        Console.WriteLine("");
                    }
                }

                /*-------------------------------------------------------------
                      PHASE 3:  Tag Each Added Vertice With a Face Number    
                --------------------------------------------------------------*/

                tagVertices(new_face_list, new_vertices, added_vertices, ref face_num, ref face_tag);

                if (DEBUG) {
                    Console.WriteLine("\n\nPhase 3: Tag Added Vertices\n");
                    Console.WriteLine("Number of added_vertices: {0}", added_vertices.Count);
                    for (int x = 0; x < added_vertices.Count; x++) {
                        Vertex v1 = (Vertex)added_vertices[x];
                        Console.WriteLine("added vertice: {0} {1} {2}", v1.GetX(), v1.GetY(), v1.GetZ());
                    }
                    for (int x = 0; x < added_vertices.Count; x++) {
                        face_tag.Add(0);
                        Console.WriteLine("face {0}", face_tag[x]);
                    }
                }

                /*---------------------------------------------------------
                      PHASE 4:  Create a New Face From Added Vertices    
                ---------------------------------------------------------*/

                createNewFaces(face_num, new_vertices, added_vertices, new_face_list, face_tag);

                if (DEBUG) {
                    Console.WriteLine("\n\nPhase 4: New Faces\n");
                    for (int i = 0; i < new_face_list.Count; i++) {
                        Console.WriteLine("New Faces:");
                        ArrayList face = (ArrayList)new_face_list[i];
                        for (int j = 0; j < face.Count; j++) {
                            Console.WriteLine("{0} ", face[j]);
                        }
                    }
                }

                // creates a new building node from the list of new_vertices
                BuildingVRMLNode temp_building = new BuildingVRMLNode(new_vertices);

                // adds the new faces to the new building
                for (int x = 0; x < new_face_list.Count; x++) {
                    temp_building.addFace((ArrayList)new_face_list[x]);
                }
                return temp_building;
            }
        }



        // calculate the lines of intersection between the face_normal and all the shadow_volume_normals
        // return true if the two points lie on the line of intersection, false otherwise
        private bool isOnLine(Vertex base_point, Vertex q_point, Vector face_normal, ArrayList shadow_volume_normals) {

            for (int x = 0; x < shadow_volume_normals.Count; x++) {
                double t = 0;
                Vector normal = (Vector)shadow_volume_normals[x];
                Vector line = normal.Cross(face_normal);

                // if line is zero vector then the two planes were perpendicular
                if (!(line.x == 0 && line.y == 0 && line.z == 0)) {
                    line.Normalize(); // do normalize after checking for 0 vector b/c otherwise get div 0 error!

                    // use the base point to calculate the line of intersection
                    if (line.x != 0) {
                        t = (q_point.GetX() - base_point.GetX()) / line.x;
                    }
                    else if (line.y != 0) {
                        t = (q_point.GetY() - base_point.GetY()) / line.y;
                    }
                    else if (line.z != 0) {
                        t = (q_point.GetZ() - base_point.GetZ()) / line.z;
                    }

                    // check to see if q_point, point in question, is on the line
                    if (base_point.GetX() + t * line.x != q_point.GetX()) {
                        return false;
                    }
                    if (base_point.GetY() + t * line.y != q_point.GetY()) {
                        return false;
                    }
                    if (base_point.GetZ() + t * line.z != q_point.GetZ()) {
                        return false;
                    }

                    return true;
                }
            }
            return false;
        }



        public void appendVertex() {
            for (int x = 0; x < faces.Count; x++) {
                ArrayList face = (ArrayList)faces[x];
                face.Add((int)face[0]);
            }
        }



        public BuildingVRMLNode reverseEnvelope(BuildingVRMLNode shadow_volume, int passNum) {
            /* First convert the building volume and shadow volume 
             * into Polyhedra
             */
            calculateNormals();
            shadow_volume.calculateNormals();

            Polyhedron building = new Polyhedron(vertices, faces, normals);
            Polyhedron shadow = new Polyhedron(shadow_volume.getVertices(), shadow_volume.getFaces(), shadow_volume.getNormals());

            // Should deep copy building and shadow into temporary Polyhedrons to modify
            Polyhedron tempBuilding = new Polyhedron(building);
            Polyhedron tempShadow = new Polyhedron(shadow);

            /* Divide the polygons in the Polyhedron into non-intersecting polygons
             * 1)  First subdivide the building
             * 2)  Subdivide the shadow
             * 3)  Subdivide the building again.  Both polyhedrons and their polygons should not intersect after this
             */

            tempBuilding.subDivide(tempShadow);
            tempShadow.subDivide(tempBuilding);
            tempBuilding.subDivide(tempShadow);


            // Categorize the polygons as inside, outside, or on the boundary of each building
            tempBuilding.markRegion(tempShadow);
            tempShadow.markRegion(tempBuilding);
            //tempBuilding.printInfo();
            //tempShadow.printInfo();

            ArrayList newPolygons = new ArrayList();
            ArrayList newVertices = new ArrayList();

            tempBuilding.selectPolygons(true, tempShadow, newPolygons, newVertices);
            tempShadow.selectPolygons(false, tempBuilding, newPolygons, newVertices);
            tempBuilding.printInfo();
            tempShadow.printInfo();


            /*
            tempBuilding.printInfo();
            Console.WriteLine("\nPrinting Vertices...");
            foreach (Vertex v in newVertices) {
                v.printInfo();
            }
            Console.WriteLine("\nPrinting Polygons...");
            foreach (Polygon p in newPolygons) {
                p.printInfo();
            }*/

            Polyhedron newPolyhedron = new Polyhedron(newVertices, newPolygons);
            //newPolyhedron.printInfo();
            newPolyhedron.reset();

            BuildingVRMLNode newBuilding = newPolyhedron.convertToBuilding(vertices);
            BuildingVRMLNode bBuilding = tempBuilding.convertToBuilding(vertices);
            BuildingVRMLNode bShadow = tempShadow.convertToBuilding(tempBuilding.getVertices());
            //BuildingVRMLNode newShadow = tempShadow.convertToBuilding();
            if (passNum == 1) {
                return newBuilding;
            }
            else {
                return bShadow; //new BuildingVRMLNode();//newBuilding;
            }
        }

        public void printInfo(string file) {

            TextWriter tw = new StreamWriter(file + ".txt");
            int max_num = 0;
            ArrayList face_x = new ArrayList();
            ArrayList face_y = new ArrayList();
            ArrayList face_z = new ArrayList();
            int index;

            //tw.WriteLine("MATLAB CODE");

            for (int x = 0; x < faces.Count; x++) {
                ArrayList face = (ArrayList)faces[x];
                if (face.Count > max_num) {
                    max_num = face.Count;
                }
            }


            tw.Write("\n\nX = [");
            for (int x = 0; x < max_num; x++) {
                for (int y = 0; y < faces.Count; y++) {
                    ArrayList face = (ArrayList)faces[y];
                    if (face.Count > x) {
                        index = (int)face[x];
                    }
                    else {
                        index = (int)face[face.Count - 1];
                    }
                    Vertex v = (Vertex)vertices[index];
                    if ((int)v.GetX() == 381152) {
                        int iop = 0;
                    }
                    tw.Write("{0} ", v.GetX());
                }
                tw.WriteLine(";\n");
            }
            tw.WriteLine("];");

            tw.Write("\n\nY = [");
            for (int x = 0; x < max_num; x++) {
                for (int y = 0; y < faces.Count; y++) {
                    ArrayList face = (ArrayList)faces[y];
                    if (face.Count > x) {
                        index = (int)face[x];
                    }
                    else {
                        index = (int)face[face.Count - 1];
                    }
                    Vertex v = (Vertex)vertices[index];
                    tw.Write("{0} ", v.GetY());
                }
                tw.WriteLine(";\n");
            }
            tw.WriteLine("];");

            tw.Write("\n\nZ = [");
            for (int x = 0; x < max_num; x++) {
                for (int y = 0; y < faces.Count; y++) {
                    ArrayList face = (ArrayList)faces[y];
                    if (face.Count > x) {
                        index = (int)face[x];
                    }
                    else {
                        index = (int)face[face.Count - 1];
                    }
                    Vertex v = (Vertex)vertices[index];
                    tw.Write("{0} ", v.GetZ());
                }
                tw.WriteLine(";\n");
            }
            tw.WriteLine("];");

            tw.Write("\n\nC = [");
            for (int x = 0; x < max_num; x++) {
                for (int y = 1; y <= faces.Count; y++) {
                    tw.Write("{0} ", y);
                }
                tw.Write(";\n");
            }
            tw.WriteLine("];");

            tw.WriteLine("fill3(X,Y,Z,C);");


            // close the stream
            tw.Close();

            /*
            int face_num = 1;
            Console.WriteLine("\n\nVertices");
            for (int x = 0; x < vertices.Count; x++) {
                ((Vertex)vertices[x]).printInfo();
            }

            for (int y = 0; y < faces.Count; y++) {
                Console.WriteLine("Face: {0}", face_num);
                face_num++;
                ArrayList face = (ArrayList)faces[y];
                for (int x = 0; x < face.Count; x++) {
                    Vertex v1 = (Vertex)vertices[(int)face[x]];
                    Console.WriteLine("{0}:  {1} {2} {3}", (int)face[x], v1.GetX(), v1.GetY(), v1.GetZ());
                }
            }
            */

        }

    }
}
