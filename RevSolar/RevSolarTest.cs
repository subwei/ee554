using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using test;

namespace Solar_Envelope_Test {
    class RevSolarTest {

        public const int BCUBE1 = 1;
        public const int BCUBE2 = 2;
        public const int BCUBE3 = 3;
        public const int SCUBE1 = 4;
        public const int SCUBE2 = 5;
        public const int SCUBE3 = 6;
        public const int SCUBE4 = 7;
        public const int SCUBE5 = 8;

        public static void Main(string[] args) {

            RevSolarTest test = new RevSolarTest();
            /* Test Cases: (1,1) (1,2) (1,3) (2,4) (2,5)
             */
            int bType = BCUBE2;
            int sType = SCUBE5;


            BuildingVRMLNode initBuilding = new BuildingVRMLNode();

            if (bType == BCUBE1) {
                // original building cube
                initBuilding = test.createCube(test.getBCube1());
            }
            else if (bType == BCUBE2) {
                initBuilding = test.createCube(test.getBCube2());
            }
            else if (bType == BCUBE3) {
                initBuilding = test.createCube(test.getBCube3());
            }


            /********************
             * BUILDING THE SHADOW
             *********************/
            BuildingVRMLNode shadow = new BuildingVRMLNode();

            if (sType == SCUBE1) {
                shadow = test.createCube(test.getSCube1());
            }
            else if (sType == SCUBE2) {
                shadow = test.createCube(test.getSCube2());
            }
            else if (sType == SCUBE3) {
                shadow = test.createCube(test.getSCube3());
            }
            else if (sType == SCUBE4) {
                shadow = test.createCube(test.getSCube4());
            }
            else if (sType == SCUBE5) {
                shadow = test.createCube(test.getSCube5());
            }

            BuildingVRMLNode revsolar1 = initBuilding.reverseEnvelope(shadow);

            BuildingVRMLNode shadow2 = test.createCube(test.getSCube2());
            BuildingVRMLNode revsolar2 = revsolar1.reverseEnvelope(shadow2);
            revsolar2.printInfo("reverseSolar");

        }

        public BuildingVRMLNode createCube(ArrayList vertices) {

            BuildingVRMLNode building = new BuildingVRMLNode(vertices);

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
            building.addFace(face1);

            face2.Add(7);
            face2.Add(6);
            face2.Add(5);
            face2.Add(4);
            building.addFace(face2);

            face3.Add(0);
            face3.Add(4);
            face3.Add(5);
            face3.Add(1);
            building.addFace(face3);

            face4.Add(1);
            face4.Add(5);
            face4.Add(6);
            face4.Add(2);
            building.addFace(face4);

            face5.Add(2);
            face5.Add(6);
            face5.Add(7);
            face5.Add(3);
            building.addFace(face5);

            face6.Add(0);
            face6.Add(3);
            face6.Add(7);
            face6.Add(4);
            building.addFace(face6);

            return building;
        }

        public ArrayList getBCube1() {
            Vertex b1 = new Vertex(0, 0, 4);
            Vertex b2 = new Vertex(4, 0, 4);
            Vertex b3 = new Vertex(4, 4, 4);
            Vertex b4 = new Vertex(0, 4, 4);
            Vertex b5 = new Vertex(0, 0, 0);
            Vertex b6 = new Vertex(4, 0, 0);
            Vertex b7 = new Vertex(4, 4, 0);
            Vertex b8 = new Vertex(0, 4, 0);
            ArrayList tempList = new ArrayList();
            tempList.Add(b1);
            tempList.Add(b2);
            tempList.Add(b3);
            tempList.Add(b4);
            tempList.Add(b5);
            tempList.Add(b6);
            tempList.Add(b7);
            tempList.Add(b8);
            return tempList;
        }

        public ArrayList getBCube2() {
            Vertex b1 = new Vertex(1, 1, 4);
            Vertex b2 = new Vertex(4, 1, 4);
            Vertex b3 = new Vertex(4, 4, 4);
            Vertex b4 = new Vertex(1, 4, 4);
            Vertex b5 = new Vertex(1, 1, 1);
            Vertex b6 = new Vertex(4, 1, 1);
            Vertex b7 = new Vertex(4, 4, 1);
            Vertex b8 = new Vertex(1, 4, 1);
            ArrayList tempList = new ArrayList();
            tempList.Add(b1);
            tempList.Add(b2);
            tempList.Add(b3);
            tempList.Add(b4);
            tempList.Add(b5);
            tempList.Add(b6);
            tempList.Add(b7);
            tempList.Add(b8);
            return tempList;
        }

