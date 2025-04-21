using System;
namespace Shared.Model
{
    public class BEDocument
    {
        public int mId { get; set; }

        public string mUrl { get; set; }

        public string mIdxTime { get; set; }

        public string mCreationTime { get; set; }

        public BEDocument() { }

        public BEDocument(int id, string idxTime, string creationTime)
        {
            mId = id;
            mIdxTime = idxTime;
            mCreationTime = creationTime;
        }
    }
}
