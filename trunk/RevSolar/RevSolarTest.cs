using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using test;

namespace Solar_Envelope_Test {
    class RevSolarTest {

        private static Vertex b1;
        private static Vertex b2;
        private static Vertex b3;
        private static Vertex b4;
        private static Vertex b5;
        private static Vertex b6;
        private static Vertex b7;
        private static Vertex b8;
        private static Vertex s1;
        private static Vertex s2;
        private static Vertex s3;
        private static Vertex s4;
        private static Vertex s5;
        private static Vertex s6;
        private static Vertex s7;
        private static Vertex s8;
        private static ArrayList bVertices;
        private static ArrayList sVertices;

        public const int BCUBE1 = 1;
        public const int BCUBE2 = 2;
        public const int BCUBE3 = 3;
        public const int SCUBE1 = 4;
        public const int SCUBE2 = 5;
        public const int SCUBE3 = 6;
        public const int SCUBE4 = 7;
        public const int SCUBE5 = 8;


        public static void Main(string[] args) {

            bVertices = new ArrayList();
            sVertices = new ArrayList();
            /* Test Cases: (1,1) (1,2) (1,3) (2,4) (2,5)
             */
            int bType = BCUBE2;
            int sType = SCUBE5;

            if (bType == BCUBE1) {
                // original building cube
                b1 = new Vertex(0, 0, 4);
                b2 = new Vertex(4, 0, 4);
                b3 = new Vertex(4, 4, 4);
                b4 = new Vertex(0, 4, 4);
                b5 = new Vertex(0, 0, 0);
                b6 = new Vertex(4, 0, 0);
                b7 = new Vertex(4, 4, 0);
                b8 = new Vertex(0, 4, 0);
            }
            else if (bType == BCUBE2) {
                b1 = new Vertex(1, 1, 4);
                b2 = new Vertex(4, 1, 4);
                b3 = new Vertex(4, 4, 4);
                b4 = new Vertex(1, 4, 4);
                b5 = new Vertex(1, 1, 1);
                b6 = new Vertex(4, 1, 1);
                b7 = new Vertex(4, 4, 1);
                b8 = new Vertex(1, 4, 1);
            }
            else if (bType == BCUBE3) {
                b1 = new Vertex(10, 10, 14);
                b2 = new Vertex(14, 10, 14);
                b3 = new Vertex(14, 14, 14);
                b4 = new Vertex(10, 14, 14);
                b5 = new Vertex(10, 10, 10);
                b6 = new Vertex(14, 10, 10);
                b7 = new Vertex(14, 14, 10);
                b8 = new Vertex(10, 14, 10);
            }

            bVertices.Add(b1);
            bVertices.Add(b2);
            bVertices.Add(b3);
            bVertices.Add(b4);
            bVertices.Add(b5);
            bVertices.Add(b6);
            bVertices.Add(b7);
            bVertices.Add(b8);

            BuildingVRMLNode initBuilding = new BuildingVRMLNode(bVertices);

            ArrayList face1 = new ArrayList();
            ArrayList face2 = new ArrayList();
            ArrayList face3 = new ArrayList();
            ArrayList face4 = new ArrayList();
            ArrayList face5 = new ArrayList();
            ArrayList face6 = new ArrayList();
            ArrayList face7 = new ArrayList();
            ArrayList face8 = new ArrayList();

            face1.Add(0);
            face1.Add(1);
            face1.Add(2);
            face1.Add(3);
            initBuilding.addFace(face1);

            face2.Add(7);
            face2.Add(6);
            face2.Add(5);
            face2.Add(4);
            initBuilding.addFace(face2);

            face3.Add(0);
            face3.Add(4);
            face3.Add(5);
            face3.Add(1);
            initBuilding.addFace(face3);

            face4.Add(1);
            face4.Add(5);
            face4.Add(6);
            face4.Add(2);
            initBuilding.addFace(face4);

            face5.Add(2);
            face5.Add(6);
            face5.Add(7);
            face5.Add(3);
            initBuilding.addFace(face5);

            face6.Add(0);
            face6.Add(3);
            face6.Add(7);
            face6.Add(4);
            initBuilding.addFace(face6);


            /********************
             * BUILDING THE SHADOW
             *********************/

            if (sType == SCUBE1) {
                // first cube
                s1 = new Vertex(0, 2, 6);
                s2 = new Vertex(4, 2, 6);
                s3 = new Vertex(4, 6, 6);
                s4 = new Vertex(0, 6, 6);
                s5 = new Vertex(0, 2, 2);
                s6 = new Vertex(4, 2, 2);
                s7 = new Vertex(4, 6, 2);
                s8 = new Vertex(0, 6, 2);
            }
            else if (sType == SCUBE2) {

                // second cube
                s1 = new Vertex(2, 2, 6);
                s2 = new Vertex(6, 2, 6);
                s3 = new Vertex(6, 6, 6);
                s4 = new Vertex(2, 6, 6);
                s5 = new Vertex(2, 2, 2);
                s6 = new Vertex(6, 2, 2);
                s7 = new Vertex(6, 6, 2);
                s8 = new Vertex(2, 6, 2);
            }
            else if (sType == SCUBE3) {
                // third cube
                s1 = new Vertex(2, 2, 4);
                s2 = new Vertex(6, 2, 4);
                s3 = new Vertex(6, 6, 4);
                s4 = new Vertex(2, 6, 4);
                s5 = new Vertex(2, 2, 0);
                s6 = new Vertex(6, 2, 0);
                s7 = new Vertex(6, 6, 0);
                s8 = new Vertex(2, 6, 0);
            }
            else if (sType == SCUBE4) {
                s1 = new Vertex(3, 2, 4);
                s2 = new Vertex(4, 2, 4);
                s3 = new Vertex(4, 3, 4);
                s4 = new Vertex(3, 3, 4);
                s5 = new Vertex(3, 2, 1);
                s6 = new Vertex(4, 2, 1);
                s7 = new Vertex(4, 3, 1);
                s8 = new Vertex(3, 3, 1);
            }
            else if (sType == SCUBE5) {
                s1 = new Vertex(2, 2, 4);
                s2 = new Vertex(3, 2, 4);
                s3 = new Vertex(3, 3, 4);
                s4 = new Vertex(2, 3, 4);
                s5 = new Vertex(2, 2, 1);
                s6 = new Vertex(3, 2, 1);
                s7 = new Vertex(3, 3, 1);
                s8 = new Vertex(2, 3, 1);
            }


            sVertices.Add(s1);
            sVertices.Add(s2);
            sVertices.Add(s3);
            sVertices.Add(s4);
            sVertices.Add(s5);
            sVertices.Add(s6);
            sVertices.Add(s7);
            sVertices.Add(s8);

            BuildingVRMLNode shadow = new BuildingVRMLNode(sVertices);


            ArrayList sface1 = new ArrayList();
            ArrayList sface2 = new ArrayList();
            ArrayList sface3 = new ArrayList();
            ArrayList sface4 = new ArrayList();
            ArrayList sface5 = new ArrayList();
            ArrayList sface6 = new ArrayList();
            ArrayList sface7 = new ArrayList();
            ArrayList sface8 = new ArrayList();

            sface1.Add(0);
            sface1.Add(1);
            sface1.Add(2);
            sface1.Add(3);
            shadow.addFace(sface1);

            sface2.Add(4);
            sface2.Add(7);
            sface2.Add(6);
            sface2.Add(5);
            shadow.addFace(sface2);

            sface3.Add(0);
            sface3.Add(4);
            sface3.Add(5);
            sface3.Add(1);
            shadow.addFace(sface3);

            sface4.Add(1);
            sface4.Add(5);
            sface4.Add(6);
            sface4.Add(2);
            shadow.addFace(sface4);

            sface5.Add(2);
            sface5.Add(6);
            sface5.Add(7);
            sface5.Add(3);
            shadow.addFace(sface5);

            sface6.Add(0);
            sface6.Add(3);
            sface6.Add(7);
            sface6.Add(4);
            shadow.addFace(sface6);



            BuildingVRMLNode revsolar = initBuilding.reverseEnvelope(shadow);
            revsolar.printInfo("reverseSolar");


        }
    }
}