        public ArrayList getBCube3() {
            Vertex b1 = new Vertex(10, 10, 14);
            Vertex b2 = new Vertex(14, 10, 14);
            Vertex b3 = new Vertex(14, 14, 14);
            Vertex b4 = new Vertex(10, 14, 14);
            Vertex b5 = new Vertex(10, 10, 10);
            Vertex b6 = new Vertex(14, 10, 10);
            Vertex b7 = new Vertex(14, 14, 10);
            Vertex b8 = new Vertex(10, 14, 10);
            ArrayList tempList = new ArrayList();
            tempList.Add(b1);
            tempList.Add(b2);
            tempList.Add(b3);
            tempList.Add(b4);
            tempList.Add(b5);
            tempList.Add(b6);
            tempList.Add(b7);
            tempList.Add(b8);
            return tempList;
        }

        public ArrayList getSCube1() {
            Vertex s1 = new Vertex(0, 2, 6);
            Vertex s2 = new Vertex(4, 2, 6);
            Vertex s3 = new Vertex(4, 6, 6);
            Vertex s4 = new Vertex(0, 6, 6);
            Vertex s5 = new Vertex(0, 2, 2);
            Vertex s6 = new Vertex(4, 2, 2);
            Vertex s7 = new Vertex(4, 6, 2);
            Vertex s8 = new Vertex(0, 6, 2);
            ArrayList tempList = new ArrayList();
            tempList.Add(s1);
            tempList.Add(s2);
            tempList.Add(s3);
            tempList.Add(s4);
            tempList.Add(s5);
            tempList.Add(s6);
            tempList.Add(s7);
            tempList.Add(s8);
            return tempList;
        }

        public ArrayList getSCube2() {
            Vertex s1 = new Vertex(2, 2, 6);
            Vertex s2 = new Vertex(6, 2, 6);
            Vertex s3 = new Vertex(6, 6, 6);
            Vertex s4 = new Vertex(2, 6, 6);
            Vertex s5 = new Vertex(2, 2, 2);
            Vertex s6 = new Vertex(6, 2, 2);
            Vertex s7 = new Vertex(6, 6, 2);
            Vertex s8 = new Vertex(2, 6, 2);
            ArrayList tempList = new ArrayList();
            tempList.Add(s1);
            tempList.Add(s2);
            tempList.Add(s3);
            tempList.Add(s4);
            tempList.Add(s5);
            tempList.Add(s6);
            tempList.Add(s7);
            tempList.Add(s8);
            return tempList;
        }

        public ArrayList getSCube3() {
            Vertex s1 = new Vertex(2, 2, 4);
            Vertex s2 = new Vertex(6, 2, 4);
            Vertex s3 = new Vertex(6, 6, 4);
            Vertex s4 = new Vertex(2, 6, 4);
            Vertex s5 = new Vertex(2, 2, 0);
            Vertex s6 = new Vertex(6, 2, 0);
            Vertex s7 = new Vertex(6, 6, 0);
            Vertex s8 = new Vertex(2, 6, 0);
            ArrayList tempList = new ArrayList();
            tempList.Add(s1);
            tempList.Add(s2);
            tempList.Add(s3);
            tempList.Add(s4);
            tempList.Add(s5);
            tempList.Add(s6);
            tempList.Add(s7);
            tempList.Add(s8);
            return tempList;
        }

        public ArrayList getSCube4() {
            Vertex s1 = new Vertex(3, 2, 4);
            Vertex s2 = new Vertex(4, 2, 4);
            Vertex s3 = new Vertex(4, 3, 4);
            Vertex s4 = new Vertex(3, 3, 4);
            Vertex s5 = new Vertex(3, 2, 1);
            Vertex s6 = new Vertex(4, 2, 1);
            Vertex s7 = new Vertex(4, 3, 1);
            Vertex s8 = new Vertex(3, 3, 1);
            ArrayList tempList = new ArrayList();
            tempList.Add(s1);
            tempList.Add(s2);
            tempList.Add(s3);
            tempList.Add(s4);
            tempList.Add(s5);
            tempList.Add(s6);
            tempList.Add(s7);
            tempList.Add(s8);
            return tempList;
        }

        public ArrayList getSCube5() {
            Vertex s1 = new Vertex(2, 2, 4);
            Vertex s2 = new Vertex(3, 2, 4);
            Vertex s3 = new Vertex(3, 3, 4);
            Vertex s4 = new Vertex(2, 3, 4);
            Vertex s5 = new Vertex(2, 2, 1);
            Vertex s6 = new Vertex(3, 2, 1);
            Vertex s7 = new Vertex(3, 3, 1);
            Vertex s8 = new Vertex(2, 3, 1);
            ArrayList tempList = new ArrayList();
            tempList.Add(s1);
            tempList.Add(s2);
            tempList.Add(s3);
            tempList.Add(s4);
            tempList.Add(s5);
            tempList.Add(s6);
            tempList.Add(s7);
            tempList.Add(s8);
            return tempList;
        }

    }
}

