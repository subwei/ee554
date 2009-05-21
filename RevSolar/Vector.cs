using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    public class Vector
    {
        public double x, y, z;

        public Vector()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        // copy constructor
        public Vector(Vector vector) {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double Dot(Vector newVec)
        {
            double sum = (x * newVec.x) + (y * newVec.y) + (z * newVec.z);
            return sum;
        }

        public Vector Cross(Vector newVec)
        {
            Vector newVec1=new Vector();
            newVec1.x = (y * newVec.z) - (z * newVec.y);
            newVec1.y = -(x * newVec.z) + (z * newVec.x);
            newVec1.z = (x * newVec.y) - (y * newVec.x);
            return newVec1;
        }

        public void Normalize()
        {
            double magnitude = Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
            this.x = this.x / magnitude;
            this.y = this.y / magnitude;
            this.z = this.z / magnitude;

        }

        public double getMagnitude() {
            return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }
    }
}
