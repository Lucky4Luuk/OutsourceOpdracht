using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class Pdf
    {
        public long id;
        public byte[] data;

        public Pdf(long id, byte[] data)
        {
            this.id = id;
            this.data = data;
        }
    }
}
