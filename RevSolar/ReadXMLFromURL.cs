//taken from http://support.microsoft.com/default.aspx?scid=kb;en-us;307643

using System;
using System.Xml;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;

namespace test
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    public class ReadXMLFromURL
    {
        static ArrayList RequestList = null; // a list of requests for line of sight
        public ReadXMLFromURL(string url)
        {
        }
        public static void readVRMLFile(string URLString, ref Hashtable nodes, double xOffset, double yOffset)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                System.IO.FileInfo objFile = new System.IO.FileInfo(URLString);

                if (objFile.Exists)
                {
                    doc.Load(URLString);
                    ArrayList coordIndexes = new ArrayList();
                    char[] delimiters = { ' ', ',', ':', '\t', '\n', '\r' };
                    XmlElement root = doc.DocumentElement;
                    XmlNodeList nodeList = root.SelectNodes("/doc/Shape");
                    string[] elements;
                    bool first = true;
                    XmlNode temp = null;
                    int key = -1;

                    foreach (XmlNode building in nodeList)
                    {
                        Node b;
                        if (first)
                        {
                            b = new GroundNode();
                            key = 0;
                            first = false;
                        }
                        else
                        {
                            b = new BuildingVRMLNode();
                            key++;
                        }

                        if (building.HasChildNodes)
                        {
                            for (int j = 0; j < building.ChildNodes.Count; j++)
                            {
                                XmlNode child = building.ChildNodes[j];

                                //parse the UNIQUE_ID of the building for the hashtable key
                                if (child.LocalName.ToUpper().Equals("UNIQUE_ID")) //jenny 10.27
                                {
                                    ((BuildingVRMLNode)b).nodeKey = child.InnerText;
                                }

                                else if (child.LocalName.ToUpper().Equals("ID")) //jenny 10.27
                                {
                                    ((BuildingVRMLNode)b).buildingId = Int32.Parse(child.InnerText);
                                    key = ((BuildingVRMLNode)b).buildingId;

                                }

                                //parse IndexedFaceSet element
                                else if (child.LocalName.Equals("IndexedFaceSet"))
                                {
                                    //IndexedFaceSet element has attributes
                                    XmlAttributeCollection lst = child.Attributes;
                                    elements = lst.GetNamedItem("coordIndex").InnerText.Split(delimiters);
                                    int i = 0;
                                    double x = -1, y = -1, z = -1;
                                    ArrayList f = new ArrayList();
                                    foreach (string s in elements)
                                    {
                                        if (!s.Equals(""))
                                        {
                                            int fIndex = Int32.Parse(s);
                                            if (fIndex != -1)
                                            {
                                                f.Add(fIndex);

                                            }
                                            else
                                            {
                                                b.addFace(f);
                                                f = new ArrayList();
                                            }

                                        }

                                    }
                                    //IndexedFaceSet element also contain other elements (e.g. Coordinate)
                                    temp = building.SelectSingleNode("IndexedFaceSet");
                                    if (temp.HasChildNodes)
                                    {
                                        for (int k = 0; k < temp.ChildNodes.Count; k++)
                                        {
                                            XmlNode child2 = temp.ChildNodes[k];
                                            if (child2.LocalName.Equals("Coordinate"))
                                            {
                                                lst = child2.Attributes;
                                                elements = lst.GetNamedItem("point").InnerText.Split(delimiters);
                                                i = 0;
                                                x = -1;
                                                y = -1;
                                                z = -1;
                                                foreach (string s in elements)
                                                {
                                                    if (!s.Equals(""))
                                                    {

                                                        switch (i % 3)
                                                        {
                                                            case 0:
                                                                x = Double.Parse(s); break;
                                                            case 1:
                                                                y = Double.Parse(s); break;
                                                            case 2:
                                                                z = Double.Parse(s);
                                                             
                                                                b.addVertex(new Vertex(x+xOffset, y+yOffset, z));
                                                                break;
                                                        }
                                                        i++;
                                                    }
                                                }
                                            }
                                            if (child2.LocalName.Equals("TextureCoordinate"))
                                            {
                                                lst = child2.Attributes;
                                                elements = lst.GetNamedItem("point").InnerText.Split(delimiters);
                                                i = 0;
                                                x = -1;
                                                y = -1;
                                                z = -1;
                                                foreach (string s in elements)
                                                {
                                                    if (!s.Equals(""))
                                                    {

                                                        switch (i % 2)
                                                        {
                                                            case 0:
                                                                x = Double.Parse(s); break;
                                                            case 1:
                                                                y = Double.Parse(s);
                                                                b.addTextureCoord(new TexCoord(x, y));
                                                                break;
                                                        }
                                                        i++;

                                                    }

                                                }

                                            }

                                        }
                                    }
                                }
                            }
                            if (!nodes.Contains(key))
                            {
                                nodes.Add(key, b);
                            }
                        }
                    }


                }
                else
                {
                     System.Windows.Forms.MessageBox.Show("XML File Not Exists in readVRMLFile.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readVRMLFile.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }


        }

        public static String getTextValue(XmlElement ele, String tagName)
        {
            String textVal = null;
            XmlNodeList nl = ele.GetElementsByTagName(tagName);
            if (nl != null && nl.Count > 0)
            {
                XmlElement el = (XmlElement)nl.Item(0);
                textVal = el.FirstChild.Value;
            }

            return textVal;
        }

        public static void readVectorDataURL(String URLString, Hashtable h, Font f) 
        {
            // ArrayList result = new ArrayList();
            XmlUrlResolver resolver = new XmlUrlResolver();
            NetworkCredential myCred;
            myCred = new System.Net.NetworkCredential("stdbm", "navteq");
            resolver.Credentials = myCred;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = resolver;
            try
            {
                // Create the reader.
                XmlReader reader = XmlReader.Create(URLString, settings);
                string name = null, color = null;
                string[] coordinates = null;
                Hashtable positions = new Hashtable();
                XmlDocument doc = new XmlDocument();

                doc.Load(reader);

                char[] delimiters = { ' ', ',', ':', '\t', '\n', '\r' };
                XmlElement root = doc.DocumentElement;
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                //nsmgr.AddNamespace(String.Empty, "");
                nsmgr.AddNamespace("spaces", "http://earth.google.com/kml/2.0");

                //XmlNodeList FolderList = root.SelectNodes("spaces:Folder/spaces:Folder", nsmgr);
                XmlNodeList nodeList = root.SelectNodes("spaces:Folder/spaces:Folder/spaces:Placemark", nsmgr); ;
                //foreach (XmlNode foldernode in FolderList)
                //{
                //    if (level == 4)
                //       if (foldernode.SelectSingleNode("spaces:name", nsmgr).InnerText.Equals("Level_4"))
                //           nodeList = foldernode.SelectNodes("spaces:Placemark", nsmgr);
                           

                //}

                //XmlNodeList nodeList = root.SelectNodes("Placemark");
                XmlNode temp = null;

                foreach (XmlNode placemark in nodeList)
                {
                    temp = placemark.SelectSingleNode("spaces:name", nsmgr);
                    name = temp.InnerText;
                    VectorDataNode v = new VectorDataNode(name, f);
                    temp = placemark.SelectSingleNode("spaces:Style/spaces:LineStyle/spaces:color", nsmgr);
                    color = temp.InnerText;
                    v.setVectorColor(color);
                    temp = placemark.SelectSingleNode("spaces:LineString/spaces:coordinates", nsmgr);
                    coordinates = temp.InnerText.Split(delimiters);
                    int i = 0;
                    double x = -1;
                    double y = -1;
                    double z = -1;
                    foreach (string s in coordinates)
                    {
                        if (!s.Equals(""))
                        {

                            switch (i % 3)
                            {
                                case 0:
                                    x = Double.Parse(s); break;
                                case 1:
                                    y = Double.Parse(s); break;
                                case 2:
                                    z = Double.Parse(s);
                                    UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, y, x);
                                    //converter1.UTMtoLL(UTMConverter.RefEllipsoid, converter1.UTMNorthing, converter1.UTMEasting, "11S", ref Latx, ref longx);
                                    v.addVertex(new Vertex(UTMConverter.UTMEasting, UTMConverter.UTMNorthing, z));
                                    break;
                            }
                            i++;
                        }
                    }
                    if (h.ContainsKey(v.name))
                    {
                        ArrayList segments = (ArrayList)h[v.name];
                        v.setId();
                        segments.Add(v);
                    }
                    else
                    {
                        //h.Add(v.name, v);
                        if (!positions.Contains(x.ToString() + y.ToString()))
                        {
                            v.setLabel(true);
                            positions.Add(x.ToString() + y.ToString(), x.ToString() + y.ToString());
                        }
                        ArrayList segments = new ArrayList();
                        v.setId();
                        segments.Add(v);
                        h[v.name] = segments;

                    }

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readVectorDataURL.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static void readTramURL(String URLString, Hashtable h, Font f)
        {
            
            // ArrayList result = new ArrayList();
            XmlTextReader reader = new XmlTextReader(URLString);
            TramNode t = null;

            try
            {
                string name = null;
                double longitude = 0d, latitude = 0d;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("name"))
                        {
                            name = reader.ReadElementString("name");
                        }

                        if (reader.Name.Equals("longitude"))
                        {
                            longitude = Double.Parse(reader.ReadElementString("longitude"));
                        }

                        if (reader.Name.Equals("latitude"))
                        {
                            latitude = Double.Parse(reader.ReadElementString("latitude"));
                            if (h.ContainsKey(name))
                            {
                                ((TramNode)h[name]).latitude = latitude;
                                ((TramNode)h[name]).longitude = longitude;
                                ((TramNode)h[name]).updateVertex(latitude, longitude);
                            }
                            else
                            {
                                t = new TramNode(name, f);
                                t.addVertex(latitude, longitude);
                                t.setId();
                                h.Add(t.getTramName(), t);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readTramURL.");
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static void readTrajectoryURL(String URLString, ArrayList TramNodes, ArrayList Trajectory, ArrayList VideoDataNodes, Font f, ref double latitude1, ref double latitude2, ref double longitude1, ref double longitude2)
        {
            XmlDocument doc = new XmlDocument();
            TramNode t = null;
            PathNode p = null;
           // VideoDataNode v = null;
            char[] delimiters = { ',', ':', '\t', '\n', '\r' };
            string coords = null;
            string[] coord = null;

            String filename = null, ip = null;
            DateTime from = new DateTime(), to = new DateTime();
            char[] delimiters2 = { '<', '>' };

            try
            {
                doc.Load(URLString);
                XmlElement root = doc.DocumentElement;
                XmlElement folder = (XmlElement)root.FirstChild;
                XmlNodeList list = folder.ChildNodes;
                XmlElement document = (XmlElement)list.Item(0);
                XmlElement folder2 = (XmlElement)document.NextSibling;

                XmlNodeList color_nodes = document.GetElementsByTagName("color");
                if (color_nodes != null && color_nodes.Count > 0)
                {
                    XmlElement el = (XmlElement)color_nodes.Item(0);
                    TramTrajectoryManager.color = el.InnerText;
                }

                XmlNodeList width_nodes = document.GetElementsByTagName("width");
                if (width_nodes != null && width_nodes.Count > 0)
                {
                    XmlElement el = (XmlElement)width_nodes.Item(0);
                    TramTrajectoryManager.width = int.Parse(el.InnerText);
                }

                XmlNodeList coord_nodes = document.GetElementsByTagName("coordinates");
                if (coord_nodes != null && coord_nodes.Count > 0)
                {
                    XmlElement el = (XmlElement)coord_nodes.Item(0);
                    coords = el.InnerText.Trim();
                }

                XmlNodeList videos = folder2.GetElementsByTagName("Placemark");
                if (videos != null && videos.Count > 0)
                {
                    for (int j = 0; j < videos.Count; j++)
                    {
                        XmlElement el = (XmlElement)videos.Item(j);
                        String description =  getTextValue(el, "description");
                        String[] s = description.Split(delimiters2, System.StringSplitOptions.RemoveEmptyEntries);

                        for (int m = 0; m < s.Length; m++)
                        {
                            switch (s[m])
                            {
                                case "Filename":
                                    filename = s[m + 1];
                                    filename = filename.Trim();
                                    break;
                                case "Ip":
                                    ip = s[m + 1];
                                    break;
                                case "Start":
                                    from = DateTime.ParseExact(s[m + 1], "dd-MMM-yy hh.mm.ss.000000 tt", null);
                                    break;
                                case "End":
                                    to = DateTime.ParseExact(s[m + 1], "dd-MMM-yy hh.mm.ss.000000 tt", null);
                                    break;
                            }
                        }

                        VideoDataNode vid = new VideoDataNode(filename, ip, from, to, f);
                        string coordinates = getTextValue(el, "coordinates");
                        string[] split = coordinates.Split(',');

                        double longitude = double.Parse(split[0]);
                        double latitude = double.Parse(split[1]);
                        vid.addVertex(latitude, longitude);

                        vid.setId();
                        VideoDataNodes.Add(vid);                
                    }
                }


                int i = 0;
                coords = coords.Replace("\"", "");
                coord = coords.Split(delimiters);
                foreach (string s in coord)
                {
                    if (!s.Equals(""))
                    {
                        switch (i % 3)
                        {
                            case 0:
                                t = new TramNode(TramTrajectoryManager.TramName, f);
                                p = new PathNode();
                                t.longitude = Double.Parse(s);
                                p.longitude = Double.Parse(s);
                                if (longitude1 == 0 && longitude2 == 0)
                                {
                                    longitude1 = t.longitude;
                                    longitude2 = t.longitude;
                                }
                                else if (t.longitude < longitude1) longitude1 = t.longitude;
                                else if (t.longitude > longitude2) longitude2 = t.longitude;
                                break;

                            case 1:
                                t.latitude = Double.Parse(s);
                                p.latitude = Double.Parse(s);

                              //  TramNodes.Add(t);
                                t.addVertex(t.latitude, t.longitude);
                                Trajectory.Add(p);
                                p.addVertex(p.latitude, p.longitude);

                                if (latitude1 == 0 && latitude2 == 0)
                                {
                                    latitude1 = t.latitude;
                                    latitude2 = t.latitude;
                                }
                                else if (t.latitude < latitude1) latitude1 = t.latitude;
                                else if (t.latitude > latitude2) latitude2 = t.latitude;
                                break;

                            case 2:
                                t.TramTime = DateTime.ParseExact(s, "dd-MMM-yy hh.mm.ss.000000 tt", null);
                               // t.line_of_sight = true;
                               // t.tram_sight = new sight(t.latitude, t.longitude);
                               
                                TramNodes.Add(t);
                                break;
                        }
                        i++;
                    }
                }



            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readTrajectoryURL.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }



            return;
        }

        public static void readVideoMetadataURL(String URLString, Hashtable h, Font f)
        {
            XmlTextReader reader = null;
            String filename = null, ip = null;
            DateTime from = new DateTime(), to = new DateTime();
            char[] delimiters = { '<', '>' };


            try
            {
                reader = new XmlTextReader(URLString);
                reader.WhitespaceHandling = WhitespaceHandling.None;
                reader.MoveToContent();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Filename")
                            {
                                filename = reader.ReadElementString("Filename");
                                filename = filename.Trim();
                            }
                            if (reader.Name == "Start")
                            {
                                string s1 = reader.ReadElementString("Start");
                                from = DateTime.ParseExact(s1, "dd-MMM-yy hh.mm.ss.000000 tt", null);

                            }
                            if (reader.Name == "End")
                            {
                                string s2 = reader.ReadElementString("End");
                                to = DateTime.ParseExact(s2, "dd-MMM-yy hh.mm.ss.000000 tt", null);

                            }
                            if (reader.Name == "Ip")
                            {
                                ip = reader.ReadElementString("Ip");
                            }

                            if (reader.Name == "coordinates")
                            {
                                string coord = reader.ReadElementString("coordinates");
                                string[] split = coord.Split(',');

                                VideoDataNode vid = new VideoDataNode(filename, ip, from, to, f);
                                double longitude = double.Parse(split[0]);
                                double latitude = double.Parse(split[1]);
                                vid.addVertex(latitude, longitude);


                                if (h.ContainsKey(vid.filename))
                                {
                                    VideoDataNode v1 = (VideoDataNode)h[vid.filename];
                                    if (vid.from < v1.from) v1.from = vid.from;
                                    if (vid.to > v1.to) v1.to = vid.to;
                                }
                                else
                                {
                                    vid.setId();
                                    h.Add(vid.filename, vid);
                                }
                            }


                            break;
                    }
                }
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        //Jenny Update 5.23.06
        public static void readEventListMetadataURL(String URLString, ArrayList eventList, ArrayList eventListTimes, Font f)
        {
            // ArrayList result = new ArrayList();
            XmlTextReader reader = new XmlTextReader(URLString);
            DateTime eventTime;
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // The node is an element.
                            if (reader.Name.Equals("NAME"))
                            {
                                string nameTag = reader.ReadElementString("NAME");
                                string dateTag = reader.ReadElementString("DATE");
                                string timeTag = reader.ReadElementString("TIME");
                                string locationTag = reader.ReadElementString("LOCATION");
                                double latitudeTag = Double.Parse(reader.ReadElementString("LATITUDE"));
                                double longitudeTag = Double.Parse(reader.ReadElementString("LONGITUDE"));
                                string typeTag = reader.ReadElementString("TYPE");

                                if (timeTag.Contains("TBA"))
                                    continue;

                                //convert time
                                eventTime = DateTime.Parse(dateTag + " " + timeTag);
                               //add to eventListTimes for internal
                                eventListTimes.Add(eventTime);

                                //add to eventList for display                            
                                String s = nameTag + " - " + eventTime;
                                eventList.Add(s);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readEventListMetadataURL.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static void readPointDataURL(String URLString, Hashtable h, Font f)
        {
            XmlTextReader reader = null;
            char[] delimiters = { ',' };
            char[] delimiters2 = { '-' };
            string[] split = null;
            try
            {
                reader = new XmlTextReader(URLString);
                PointDataNode t = null;

                try
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name.Equals("name"))
                            {
                                string name = reader.ReadElementString("name");
                                split = name.Split(delimiters2);   // Songhua_514
                                name = split[0];
                                t = new PointDataNode(name, f);
                            }

                            if (reader.Name.Equals("coordinates"))
                            {
                                string coord = reader.ReadElementString("coordinates");
                                split = coord.Split(delimiters);

                                t.longitude = Double.Parse(split[0]);
                                t.latitude = Double.Parse(split[1]);
                                double z = Double.Parse(split[2]);
                                t.addVertex(t.latitude, t.longitude);
                                if (!h.ContainsKey(t.name)) h.Add(t.name, t);
                                
                            }

                        }
                    }
                }
                catch (Exception)
                {
                    // print out an error message saying that the connection timed out or failed
                    System.Windows.Forms.MessageBox.Show("Error in read Point Data.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return;
        }

        public static void readPathURL(String URLString, ref ArrayList list)
        {
            XmlTextReader reader = new XmlTextReader(URLString);
            PathNode path = null;
            string[] split = null;
            char[] delimiters = { ' ' };
            char[] delimiters2 ={ ',' };
            int i = 0;
            double z = 0;

            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("color"))
                        {
                            string color = reader.ReadElementString("color");
                            list.Add(color);
                        }
                        if (reader.Name.Equals("coordinates"))
                        {

                            string coord = reader.ReadElementString("coordinates");
                            split = coord.Split(delimiters);

                        }

                    }
                }
                foreach (string s in split)
                {
                    string[] split2 = s.Split(delimiters2);
                    path = new PathNode();
                    int j = 0;
                    foreach (string element in split2)
                    {
                        if (!element.Equals(""))
                        {
                            switch (j % 3)
                            {
                                case 0:
                                    path.longitude = Double.Parse(element); break;
                                case 1:
                                    path.latitude = Double.Parse(element); break;
                                case 2:
                                    z = Double.Parse(element);
                                    path.id = i++;
                                    UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, path.latitude, path.longitude);
                                    path.addVertex(new Vertex(UTMConverter.UTMEasting, UTMConverter.UTMNorthing, z));
                                    break;
                            }
                            j++;
                        }

                    }
                    list.Add(path);

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readPathURL.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static void readVRMLToXMLURL(string URLString, ref Hashtable Buildings)
        {

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            try
            {
                doc.Load(URLString);
                ArrayList coordIndexes = new ArrayList();
                char[] delimiters = { ' ', ',', ':', '\t', '\n', '\r' };
                XmlElement root = doc.DocumentElement;
                XmlNodeList nodeList = root.SelectNodes("/doc/Shape");
                string textureImageURL;
                string repeatS = null;
                string repeatT = null;

                XmlNode temp = null;

                foreach (XmlNode building in nodeList)
                {

                    BuildingVRMLNode b = new BuildingVRMLNode(false);

                    if (building.HasChildNodes)
                    {
                        for (int j = 0; j < building.ChildNodes.Count; j++)
                        {
                            XmlNode child = building.ChildNodes[j];

                            //parse the UNIQUE_ID of the building for the hashtable key
                            if (child.LocalName.ToUpper().Equals("UNIQUE_ID")) //jenny 10.27
                            {
                                ((BuildingVRMLNode)b).nodeKey = child.InnerText;
                            }

                            else if (child.LocalName.ToUpper().Equals("ID")) //jenny 10.27
                            {
                                ((BuildingVRMLNode)b).buildingId = Int32.Parse(child.InnerText);
                            }

                            else if (child.LocalName.ToUpper().Equals("FROM_DATE")) //jenny 10.27
                            {
                                ((BuildingVRMLNode)b).start_time = DateTime.ParseExact(child.InnerText, "dd-MMM-yyyy", null); ;
                            }

                            else if (child.LocalName.ToUpper().Equals("TO_DATE")) //jenny 10.27
                            {
                                ((BuildingVRMLNode)b).end_time = DateTime.ParseExact(child.InnerText, "dd-MMM-yyyy", null); ;
                            }

                            //parse Appearance element
                            else if (child.LocalName.Equals("Appearance"))
                            {
                                //inside Appearance element, you'll find ImageTexture element
                                temp = building.SelectSingleNode("Appearance/ImageTexture");
                                XmlAttributeCollection lst = temp.Attributes;

                                //read single element value from attribute in ImageTexture
                                textureImageURL = lst.GetNamedItem("url").Value;
                                repeatS = lst.GetNamedItem("repeatS").Value;
                                repeatT = lst.GetNamedItem("repeatT").Value;

                                if (repeatS.Contains("TRUE") && repeatT.Contains("TRUE"))
                                    ((BuildingVRMLNode)(b)).URL_repeat = true;
                                else 
                                    ((BuildingVRMLNode)(b)).URL_repeat = false;

                                ((BuildingVRMLNode)(b)).addTextureImageURL(Scene.ServerName + ":8080/vrml/", textureImageURL);
                            }
                        }
                    }

                    //create the hashtable list
                    if (!Buildings.Contains(((BuildingVRMLNode)(b)).buildingId))
                    {
                        Buildings.Add(((BuildingVRMLNode)(b)).buildingId, new Hashtable());
                    }

                    //for each key's element list                         
                    string uniqueIdKey = ((BuildingVRMLNode)(b)).nodeKey;

                    if (((Hashtable)(Buildings[((BuildingVRMLNode)(b)).buildingId])).Contains(uniqueIdKey))
                    {
                        ((Hashtable)(Buildings[((BuildingVRMLNode)(b)).buildingId])).Remove(uniqueIdKey);
                    }

                    ((Hashtable)(Buildings[((BuildingVRMLNode)(b)).buildingId])).Add(uniqueIdKey, b);

                } 

                //Console.WriteLine("done");

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readVRMLToXMLURL.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }

        public static void readTrafficURL(String URLString, ArrayList list, Font f)
        {
            //list.Clear();
            XmlTextReader reader = new XmlTextReader(URLString);
            TrafficNode traffic = null;
            String freeway_name = null, freeway_direction = null;
            char[] delimiters = { ',' };
            string[] split = null;

            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("Placemark"))
                        {
                            traffic = new TrafficNode(f);
                        }

                        if (reader.Name.Equals("freeway"))
                        {
                            freeway_name = reader.ReadElementString("freeway");
                        }

                        if (reader.Name.Equals("direction"))
                        {
                            freeway_direction = reader.ReadElementString("direction");
                            traffic.freeway = freeway_name + freeway_direction;
                        }

                        if (reader.Name.Equals("speed"))
                        {
                            traffic.speed = Int32.Parse(reader.ReadElementString("speed"));
                        }

                        if (reader.Name.Equals("href"))
                        {
                            traffic.image = reader.ReadElementString("href");
                        }

                        if (reader.Name.Equals("y"))
                        {
                            traffic.image_y = Int32.Parse(reader.ReadElementString("y"));
                        }

                        if (reader.Name.Equals("w"))
                        {
                            traffic.image_w = Int32.Parse(reader.ReadElementString("w"));
                        }

                        if (reader.Name.Equals("h"))
                        {
                            traffic.image_h = Int32.Parse(reader.ReadElementString("h"));
                        }

                        if (reader.Name.Equals("coordinates"))
                        {

                            string coord = reader.ReadElementString("coordinates");
                            split = coord.Split(delimiters);

                            traffic.longitude = Double.Parse(split[0]);
                            traffic.latitude = Double.Parse(split[1]);
                            double z = Double.Parse(split[2]);
                            UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, traffic.latitude, traffic.longitude);
                            traffic.addVertex(new Vertex(UTMConverter.UTMEasting, UTMConverter.UTMNorthing, z));
                            list.Add(traffic);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readTrafficURL.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //public static bool getline_ofsight(String URLString)
        //{       
        //    WebRequest req = WebRequest.Create(URLString);
        //    WebResponse resp = req.GetResponse();

        //    Stream s = resp.GetResponseStream();
        //    StreamReader sr = new StreamReader(s, Encoding.ASCII);
        //    string doc = sr.ReadToEnd();
        //    if (doc.Contains("true")) return true;
        //    else return false;
        //}

        public static string get_sight_grid(String URLString)
        {
            try
            {
                if (RequestList == null)
                    RequestList = new ArrayList();
                WebRequest req = WebRequest.Create(URLString);
                RequestList.Add(req);
                req.Timeout = 360000;
                  WebResponse resp = req.GetResponse();
                Stream s = resp.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.ASCII);
                string doc = sr.ReadToEnd();
                resp.Close();
                if (doc.Contains("cannot"))
                    return null;
                else
                    return doc;
            }
            catch (Exception ex)
            {
               //  System.Windows.Forms.MessageBox.Show("Error Occured while quering about line of sight. The previous query is cancelled.Please wait...");
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
                return null;
            }

        }

        public static void TerminateReq()
        {
            try
            {
                if (RequestList != null)
                {
                    foreach (WebRequest wr in RequestList)
                    {
                        wr.Abort();
                    }
                    RequestList.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured! Please wait to click submit button.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static void readPOIURL(string URLString,ArrayList poiNodes, bool QueryPoint)
        {
            XmlUrlResolver resolver = new XmlUrlResolver();
            NetworkCredential myCred;
            myCred = new System.Net.NetworkCredential("stdbm", "navteq");
            resolver.Credentials = myCred;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = resolver;
            try
            {
                // Create the reader.
                XmlReader reader = XmlReader.Create(URLString, settings);
                poiNode t = null;
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                XmlElement root = doc.DocumentElement;
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("spaces", "http://earth.google.com/kml/2.0");
                XmlNodeList nodeList = root.SelectNodes("spaces:Folder/spaces:Folder/spaces:Placemark", nsmgr);
                XmlNode temp = null;
                string[] coordinates = null;
                string name = null;
                char[] delimiters = { ' ', ',', ':', '\t', '\n', '\r' };
                if(QueryPoint)
                {
                    XmlNode QueryNode = root.SelectSingleNode("spaces:Folder/spaces:Placemark", nsmgr);
                    temp = QueryNode.SelectSingleNode("spaces:name", nsmgr);
                    name = temp.InnerText;
                    t = new poiNode(name);

                    temp = QueryNode.SelectSingleNode("spaces:description", nsmgr);
                    t.description = temp.InnerText;

                    temp = QueryNode.SelectSingleNode("spaces:Style/spaces:IconStyle/spaces:Icon/spaces:href", nsmgr);
                    string image_name = temp.InnerText;
                    if (image_name.Contains("arrow")) image_name = "placemarks/Arrow.png";

                    t.POI_icon = new Icon(image_name);
                    t.contents = "<table><tr><th><a href=\"http://www.google.com/search?hl=en&q=" + t.point_name + "&btnG=Google+Search\">" + t.point_name + "</th></tr><tr><th>" + t.description + "</th></tr>";

                    temp = QueryNode.SelectSingleNode("spaces:Point/spaces:coordinates", nsmgr);
                    coordinates = temp.InnerText.Split(delimiters, System.StringSplitOptions.RemoveEmptyEntries);
                    t.longitude = Double.Parse(coordinates[0]);
                    t.latitude = Double.Parse(coordinates[1]);
                    UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, t.latitude, t.longitude);
                    t.y = UTMConverter.UTMNorthing;
                    t.x = UTMConverter.UTMEasting;
                    poiNodes.Add(t);
                }

                foreach (XmlNode placemark in nodeList)
                {
                    temp = placemark.SelectSingleNode("spaces:name", nsmgr);
                    name = temp.InnerText;
                    t = new poiNode(name);

                    temp = placemark.SelectSingleNode("spaces:description", nsmgr);
                    t.description = temp.InnerText;

                    temp = placemark.SelectSingleNode("spaces:Style/spaces:IconStyle/spaces:Icon/spaces:href", nsmgr);
                    string image_name = temp.InnerText;
                    if (image_name.Contains("arrow")) image_name = "placemarks/Arrow.png";
                    else if (image_name.Contains("AutoSvc")) image_name = "placemarks/AutoSvc.png";
                    else if (image_name.Contains("Business")) image_name = "placemarks/Business.png";
                    else if (image_name.Contains("CommSvc")) image_name = "placemarks/CommSvc.png";
                    else if (image_name.Contains("EduInsts")) image_name = "placemarks/EduInsts.png";
                    else if (image_name.Contains("Entertn")) image_name = "placemarks/Entertn.png";
                    else if (image_name.Contains("FinInsts")) image_name = "placemarks/FinInsts.png";
                    else if (image_name.Contains("Hospital")) image_name = "placemarks/Hospital.png";
                    else if (image_name.Contains("NamedPlc")) image_name = "placemarks/NamedPlc.png";
                    else if (image_name.Contains("Parking")) image_name = "placemarks/Parking.png";
                    else if (image_name.Contains("ParkRec")) image_name = "placemarks/ParkRec.png";
                    else if (image_name.Contains("Restrnts")) image_name = "placemarks/Restrnts.png";
                    else if (image_name.Contains("Shopping")) image_name = "placemarks/Shopping.png";
                    else if (image_name.Contains("TransHubs")) image_name = "placemarks/TransHubs.png";
                    else if (image_name.Contains("TravDest")) image_name = "placemarks/TravDest.png";
                    t.POI_icon = new Icon(image_name);
                    t.contents = "<table><tr><th><a href=\"http://www.google.com/search?hl=en&q=" + t.point_name + "&btnG=Google+Search\">" + t.point_name + "</th></tr><tr><th>" + t.description + "</th></tr>";

                    temp = placemark.SelectSingleNode("spaces:Point/spaces:coordinates", nsmgr);
                    coordinates = temp.InnerText.Split(delimiters, System.StringSplitOptions.RemoveEmptyEntries);
                    t.longitude = Double.Parse(coordinates[0]);
                    t.latitude = Double.Parse(coordinates[1]);
                    UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, t.latitude, t.longitude);
                    t.y = UTMConverter.UTMNorthing;
                    t.x = UTMConverter.UTMEasting;

                    poiNodes.Add(t);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readPOIURL.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static void readPhotoURL(string URLString, ref ArrayList photos)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.Load(URLString);
                XmlElement root = doc.DocumentElement;
                XmlNodeList nodeList = root.SelectNodes("/Folder/Photo");
                PhotoNode photo;
                foreach (XmlNode photomark in nodeList)
                {
                    if (photomark.HasChildNodes)
                    {
                        photo = new PhotoNode();
                        for (int j = 0; j < photomark.ChildNodes.Count; j++)
                        {
                            XmlNode child = photomark.ChildNodes[j];
                            if (child.LocalName.ToUpper().Equals("ID"))
                            {
                                photo.ID = child.InnerText;
                            }
                            else if (child.LocalName.ToUpper().Equals("LAT"))
                            {
                                photo.latitude = Double.Parse(child.InnerText);
                            }
                            else if (child.LocalName.ToUpper().Equals("LON"))
                            {
                                photo.longitude = Double.Parse(child.InnerText);
                                UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, photo.latitude, photo.longitude);
                                photo.y = UTMConverter.UTMNorthing;
                                photo.x = UTMConverter.UTMEasting;
                            }
                            else if (child.LocalName.ToUpper().Equals("URL"))
                            {
                                photo.setphotoURL(child.InnerText);
                            }
                            else if (child.LocalName.ToUpper().Equals("TIME"))
                            {
                                photo.photoTime = child.InnerText;
                            }
                            else if (child.LocalName.ToUpper().Equals("IMEI"))
                            {
                                photo.IMEI = child.InnerText;
                                photo.setPhoneOwnerName();
                                photos.Add(photo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in Photos.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static void readPOIInfoURL(string URLString, ref ArrayList POIList, string query)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.Load(URLString);
                XmlElement root = doc.DocumentElement;
                XmlNodeList nodeList = root.SelectNodes("/Folder/POIINFO");
                poiNode poiInfoNode = null;
                string[] queryTypes = query.Split(' ');
                foreach (XmlNode pinfo in nodeList)
                {
                    if (pinfo.HasChildNodes)
                    {
                        for (int j = 0; j < pinfo.ChildNodes.Count; j++)
                        {
                            XmlNode child = pinfo.ChildNodes[j];
                            if (child.LocalName.ToUpper().Equals("TYPE"))
                            {
                                string poiType = child.InnerText;
                                if (poiType == "BUILDINGINFO" && queryTypes[0] == "False") break;
                                if (poiType == "AUDIOINFO" && queryTypes[1] == "False") break;
                                if (poiType == "VIDEOINFO" && queryTypes[2] == "False") break;

                                if (poiType == "BUILDINGINFO")
                                {
                                    poiInfoNode = new BuildingInfoNode();
                                }
                                else if (poiType == "AUDIOINFO")
                                {
                                    poiInfoNode = new AudioInfoNode();
                                }
                                else if (poiType == "VIDEOINFO")
                                {
                                    poiInfoNode = new VideoInfoNode();
                                }
                            }
                            
                            else if (child.LocalName.ToUpper().Equals("NAME"))
                            {
                                poiInfoNode.point_name = child.InnerText;
                            }
                            else if (child.LocalName.ToUpper().Equals("FLOOR"))
                            {
                                if (poiInfoNode.GetType() == typeof(BuildingInfoNode))
                                {
                                    ((BuildingInfoNode)poiInfoNode).numberOffloors = int.Parse(child.InnerText);
                                }
                            }
                            else if (child.LocalName.ToUpper().Equals("BUILT"))
                            {
                                if (poiInfoNode.GetType() == typeof(BuildingInfoNode))
                                {
                                    ((BuildingInfoNode)poiInfoNode).yearBuilt = int.Parse(child.InnerText);
                                }
                            }
                            else if (child.LocalName.ToUpper().Equals("BLDG_ASSIGN"))
                            {
                                if (poiInfoNode.GetType() == typeof(BuildingInfoNode))
                                {
                                    ((BuildingInfoNode)poiInfoNode).assignableSurfaceArea = child.InnerText;
                                }
                            }
                            else if (child.LocalName.ToUpper().Equals("BLDG_NONASSIGN"))
                            {
                                if (poiInfoNode.GetType() == typeof(BuildingInfoNode))
                                {
                                    ((BuildingInfoNode)poiInfoNode).nonAssignableSurfaceArea = child.InnerText;
                                }
                            }
                            else if (child.LocalName.ToUpper().Equals("BLDG_GROSS"))
                            {
                                if (poiInfoNode.GetType() == typeof(BuildingInfoNode))
                                {
                                    ((BuildingInfoNode)poiInfoNode).grossSurfaceArea = child.InnerText;
                                }
                            }
                            else if (child.LocalName.ToUpper().Equals("LAT"))
                            {
                                poiInfoNode.latitude = Double.Parse(child.InnerText);
                            }
                            else if (child.LocalName.ToUpper().Equals("LON"))
                            {
                                poiInfoNode.longitude = Double.Parse(child.InnerText);
                                UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, poiInfoNode.latitude, poiInfoNode.longitude);
                                poiInfoNode.y = UTMConverter.UTMNorthing;
                                poiInfoNode.x = UTMConverter.UTMEasting;
                            }
                            else if (child.LocalName.ToUpper().Equals("URL"))
                            {
                                if (poiInfoNode.GetType() == typeof(BuildingInfoNode))
                                {
                                    ((BuildingInfoNode)poiInfoNode).fileURL = child.InnerText;
                                }
                                else if (poiInfoNode.GetType() == typeof(AudioInfoNode))
                                {
                                    ((AudioInfoNode)poiInfoNode).fileURL = child.InnerText;
                                }
                                else if (poiInfoNode.GetType() == typeof(VideoInfoNode))
                                {
                                    ((VideoInfoNode)poiInfoNode).fileURL = child.InnerText;
                                }
                                POIList.Add(poiInfoNode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in POIInfo.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //public static void readPOIURL(string URLString, ArrayList poiNodes)
        //{
        //    XmlUrlResolver resolver = new XmlUrlResolver();
        //    NetworkCredential myCred;
        //    myCred = new System.Net.NetworkCredential("stdbm", "navteq");
        //    resolver.Credentials = myCred;

        //    XmlReaderSettings settings = new XmlReaderSettings();
        //    settings.XmlResolver = resolver;
        //    try
        //    {
        //        // Create the reader.
        //        XmlReader reader = XmlReader.Create(URLString, settings);
        //        poiNode t = null;
        //        while (reader.Read())
        //        {
        //            if (reader.NodeType == XmlNodeType.Element)
        //            {
        //                if (reader.Name.Equals("name"))
        //                {
        //                    string name = reader.ReadElementString("name");
        //                    if (!name.Contains(".xml"))
        //                        t = new poiNode(name);
        //                }

        //                if (reader.Name.Equals("description"))
        //                {
        //                    t.description = reader.ReadElementString("description");
        //                }

        //                if (reader.Name.Equals("longitude"))
        //                {
        //                    t.longitude = Double.Parse(reader.ReadElementString("longitude"));
        //                }

        //                if (reader.Name.Equals("latitude"))
        //                {
        //                    t.latitude = Double.Parse(reader.ReadElementString("latitude"));
        //                    UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, t.latitude, t.longitude);
        //                    t.y = UTMConverter.UTMNorthing;
        //                    t.x = UTMConverter.UTMEasting;
        //                }

        //                if (reader.Name.Equals("href"))
        //                {
        //                    string image_name = reader.ReadElementString("href");
        //                    if (image_name.Contains("arrow")) image_name = "icons/arrow.gif";
        //                    else if (image_name.Contains("AutoSvc")) image_name = "icons/AutoSvc.png";
        //                    else if (image_name.Contains("Business")) image_name = "icons/Business.gif";
        //                    else if (image_name.Contains("CommSvc")) image_name = "icons/CommSvc.jpg";
        //                    else if (image_name.Contains("EduInsts")) image_name = "icons/EduInsts.png";
        //                    else if (image_name.Contains("Entertn")) image_name = "icons/Entertn.png";
        //                    else if (image_name.Contains("FinInsts")) image_name = "icons/FinInsts.gif";
        //                    else if (image_name.Contains("Hospital")) image_name = "icons/Hospital.png";
        //                    else if (image_name.Contains("NamedPlc")) image_name = "icons/NamedPlc.png";
        //                    else if (image_name.Contains("Parking")) image_name = "icons/Parking.png";
        //                    else if (image_name.Contains("ParkRec")) image_name = "icons/ParkRec.png";
        //                    else if (image_name.Contains("Restrnts")) image_name = "icons/Restrnts.png";
        //                    else if (image_name.Contains("Shopping")) image_name = "icons/Shopping.jpg";
        //                    else if (image_name.Contains("TransHubs")) image_name = "icons/TransHubs.png";
        //                    else if (image_name.Contains("TravDest")) image_name = "icons/TravDest.jpg";
        //                    t.POI_icon = new Icon(image_name);
        //                    t.POI_icon.setIconSize(t.getIconWidth(), t.getIconHeight());
        //                    t.contents = "<table><tr><th><a href=\"http://www.google.com/search?hl=en&q=" + t.point_name + "&btnG=Google+Search\">" + t.point_name + "</th></tr><tr><th>" + t.description + "</th></tr>";
        //                    poiNodes.Add(t);
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readPOIURL.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        //    }
        //}

        public static void readNearestNeighbor(string URLString, ref poiNode NearestPOI)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.Load(URLString);
                XmlElement root = doc.DocumentElement;
                XmlNodeList nodeList = root.SelectNodes("/markers/marker/neighbors");
                foreach (XmlNode NearestNeighbor in nodeList)
                {
                    if (NearestNeighbor.HasChildNodes)
                    {
                        string streetName = null;
                        string streetNo = null;
                        for (int j = 0; j < NearestNeighbor.ChildNodes.Count; j++)
                        {
                            XmlNode child = NearestNeighbor.ChildNodes[j];
                            if (child.LocalName.ToUpper().Equals("POI"))
                            {
                                string POIType = child.InnerText;
                                if (POIType.ToLower().Contains("autosvc")) POIType = "placemarks/AutoSvc.png";
                                else if (POIType.ToLower().Contains("business")) POIType = "placemarks/Business.png";
                                else if (POIType.ToLower().Contains("commsvc")) POIType = "placemarks/CommSvc.png";
                                else if (POIType.ToLower().Contains("eduinsts")) POIType = "placemarks/EduInsts.png";
                                else if (POIType.ToLower().Contains("entertn")) POIType = "placemarks/Entertn.png";
                                else if (POIType.ToLower().Contains("fininsts")) POIType = "placemarks/FinInsts.png";
                                else if (POIType.ToLower().Contains("hospital")) POIType = "placemarks/Hospital.png";
                                else if (POIType.ToLower().Contains("namedplc")) POIType = "placemarks/NamedPlc.png";
                                else if (POIType.ToLower().Contains("parking")) POIType = "placemarks/Parking.png";
                                else if (POIType.ToLower().Contains("parkrec")) POIType = "placemarks/ParkRec.png";
                                else if (POIType.ToLower().Contains("restrnts")) POIType = "placemarks/Restrnts.png";
                                else if (POIType.ToLower().Contains("shopping")) POIType = "placemarks/Shopping.png";
                                else if (POIType.ToLower().Contains("transhubs")) POIType = "placemarks/TransHubs.png";
                                else if (POIType.ToLower().Contains("travdest")) POIType = "placemarks/TravDest.png";
                                if (NearestPOI.POI_icon == null)
                                {
                                    NearestPOI.POI_icon = new Icon(POIType);
                                }
                            }
                            else if (child.LocalName.ToUpper().Equals("POI_NAME"))
                            {
                                NearestPOI.point_name = child.InnerText;
                            }
                            else if (child.LocalName.ToUpper().Equals("ST_NAME"))
                            {
                                streetName  = child.InnerText;
                            }
                            else if (child.LocalName.ToUpper().Equals("POI_ST_NUM"))
                            {
                                streetNo = child.InnerText;
                                NearestPOI.description = streetNo + " " + streetName;
                            }
                            else if (child.LocalName.ToUpper().Equals("NLAT"))
                            {
                                NearestPOI.latitude = Double.Parse(child.InnerText);
                            }
                            else if (child.LocalName.ToUpper().Equals("NLNG"))
                            {
                                NearestPOI.longitude = Double.Parse(child.InnerText);
                                UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, NearestPOI.latitude, NearestPOI.longitude);
                                NearestPOI.y = UTMConverter.UTMNorthing;
                                NearestPOI.x = UTMConverter.UTMEasting;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in Moving Object Nearest Neighbor Query.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static void readTextures(string URLString, ref Hashtable textures)
        {
            textures.Clear();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.Load(URLString);
                XmlElement root = doc.DocumentElement;
                XmlNodeList nodeList = root.SelectNodes("/textures/texture");
                char[] delimiters = { ' ', ',', ':', '\t', '\n', '\r' };

                foreach (XmlNode texture in nodeList)
                {
                    if (texture.HasChildNodes)
                    {
                        string ID = null, photoID = null, imageName = null;
                        double width = 0, height = 0;
                        int faceIndex = -1;
                        ArrayList vertices = new ArrayList();
                        ArrayList TextureVertices = new ArrayList();

                        for (int j = 0; j < texture.ChildNodes.Count; j++)
                        {
                            XmlNode child = texture.ChildNodes[j];
                            if (child.LocalName.ToUpper().Equals("ID"))
                            {
                                ID = child.InnerText;
                            }
                            else if (child.LocalName.ToUpper().Equals("FACEINDEX"))
                            {
                                faceIndex = int.Parse(child.InnerText);
                            }
                            else if (child.LocalName.ToUpper().Equals("PHOTOID"))
                            {
                                photoID = child.InnerText;
                            }
                            else if (child.LocalName.ToUpper().Equals("HEIGHT"))
                            {
                                height = double.Parse(child.InnerText);
                            }
                            else if (child.LocalName.ToUpper().Equals("WIDTH"))
                            {
                                width = double.Parse(child.InnerText);
                            }
                            else if (child.LocalName.ToUpper().Equals("IMAGE"))
                            {
                                imageName = "user_data/USCCampus/textures/" + child.InnerText;
                            }
                            else if (child.LocalName.ToUpper().Equals("TEXTURECOORDINATE"))
                            {
                                
                                XmlAttributeCollection lst = child.Attributes;
                                string[] elements = lst.GetNamedItem("point").InnerText.Split(delimiters);
                                int i = 0;
                                double x = -1,y = -1;
                                foreach (string s in elements)
                                {
                                    if (!s.Equals(""))
                                    {

                                        switch (i % 2)
                                        {
                                            case 0:
                                                x = Double.Parse(s); break;
                                            case 1:
                                                y = Double.Parse(s);
                                                TextureVertices.Add(new TexCoord(x, y));
                                                break;
                                        }
                                        i++;

                                    }

                                }

                            }
                            else if (child.LocalName.ToUpper().Equals("COORDINATE"))
                            {
                                
                                
                                XmlAttributeCollection lst = child.Attributes;
                                string[] elements = lst.GetNamedItem("point").InnerText.Split(delimiters);
                                int i = 0;
                                double x = -1;
                                double y = -1;
                                double z = -1;
                                foreach (string s in elements)
                                {
                                    if (!s.Equals(""))
                                    {

                                        switch (i % 3)
                                        {
                                            case 0:
                                                x = Double.Parse(s); break;
                                            case 1:
                                                y = Double.Parse(s); break;
                                            case 2:
                                                z = Double.Parse(s);

                                                vertices.Add(new Vertex(x, y, z));
                                                break;
                                        }
                                        i++;
                                    }
                                }
                                SideTextureNode stNode = new SideTextureNode(imageName, vertices,TextureVertices);
                                stNode.height = height;
                                stNode.width = width;
                                stNode.TextureNodeID = ID;
                                stNode.PhotoID = photoID;
                                if (!BuildingVRMLManager.textureFacesIndex.Contains(int.Parse(ID.Substring(0, 4))))
                                {
                                    ArrayList faceIndices = new ArrayList();
                                    faceIndices.Add(faceIndex);
                                    BuildingVRMLManager.textureFacesIndex.Add(int.Parse(ID.Substring(0, 4)), faceIndices);
                                }
                                else
                                {
                                    ((ArrayList)BuildingVRMLManager.textureFacesIndex[int.Parse(ID.Substring(0, 4))]).Add(faceIndex);
                                }
                                if (textures.Contains(ID))
                                {
                                    textures.Remove(ID);                                 
                                }
                                textures.Add(ID, stNode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in Texture Query.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /* Created this Class for 599 - by Rushit, Mridul, Ruchika - 11/25/2007
*  This function is to read XML containing tree information from local drive
* 
* 
* 
* 
*/
        public static void readTreeURL(String URLString, ref Hashtable TreeDataNodes, Font f)
        {
            int key = -1;
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                System.IO.FileInfo objFile = new System.IO.FileInfo(URLString);

                if (objFile.Exists)
                {
                    doc.Load(URLString);
                    ArrayList coordIndexes = new ArrayList();
                    char[] delimiters = { ' ', ',', ':', '\t', '\n', '\r' };
                    XmlElement root = doc.DocumentElement;
                    XmlNodeList nodeList = root.SelectNodes("/doc/Shape");

                    foreach (XmlNode tree in nodeList)
                    {
                        Node t;
                        key++;
                        t = new TreeNode(f);

                        if (tree.HasChildNodes)
                        {
                            for (int j = 0; j < tree.ChildNodes.Count; j++)
                            {

                                XmlNode child = tree.ChildNodes[j];
                                if (child.LocalName.ToUpper().Equals("UNIQUE_ID")) //jenny 10.27
                                {
                                    ((TreeNode)t).nodeKey = child.InnerText;
                                }

                                else if (child.LocalName.ToUpper().Equals("ID")) //jenny 10.27
                                {
                                    ((TreeNode)t).setTreeID(Int32.Parse(child.InnerText));
                                    //key = ((TreeNode)t).getTreeID();
                                }

                                if (child.LocalName.Equals("LONGITUDE"))
                                {
                                    ((TreeNode)t).setLongtitude(double.Parse(child.InnerText));

                                }
                                else if (child.LocalName.Equals("LATITUDE"))
                                {
                                    ((TreeNode)t).setLatitude(double.Parse(child.InnerText));
                                }
                                else if (child.LocalName.Equals("HEIGHT"))
                                {
                                    ((TreeNode)t).setHeight(double.Parse(child.InnerText));
                                }
                                else if (child.LocalName.Equals("WIDTH"))
                                {
                                    ((TreeNode)t).setWidth(double.Parse(child.InnerText));
                                }
                                else if (child.LocalName.Equals("LENGTH"))
                                {
                                    ((TreeNode)t).setLenght(double.Parse(child.InnerText));
                                }
                                else if (child.LocalName.Equals("IMAGE_URL"))
                                {
                                    //IndexedFaceSet element has attributes
                                    XmlNodeList urls = child.SelectNodes("URL");

                                    foreach (XmlNode textureURL in urls)
                                    {

                                        //ArrayList tempURL = new ArrayList();
                                        String url = textureURL.InnerText;
                                        //tempURL.Add(url);

                                        ((TreeNode)t).addTextureURL(url);
                                    }
                                    //IndexedFaceSet element also contain other elements (e.g. Coordinate)
                                }
                            }
                        }
                        //((TreeNode)t).addVertex(((TreeNode)t).getLatitude(), ((TreeNode)t).getLongitude());
                        TreeDataNodes.Add(key, t);
                        TreeNode.idCounter++;

                    }

                }
            }
            catch (Exception ex)
            {
                // System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readTramURL.");
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }

        public static void readTrajectoryURL(String URLString, ref ArrayList TramNodes, ref ArrayList Trajectory)
        {
            XmlDocument doc = new XmlDocument();
            TramNode t = null;
            PathNode p = null;
            // VideoDataNode v = null;
            char[] delimiters = { ',', ':', '\t', '\n', '\r' };
            string coords = null;
            string[] coord = null;

            try
            {
                doc.Load(URLString);
                XmlElement root = doc.DocumentElement;
                XmlElement folder = (XmlElement)root.FirstChild;
                XmlNodeList list = folder.ChildNodes;
                XmlElement document = (XmlElement)list.Item(0);
                XmlElement folder2 = (XmlElement)document.NextSibling;

                XmlNodeList coord_nodes = document.GetElementsByTagName("coordinates");
                if (coord_nodes != null && coord_nodes.Count > 0)
                {
                    XmlElement el = (XmlElement)coord_nodes.Item(0);
                    coords = el.InnerText;
                }

                int i = 0;
                coords = coords.Replace("\"", "");
                coord = coords.Split(delimiters);
                foreach (string s in coord)
                {
                    if (!s.Equals(""))
                    {
                        switch (i % 3)
                        {
                            case 0:
                                t = new TramNode();
                                p = new PathNode();
                                t.longitude = Double.Parse(s);
                                p.longitude = Double.Parse(s);
                                break;
                            case 1:
                                t.latitude = Double.Parse(s);
                                p.latitude = Double.Parse(s);

                                //  TramNodes.Add(t);
                                t.addVertex(t.latitude, t.longitude);
                                Trajectory.Add(p);
                                p.addVertex(p.latitude, p.longitude);
                                break;
                            case 2:
                                t.TramTime = DateTime.ParseExact(s, "dd-MMM-yy hh.mm.ss.000000 tt", null);
                                TramNodes.Add(t);
                                break;
                        }
                        i++;
                    }
                }
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Occured while reading XML File in readTrajectoryURL.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (NegaahMain.debug) System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            return;
        }
    }

}
