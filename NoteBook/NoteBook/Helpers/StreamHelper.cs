using System.IO;

namespace NoteBook.Helpers
{
    public class StreamHelper
    {
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer;

            using (BinaryReader reader = new BinaryReader(input))
            {
                buffer = reader.ReadBytes((int)input.Length);
            }
            return buffer;
        }
    }
}