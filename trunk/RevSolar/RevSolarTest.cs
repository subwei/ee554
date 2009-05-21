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

            BuildingVRMLNode bcube1 = test.createCube(test.getBCube1());
            BuildingVRMLNode bcube2 = test.createCube(test.getBCube2());
            BuildingVRMLNode bcube3 = test.createCube(test.getBCube3());
            BuildingVRMLNode scube1 = test.createCube(test.getSCube1());
            BuildingVRMLNode scube2 = test.createCube(test.getSCube2());
            BuildingVRMLNode scube3 = test.createCube(test.getSCube3());
            BuildingVRMLNode scube4 = test.createCube(test.getSCube4());
            BuildingVRMLNode scube5 = test.createCube(test.getSCube5());
            BuildingVRMLNode ramp1 = test.getRamp1();
            BuildingVRMLNode specialCube = test.createSpecialCube(test.specialCube1());

            if (true) {
                BuildingVRMLNode btest = test.getBuildingTest();
                BuildingVRMLNode stest = test.getShadowTest();

                //btest.printInfo("buildingtest");
                //stest.printInfo("stest");
                BuildingVRMLNode final = btest.reverseEnvelope(stest, 1);
                final.printInfo("final");
            }
            else {

                // BASIC TEST CASES
                
                BuildingVRMLNode revsolar1 = bcube1.reverseEnvelope(scube1, 1);
                BuildingVRMLNode revsolar2 = bcube1.reverseEnvelope(scube2, 1);
                BuildingVRMLNode revsolar3 = bcube1.reverseEnvelope(scube3, 1);
                BuildingVRMLNode revsolar4 = bcube2.reverseEnvelope(scube4, 1);
                BuildingVRMLNode revsolar5 = bcube2.reverseEnvelope(scube5, 1);

                // COMPOUND CASES (2 iterations)
                BuildingVRMLNode revsolar6 = revsolar4.reverseEnvelope(scube2, 1);
                BuildingVRMLNode revsolar7 = revsolar5.reverseEnvelope(scube2, 1);

                
                // RAMP CASES (3 iterations)

                BuildingVRMLNode revsolar8 = specialCube.reverseEnvelope(ramp1, 1);
                //BuildingVRMLNode revsolar9 = revsolar6.reverseEnvelope(ramp1, 1);
                //BuildingVRMLNode revsolar10 = revsolar7.reverseEnvelope(ramp1, 1);


                /*revsolar1.printInfo("revsolar1");
                revsolar2.printInfo("revsolar2");
                revsolar3.printInfo("revsolar3");
                revsolar4.printInfo("revsolar4");
                revsolar5.printInfo("revsolar5");
                revsolar6.printInfo("revsolar6");
                revsolar7.printInfo("revsolar7");*/
                revsolar8.printInfo("revsolar8");
                //revsolar9.printInfo("revsolar9");
                specialCube.printInfo("special");
                //revsolar10.printInfo("revsolar10");
                //revsolar11.printInfo("revsolar11");
            }
             
        }


        public BuildingVRMLNode createFaultyCube(ArrayList vertices) {

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
            //building.addFace(face1);

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


        public BuildingVRMLNode getRamp1() {
            Vertex s1 = new Vertex(5, 0, 3);
            Vertex s2 = new Vertex(5, 0, -1);
            Vertex s3 = new Vertex(5, 5, -1);
            Vertex s4 = new Vertex(5, 5, 3);
            Vertex s5 = new Vertex(-2, 0, -1);
            Vertex s6 = new Vertex(-2, 5, -1);
            ArrayList tempList = new ArrayList();
            tempList.Add(s1);
            tempList.Add(s2);
            tempList.Add(s3);
            tempList.Add(s4);
            tempList.Add(s5);
            tempList.Add(s6);

            BuildingVRMLNode building = new BuildingVRMLNode(tempList);
            ArrayList face1 = new ArrayList();
            ArrayList face2 = new ArrayList();
            ArrayList face3 = new ArrayList();
            ArrayList face4 = new ArrayList();
            ArrayList face5 = new ArrayList();

            face1.Add(0);
            face1.Add(1);
            face1.Add(2);
            face1.Add(3);
            building.addFace(face1);

            face2.Add(5);
            face2.Add(2);
            face2.Add(1);
            face2.Add(4);
            building.addFace(face2);

            face3.Add(3);
            face3.Add(2);
            face3.Add(5);
            building.addFace(face3);

            face4.Add(0);
            face4.Add(4);
            face4.Add(1);
            building.addFace(face4);

            face5.Add(3);
            face5.Add(5);
            face5.Add(4);
            face5.Add(0);
            building.addFace(face5);

            return building;
        }

        public BuildingVRMLNode getShadowTest() {

            ArrayList vertices = new ArrayList();
            Vertex v01 = new Vertex(381184, 3765505, -22);
            vertices.Add(v01);
            Vertex v02 = new Vertex(381184, 3765505, -32);
            vertices.Add(v02);
            Vertex v03 = new Vertex(381172, 3765482, -22);
            vertices.Add(v03);
            Vertex v04 = new Vertex(381172, 3765482, -32);
            vertices.Add(v04);
            Vertex v05 = new Vertex(381122, 3765510, -22);
            vertices.Add(v05);
            Vertex v06 = new Vertex(381122, 3765510, -32);
            vertices.Add(v06);
            Vertex v07 = new Vertex(381134, 3765533, -22);
            vertices.Add(v07);
            Vertex v08 = new Vertex(381134, 3765533, -32);
            vertices.Add(v08);
            Vertex v09 = new Vertex(381104.52801378, 3765543.11772407, -36);
            vertices.Add(v09);
            Vertex v10 = new Vertex(381092.487807574, 3765520.04066218, -36);
            vertices.Add(v10);
            Vertex v11 = new Vertex(381154.695539636, 3765515.02390959, -36);
            vertices.Add(v11);
            Vertex v12 = new Vertex(381142.65533343, 3765491.9468477, -36);
            vertices.Add(v12);
            Vertex v13 = new Vertex(381175.647286973, 3765507.8571364, -36);
            vertices.Add(v13);
            Vertex v14 = new Vertex(381163.635826912, 3765484.83517128, -36);
            vertices.Add(v14);
            Vertex v15 = new Vertex(381125.59953672, 3765535.88387654, -36);
            vertices.Add(v15);
            BuildingVRMLNode temp4 = new BuildingVRMLNode(vertices);
            ArrayList face01 = new ArrayList();
            face01.Add(6);
            face01.Add(4);
            face01.Add(2);
            face01.Add(0);
            temp4.addFace(face01);

            ArrayList face02 = new ArrayList();
            face02.Add(4);
            face02.Add(6);
            face02.Add(8);
            face02.Add(9);
            temp4.addFace(face02);

            ArrayList face03 = new ArrayList();
            face03.Add(6);
            face03.Add(0);
            face03.Add(10);
            face03.Add(8);
            temp4.addFace(face03);

            ArrayList face04 = new ArrayList();
            face04.Add(0);
            face04.Add(2);
            face04.Add(11);
            face04.Add(10);
            temp4.addFace(face04);

            ArrayList face05 = new ArrayList();
            face05.Add(2);
            face05.Add(4);
            face05.Add(9);
            face05.Add(11);
            temp4.addFace(face05);

            ArrayList face06 = new ArrayList();
            face06.Add(2);
            face06.Add(3);
            face06.Add(1);
            face06.Add(0);
            temp4.addFace(face06);

            ArrayList face07 = new ArrayList();
            face07.Add(4);
            face07.Add(6);
            face07.Add(8);
            face07.Add(9);
            temp4.addFace(face07);

            ArrayList face08 = new ArrayList();
            face08.Add(6);
            face08.Add(0);
            face08.Add(10);
            face08.Add(8);
            temp4.addFace(face08);

            ArrayList face09 = new ArrayList();
            face09.Add(0);
            face09.Add(1);
            face09.Add(12);
            face09.Add(10);
            temp4.addFace(face09);

            ArrayList face10 = new ArrayList();
            face10.Add(1);
            face10.Add(3);
            face10.Add(13);
            face10.Add(12);
            temp4.addFace(face10);

            ArrayList face11 = new ArrayList();
            face11.Add(3);
            face11.Add(2);
            face11.Add(11);
            face11.Add(13);
            temp4.addFace(face11);

            ArrayList face12 = new ArrayList();
            face12.Add(2);
            face12.Add(4);
            face12.Add(9);
            face12.Add(11);
            temp4.addFace(face12);

            ArrayList face13 = new ArrayList();
            face13.Add(0);
            face13.Add(1);
            face13.Add(7);
            face13.Add(6);
            temp4.addFace(face13);

            ArrayList face14 = new ArrayList();
            face14.Add(4);
            face14.Add(6);
            face14.Add(8);
            face14.Add(9);
            temp4.addFace(face14);

            ArrayList face15 = new ArrayList();
            face15.Add(6);
            face15.Add(7);
            face15.Add(14);
            face15.Add(8);
            temp4.addFace(face15);

            ArrayList face16 = new ArrayList();
            face16.Add(7);
            face16.Add(1);
            face16.Add(12);
            face16.Add(14);
            temp4.addFace(face16);

            ArrayList face17 = new ArrayList();
            face17.Add(1);
            face17.Add(3);
            face17.Add(13);
            face17.Add(12);
            temp4.addFace(face17);

            ArrayList face18 = new ArrayList();
            face18.Add(3);
            face18.Add(2);
            face18.Add(11);
            face18.Add(13);
            temp4.addFace(face18);

            ArrayList face19 = new ArrayList();
            face19.Add(2);
            face19.Add(4);
            face19.Add(9);
            face19.Add(11);
            temp4.addFace(face19);

            ArrayList face20 = new ArrayList();
            face20.Add(9);
            face20.Add(8);
            face20.Add(10);
            face20.Add(11);
            face20.Add(12);
            face20.Add(13);
            face20.Add(14);
            temp4.addFace(face20);

            return temp4;
        }

        public BuildingVRMLNode getBuildingTest() {

            ArrayList vertices = new ArrayList();
            Vertex v01 = new Vertex(381074.676981519, 3765531.23075329, 14.1);
            vertices.Add(v01);
            Vertex v02 = new Vertex(381117.288085574, 3765531.23075329, 14.1);
            vertices.Add(v02);
            Vertex v03 = new Vertex(381117.288085574, 3765490.31378404, 14.1);
            vertices.Add(v03);
            Vertex v04 = new Vertex(381074.676981519, 3765490.31378404, 14.1);
            vertices.Add(v04);
            Vertex v05 = new Vertex(381074.676981519, 3765531.23075329, -35.9);
            vertices.Add(v05);
            Vertex v06 = new Vertex(381117.288085574, 3765531.23075329, -35.9);
            vertices.Add(v06);
            Vertex v07 = new Vertex(381117.288085574, 3765490.31378404, -35.9);
            vertices.Add(v07);
            Vertex v08 = new Vertex(381074.676981519, 3765490.31378404, -35.9);
            vertices.Add(v08);
            BuildingVRMLNode temp4 = new BuildingVRMLNode(vertices);
            ArrayList face01 = new ArrayList();
            face01.Add(3);
            face01.Add(2);
            face01.Add(1);
            face01.Add(0);
            temp4.addFace(face01);

            ArrayList face02 = new ArrayList();
            face02.Add(7);
            face02.Add(6);
            face02.Add(5);
            face02.Add(4);
            temp4.addFace(face02);

            ArrayList face03 = new ArrayList();
            face03.Add(1);
            face03.Add(5);
            face03.Add(4);
            face03.Add(0);
            temp4.addFace(face03);

            ArrayList face04 = new ArrayList();
            face04.Add(2);
            face04.Add(6);
            face04.Add(5);
            face04.Add(1);
            temp4.addFace(face04);

            ArrayList face05 = new ArrayList();
            face05.Add(3);
            face05.Add(7);
            face05.Add(6);
            face05.Add(2);
            temp4.addFace(face05);

            ArrayList face06 = new ArrayList();
            face06.Add(0);
            face06.Add(4);
            face06.Add(7);
            face06.Add(3);
            temp4.addFace(face06);

            return temp4;
        }

    }
}

