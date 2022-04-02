using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribbleHunter
{
    class Wave
    {
        private List<WaveEntity> entities = new List<WaveEntity>();

        public Wave()
        {

        }

        public void AddEntry(WaveEntity entry)
        {
            this.entities.Add(entry);
        }

        public List<WaveEntity> Entries
        {
            get
            {
                return this.entities;
            }
        }
    }
}
