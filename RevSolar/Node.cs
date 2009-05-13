using System;
using System.Collections;

namespace test
{
	/// <summary>
	/// Summary description for Node.
	/// </summary>
	public abstract class Node
	{
		public Node()
		{
			//
			// TODO: Add constructor logic here
			//
            faces = new ArrayList();
            normals = new ArrayList();
		}
		public virtual void draw() { }
        public virtual void draw(float r, float g, float b) { }
        public virtual void draw(float r, float g, float b, float a) { }
        public virtual void addVertex(Vertex v) { }
        //public virtual void addTextureCoord(TexCoord t) { }
        public virtual void addFace(ArrayList f) { }
        protected ArrayList faces = null;
        protected ArrayList normals = null;

	}
}
