using System;
using System.Collections.Generic;
using System.Text;

namespace DescriptionLibs.Model
{
    public class RandomRotationConstraints
    {
        public class _X
        {
            public float Min;
            public float Max;
        }
        public class _Y
        {
            public float Min;
            public float Max;
        }
        public class _Z
        {
            public float Min;
            public float Max;
        }

        public _X X;
        public _Y Y;
        public _Z Z;
    }
}
