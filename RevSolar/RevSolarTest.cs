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

            BuildingVRMLNode revsolar1 = initBuilding.reverseEnvelope(shadow, 1);
            //BuildingVRMLNode revsolar1 = test.createSpecialCube(test.specialCube1());
            BuildingVRMLNode shadow2 = test.createCube(test.getSCube2());
            BuildingVRMLNode revsolar2 = revsolar1.reverseEnvelope(shadow2, 1);
            //initBuilding.printInfo("building");
            //shadow.printInfo("shadow");
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

        public BuildingVRMLNode createSpecialCube(ArrayList vertices) {
            BuildingVRMLNode building = new BuildingVRMLNode(vertices);

            ArrayList face1 = new ArrayList();
            ArrayList face2 = new ArrayList();
            ArrayList face3 = new ArrayList();
            ArrayList face4 = new ArrayList();
            ArrayList face5 = new ArrayList();
            ArrayList face6 = new ArrayList();
            ArrayList face7 = new ArrayList();
            ArrayList face8 = new ArrayList();
            ArrayList face9 = new ArrayList();
            ArrayList face10 = new ArrayList();
            ArrayList face11 = new ArrayList();
            ArrayList face12 = new ArrayList();
            ArrayList face13 = new ArrayList();
            ArrayList face14 = new ArrayList();

            face1.Add(0);
            face1.Add(1);
            face1.Add(9);
            face1.Add(8);
            building.addFace(face1);

            face2.Add(0);
            face2.Add(8);
            face2.Add(11);
            face2.Add(3);
            building.addFace(face2);

            face3.Add(3);
            face3.Add(11);
            face3.Add(10);
            face3.Add(2);
            building.addFace(face3);

            face4.Add(4);
            face4.Add(12);
            face4.Add(13);
            face4.Add(5);
            building.addFace(face4);

            face5.Add(4);
            face5.Add(7);
            face5.Add(15);
            face5.Add(12);
            building.addFace(face5);

            face6.Add(7);
            face6.Add(6);
            face6.Add(14);
            face6.Add(15);
            building.addFace(face6);



            face7.Add(0);
            face7.Add(4);
            face7.Add(5);
            face7.Add(1);
            building.addFace(face7);


            face8.Add(1);
            face8.Add(5);
            face8.Add(13);
            face8.Add(9);
            building.addFace(face8);


            face9.Add(10);
            face9.Add(14);
            face9.Add(6);
            face9.Add(2);
            building.addFace(face9);


            face10.Add(2);
            face10.Add(6);
            face10.Add(7);
            face10.Add(3);
            building.addFace(face10);


            face11.Add(0);
            face11.Add(3);
            face11.Add(7);
            face11.Add(4);
            building.addFace(face11);


            face12.Add(9);
            face12.Add(13);
            face12.Add(12);
            face12.Add(8);
            building.addFace(face12);


            face13.Add(8);
            face13.Add(12);
            face13.Add(15);
            face13.Add(11);
            building.addFace(face13);


            face14.Add(11);
            face14.Add(15);
            face14.Add(14);
            face14.Add(10);
            building.addFace(face14);
            return building;
        }

        public ArrayList specialCube1() {
            Vertex b1 = new Vertex(1, 1, 4);
            Vertex b2 = new Vertex(4, 1, 4);
            Vertex b3 = new Vertex(4, 4, 4);
            Vertex b4 = new Vertex(1, 4, 4);
            Vertex b5 = new Vertex(1, 1, 1);
            Vertex b6 = new Vertex(4, 1, 1);
            Vertex b7 = new Vertex(4, 4, 1);
            Vertex b8 = new Vertex(1, 4, 1);

            Vertex s1 = new Vertex(3, 2, 4);
            Vertex s2 = new Vertex(4, 2, 4);
            Vertex s3 = new Vertex(4, 3, 4);
            Vertex s4 = new Vertex(3, 3, 4);
            Vertex s5 = new Vertex(3, 2, 1);
            Vertex s6 = new Vertex(4, 2, 1);
            Vertex s7 = new Vertex(4, 3, 1);
            Vertex s8 = new Vertex(3, 3, 1);
            ArrayList tempList = new ArrayList();
            tempList.Add(b1);
            tempList.Add(b2);
            tempList.Add(b3);
            tempList.Add(b4);
            tempList.Add(b5);
            tempList.Add(b6);
            tempList.Add(b7);
            tempList.Add(b8);
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

