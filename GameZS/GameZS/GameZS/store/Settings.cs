using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ZombieSmashers.store
{
    public class Settings
    {
        private bool rumble = true;

        public bool Rumble
        {
            get { return rumble; }
            set { rumble = value; }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(rumble);
        }

        public void Read(BinaryReader reader)
        {
            rumble = reader.ReadBoolean();
        }
    }
}