using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using test;

namespace Solar_Envelope_Test
{
    class Program
    {




        static void blah(string[] args)
        {
            int DEBUG = 2;

            if (DEBUG == 1) {

                ArrayList vertices = new ArrayList();

                Vertex v01 = new Vertex(381088.905867544, 3765322.47544753, -30.9);
                vertices.Add(v01);
                Vertex v02 = new Vertex(381132.677099786, 3765322.47544753, -30.9);
                vertices.Add(v02);
                Vertex v03 = new Vertex(381132.677099786, 3765372.37817687, -30.9);
                vertices.Add(v03);
                Vertex v04 = new Vertex(381088.905867544, 3765372.37817687, -30.9);
                vertices.Add(v04);
                Vertex v05 = new Vertex(381132.677099786, 3765372.37817687, -25.3034431105004);
                vertices.Add(v05);
                Vertex v06 = new Vertex(381088.905867544, 3765372.37817687, -24.3701175158597);
                vertices.Add(v06);
                Vertex v07 = new Vertex(381130.786072452, 3765372.37817687, -24.3701175158372);
                vertices.Add(v07);
                Vertex v08 = new Vertex(381132.677099786, 3765322.47544753, -25.3034431104898);
                vertices.Add(v08);
                Vertex v09 = new Vertex(381088.905867544, 3765322.47544753, -23.8949346077561);
                vertices.Add(v09);
                Vertex v10 = new Vertex(381109.803573542, 3765322.47544753, -14.0141047176196);
                vertices.Add(v10);
                Vertex v11 = new Vertex(381088.905867544, 3765371.2801001, -23.8949346077491);
                vertices.Add(v11);
                Vertex v12 = new Vertex(381109.803573542, 3765348.44697708, -14.0141047176187);
                vertices.Add(v12);
                BuildingVRMLNode temp4 = new BuildingVRMLNode(vertices);
                ArrayList face01 = new ArrayList();
                face01.Add(0);
                face01.Add(1);
                face01.Add(2);
                face01.Add(3);
                temp4.addFace(face01);

                ArrayList face02 = new ArrayList();
                face02.Add(4);
                face02.Add(2);
                face02.Add(3);
                face02.Add(5);
                face02.Add(6);
                temp4.addFace(face02);

                ArrayList face03 = new ArrayList();
                face03.Add(7);
                face03.Add(1);
                face03.Add(2);
                face03.Add(4);
                temp4.addFace(face03);

                ArrayList face04 = new ArrayList();
                face04.Add(8);
                face04.Add(0);
                face04.Add(1);
                face04.Add(7);
                face04.Add(9);
                temp4.addFace(face04);

                ArrayList face05 = new ArrayList();
                face05.Add(5);
                face05.Add(3);
                face05.Add(0);
                face05.Add(8);
                face05.Add(10);
                temp4.addFace(face05);

                ArrayList face06 = new ArrayList();
                face06.Add(10);
                face06.Add(8);
                face06.Add(9);
                face06.Add(11);
                temp4.addFace(face06);

                ArrayList face07 = new ArrayList();
                face07.Add(4);
                face07.Add(6);
                face07.Add(11);
                face07.Add(9);
                face07.Add(7);
                temp4.addFace(face07);

                ArrayList face08 = new ArrayList();
                face08.Add(5);
                face08.Add(6);
                face08.Add(11);
                face08.Add(10);
                temp4.addFace(face08);



                int filenum = 1;

                Vertex sun;
                Vertex p1;
                Vertex p2;

                //temp4.printInfo("temp4");
                filenum++;

                temp4.printInfo("b1");


                sun = new Vertex(374544.15625, 3758494.25, 2952.07836914063);
                p1 = new Vertex(381144.054461845, 3765370.17834478, -30.9);
                p2 = new Vertex(381144.054461845, 3765310.32355185, -30.9);



                BuildingVRMLNode b2 = temp4.calcVolume(sun, p1, p2);
                b2.printInfo("b2");


                /*
                sun = new Vertex(373218.53125, 3766113.25, 5876.4404296875);
                p1 = new Vertex(381087.350636482, 3765306.8599774, -30.9);
                p2 = new Vertex(381151.494647746, 3765306.8599774, -30.9);

                BuildingVRMLNode b3 = b2.calcVolume(sun, p1, p2);
                b3.printInfo("b3");

                


                sun = new Vertex(389100.8125, 3765906.5, 6184.083984375);
                p1 = new Vertex(381087.350636482, 3765306.8599774, -30.9);
                p2 = new Vertex(381151.494647746, 3765306.8599774, -30.9);


                BuildingVRMLNode b4 = b3.calcVolume(sun, p1, p2);
                b4.printInfo("b4");


                sun = new Vertex(389100.8125, 3765906.5, 6184.083984375);
                p1 = new Vertex(381087.350636482, 3765366.26230723, -30.9);
                p2 = new Vertex(381151.494647746, 3765366.26230723, -30.9);

                BuildingVRMLNode b5 = b4.calcVolume(sun, p1, p2);
                b5.printInfo("b5");



                sun = new Vertex(389100.8125, 3765906.5, 6184.083984375);
                p1 = new Vertex(381151.494647746, 3765306.8599774, -30.9);
                p2 = new Vertex(381151.494647746, 3765366.26230723, -30.9);

                BuildingVRMLNode b6 = b5.calcVolume(sun, p1, p2);
                b6.printInfo("b6");





                sun = new Vertex(372611.71875, 3762524, 4180.25634765625);
                p1 = new Vertex(381087.350636482, 3765306.8599774, -30.9);
                p2 = new Vertex(381151.494647746, 3765306.8599774, -30.9);


                BuildingVRMLNode b7 = b6.calcVolume(sun, p1, p2);
                b7.printInfo("b7");


                


                sun = new Vertex(385551.5625 , 3757447.25,  4499.42578125);
                p1 = new Vertex(381123.349363435 , 3765318.87607393 , -30.9);
                p2 = new Vertex(381079.021195213 , 3765318.87607393 , -30.9);

                BuildingVRMLNode b8 = b7.calcVolume(sun, p1, p2);
                b8.printInfo("b8");




                sun = new Vertex(374544.15625 , 3758494.25 , 2952.07836914063);
                p1 = new Vertex(381123.349363435 , 3765361.62203392 , -30.9);
                p2 = new Vertex(381123.349363435 , 3765318.87607393 , -30.9);

                BuildingVRMLNode b9 = b8.calcVolume(sun, p1, p2);
                b9.printInfo("b9");




                sun = new Vertex(374544.15625 , 3758494.25 , 2952.07836914063);
                p1 = new Vertex(381123.349363435 , 3765318.87607393 , -30.9);
                p2 = new Vertex(381079.021195213 , 3765318.87607393 , -30.9);

                BuildingVRMLNode b10 = b9.calcVolume(sun, p1, p2);
                b10.printInfo("b10");

                /*
                sun = new Vertex(374544.15625, 3758494.25, 2952.07836914063);
                p1 = new Vertex(381123.349363435, 3765361.62203392, -30.9);
                p2 = new Vertex(381123.349363435, 3765318.87607393, -30.9);

                BuildingVRMLNode b11 = b10.calcVolume(sun, p1, p2);
                b10.printInfo("b11");
                */


            }
            else if (DEBUG == 2) {

                ArrayList vertices = new ArrayList();

                /*
                // original building cube
                Vertex one = new Vertex(0, 0, 4);
                Vertex two = new Vertex(4, 0, 4);
                Vertex three = new Vertex(4, 4, 4);
                Vertex four = new Vertex(0, 4, 4);
                Vertex five = new Vertex(0, 0, 0);
                Vertex six = new Vertex(4, 0, 0);
                Vertex seven = new Vertex(4, 4, 0);
                Vertex eight = new Vertex(0, 4, 0);
                 */

                Vertex one = new Vertex(1, 1, 4);
                Vertex two = new Vertex(4, 1, 4);
                Vertex three = new Vertex(4, 4, 4);
                Vertex four = new Vertex(1, 4, 4);
                Vertex five = new Vertex(1, 1, 1);
                Vertex six = new Vertex(4, 1, 1);
                Vertex seven = new Vertex(4, 4, 1);
                Vertex eight = new Vertex(1, 4, 1);


                vertices.Add(one);
                vertices.Add(two);
                vertices.Add(three);
                vertices.Add(four);
                vertices.Add(five);
                vertices.Add(six);
                vertices.Add(seven);
                vertices.Add(eight);

                BuildingVRMLNode initBuilding = new BuildingVRMLNode(vertices);

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



                ArrayList svertices = new ArrayList();

                /*
                
                // first cube
                Vertex v1 = new Vertex(0, 2, 6);
                Vertex v2 = new Vertex(4, 2, 6);
                Vertex v3 = new Vertex(4, 6, 6);
                Vertex v4 = new Vertex(0, 6, 6);
                Vertex v5 = new Vertex(0, 2, 2);
                Vertex v6 = new Vertex(4, 2, 2);
                Vertex v7 = new Vertex(4, 6, 2);
                Vertex v8 = new Vertex(0, 6, 2);
                
                // second cube
                Vertex v1 = new Vertex(2, 2, 6);
                Vertex v2 = new Vertex(6, 2, 6);
                Vertex v3 = new Vertex(6, 6, 6);
                Vertex v4 = new Vertex(2, 6, 6);
                Vertex v5 = new Vertex(2, 2, 2);
                Vertex v6 = new Vertex(6, 2, 2);
                Vertex v7 = new Vertex(6, 6, 2);
                Vertex v8 = new Vertex(2, 6, 2);
                
                // third cube
                Vertex v1 = new Vertex(2, 2, 4);
                Vertex v2 = new Vertex(6, 2, 4);
                Vertex v3 = new Vertex(6, 6, 4);
                Vertex v4 = new Vertex(2, 6, 4);
                Vertex v5 = new Vertex(2, 2, 0);
                Vertex v6 = new Vertex(6, 2, 0);
                Vertex v7 = new Vertex(6, 6, 0);
                Vertex v8 = new Vertex(2, 6, 0);
                */
                Vertex v1 = new Vertex(3, 2, 4);
                Vertex v2 = new Vertex(4, 2, 4);
                Vertex v3 = new Vertex(4, 3, 4);
                Vertex v4 = new Vertex(3, 3, 4);
                Vertex v5 = new Vertex(3, 2, 1);
                Vertex v6 = new Vertex(4, 2, 1);
                Vertex v7 = new Vertex(4, 3, 1);
                Vertex v8 = new Vertex(3, 3, 1);

                
                svertices.Add(v1);
                svertices.Add(v2);
                svertices.Add(v3);
                svertices.Add(v4);
                svertices.Add(v5);
                svertices.Add(v6);
                svertices.Add(v7);
                svertices.Add(v8);

                BuildingVRMLNode shadow = new BuildingVRMLNode(svertices);


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


                /*
                ArrayList svertices = new ArrayList();

                Vertex v1 = new Vertex(5, 1, 3);
                Vertex v2 = new Vertex(5, 3, 3);
                Vertex v3 = new Vertex(5, 3, 0);
                Vertex v4 = new Vertex(5, 1, 0);
                Vertex v5 = new Vertex(2, 1, 0);
                Vertex v6 = new Vertex(2, 3, 0);

                svertices.Add(v1);
                svertices.Add(v2);
                svertices.Add(v3);
                svertices.Add(v4);
                svertices.Add(v5);
                svertices.Add(v6);

                BuildingVRMLNode shadow = new BuildingVRMLNode(svertices);


                ArrayList sface1 = new ArrayList();
                ArrayList sface2 = new ArrayList();
                ArrayList sface3 = new ArrayList();
                ArrayList sface4 = new ArrayList();
                ArrayList sface5 = new ArrayList();

                sface1.Add(0);
                sface1.Add(1);
                sface1.Add(2);
                sface1.Add(3);

                sface2.Add(1);
                sface2.Add(2);
                sface2.Add(5);

                sface3.Add(0);
                sface3.Add(3);
                sface3.Add(4);

                sface4.Add(0);
                sface4.Add(1);
                sface4.Add(5);
                sface4.Add(4);

                sface5.Add(3);
                sface5.Add(2);
                sface5.Add(5);
                sface5.Add(4);


                shadow.addFace(sface1);
                shadow.addFace(sface2);
                shadow.addFace(sface3);
                shadow.addFace(sface4);
                shadow.addFace(sface5);
                 */



                BuildingVRMLNode revsolar = initBuilding.reverseEnvelope(shadow);
                revsolar.printInfo("reverseSolar");

                /*


                ArrayList vertices = new ArrayList();
                Vertex one = new Vertex(0, 0, 0);
                Vertex two = new Vertex(4, 0, 0);
                Vertex three = new Vertex(4, 4, 0);
                Vertex four = new Vertex(0, 4, 0);
                Vertex five = new Vertex(0, 0, 4);
                Vertex six = new Vertex(4, 0, 4);
                Vertex seven = new Vertex(4, 4, 4);
                Vertex eight = new Vertex(0, 4, 4);

                vertices.Add(one);
                vertices.Add(two);
                vertices.Add(three);
                vertices.Add(four);
                vertices.Add(five);
                vertices.Add(six);
                vertices.Add(seven);
                vertices.Add(eight);

                BuildingVRMLNode temp4 = new BuildingVRMLNode(vertices);

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
                temp4.addFace(face1);

                face2.Add(4);
                face2.Add(5);
                face2.Add(6);
                face2.Add(7);
                temp4.addFace(face2);

                face3.Add(0);
                face3.Add(1);
                face3.Add(5);
                face3.Add(4);
                temp4.addFace(face3);

                face4.Add(1);
                face4.Add(2);
                face4.Add(6);
                face4.Add(5);
                temp4.addFace(face4);

                face5.Add(2);
                face5.Add(3);
                face5.Add(7);
                face5.Add(6);
                temp4.addFace(face5);

                face6.Add(0);
                face6.Add(3);
                face6.Add(7);
                face6.Add(4);
                temp4.addFace(face6);

                Vertex sun = new Vertex(5, 5, 5);
                Vertex p1 = new Vertex(-1, -1, 0);
                Vertex p2 = new Vertex(0, -1, 0);

                BuildingVRMLNode temp = temp4.calcVolume(sun, p1, p2);

                sun = new Vertex(5, 0, 5);
                p1 = new Vertex(-1, 3, 0);
                p2 = new Vertex(1, 5, 0);

                //sun = new Vertex(-1, -1, 5);
                //p1 = new Vertex(0, 5, 0);
                //p2 = new Vertex(4, 5, 0);


                BuildingVRMLNode final = temp.calcVolume(sun, p1, p2);

                Console.WriteLine("\nOriginal Pass");
                temp4.printInfo("temp4");
                Console.WriteLine("\nFirst Pass");
                //temp.printInfo("temp");
                Console.WriteLine("\n\n\nFinal Pass");
                //final.printInfo("final");

                /*
                ArrayList vertices = new ArrayList();
                Vertex one = new Vertex(0, 0, 0);
                Vertex two = new Vertex(4, 0, 0);
                Vertex three = new Vertex(4, 4, 0);
                Vertex four = new Vertex(0, 4, 0);
                Vertex five = new Vertex(2,2,4);

                vertices.Add(one);
                vertices.Add(two);
                vertices.Add(three);
                vertices.Add(four);
                vertices.Add(five);

                BuildingVRMLNode temp4 = new BuildingVRMLNode(vertices);

                ArrayList face1 = new ArrayList();
                ArrayList face2 = new ArrayList();
                ArrayList face3 = new ArrayList();
                ArrayList face4 = new ArrayList();
                ArrayList face5 = new ArrayList();

                face1.Add(0);
                face1.Add(1);
                face1.Add(2);
                face1.Add(3);
                temp4.addFace(face1);

                face2.Add(0);
                face2.Add(1);
                face2.Add(4);
                temp4.addFace(face2);

                face3.Add(1);
                face3.Add(2);
                face3.Add(4);
                temp4.addFace(face3);

                face4.Add(2);
                face4.Add(3);
                face4.Add(4);
                temp4.addFace(face4);

                face5.Add(3);
                face5.Add(0);
                face5.Add(4);
                temp4.addFace(face5);

                Vertex sun = new Vertex(5, 5, 5);
                Vertex p1 = new Vertex(-1, -1, 0);
                Vertex p2 = new Vertex(0, -1, 0);

                BuildingVRMLNode temp = temp4.calcVolume(sun, p1, p2);

                sun = new Vertex(5, 0, 5);
                p1 = new Vertex(-1, 3, 0);
                p2 = new Vertex(1, 5, 0);

                //sun = new Vertex(-1, -1, 5);
                //p1 = new Vertex(0, 5, 0);
                //p2 = new Vertex(4, 5, 0);


                BuildingVRMLNode final = temp.calcVolume(sun, p1, p2);

                Console.WriteLine("\nOriginal Pass");
                temp4.printInfo();
                Console.WriteLine("\nFirst Pass");
                temp.printInfo();
                Console.WriteLine("\n\n\nFinal Pass");
                final.printInfo();
                */


            }
            else {

                ArrayList vertices = new ArrayList();
                vertices.Add(new Vertex(1, 1, 1));
                vertices.Add(new Vertex(2, 2, 2));
                vertices.Add(new Vertex(3, 3, 3));
                vertices.Add(new Vertex(4, 4, 4));
                vertices.Add(new Vertex(5, 5, 5));
                vertices.Add(new Vertex(6, 6, 6));

                ArrayList temp = new ArrayList();
                temp.Add((Vertex)vertices[1]);
                temp.Add((Vertex)vertices[3]);
                temp.Add((Vertex)vertices[5]);

                Console.WriteLine(vertices.IndexOf((Vertex)temp[0]));
                Console.WriteLine(vertices.IndexOf((Vertex)temp[1]));
                Console.WriteLine(vertices.IndexOf((Vertex)temp[2]));



                for (int x = 0; x < vertices.Count; x++) {
                    Console.WriteLine("{0} {1} {2}", ((Vertex)vertices[x]).GetX(), ((Vertex)vertices[x]).GetY(), ((Vertex)vertices[x]).GetZ());
                }
                Console.WriteLine();
                Polygon poly = new Polygon(vertices);
                for (int x = 0; x < poly.getSize(); x++) {
                    Vertex v = poly.getVertex(x);
                    v.coords[0] = 0;
                }

                for (int x = 0; x < vertices.Count; x++) {
                    Console.WriteLine("{0} {1} {2}", ((Vertex)vertices[x]).GetX(), ((Vertex)vertices[x]).GetY(), ((Vertex)vertices[x]).GetZ());
                }
                Console.WriteLine();
                Polygon copy = new Polygon(poly);
                for (int x = 0; x < copy.getSize(); x++) {
                    Vertex v = copy.getVertex(x);
                    v.coords[1] = 0;
                }

                for (int x = 0; x < vertices.Count; x++) {
                    Console.WriteLine("{0} {1} {2}", ((Vertex)vertices[x]).GetX(), ((Vertex)vertices[x]).GetY(), ((Vertex)vertices[x]).GetZ());
                }

            }




        }
    }
}

